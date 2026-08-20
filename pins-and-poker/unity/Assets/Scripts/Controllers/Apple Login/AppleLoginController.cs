using UnityEngine;
using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Text;
using static Global;

public class AppleLoginController : Singleton<AppleLoginController>, ILoginWithAppleIdResponse
{
    public bool Success => true;
    public IAppleError Error => throw new System.NotImplementedException();
    public IAppleIDCredential AppleIDCredential => throw new System.NotImplementedException();
    public IPasswordCredential PasswordCredential => throw new System.NotImplementedException();
    public IAppleAuthManager appleAuthManager;

    [Space]
    [Header("User Details")]
    [SerializeField] string _userName;
    [SerializeField] string _userEmail;
    [SerializeField] string _userAge;
    [SerializeField] string _userSocialID;
    [SerializeField] string _userSocialToken;

    void Start()
    {
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            var deserializer = new PayloadDeserializer();
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }
        if (this.appleAuthManager == null)
        {
            var deserializer = new PayloadDeserializer();
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }
        if (this.appleAuthManager != null)
        {
            //Debug.Log("Apple Auth manager not null ");
        }
    }

    void Update()
    {
        AppleManagerUpdate();
    }

    void AppleManagerUpdate()
    {
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            if (this.appleAuthManager == null)
            {
                //Debug.Log("Null AuthManager");
                var deserializer = new PayloadDeserializer();
                this.appleAuthManager = new AppleAuthManager(deserializer);
            }

            if (this.appleAuthManager != null)
            {
                appleAuthManager.Update();
            }
        }
    }

    public void PerformSigninWithApple()
    {
        var login = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        if (this.appleAuthManager != null)
        {
            //Debug.Log("PerformSigninWithApple() =>  this.appleAuthManager != null ");

            this.appleAuthManager.LoginWithAppleId(login, credential =>
            {
                //Debug.Log(credential + "Credentials");
                Debug.Log("PerformSigninWithApple credential.User" + credential.User);

                var appleIdCredential = credential as IAppleIDCredential;
                int val = Random.Range(0, 9999);

                if (appleIdCredential != null)
                {

                    // Sets Credentials from IApplecredentials
                    //Debug.Log(appleIdCredential + "above if");

                    if (appleIdCredential != null)
                    {
                        //USER SOCIAL ID
                        if (appleIdCredential.User != null || appleIdCredential.User != "" || !string.IsNullOrEmpty(appleIdCredential.User))
                        {
                            _userSocialID = appleIdCredential.User.ToString();
                            //Debug.Log("appleIdCredential.User: " + appleIdCredential.User.ToString());
                        }
                        else
                        {
                            _userSocialID = "appleuser" + val + (val * 10);
                        }


                        //USER EMAIL
                        if (appleIdCredential.Email != null || appleIdCredential.Email != "" || !string.IsNullOrEmpty(appleIdCredential.Email))
                        {
                            _userEmail = appleIdCredential.Email.ToString();
                        }
                        else
                        {
                            _userEmail = "appleUser" + val + "@faith.com";
                            //Debug.Log(_userEmail);
                        }

                        //USER FULL NAME
                        if (appleIdCredential.FullName.GivenName != null || appleIdCredential.FullName.GivenName != "" || !string.IsNullOrEmpty(appleIdCredential.FullName.GivenName))
                        {
                            _userName = appleIdCredential.FullName.GivenName.ToString();
                        }
                        else
                        {
                            _userName = "appleUser" + val;
                            //Debug.Log(_userName);
                        }
                    }
                    else
                    {
                        Debug.Log("apple credentials are null" + appleIdCredential);
                    }
                    Debug.Log("Data Fetch Complete Apple");

                    _userSocialToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken,
                        0,
                        appleIdCredential.IdentityToken.Length);

                    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode,
                        0,
                        appleIdCredential.AuthorizationCode.Length);


                    SendDataToServer();

                }
                else
                {
                    AddToInformation("Credentials are null");
                }
            },
            error =>
            {
                Debug.Log("Credential Error");
                var authoriztionErrorCode = error.GetAuthorizationErrorCode();
                AddToInformation(authoriztionErrorCode.ToString());
            });
        }
        else
        {
            AddToInformation("this apple auth manager is null");
            AddToInformation("Updating");
            if (this.appleAuthManager != null)
            {
                AddToInformation("Updated");
            }
        }
    }

    private void SendDataToServer()
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.userName, _userName },
                { Db_Keys.userEmail, _userEmail },
                { Db_Keys.socialId, _userSocialToken },
                { Db_Keys.userType,  currentUserType.ToString()},
                { Db_Keys.loginType, currentLoginType.ToString()},
                { Db_Keys.authProvider, currentAuthProvider.ToString() },
                { Db_Keys.deviceToken, SystemInfo.deviceUniqueIdentifier },
                { Db_Keys.platform, (Application.platform == RuntimePlatform.Android) ? "android" : "ios" },
            };

        WebServices.Instance.MakeRequest<PlayerData>(
            ApiRoutes.logIn,
            WebServices.HttpMethod.POST,
            OnSuccess,
            OnFailure,
            null,
            formData,
            null,
            true
        ); 
    }

    void OnSuccess(PlayerData resp, long statusCode)
    {
        PlayerPrefs.SetInt(Db_Keys.islogedIn, 1);
        PlayerPrefs.SetString(Db_Keys.playerID, resp.PlayerId);
        PlayerPrefs.SetString(Db_Keys.userName, resp.Username);
        PlayerPrefs.SetString(Db_Keys.userEmail, resp.Email);
        PlayerPrefs.SetString(Db_Keys.userType, resp.UserType);
        PlayerPrefs.SetString(Db_Keys.authProvider, resp.AuthProvider);
        PlayerPrefs.SetString(Db_Keys.token, resp.AccessToken);
        PlayerPrefs.Save();

        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.waving);
        MessagePopUpScreen.Instance.ShowMessage("Your profile setup has been completed!", "Congratulations", null, LoadScene);

        void LoadScene()
        {
            SceneLoader.Instance.LoadScene(SceneLoader.Scene.Gameplay);
        }

        Debug.Log("AccessToken = " + resp.AccessToken);
    }

    void OnFailure(string error)
    {
        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.shocking);

        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
        return;
    }

    //public void Signout()
    //{
    //    this.appleAuthManager.SetCredentialsRevokedCallback(null);
    //}

    public void AddToInformation(string str)
    {
        Debug.LogError("\n" + str);
    }
}