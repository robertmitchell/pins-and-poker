using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class HowItWorksScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public TMP_Text contentTxt;
    public ScrollRect contentScrollRect;

    private const string PLACEHOLDER_CONTENT =
        "How Pins & Poker Works\n\n" +
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\n\n" +
        "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\n\n" +
        "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. " +
        "Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper.";

    private void OnEnable()
    {
        contentScrollRect.verticalNormalizedPosition = 1;
        contentTxt.text = PLACEHOLDER_CONTENT;
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
    }

    void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<HomeScreen>();
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
