using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class CapabilityListScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public TMP_Text contentTxt;
    public ScrollRect contentScrollRect;

    private const string PLACEHOLDER_CONTENT =
        "What Pins & Poker Can Do\n\n" +
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\n\n" +
        "• Lorem ipsum feature one — consectetur adipiscing elit\n" +
        "• Lorem ipsum feature two — sed do eiusmod tempor\n" +
        "• Lorem ipsum feature three — incididunt ut labore\n" +
        "• Lorem ipsum feature four — et dolore magna aliqua\n" +
        "• Lorem ipsum feature five — ut enim ad minim veniam\n" +
        "• Lorem ipsum feature six — quis nostrud exercitation\n\n" +
        "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

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
