using UIAnimatrix;
using UnityEngine;
using static Global;

public class RoleSelectionScreen : UIScreenBase
{
    public AnimatrixButton playerBtn;
    public AnimatrixButton moderatorBtn;

    private void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
    }

    private void Start()
    {
        playerBtn.onClick.AddListener(() => PlayerBtnClicked());
        moderatorBtn.onClick.AddListener(() => ModeratorBtnClicked());
    }

    void PlayerBtnClicked()
    {
        currentUserType = UserType.user;
        UIManager.instance.Hide();
        UIManager.instance.Show<SocialLoginScreen>();
    }

    void ModeratorBtnClicked()
    {
        currentUserType = UserType.moderator;
        currentAuthProvider = AuthProvider.normal;
        //PlayerPrefs.SetString(Db_Keys.Moderator, "Moderator");

        UIManager.instance.Hide();
        UIManager.instance.Show<ModeratorLoginScreen>();
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
