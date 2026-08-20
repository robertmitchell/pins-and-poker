using DG.Tweening;
using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;
using static Global;

public class GuestLoginScreen : UIScreenBase
{
    public AnimatrixButton imageUploadBtn;
    public AnimatrixButton cancelBtn;
    public AnimatrixButton createBtn;
    public InputField nameInputField;
    public InputField emailInputField;
    public RawImage profilePic;
    public Texture2D defaultPic;
    public bool isModerator;
    string deviceId;
    Tweener _tweener;

    private void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
    }

    private void OnDisable()
    {
        CancelInvoke();
        _tweener.Kill();
        ResetFields();
    }

    void Start()
    {
        cancelBtn.onClick.AddListener(() => CancelBTnClicked());
        createBtn.onClick.AddListener(() => CreateBTnClicked());
        imageUploadBtn.onClick.AddListener(() => GalleryImageUploader.Instance.PickImage(profilePic));
    }

    void CancelBTnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<SocialLoginScreen>();
    }

    [ContextMenu("Test InputField")]
    void CreateBTnClicked()
    {
        SaveData();
    }

    public void SaveData()
    {
        Texture2D profileImage = profilePic.texture as Texture2D;

        #if UNITY_EDITOR
                deviceId = SystemInfo.deviceUniqueIdentifier + (isModerator == true? "mod":"user");
                Debug.Log("iOS Device ID: " + deviceId);
        #else
                deviceId = SystemInfo.deviceUniqueIdentifier;
                Debug.Log("Device ID: " + deviceId);
        #endif

        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.userName, nameInputField.text },
                { Db_Keys.userType,  currentUserType.ToString()},
                { Db_Keys.loginType, currentLoginType.ToString()},
                { Db_Keys.authProvider, currentAuthProvider.ToString() },
                { Db_Keys.platform, (Application.platform == RuntimePlatform.Android) ? "android" : "ios" },
                { Db_Keys.deviceToken,deviceId }
                //{ Db_Keys.deviceToken,"Moderator" }  //testingMod
                //{ Db_Keys.deviceToken,"user" }   //testinguser
            };

            WebServices.Instance.MakeRequest<PlayerData>(
                ApiRoutes.logIn,
                WebServices.HttpMethod.POST,
                OnSuccess,
                OnFailure,
                null,
                formData,
                profileImage,
                true
            );
        }
        else
        {
            ShowExceptionMessage("Please enter your name!", nameInputField);
            return;
        }
    }

    void OnSuccess(PlayerData resp, long statusCode)
    {
        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.waving);

        PlayerPrefs.SetInt(Db_Keys.islogedIn, 1);
        PlayerPrefs.SetString(Db_Keys.playerID, resp.PlayerId.ToString());
        PlayerPrefs.SetString(Db_Keys.userName, resp.Username);
        PlayerPrefs.SetString(Db_Keys.userEmail, resp.Email);
        PlayerPrefs.SetString(Db_Keys.userType, resp.UserType);
        PlayerPrefs.SetString(Db_Keys.authProvider, resp.AuthProvider);
        PlayerPrefs.SetString(Db_Keys.token, resp.AccessToken);

        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.moderator.ToString())
        {
            PlayerPrefs.SetString(Db_Keys.moderatorId, PlayerPrefs.GetString(Db_Keys.playerID));
        }

            ImageSaveLoad.Instance.SaveRawImage(profilePic);
        PlayerPrefs.Save();

        MessagePopUpScreen.Instance.ShowMessage("Your profile has been created!", "Congratulations", null, LoadScene);

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

    #region EXCEPTION MESSGAGE
    internal void ShowExceptionMessage(string msgTxt, InputField inputField)
    {
        inputField.placeholder.gameObject.SetActive(false);
        inputField.placeholder.gameObject.SetActive(true);
        ((Text)inputField.placeholder).text = msgTxt;
    }
    #endregion

    void ResetFields()
    {
        ((Text)nameInputField.placeholder).text = "Enter User Name...";
        nameInputField.text = string.Empty;
        profilePic.texture = defaultPic;
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
