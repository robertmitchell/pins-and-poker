using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class HandRankingsScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public TMP_Text contentTxt;
    public ScrollRect contentScrollRect;

    private const string PLACEHOLDER_CONTENT =
        "Poker Hand Rankings\n\n" +
        "Hands are listed from highest to lowest value:\n\n" +
        "1. Royal Flush\n" +
        "   Lorem ipsum — A, K, Q, J, 10 of the same suit.\n\n" +
        "2. Straight Flush\n" +
        "   Lorem ipsum — Five consecutive cards of the same suit.\n\n" +
        "3. Four of a Kind\n" +
        "   Lorem ipsum — Four cards of the same rank.\n\n" +
        "4. Full House\n" +
        "   Lorem ipsum — Three of a kind plus a pair.\n\n" +
        "5. Flush\n" +
        "   Lorem ipsum — Five cards of the same suit, not in sequence.\n\n" +
        "6. Straight\n" +
        "   Lorem ipsum — Five consecutive cards of mixed suits.\n\n" +
        "7. Three of a Kind\n" +
        "   Lorem ipsum — Three cards of the same rank.\n\n" +
        "8. Two Pair\n" +
        "   Lorem ipsum — Two different pairs.\n\n" +
        "9. One Pair\n" +
        "   Lorem ipsum — Two cards of the same rank.\n\n" +
        "10. High Card\n" +
        "   Lorem ipsum — No matching cards; highest card plays.";

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
