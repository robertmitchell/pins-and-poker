using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGuideScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public TMP_Text contentTxt;
    public ScrollRect contentScrollRect;

    private const string PLACEHOLDER_CONTENT =
        "Button & Card Controls Guide\n\n" +
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n\n" +
        "CARD BUTTONS\n\n" +
        "• [Button Name] — Lorem ipsum: what this button does on a card.\n" +
        "• [Button Name] — Sed do eiusmod tempor: what this button does on a card.\n" +
        "• [Button Name] — Incididunt ut labore: what this button does on a card.\n" +
        "• [Button Name] — Et dolore magna aliqua: what this button does on a card.\n\n" +
        "GAME CONTROLS\n\n" +
        "• [Button Name] — Ut enim ad minim veniam: description of action.\n" +
        "• [Button Name] — Quis nostrud exercitation: description of action.\n" +
        "• [Button Name] — Ullamco laboris nisi: description of action.\n\n" +
        "LEAGUE CONTROLS\n\n" +
        "• [Button Name] — Duis aute irure dolor: description of action.\n" +
        "• [Button Name] — In reprehenderit in voluptate: description of action.\n\n" +
        "If you have questions about a specific button not listed here, please submit a dispute through the Settings menu.";

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
