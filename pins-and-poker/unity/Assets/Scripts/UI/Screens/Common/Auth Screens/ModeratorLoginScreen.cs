using DG.Tweening;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;
using static Global;

public class ModeratorLoginScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton loginBtn;
    public InputField emailInputField;
    public InputField passwordInputField;
    string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
    Tweener _tweener;

    private void OnDisable()
    {
        _tweener.Kill();
        CancelInvoke();
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
        loginBtn.onClick.AddListener(() => LoginBtnClicked());
    }

    void BackBtnClicked()
    {
        ResetFields();
        UIManager.instance.Hide();
        UIManager.instance.Show<RoleSelectionScreen>();
    }

    private void LoginBtnClicked()
    {
        if (!string.IsNullOrEmpty(emailInputField.text) && !string.IsNullOrEmpty(passwordInputField.text) && Regex.IsMatch(emailInputField.text, emailPattern))
        {       
            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.userEmail, emailInputField.text },
                { Db_Keys.userPassword,  passwordInputField.text},
                { Db_Keys.authProvider, currentAuthProvider.ToString() },
                { Db_Keys.deviceToken, SystemInfo.deviceUniqueIdentifier },
                { Db_Keys.platform, (Application.platform == RuntimePlatform.Android) ? "android" : "ios" },
                };

            WebServices.Instance.MakeRequest<PlayerData>(
            ApiRoutes.modlogin,
            WebServices.HttpMethod.POST,
            OnSuccess,
            OnFailure,
            null,
            formData,
            null,
            true
            );        
        }
        else
        {
            ShowExceptionMessage(string.IsNullOrEmpty(emailInputField.text) ? "Please enter your email!" : "Please enter a valid email!", emailInputField);     
            if (string.IsNullOrEmpty(passwordInputField.text)) ShowExceptionMessage("Please enter your password!", passwordInputField);
            return;
        }
    }

    void OnSuccess(PlayerData resp, long statusCode)
    {
        PlayerPrefs.SetString(Db_Keys.token, resp.AccessToken);
        PlayerPrefs.Save();
        UIManager.instance.Hide();
        UIManager.instance.Show<EditModeratorProfileScreen>();     
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
        ((Text)emailInputField.placeholder).text = "Enter Email...";
        ((Text)passwordInputField.placeholder).text = "Enter Password...";
        emailInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }

}
