using DG.Tweening;
using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;
using static Global;

public class EditProfileScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton imageUploadBtn;
    public AnimatrixButton saveChangesBtn;
    public InputField fullNameInputField;
    public InputField emailInputField;
    public InputField phoneNumberInputField;
    public GameObject emailInputPanel;
    public GameObject phoneNumberPanel;
    public RawImage profilePic;
    public Texture2D defaultPic;
    Tweener _tweener;

    private void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
        fullNameInputField.text = PlayerPrefs.GetString(Db_Keys.userName);
        emailInputField.text = PlayerPrefs.GetString(Db_Keys.userEmail);
        phoneNumberInputField.text = PlayerPrefs.GetString(Db_Keys.userPhoneNumber);

        if (PlayerPrefs.HasKey(Db_Keys.userImage)) ImageSaveLoad.Instance.LoadImageFromPlayerPrefs(profilePic);
        else Debug.Log("UserImage Not Available PlayerPrefs");

        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.user.ToString()) ReplaceObjects(false);
        else ReplaceObjects(true);
    }

    private void OnDisable()
    {
        ResetFields();
        CancelInvoke();
        _tweener.Kill();
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BacBtnClicked());
        imageUploadBtn.onClick.AddListener(() => GalleryImageUploader.Instance.PickImage(profilePic));
        saveChangesBtn.onClick.AddListener(() => SaveChangesBtnClicked());
    }

    void BacBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<ProfileScreen>();
    }


    void SaveChangesBtnClicked()
    {
        Texture2D profileImage = profilePic.texture as Texture2D;

        if (!string.IsNullOrEmpty(fullNameInputField.text) || (!string.IsNullOrEmpty(fullNameInputField.text) && !string.IsNullOrEmpty(phoneNumberInputField.text)))
        {
            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.userName, fullNameInputField.text },
                { Db_Keys.userPhoneNumber, phoneNumberInputField.text }
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
        PlayerPrefs.SetString(Db_Keys.userName, resp.Username);
        PlayerPrefs.SetString(Db_Keys.userPhoneNumber, resp.PhoneNumber);
        PlayerPrefs.SetString(Db_Keys.token, resp.AccessToken);
        ImageSaveLoad.Instance.SaveRawImage(profilePic);
        PlayerPrefs.Save();

        MessagePopUpScreen.Instance.ShowMessage("Your profile has been updated!", "Congratulations", null, ChangeScreen);

        void ChangeScreen()
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<ProfileScreen>();
        }
    }

    void OnFailure(string error)
    {
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

    void ReplaceObjects(bool active)
    {
        emailInputPanel.SetActive(active);
        phoneNumberPanel.SetActive(active);
    }

    void ResetFields()
    {
        fullNameInputField.text = string.Empty;
        phoneNumberInputField.text = string.Empty;
        profilePic.texture = defaultPic;
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
