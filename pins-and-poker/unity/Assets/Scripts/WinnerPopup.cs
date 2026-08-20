using DG.Tweening;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class WinnerPopup : UIScreenBase
{
    [SerializeField] AnimatrixButton closeBtn;
    [SerializeField] Image _fadeImg;
    [SerializeField] RawImage winnerImage;
    [SerializeField] GameObject _winnerPanel;
    [SerializeField] GameObject _winnerTitle;
    [SerializeField] string playerName;
    [SerializeField] TMP_Text handNameTxt;
    [SerializeField] TMP_Text playerNameTxt;

    public Transform content;
    public ScrollRect handsScrollView;
    public Texture imageTexture;
    public CardPrefab winCard_Prefab;
    public PlayerScoreCardData user;

    Tweener _tweener;

    private void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
        if (user.Cards.Count > 0)
        {
            //Debug.Log("Winner Cards here Count  :  " + user.Cards.Count);
            handNameTxt.text = user.PokerHands;
            playerNameTxt.text = user.Username;
            winnerImage.texture = imageTexture;
            int index = 1;
            foreach (var card in user.Cards)
            {
                CardPrefab obj = Instantiate(winCard_Prefab, content).GetComponent<CardPrefab>();
                obj.id = card;
                obj.index = index;
                index++;
            }
            ScrollLeftToFirstElement();
        }
        //Debug.Log("IS WINNER: (Popup)" + user.IsWinner);
        if (user.IsWinner)
        {
            _winnerTitle.transform.GetChild(0).GetComponent<TMP_Text>().text = "Winner";
            _winnerTitle.SetActive(true);
        }
        else
        {
            _winnerTitle.transform.GetChild(0).GetComponent<TMP_Text>().text = "Loser";
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
        _tweener.Kill();
        handsScrollView.horizontalNormalizedPosition = 0;
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
    }

    void Start()
    {
        closeBtn.onClick.AddListener(() => ExitBtnClicked());
    }

    void ScrollLeftToFirstElement(float timeToComplete = 0.3f)
    {
        DOTween.To(() => handsScrollView.horizontalNormalizedPosition,
            val => handsScrollView.horizontalNormalizedPosition = val,
            0f, timeToComplete).OnComplete(() => handsScrollView.verticalNormalizedPosition = 0f);
    }

    void ExitBtnClicked()
    {
        DisableGameObject();
    }
 
    public void DisableGameObject()
    {
        _tweener = _winnerPanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _winnerPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _winnerPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    UIManager.instance.GetScreen<WinnerPopup>().Hide();
                });
            });
        });
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
