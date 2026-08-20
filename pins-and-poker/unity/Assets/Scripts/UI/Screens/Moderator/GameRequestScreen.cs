using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class GameRequestScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton acceptAllRequestBtn;
    public AnimatrixButton rejectAllRequestBtn;
    public GameObject noGamesText;
    public Transform gamesContent;
    public ScrollRect gameRequestsScrollRect;
    public GameRequestPrefab gameRequestPrefab;
    public string lane;

    private void OnEnable()
    {
        gameRequestsScrollRect.verticalNormalizedPosition = 1;
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId)  },
            { Db_Keys.gameId, PlayerPrefs.GetString(Db_Keys.gameId)  }
        };
        WebServices.Instance.MakeRequest<List<Request>>(ApiRoutes.getGameRequests, WebServices.HttpMethod.GET, OnSuccess, OnFail, null, formData, null, true);
    }

    private void OnDisable()
    {
        ButtonsState(false);
        noGamesText.SetActive(false);
        foreach (Transform item in gamesContent)
        {
            Destroy(item.gameObject);
        }
    }

    void Start()
    {
        ButtonsState(false);
        backBtn.onClick.AddListener(() => BackBtnClicked());
        acceptAllRequestBtn.onClick.AddListener(() => AcceptAllRequestsBtnClicked());
        rejectAllRequestBtn.onClick.AddListener(() => RejectAllRequestsBtnClicked());
    }

    void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<LeagueScreen>();
        UIManager.instance.GetScreen<LeagueScreen>().ResumeApis();
    }

    void AcceptAllRequestsBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        SendData(Global.Status.accepted.ToString());
    }

    void RejectAllRequestsBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        SendData(Global.Status.rejected.ToString());
    }

    void ButtonsState(bool active)
    {
        acceptAllRequestBtn.gameObject.SetActive(active);
        rejectAllRequestBtn.gameObject.SetActive(active);
    }

    void SendData(string status)
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId) },
                { Db_Keys.gameId, PlayerPrefs.GetString(Db_Keys.gameId) },
                { Db_Keys.status, status }
            };
        WebServices.Instance.MakeRequest<ResponseData>(
           ApiRoutes.manageAllGameReq,
           WebServices.HttpMethod.POST,
           OnSuccessAll,
           OnFail,
           null,
           formData,
           null,
           true
           );
    }

    void OnSuccessAll(ResponseData response, long statusCode)
    {
        foreach (Transform item in gamesContent)
        {
            Destroy(item.gameObject);
        }
        ButtonsState(false);
        noGamesText.SetActive(true);
    }

    private void OnSuccess(List<Request> requests, long arg2)
    {
        noGamesText.SetActive(false);
        ButtonsState(requests.Count > 0);
        foreach (Transform item in gamesContent)
        {
            Destroy(item.gameObject);
        }      
        foreach (var request in requests)
        {          
            GameRequestPrefab gameRequestObj = Instantiate(gameRequestPrefab, gamesContent);
            gameRequestObj.request = request;
            gameRequestObj.lane = lane;
        }
    }

    private void OnFail(string error)
    {
        Debug.LogError("Request failed: " + error);
        if (error != "Game Requests not found") MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
        if (error == "No pending game requests found.")
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<GameRequestScreen>();
            return;
        }
        if (gamesContent.childCount <= 0) noGamesText.SetActive(true);
        else noGamesText.SetActive(false);
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
