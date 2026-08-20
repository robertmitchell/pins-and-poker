using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton homeBtn;
    public AnimatrixButton myLeagueBtn;
    public AnimatrixButton profileBtn;
    public AnimatrixButton termsAndCondBtn;
    public AnimatrixButton privacyPolicyBtn;
    public AnimatrixButton deleteAccountBtn;
    public AnimatrixButton logoutBtn;
    public AnimatrixButton submitDisputeBtn;
    public ScrollRect settingsScrollRect;

    private void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
        settingsScrollRect.verticalNormalizedPosition = 1;
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBTnClicked());
        homeBtn.onClick.AddListener(() => HomeBTnClicked());
        myLeagueBtn.onClick.AddListener(() => MyLeagueBTnClicked());
        profileBtn.onClick.AddListener(() => ProfileBtnClicked());
        termsAndCondBtn.onClick.AddListener(() => TermsAndCondBtnClicked());
        privacyPolicyBtn.onClick.AddListener(() => PrivacyPolicyBtnClicked());
        deleteAccountBtn.onClick.AddListener(() => DeleteAccountBtnClicked());
        logoutBtn.onClick.AddListener(() => LogoutBtnClicked());
        if (submitDisputeBtn) submitDisputeBtn.onClick.AddListener(() => SubmitDisputeBtnClicked());
        //notificatioBtn.onClick.AddListener(() => NotificationBTnClicked());
        //myAssignCardBtn.onClick.AddListener(() => MyAssignCardBtn());
    }

    public void BackBTnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<HomeScreen>();
    }

    //public void NotificationBTnClicked()
    //{
    //    UIManager.instance.Hide();
    //    UIManager.instance.Show<NotificationScreen>();
    //}

    public void HomeBTnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<HomeScreen>();
    }

    public void MyLeagueBTnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<MyLeaguesScreen>();
    }

    //public void MyAssignCardBtn()
    //{
    //    ScreensManager.Instance.EnableDisableMyAssignedCardsScreen_ref(true);
    //    gameObject.SetActive(false);
    //}

    public void ProfileBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<ProfileScreen>();
    }

    public void TermsAndCondBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<TermsAndConditionsScreen>();
    }

    public void PrivacyPolicyBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<PrivacyPolicyScreen>();
    }

    public void SubmitDisputeBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<SubmitDisputeScreen>();
    }

    public void DeleteAccountBtnClicked()
    {
        ConfirmationPopupScreen.Instance.ShowConfirmationMessage("Are you sure you want to delete\r\nyour account?", "Delete Account", "Cancel", "Delete", SendPaylaod, 1);

        void SendPaylaod()
        {
            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.token, PlayerPrefs.GetString(Db_Keys.token)  }
            };
            WebServices.Instance.MakeRequest<ResponseData>(
                ApiRoutes.deleteAccount,
                WebServices.HttpMethod.DELETE,
                OnSuccess,
                OnFailure,
                null,
                formData,
                null,
                true
            );
        }
    }

    public void LogoutBtnClicked()
    {
        ConfirmationPopupScreen.Instance.ShowConfirmationMessage("Are you sure to log out of your\r\naccount?", "Logout", "Cancel", "Logout", SendPayload);

        void SendPayload()
        {
            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.token, PlayerPrefs.GetString(Db_Keys.token)  }
            };
            WebServices.Instance.MakeRequest<ResponseData>(
                ApiRoutes.logOut,
                WebServices.HttpMethod.POST,
                OnSuccess,
                OnFailure,
                null,
                formData,
                null,
                true
            );
        }
    }

    void OnSuccess(ResponseData response, long statusCode)
    {
        PlayerPrefs.DeleteAll();
        if (PlayerPrefs.GetString(Db_Keys.authProvider) == Global.AuthProvider.google.ToString())
            GoogleLoginController.Instance.OnSignOut();
        SceneManager.LoadScene(0);
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
    }
    
    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
