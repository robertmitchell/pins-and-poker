using UIAnimatrix;
using UnityEngine;
using static Global;

public class SocialLoginScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton guestBtn;
    public AnimatrixButton googleBtn;
    public AnimatrixButton appleBtn; 

    private void OnEnable()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            appleBtn.gameObject.SetActive(false);
        }
        else
        {
            appleBtn.gameObject.SetActive(true);
        }
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
        guestBtn.onClick.AddListener(() =>  GuestBTnClicked());
        googleBtn.onClick.AddListener(() => GoogleBTnClicked());
        appleBtn.onClick.AddListener(() =>  AppleBTnClicked());
    }

    private void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<RoleSelectionScreen>();
    }

    void GuestBTnClicked()
    {
        currentLoginType = LoginType.guest;
        currentAuthProvider = AuthProvider.guest;

        UIManager.instance.Hide();
        UIManager.instance.Show<GuestLoginScreen>();

    }
    void GoogleBTnClicked()
    {
        currentLoginType = LoginType.social;
        currentAuthProvider = AuthProvider.google;
        GoogleLoginController.Instance.OnSignIn();
    }

    void AppleBTnClicked()
    {
        currentLoginType = LoginType.social;
        currentAuthProvider = AuthProvider.apple;
        AppleLoginController.Instance.PerformSigninWithApple();
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
