using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class HowToCreateGameScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public TMP_Text contentTxt;
    public ScrollRect contentScrollRect;

    private const string PLACEHOLDER_CONTENT =
        "How to Create a Game\n\n" +
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\n\n" +
        "Step 1 — Lorem ipsum\n" +
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\n\n" +
        "Step 2 — Duis aute irure\n" +
        "Dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.\n\n" +
        "Step 3 — Excepteur sint\n" +
        "Occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\n\n" +
        "Step 4 — Pellentesque habitant\n" +
        "Morbi tristique senectus et netus et malesuada fames ac turpis egestas.\n\n" +
        "Step 5 — Vestibulum tortor\n" +
        "Quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper.";

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
