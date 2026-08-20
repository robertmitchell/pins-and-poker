using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;

public class LeaguesNameScreen : UIScreenBase
{
    public AnimatrixButton exitBtn;
    public List<PlayerInfo> playerInfo;
    public List<GameObject> cardsPositions;
    public BowlingScoreCardData playersScoreCardData;

    Dictionary<int, int[]> PlayerPositions = new()
    {
        { 2, new int[] { 0, 4 } },
        { 3, new int[] { 0, 3, 5 } },
        { 4, new int[] { 0, 2, 4, 6 } },
        { 5, new int[] { 0, 2, 3, 5, 6 } },
        { 6, new int[] { 0, 2, 3, 4, 5, 6 } },
        { 7, new int[] { 0, 2, 3, 4, 5, 6, 7 } },
        { 8, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 } }
    };

    private void OnEnable()
    {
        APIInvoker.Instance.RemoveApiRequest(UIManager.instance.GetScreen<TableDataManager>().SendRequestToGetScores);
        //StopCoroutine(UIManager.instance.GetScreen<TableDataManager>().RecursiveAPICall());
        SetResultData(playersScoreCardData.score);
    }

    private void OnDisable()
    {
        foreach (var player in playerInfo)
        {
            player.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        exitBtn.onClick.AddListener(() => ExitBtnClicked());
    }

    void ExitBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<LeagueScreen>();
        UIManager.instance.GetScreen<LeagueScreen>().ResumeApis();
    }

    void SetResultData(List<PlayerScoreCardData> users)
    {
        int[] playerList;
        if (users.Count > 8)
            playerList = PlayerPositions[8];
        else
            playerList = PlayerPositions[users.Count];

        //int iteration = 0;
        //foreach (PlayerScoreCardData user in users)
        //{
        //    PlayerInfo playerinfo = playerInfo[playerList[iteration]];
        //    //playerinfo.user = user;
        //    playerinfo.gameObject.SetActive(true);
        //    iteration++;
        //    if (iteration == 8)
        //        return;
        //}

        for (int i = 0; i < users.Count; i++)
        {
            PlayerInfo playerinfo = playerInfo[playerList[i]];
            playerInfo[playerList[i]].user = users[i];
            playerinfo.gameObject.SetActive(true);
        }
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
