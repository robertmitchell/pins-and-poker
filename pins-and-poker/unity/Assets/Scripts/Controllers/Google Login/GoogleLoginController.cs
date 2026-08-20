using Google;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Global;

public class GoogleLoginController : Singleton<GoogleLoginController>
{
    private GoogleSignInConfiguration configuration;
    public string webClientId = "402426576620-t2sb9sppv7o8qh8f9cg8ejcia8ur2v9q.apps.googleusercontent.com";
    public override void Awake()
    {
        if (GoogleSignIn.Configuration == null)
        {
            configuration = new GoogleSignInConfiguration
            {                     
                WebClientId = webClientId,
                RequestIdToken = true,
                UseGameSignIn = false,
                RequestEmail = true
            };
            GoogleSignIn.Configuration = configuration;
        }
    }

    public void OnSignIn()
    {
        if (GoogleSignIn.DefaultInstance == null)
        {
            GoogleSignIn.Configuration = configuration;
        }
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished, TaskScheduler.Default);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                        (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogError("Got Error: " + error.Status + " " + error.Message);

                }
                else
                {
                    Debug.LogError("Got unexpected exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Cancelled");
        }
        else
        {
            SendDataToServer(task.Result);
        }
    }

    void SendDataToServer(GoogleSignInUser user)
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.userEmail, user.Email },
            { Db_Keys.userName, user.DisplayName },
            { Db_Keys.socialId, user.IdToken },
            { Db_Keys.userType, currentUserType.ToString()},
            { Db_Keys.loginType, currentLoginType.ToString()},
            { Db_Keys.authProvider, currentAuthProvider.ToString() },
            { Db_Keys.deviceToken, SystemInfo.deviceUniqueIdentifier },
            { Db_Keys.platform, (Application.platform == RuntimePlatform.Android) ? "android" : "ios"},
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

    public void OnSignOut()
    {
        Debug.Log("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }
}
