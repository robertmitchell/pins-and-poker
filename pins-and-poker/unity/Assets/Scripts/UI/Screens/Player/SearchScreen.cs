using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class SearchScreen : UIScreenBase
{
    public AnimatrixButton BackBtn;
    public ScrollRect leaguesScrollRect;
    public Transform contentView;
    public JoinLeaguePrefab joinLeaguePrefab;
    public MyLeaguePrefab myleaguePrefab;
    public SearchResult searchObj;
    [HideInInspector] public bool searchScreenActive;

    private void OnEnable()
    {
        searchScreenActive = true;
        //leaguesScrollRect.horizontalNormalizedPosition = 0;
        Invoke(nameof(GetSearchResult), 0.1f);  
    }

    private void OnDisable()
    {
        foreach (Transform item in contentView)
        {
            Destroy(item.gameObject);
        }
    }

    void Start()
    {
        BackBtn.onClick.AddListener(() => BackBtnClicked());
    }

    void BackBtnClicked()
    {
        searchScreenActive = false;
        UIManager.instance.Hide();
        UIManager.instance.Show<HomeScreen>();
    }

    void GetSearchResult()
    {
        if (searchObj.Leagues.Count != 0)
        {
            //Debug.Log("notification.Leagues.Count" + searchObj.Leagues.Count);
            searchObj.Leagues.ForEach(league =>  
            {
                Request user = league.leagueRequests.Find(x => x.User.PlayerId == PlayerPrefs.GetString(Db_Keys.playerID));
                if (league.leagueRequests.Count <= 0 || user == null)
                {
                    JoinLeaguePrefab GameObj = Instantiate(joinLeaguePrefab, contentView).GetComponent<JoinLeaguePrefab>();
                    GameObj.getLeaguesByUser = league;
                }
                league.leagueRequests.ForEach(request =>
                {
                    if (request.User.PlayerId == PlayerPrefs.GetString(Db_Keys.playerID))
                    {
                        MyLeaguePrefab myLeague = Instantiate(myleaguePrefab, contentView).GetComponent<MyLeaguePrefab>();
                        myLeague.getLeaguesByUser = league;                       
                    }
                });
            });
        }
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
