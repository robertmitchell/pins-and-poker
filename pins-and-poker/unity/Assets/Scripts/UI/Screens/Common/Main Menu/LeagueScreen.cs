using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;
using static Global;

public class LeagueScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton createGameBtn;
    public AnimatrixButton editLeagueBtn;
    public AnimatrixButton gameRequestsBtn;
    public AnimatrixButton refreshLeagueBtn;
    public AnimatrixButton leagueRequestsBtn;
    public AnimatrixButton leagueParticipantsBtn;
    public Transform gamesContent;
    public ScrollRect gamesScrollRect;
    public GamePrefab gamePrefab;
    public LeagueGamePrefab leagueGamePrefab;
    public TMP_Text noGamesFoundText;
    public TMP_Text leagueRequestsCountText;
    public RawImage leagueBackground;
    public GameObject leagueRequestCountPnl;
    public CanvasGroup leagueScreenCanvas;
    public bool spawnedOnce;

    [Space]
    [Header("LEAGUE INFO")]
    [Space]
    public TMP_Text leagueNameTxt;
    public TMP_Text startTimeTxt;
    public TMP_Text participantsTxt;
    public TMP_Text pointsPoolTxt;
    public GetLeaguesByModerator getLeaguesByModerator = new();
    public GetLeaguesByUser getLeaguesByUser = new();
    public string leagueID;
    public string imageEndpoint;

    public GameObject[] buttonDetails;

    #region MONOBEHAVIOURS
    private void OnEnable()
    {
       
        spawnedOnce = true;
        leagueScreenCanvas.alpha = 1;
        gamesScrollRect.verticalNormalizedPosition = 1;
        APIInvoker.Instance.AddApiRequest(GetGamesWithDelay, 2f);
        //StopCoroutine(UIManager.instance.GetScreen<TableDataManager>().RecursiveAPICall());
        APIInvoker.Instance.RemoveApiRequest(UIManager.instance.GetScreen<TableDataManager>().SendRequestToGetScores);
        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.moderator.ToString())
        {
            ReplaceObjects(true);
            SetScreenDetail(PlayerPrefs.GetString(Db_Keys.leagueId), getLeaguesByModerator.leagueName, getLeaguesByModerator.participants, getLeaguesByModerator.StartTime, getLeaguesByModerator.PrizePool);
            APIInvoker.Instance.AddApiRequest(GetLeagueRequestCount, 5f);
        }
        else
        {
            ReplaceObjects(false);
            SetScreenDetail(getLeaguesByUser.leagueId, getLeaguesByUser.leagueName, getLeaguesByUser.participants, getLeaguesByUser.start_time, getLeaguesByUser.prize_pool);
        }

        UIManager.instance.GetScreen<TableDataManager>().scoreManagers.Clear();
        UIManager.instance.GetScreen<TableDataManager>().bowlingScoreCardData.score.Clear();
        UIManager.instance.GetScreen<TableDataManager>().PlayerScoreRowSpawned = false;        
    }

    private void OnDisable()
    {
        spawnedOnce = false;
        leagueRequestsCountText.text = string.Empty;
        leagueRequestCountPnl.SetActive(false);
        noGamesFoundText.gameObject.SetActive(false);
        APIInvoker.Instance.RemoveApiRequest(GetGamesWithDelay);
        APIInvoker.Instance.RemoveApiRequest(GetLeagueRequestCount);
        UIManager.instance.GetScreen<CreateEditLeagueScreen>().isEdit = false;
        foreach (Transform item in gamesContent)
        {
            Destroy(item.gameObject);
        }
        gamesContent.gameObject.SetActive(false);
        foreach (var button in buttonDetails)
        {
            button.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
        createGameBtn.onClick.AddListener(() => CreateGameBtnClicked());
        editLeagueBtn.onClick.AddListener(() => EditLeagueBtnClicked());
        //gameRequestsBtn.onClick.AddListener(() => GameRequestsBtnClicked());
        refreshLeagueBtn.onClick.AddListener(() => RefreshLeagueBtnClicked());
        leagueRequestsBtn.onClick.AddListener(() => LeagueRequestsBtnClicked());
        leagueParticipantsBtn.onClick.AddListener(() => LeagueParticipantsBtnClicked());

        //Debug.Log(Equals(PlayerPrefs.GetString(Db_Keys.userType), UserType.moderator.ToString()) + " userType :" + PlayerPrefs.GetString(Db_Keys.userType));
        if (ShowTutorial == true && PlayerPrefs.GetString(Db_Keys.userType) == UserType.moderator.ToString())
        {
            TutorialManager.Instance.tutorialPanel.SetActive(true);
            TutorialManager.Instance.NextBtnClicked();
            //PlayerPrefs.SetInt(Db_Keys.isFirstTimeInfo, 1);
            foreach (var button in buttonDetails)
            {
                button.gameObject.SetActive(false);
            }
        }
        else
        {
            /* foreach (var button in buttonDetails)
             {
                 button.gameObject.SetActive(true);
             }*/
            foreach (var button in buttonDetails)
            {
                button.gameObject.SetActive(true);
            }
           /* if (IsFirstTimeOpening())
            {
                foreach (var button in buttonDetails)
                {
                    button.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (var button in buttonDetails)
                {
                    button.gameObject.SetActive(true);
                }
            }*/
        }
       
    }
    #endregion

    #region COMMON METHODS
    void SetScreenDetail(string leagueID, string leagueName, string Participants, string startTime, string pointsPool)
    {
        PlayerPrefs.SetString(Db_Keys.leagueId, leagueID);
        leagueNameTxt.text = "League Name : <color=#FFFFFF>" + leagueName + "</color>";
        pointsPoolTxt.text = "Points Pool: <color=#FFFFFF>" + pointsPool + "</color>";
        participantsTxt.text = "Participants : <color=#FFFFFF>" + Participants + "</color>";
        string time = startTime.Insert(2, ":");
        startTimeTxt.text = "Start Time : <color=#FFFFFF>" + time + "</color>";
    }

    void ReplaceObjects(bool active)
    {
        createGameBtn.gameObject.SetActive(active);
        editLeagueBtn.gameObject.SetActive(active);
        //gameRequestsBtn.gameObject.SetActive(active);
        leagueRequestsBtn.gameObject.SetActive(active);
        leagueParticipantsBtn.interactable = active;
        leagueParticipantsBtn.GetComponent<Image>().enabled = active;
        noGamesFoundText.text = active ? "No Games Found!" : "No Games Available!";
    }

    void OpenObjectsWithDelay()
    {
        gamesContent.gameObject.SetActive(true);
    }

    void ScrollUpToFirstElement(float timeToComplete = 0.3f)
    {
        DOTween.To(() => gamesScrollRect.verticalNormalizedPosition,
            val => gamesScrollRect.verticalNormalizedPosition = val,
            1f, timeToComplete).OnComplete(() => gamesScrollRect.verticalNormalizedPosition = 1f);
    }

    void BackBtnClicked()
    {
        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.user.ToString())
        {
            UIManager.instance.Hide();
            if (UIManager.instance.GetScreen<SearchScreen>().searchScreenActive == true) UIManager.instance.Show<SearchScreen>();
            else UIManager.instance.Show<MyLeaguesScreen>();
        }
        else
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<HomeScreen>();
        }
    }

    private void CreateGameBtnClicked()
    {
        UIManager.instance.Show<CreateGamePopupScreen>();
    }

    private void EditLeagueBtnClicked()
    {
        //UIManager.instance.Hide();
        PauseApis();
        UIManager.instance.GetScreen<CreateEditLeagueScreen>().isEdit = true;
        UIManager.instance.Show<CreateEditLeagueScreen>();
    }

    private void LeagueRequestsBtnClicked()
    {
        //UIManager.instance.Hide();
        PauseApis();
        UIManager.instance.Show<ModLeagueRequestScreen>();
    }

    private void GameRequestsBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<GameRequestScreen>();
    }

    private void RefreshLeagueBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<LeagueScreen>();
    }

    private void LeagueParticipantsBtnClicked()
    {
        //UIManager.instance.Hide();
        PauseApis();
        UIManager.instance.Show<LeagueParticipantsScreen>();
    }

    public void PauseApis()
    {
        leagueScreenCanvas.alpha = 0;
        APIInvoker.Instance.RemoveApiRequest(GetGamesWithDelay);
        APIInvoker.Instance.RemoveApiRequest(GetLeagueRequestCount);
    }

    public void ResumeApis()
    {
        leagueScreenCanvas.alpha = 1;
        APIInvoker.Instance.AddApiRequest(GetGamesWithDelay, 2f);
        APIInvoker.Instance.AddApiRequest(GetLeagueRequestCount, 5f);
    }
    private bool IsFirstTimeOpening()
    {
        return PlayerPrefs.GetInt(Db_Keys.isFirstTimeInfo, 0) == 0;
    }
    #endregion

    #region API'S
    public IEnumerator GetGamesWithDelay()
    {
        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.user.ToString())
        {
            //ReplaceObjects(false);
            //SetScreenDetail(getLeaguesByUser.leagueId, getLeaguesByUser.leagueName, getLeaguesByUser.participants, getLeaguesByUser.start_time, getLeaguesByUser.prize_pool);

            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.leagueId,  PlayerPrefs.GetString(Db_Keys.leagueId) }
            };
            WebServices.Instance.MakeRequest<GetgamesByModerator>(
               ApiRoutes.getUserLeagueGames,
               WebServices.HttpMethod.GET,
               OnUserSuccess,
               OnFailure,
               null,
               formData,
               null,
               spawnedOnce
               );
        }
        else
        {
            //StartCoroutine(GetLeagueRequestCount());
            //Invoke(nameof(GetLeagueRequestCount), 1f);
            //ReplaceObjects(true);
            //SetScreenDetail(PlayerPrefs.GetString(Db_Keys.leagueId), getLeaguesByModerator.leagueName, getLeaguesByModerator.participants, getLeaguesByModerator.StartTime, getLeaguesByModerator.PrizePool);

            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.leagueId,  PlayerPrefs.GetString(Db_Keys.leagueId) }
            };
            WebServices.Instance.MakeRequest<GetgamesByModerator > (
               ApiRoutes.getModeratorLeagueGames,
               WebServices.HttpMethod.GET,
               OnModSuccess,
               OnFailure,
               null,
               formData,
               null,
               spawnedOnce
               );
        }
        yield return null;
    }

    void OnUserSuccess(GetgamesByModerator gamesData, long statusCode)
    {
        if (gamesData.Games.Count <= 0) noGamesFoundText.gameObject.SetActive(true);       
        else noGamesFoundText.gameObject.SetActive(false);

        if (!spawnedOnce)
        {
            participantsTxt.text = "Participants : <color=#EAD188>" + gamesData.LeagueParticipants + "</color>";
            for (int i = 0; i < gamesContent.childCount; i++)
            {
                var leagueGamePrefab = gamesContent.GetChild(i).GetComponent<GamePrefab>();
                if (leagueGamePrefab != null && i < gamesData.Games.Count)
                {
                    leagueGamePrefab.gamesForUser = gamesData.Games[i];
                    leagueGamePrefab.SetGameInfo();
                }
            }
            return;
        }
        gamesContent.gameObject.SetActive(false);
        foreach (Transform item in gamesContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var game in gamesData.Games)
        {
            GamePrefab GameObj = Instantiate(gamePrefab, gamesContent).GetComponent<GamePrefab>();
            GameObj.gamesForUser = game;
        }
        spawnedOnce = false;
        Invoke(nameof(OpenObjectsWithDelay), 0.2f);
        ScrollUpToFirstElement();
    }

    void OnModSuccess(GetgamesByModerator gamesData, long statusCode)
    {
        noGamesFoundText.gameObject.SetActive(false);
        if (!spawnedOnce)
        {
            participantsTxt.text = "Participants : <color=#AB0000>" + gamesData.LeagueParticipants + "</color>";
            for (int i = 0; i < gamesContent.childCount; i++)
            {
                var leagueGamePrefab = gamesContent.GetChild(i).GetComponent<LeagueGamePrefab>();
                if (leagueGamePrefab != null && i < gamesData.Games.Count)
                {
                    leagueGamePrefab.getgamesByModerator = gamesData.Games[i];
                    leagueGamePrefab.SetGameInfo();
                    leagueGamePrefab.SetGamesRequestCount();
                }
            }
            return;
        }
        gamesContent.gameObject.SetActive(false);
        foreach (Transform item in gamesContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var game in gamesData.Games)
        {
            LeagueGamePrefab leagueGameObj = Instantiate(leagueGamePrefab, gamesContent);
            leagueGameObj.getgamesByModerator = game;
        }
        spawnedOnce = false;
        Invoke(nameof(OpenObjectsWithDelay), 0.2f);
        ScrollUpToFirstElement();
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        spawnedOnce = false;
        if (gamesContent.childCount <= 0) noGamesFoundText.gameObject.SetActive(true);
        else noGamesFoundText.gameObject.SetActive(false);
    }

    public IEnumerator GetLeagueRequestCount()
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId)}
        };
        WebServices.Instance.MakeRequest<List<LeagueRequest>>(
            ApiRoutes.getLeagueReqModerator,
            WebServices.HttpMethod.GET,
            Success,
            OnFail,
            null,
            formData,
            null,
            false
        );
        yield return null;
    }

    void Success(List<LeagueRequest> resp, long statusCode)
    {
        if (resp.Count > 0)
        {
            leagueRequestsCountText.text = resp.Count.ToString();
            leagueRequestCountPnl.SetActive(true);
        }
        else
        {
            leagueRequestsCountText.text = string.Empty;
            leagueRequestCountPnl.SetActive(false);
        }
    }

    void OnFail(string error)
    {
        Debug.LogError("Request failed: " + error);
        leagueRequestsCountText.text = string.Empty;
        leagueRequestCountPnl.SetActive(false);
    }
    #endregion

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
