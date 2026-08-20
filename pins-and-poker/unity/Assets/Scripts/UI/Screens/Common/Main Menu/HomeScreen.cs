using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;
using static Global;

public class HomeScreen : UIScreenBase
{
    public TMPro.TMP_Text noLeaguesText;
    public TMPro.TMP_Text notificaitonsCountText;
    public AnimatrixButton searchBtn;
    public AnimatrixButton settingsBtn;
    public AnimatrixButton notificationsBtn;
    public AnimatrixButton leagueRequestsBtn;
    public AnimatrixButton createLeagueBtn;
    public AnimatrixButton disputesBtn;
    public AnimatrixButton refreshBtn;
    public Transform leaguesContent;
    public GameObject leaguesScrollViewShadows;
    public ScrollRect leaguesScrollView;
    public GameObject notificationsCountPnl;
    public JoinLeaguePrefab joinLeaguePrefab;
    public ModeratorLeaguePrefab modLeaguePrefab;
    public Dictionary<string, Texture2D> leagueImageCache = new();
    public bool spawnedOnce;
    public GameObject[] buttonDetails;
    public TMPro.TMP_Text titleText;
    public AnimatrixButton howItWorksBtn;
    public AnimatrixButton capabilityListBtn;
    public AnimatrixButton handRankingsBtn;
    public AnimatrixButton howToCreateGameBtn;
    public AnimatrixButton buttonGuideBtn;

    #region MONOBEHAVIOURS
    private void OnEnable()
    {
       
        spawnedOnce = true;
        leaguesScrollView.horizontalNormalizedPosition = 0f;
        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.user.ToString()) ReplaceObjects(true);
        else ReplaceObjects(false);

        StopAllCoroutines();
        APIInvoker.Instance.AddApiRequest(GetLeagues, 2f);
        APIInvoker.Instance.AddApiRequest(GetNotifications, 10f);
    }

    private void OnDisable()
    {
        spawnedOnce = false;
        StopAllCoroutines();
        APIInvoker.Instance.RemoveApiRequest(GetLeagues);
        APIInvoker.Instance.RemoveApiRequest(GetNotifications);
        noLeaguesText.gameObject.SetActive(false);
        leaguesScrollViewShadows.SetActive(false);
        leaguesContent.GetComponent<HorizontalLayoutGroup>().reverseArrangement = false;
        foreach (Transform item in leaguesContent.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (var button in buttonDetails)
        {
            button.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        searchBtn.onClick.AddListener(() => SearchBtnClicked());
        settingsBtn.onClick.AddListener(() => SettingsBtnClicked());
        notificationsBtn.onClick.AddListener(() => NotificationBtnClicked());
        createLeagueBtn.onClick.AddListener(() => CreateLeagueBtnClicked());
        leagueRequestsBtn.onClick.AddListener(() => MyLeagueRequestsBtnClicked());
        disputesBtn.onClick.AddListener(() => DisputesBtnClicked());
        refreshBtn.onClick.AddListener(() => RefreshBtnClicked());
        if (howItWorksBtn) howItWorksBtn.onClick.AddListener(() => { UIManager.instance.Hide(); UIManager.instance.Show<HowItWorksScreen>(); });
        if (capabilityListBtn) capabilityListBtn.onClick.AddListener(() => { UIManager.instance.Hide(); UIManager.instance.Show<CapabilityListScreen>(); });
        if (handRankingsBtn) handRankingsBtn.onClick.AddListener(() => { UIManager.instance.Hide(); UIManager.instance.Show<HandRankingsScreen>(); });
        if (howToCreateGameBtn) howToCreateGameBtn.onClick.AddListener(() => { UIManager.instance.Hide(); UIManager.instance.Show<HowToCreateGameScreen>(); });
        if (buttonGuideBtn) buttonGuideBtn.onClick.AddListener(() => { UIManager.instance.Hide(); UIManager.instance.Show<ButtonGuideScreen>(); });
        if (titleText) titleText.text = "Pins & Poker";
        if (IsFirstTimeOpening())
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
        }
    }
    #endregion

    #region PLAYER
    void SearchBtnClicked()
    {
        //UIManager.instance.Hide();
        UIManager.instance.GetScreen<SearchPopup>().Show();
    }

    void MyLeagueRequestsBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<MyLeaguesScreen>();
    }
    #endregion

    #region MODERATOR
    void CreateLeagueBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<CreateEditLeagueScreen>();
    }
    void RefreshBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<HomeScreen>();
        refreshBtn.interactable = false;
    }

    private void DisputesBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<DisputeScreen>();
    }
    #endregion

    #region COMMON METHODS
    private bool IsFirstTimeOpening()
    {
        return PlayerPrefs.GetInt(Db_Keys.isFirstTime, 0) == 0;
    }
    void SettingsBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<SettingScreen>();
    }

    void NotificationBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<NotificationScreen>();
    }

    void ReplaceObjects(bool active)
    {
        searchBtn.gameObject.SetActive(active);
        leagueRequestsBtn.gameObject.SetActive(active);
        refreshBtn.gameObject.SetActive(active);
        createLeagueBtn.gameObject.SetActive(!active);
        noLeaguesText.text = active ? "No Leagues Available!" : "No Leagues Found!";
    }
    #endregion

    #region API'S
    IEnumerator GetLeagues()
    {
        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.user.ToString())
        {
            leaguesScrollViewShadows.SetActive(false);
            WebServices.Instance.MakeRequest<List<GetLeaguesByUser>>(ApiRoutes.getLeaguesForUser, WebServices.HttpMethod.GET, UserOnSuccess, OnFail, null, null, null, true);
            APIInvoker.Instance.RemoveApiRequest(GetLeagues);
        }
        else
        {
            PlayerPrefs.SetString(Db_Keys.moderatorId, PlayerPrefs.GetString(Db_Keys.playerID));
            WebServices.Instance.MakeRequest<List<GetLeaguesByModerator>>(ApiRoutes.getLeaguesForModerator, WebServices.HttpMethod.GET, ModOnSuccess, OnFail, null, null, null, spawnedOnce);
        }
        yield return null;
    }

    private void UserOnSuccess(List<GetLeaguesByUser> leagues, long arg2)
    {
       
        if (noLeaguesText.gameObject.activeSelf) noLeaguesText.gameObject.SetActive(false);
        foreach (Transform item in leaguesContent.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (var league in leagues)
        {
            JoinLeaguePrefab joinLeagueObj = Instantiate(joinLeaguePrefab.gameObject, leaguesContent).GetComponent<JoinLeaguePrefab>();
            joinLeagueObj.getLeaguesByUser = league;
            joinLeagueObj.gameRulesList = league.rules;
        }
        refreshBtn.interactable = true;
        leaguesScrollViewShadows.SetActive(true);
    }

    private void ModOnSuccess(List<GetLeaguesByModerator> leagues, long arg2)
    {
       
        if (noLeaguesText.gameObject.activeSelf) noLeaguesText.gameObject.SetActive(false);
        if (!spawnedOnce)
        {
            for (int i = 0; i < leaguesContent.childCount; i++)
            {
                var leaguePrefab = leaguesContent.GetChild(i).GetComponent<ModeratorLeaguePrefab>();
                if (leaguePrefab != null && i < leagues.Count)
                {
                    //leaguePrefab.leaguesAndGamesCountTxt.text = leagues[i].leagueInfo;
                    leaguePrefab.getLeaguesByModerator = leagues[i];
                    leaguePrefab.SetLeaguesAndGamesRequestCount();
                }
            }
            return;
        }
        foreach (Transform item in leaguesContent)
        {
            Destroy(item.gameObject);
        }
        StartCoroutine(InstantiateModLeagues(leagues)); //Giving a slight delay while instantiating the moderator league prefabs
    }

    IEnumerator InstantiateModLeagues(List<GetLeaguesByModerator> leagues)
    {
        foreach (var league in leagues)
        {
            ModeratorLeaguePrefab modLeagueObj = Instantiate(modLeaguePrefab.gameObject, leaguesContent).GetComponent<ModeratorLeaguePrefab>();
            modLeagueObj.getLeaguesByModerator = league;
            yield return new WaitForSeconds(0f);
        }
        spawnedOnce = false;
        leaguesContent.GetComponent<HorizontalLayoutGroup>().reverseArrangement = true;
        leaguesScrollViewShadows.SetActive(true);
    }

    private void OnFail(string error)
    {
       
        Debug.LogError("Request failed: " + error);
        refreshBtn.interactable = true;
        spawnedOnce = false;
        if (leaguesContent.childCount <= 0) noLeaguesText.gameObject.SetActive(true);
        else noLeaguesText.gameObject.SetActive(false);
    }

    IEnumerator GetNotifications()
    {
        WebServices.Instance.MakeRequest<List<NotificationData>>(
           ApiRoutes.getNotifications,
           WebServices.HttpMethod.GET,
           NotificationsOnSuccess,
           NotificationsOnFail,
           null,
           null,
           null,
           false
           );
        yield return null;
    }

    void NotificationsOnSuccess(List<NotificationData> resp, long arg2)
    {
        List<NotificationData> tempList = resp.Where(data => data.IsRead == "0").ToList();

        if (tempList.Count > 0)
        {
            notificaitonsCountText.text = tempList.Count.ToString();
            notificationsCountPnl.SetActive(true);
        }
        else
        {
            notificationsCountPnl.SetActive(false);
        }
    }

    void NotificationsOnFail(string error)
    {
        Debug.LogError("Request failed: " + error);
    }
    #endregion

    public override void UpdateScreen<T>(T data)
    {
        throw new NotImplementedException();
    }
}