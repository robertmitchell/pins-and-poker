using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class MyAssignedCardsScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public Button leagueBtn;
    public Button cardExchangeBtn;
    public string cardExchangeID;
    public string cardExchangeIndex;
    public GameObject cardImagePopup;
    public GameObject myCardsExchange;
    public GameObject showCardExchangePanel;
    public CardPrefab cardPrefab;
    public CardPrefab SelectedcardPrefab;
    public Transform content;
    public BowlingScoreCardData playersScoreCardData;

    private void OnEnable()
    {
        TableGraphScreen.canShowPopup = false;
        foreach (var item in playersScoreCardData.score)
        {
            if (item.PlayerId == PlayerPrefs.GetString(Db_Keys.playerID))
            {
                if (item.ExchangeCards)
                {
                    cardExchangeBtn.interactable = true; //Show message Card Exchange kr skty ho                   
                }
                else
                {
                    cardExchangeBtn.interactable = false;
                }
                if (item.Cards.Count > 0)
                {
                    int index = 1; 

                    foreach (var card in item.Cards)
                    {
                        CardPrefab obj = Instantiate(cardPrefab, content).GetComponent<CardPrefab>();
                        obj.id = card;
                        obj.index = index; 
                        index++;           
                    }
                }

            }
        }
    }

    void OnDisable()
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
        cardExchangeID = "";
        cardExchangeIndex = "";
        TableGraphScreen.canShowPopup = true;
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
        leagueBtn.onClick.AddListener(() => LeagueBtnClicked());
        cardExchangeBtn.onClick.AddListener(() => CardExchangeBtnClicked());
    }

    void BackBtnClicked()
    {
        UIManager.instance.GetScreen<MyAssignedCardsScreen>().Hide();
    }

    void LeagueBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.GetScreen<MyAssignedCardsScreen>().Hide();
        UIManager.instance.Show<LeaguesNameScreen>();
    }

    void CardExchangeBtnClicked()
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
         {
            { Db_Keys.cardIndex,(int.Parse(cardExchangeIndex)-1 ).ToString()},
            { Db_Keys.gameId,PlayerPrefs.GetString(Db_Keys.gameId) }
         };
                
        WaitingLoaderCanvas.Instance.ShowCardsLoader();
        WebServices.Instance.MakeRequest<ExchangedCards>(ApiRoutes.cardExchange, WebServices.HttpMethod.POST, OnSuccess, OnFail, null, formData, null, false);
    }

    private void OnSuccess(ExchangedCards cards, long arg2)
    {  
        cardExchangeBtn.interactable = false;
        foreach (Transform item in content.transform)
        {
            Destroy(item.gameObject);
        }
       
        int index = 1;
        foreach (var item in cards.Cards)
        {
            CardPrefab card = Instantiate(cardPrefab, content).GetComponent<CardPrefab>();
            card.id = item;
            card.index = index;
            index++;
        }
        TableGraphScreen.canShowPopup = true;
        WaitingLoaderCanvas.Instance.HideCardsLoader();
        MessagePopUpScreen.Instance.ShowMessage("Card Exchanged Successfully", "Response", "OK", null, true);
        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.happyThumbsUp);
    }

    private void OnFail(string obj)
    {
        Debug.LogError("error: " + obj);
        WaitingLoaderCanvas.Instance.HideCardsLoader();
        MessagePopUpScreen.Instance.ShowMessage(obj, "Response", "OK", null, true);
    }

    public void InitializeData(BowlingScoreCardData player)
    {
        this.playersScoreCardData = player;
    }
 
    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
