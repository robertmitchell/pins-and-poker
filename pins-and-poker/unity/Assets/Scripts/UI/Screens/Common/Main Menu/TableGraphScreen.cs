using UIAnimatrix;
using static Global;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TableGraphScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton volumeBtn;
    public AnimatrixButton changeCardBtn;
    public AnimatrixButton exitBtn;
    public AnimatrixButton disputeBtn;
    public ScrollRect tableGraphScrollRect;
    public List<Sprite> cardsSprites = new List<Sprite>();
    public TableDataManager tableDataManager;
    public static bool canShowPopup = true;

    private void OnEnable()
    {
        Transform parent = UIManager.instance.GetScreen<TableDataManager>().PointToInstantiate.transform;
        for (int i = parent.childCount - 1; i > 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
        canShowPopup = true;
        tableGraphScrollRect.verticalNormalizedPosition = 1;
        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.user.ToString()) ReplaceObjects(true);
        else ReplaceObjects(false);
    }

    private void OnDisable()
    {
        //StopCoroutine(UIManager.instance.GetScreen<TableDataManager>().RecursiveAPICall());
        APIInvoker.Instance.RemoveApiRequest(UIManager.instance.GetScreen<TableDataManager>().SendRequestToGetScores);
        UIManager.instance.GetScreen<TableDataManager>().PlayerScoreRowSpawned = false;
        UIManager.instance.GetScreen<TableDataManager>().scoreManagers.Clear();
        UIManager.instance.GetScreen<TableDataManager>().bowlingScoreCardData.score.Clear();   
        Transform parent = UIManager.instance.GetScreen<TableDataManager>().PointToInstantiate.transform;
        for (int i = parent.childCount - 1; i > 0; i--) 
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBTnClicked());
        //volumeBtn.onClick.AddListener(() => VolumeBtnClicked());
        changeCardBtn.onClick.AddListener(() => ExchangeCardBtnClicked());
        disputeBtn.onClick.AddListener(() => DisputeBtnClicked());
        //exitBtn.onClick.AddListener(() => ExitBtnClicked());
    }

    void BackBTnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<LeagueScreen>();
        UIManager.instance.GetScreen<LeagueScreen>().ResumeApis();
    }

    void DisputeBtnClicked()
    {
        //UIManager.instance.Hide();
        UIManager.instance.Show<DisputeScreen>();
    }

    void VolumeBtnClicked()
    {
        AudioListener.volume = AudioListener.volume > 0 ? 0 : 1;
    }

    void ExchangeCardBtnClicked()
    {
        UIManager.instance.GetScreen<MyAssignedCardsScreen>().Show();
        //UIManager.instance.GetScreen<MyAssignedCardsScreen>().backBtn.gameObject.SetActive(true);
    }

    void ExitBtnClicked()
    {
        ConfirmationPopupScreen.Instance.ShowConfirmationMessage("Are you sure you want to exit?", "Exit", null, null, ChangeScreen);
    }

    void ChangeScreen()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<MyLeaguesScreen>();
    }

    void ReplaceObjects(bool active)
    {
        //volumeBtn.gameObject.SetActive(active);
        changeCardBtn.gameObject.SetActive(active);
        //exitBtn.gameObject.SetActive(active);
        //backBtn.gameObject.SetActive(active);     
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
