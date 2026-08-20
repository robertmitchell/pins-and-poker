using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class CardPrefab : MonoBehaviour
{
    [HideInInspector]
    AnimatrixButton cardButton;
    public Image cardImage;
    public int id;
    public int index;
    
    void Start()
    {
        cardImage = GetComponent<Image>();
        cardButton = GetComponent<AnimatrixButton>();
        if (cardButton!=null)
        {
             cardButton.onClick.AddListener(CardSelected);
        }
        SetData();
    }

    private void SetData()
    {
        cardImage.sprite = UIManager.instance.GetScreen<TableGraphScreen>().cardsSprites[id - 1];
    }

    void CardSelected()
    {
       // PlayerPrefs.SetString(Db_Keys.cardIdSaved, id.ToString());
        BGMusic.Instance.btn_audioSource.Play();
       UIManager.instance.GetScreen<MyAssignedCardsScreen>().SelectedcardPrefab = this;

        foreach (Transform item in UIManager.instance.GetScreen<MyAssignedCardsScreen>().content)
        {
            item.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        UIManager.instance.GetScreen<MyAssignedCardsScreen>().SelectedcardPrefab.GetComponent<Image>().color = new Color32(255, 90, 90, 255);
        UIManager.instance.GetScreen<MyAssignedCardsScreen>().cardExchangeID = id.ToString();
       UIManager.instance.GetScreen<MyAssignedCardsScreen>().cardExchangeIndex = index.ToString();
    }


}
