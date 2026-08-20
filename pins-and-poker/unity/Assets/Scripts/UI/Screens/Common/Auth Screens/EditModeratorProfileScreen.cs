using DG.Tweening;
using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class EditModeratorProfileScreen : UIScreenBase
{
    //public AnimatrixButton exitBtn;
    public AnimatrixButton proceedBtn;
    public AnimatrixButton imageUploadBtn;
    public InputField fullNameInputField;
    public InputField emailInputField;
    public InputField phoneNumberInputField;
    public RawImage profilePic;
    public Texture2D defaultPic;
    Tweener _tweener;

    private void OnEnable()
    {
        emailInputField.text = UIManager.instance.GetScreen<ModeratorLoginScreen>().emailInputField.text;
    }

    private void OnDisable()
    {
        ResetFields();
        _tweener.Kill();
        CancelInvoke();
    }

    private void Start()
    {
        proceedBtn.onClick.AddListener(() => ProceedBtnClicked());
        imageUploadBtn.onClick.AddListener(() => GalleryImageUploader.Instance.PickImage(profilePic));
    }

    private void ProceedBtnClicked()
    {
        Texture2D profileImage = profilePic.texture as Texture2D;

        if (!string.IsNullOrEmpty(fullNameInputField.text) && !string.IsNullOrEmpty(phoneNumberInputField.text))
        {         
            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.userName, fullNameInputField.text },
                { Db_Keys.userPhoneNumber, phoneNumberInputField.text },
            };

            WebServices.Instance.MakeRequest<PlayerData>(
            ApiRoutes.updateProfile,
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
            if (string.IsNullOrEmpty(fullNameInputField.text)) ShowExceptionMessage("Please enter your name!", fullNameInputField);
            if (string.IsNullOrEmpty(phoneNumberInputField.text)) ShowExceptionMessage("Please enter your phone number!", phoneNumberInputField);
            return;
        }
    }

    void OnSuccess(PlayerData resp, long statusCode)
    {
        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.waving);

        PlayerPrefs.SetInt(Db_Keys.islogedIn, 1);
        PlayerPrefs.SetString(Db_Keys.playerID, resp.PlayerId);
        PlayerPrefs.SetString(Db_Keys.userName, resp.Username);
        PlayerPrefs.SetString(Db_Keys.userEmail, resp.Email);
        PlayerPrefs.SetString(Db_Keys.userPhoneNumber, resp.PhoneNumber);
        PlayerPrefs.SetString(Db_Keys.userType, resp.UserType);
        //PlayerPrefs.SetString(Db_Keys.token, resp.AccessToken);
        ImageSaveLoad.Instance.SaveRawImage(profilePic);
        PlayerPrefs.Save();

        MessagePopUpScreen.Instance.ShowMessage("Your profile has been created!", "Congratulations", null, LoadScene);

        void LoadScene()
        {
            SceneLoader.Instance.LoadScene(SceneLoader.Scene.Gameplay);
        }
    }

    void OnFailure(string error)
    {
        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.shocking);

        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
    }

    #region EXCEPTION MESSGAGE
    internal void ShowExceptionMessage(string msgTxt, InputField inputField)
    {
        inputField.placeholder.gameObject.SetActive(false);
        inputField.placeholder.gameObject.SetActive(true);
        inputField.text = string.Empty;
        ((Text)inputField.placeholder).text = msgTxt;
    }
    #endregion

    internal void ResetFields()
    {
        ((Text)fullNameInputField.placeholder).text = "Enter Full Name...";
        ((Text)phoneNumberInputField.placeholder).text = "Enter Phone Number...";
        fullNameInputField.text = string.Empty;
        emailInputField.text = string.Empty;
        phoneNumberInputField.text = string.Empty;
        profilePic.texture = defaultPic;
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
