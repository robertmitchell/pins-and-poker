using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableDataManager : UIScreenBase
{
    public ScoreManager UserdataPrefab;
    public Image _fadeImg;
    public TMP_Text gameStausText;
    public Button startGameBtn;
    public Button backToLeagueBtn;
    public GameObject gameStausPopup;
    public GameObject gameStausPanel;
    public GameObject PointToInstantiate;

    public BowlingScoreCardData bowlingScoreCardData;
    public List<ScoreManager> scoreManagers = new List<ScoreManager>();
    public bool PlayerScoreRowSpawned = false;

    Tweener _tweener;

    void OnEnable()
    {
        if (PointToInstantiate.transform.childCount > 1)
        {
            for (int i = (PointToInstantiate.transform.childCount - 1); i < 0; i++)
            {
                Destroy(PointToInstantiate.transform.GetChild(i));
            }
        }
        APIInvoker.Instance.AddApiRequest(SendRequestToGetScores, 4f);
        //StartCoroutine(RecursiveAPICall());     
    }

    private void OnDisable()
    {
        if (gameStausPopup.activeSelf) DisableGameObject();
        CancelInvoke();
        _tweener.Kill();
    }

    private void Start()
    {
        startGameBtn.onClick.AddListener(() => StartGameBtnClicked());
        backToLeagueBtn.onClick.AddListener(() => BacktoLeagueBtnClicked());
    }

    //public IEnumerator RecursiveAPICall()
    //{
    //    SendRequestToGetScores();
    //    yield return new WaitForSeconds(5);
    //    StartCoroutine(RecursiveAPICall());
    //}

    void BacktoLeagueBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<LeagueScreen>();
        UIManager.instance.GetScreen<LeagueScreen>().ResumeApis();
        DisableGameObject();
    }

    public IEnumerator SendRequestToGetScores()
    {
        Dictionary<string, string> formdata = new()
        {
            { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId) },
            { Db_Keys.gameId, PlayerPrefs.GetString(Db_Keys.gameId) }
        };
        WebServices.Instance.MakeRequest<BowlingScoreCardData>(ApiRoutes.getScores, WebServices.HttpMethod.GET, OnGetScoreSuccess, OnGetScoreFail, null, formdata, null, false);
        yield return null;
    }

    void OnGetScoreSuccess(BowlingScoreCardData respData, long code)
    {
        if (respData.status == Global.Status.started.ToString())
        {
            bowlingScoreCardData.score = respData.score;

            if (gameStausPopup.activeSelf) DisableGameObject();
            if (!PlayerScoreRowSpawned)
            {
                //to restrict recursive api to avoid instantiate everytime
                SpawnRowsOneTime(bowlingScoreCardData);
                PlayerScoreRowSpawned = true;
            }
            UpdateScoreCard(bowlingScoreCardData);
        }
        else if (respData.status == Global.Status.pending.ToString())
        {
            if (UIManager.instance.GetScreen<LeagueScreen>().leagueScreenCanvas.alpha == 1) DisableGameObject();
            else gameStausPopup.SetActive(true);

            if (PlayerPrefs.GetString(Db_Keys.userType) == Global.UserType.user.ToString())
            {
                startGameBtn.gameObject.SetActive(false);
                gameStausText.gameObject.SetActive(true);
            }
            else
            {
                gameStausText.gameObject.SetActive(false);
                startGameBtn.gameObject.SetActive(true);
            }
        }
        else if (respData.status == Global.Status.ended.ToString())   // stop recursive calling
        {
            UIManager.instance.GetScreen<LeaguesNameScreen>().InitializeData(respData);
            Invoke(nameof(GameEnd), 5f);
        }
    }

    void GameEnd()
    {
        MessagePopUpScreen.Instance.ShowMessage("Game End", "Status", "OK", null, true);
        UIManager.instance.Hide();
        UIManager.instance.Show<LeaguesNameScreen>();
    }

    public void SpawnRowsOneTime(BowlingScoreCardData bowlingScoreCardData)
    {
        this.bowlingScoreCardData = bowlingScoreCardData;
        if (bowlingScoreCardData != null && bowlingScoreCardData.score != null)
        {
            foreach (PlayerScoreCardData player in bowlingScoreCardData.score)
            {
                ScoreManager row = Instantiate(UserdataPrefab.gameObject, PointToInstantiate.transform.position, Quaternion.identity, PointToInstantiate.transform).GetComponent<ScoreManager>();
                row.PickColumns();
                //scrollbars.Add(row.scrollbar);
                row.InitializeData(player);
                scoreManagers.Add(row);
                if (player.PlayerId == PlayerPrefs.GetString(Db_Keys.playerID))
                {
                    row.gameObject.transform.SetSiblingIndex(1);
                    row.highlightImg.gameObject.SetActive(true);
                    row.isPlayerRow = true;
                }
            }
        }
    }

    public void UpdateScoreCard(BowlingScoreCardData bowlingScoreCardData)
    {      
        UIManager.instance.GetScreen<MyAssignedCardsScreen>().InitializeData(bowlingScoreCardData);
        this.bowlingScoreCardData = bowlingScoreCardData;
        if (bowlingScoreCardData != null)
        {
            foreach (PlayerScoreCardData player in bowlingScoreCardData.score)
            {
                ScoreManager scoreManager = scoreManagers.Find(x => x.playerScoreCardData.PlayerId == player.PlayerId);

                if (scoreManager != null)
                {
                    scoreManager.SetData(player);
                }
            }
        }
    }

    void StartGameBtnClicked()
    {
        Dictionary<string, string> formdata = new()
        {
            { Db_Keys.status, Global.Status.started.ToString() },
            { Db_Keys.gameId, PlayerPrefs.GetString(Db_Keys.gameId) }
        };
        foreach (KeyValuePair<string, string> field in formdata)
        {
            Debug.Log("field.Key == " + field.Key + " field.Value == " + field.Value);
        }
        WebServices.Instance.MakeRequest<BowlingScoreCardData>(ApiRoutes.manageGame, WebServices.HttpMethod.POST, OnGameStartedSuccess, OnGetScoreFail, null, formdata, null, true);
    }

    void OnGameStartedSuccess(BowlingScoreCardData respData, long code)  // response mn just status aa rha h
    {
        if (respData.status == Global.Status.started.ToString())
        {
            if(gameStausPopup.activeSelf) DisableGameObject();
            if (respData.score != null)
                bowlingScoreCardData.score = respData.score;
            if (!PlayerScoreRowSpawned && bowlingScoreCardData.score != null)
            {
                Debug.Log("Has not Spawned Raw before");
                SpawnRowsOneTime(bowlingScoreCardData);
                PlayerScoreRowSpawned = true;
            }
            UpdateScoreCard(bowlingScoreCardData);
        }
    }

    void OnGetScoreFail(string errorMsg)
    {
        Debug.LogError("Request failed: " + errorMsg);
        MessagePopUpScreen.Instance.ShowMessage(errorMsg, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
    }

    internal void DisableGameObject()
    {
        _tweener = gameStausPanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameStausPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            gameStausPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.GetComponent<CanvasGroup>().DOFade(0f, 0.2f).OnComplete(() =>
                {                 
                    gameStausPopup.SetActive(false);
                });
            });
        });
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
