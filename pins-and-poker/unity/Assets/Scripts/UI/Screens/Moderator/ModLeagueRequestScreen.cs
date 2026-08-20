using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine.UI;
using UnityEngine;

public class ModLeagueRequestScreen: UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton acceptAllRequestsBtn;
    public AnimatrixButton rejectAllRequestsBtn;
    public GameObject noLeaguesText;
    public Transform leaguesReqContent;
    public ScrollRect leagueInvitationsScrollRect;
    public LeagueRequestPrefab leagueRequestPrefab;

    void OnEnable()
    {
        leagueInvitationsScrollRect.horizontalNormalizedPosition = 0;
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId)}
        };
        WebServices.Instance.MakeRequest<List<LeagueRequest>>(
            ApiRoutes.getLeagueReqModerator,
            WebServices.HttpMethod.GET,
            OnSuccess,
            OnFailure,
            null,
            formData,
            null,
            true
        );
    }

    void OnDisable()
    {
        ButtonsState(false);
        noLeaguesText.SetActive(false);
        foreach (Transform item in leaguesReqContent.transform)
        {
            Destroy(item.gameObject);
        }
    }

    void Start()
    {
        ButtonsState(false);
        backBtn.onClick.AddListener(() => BackBtnClicked());
        acceptAllRequestsBtn.onClick.AddListener(() => AcceptAllRequestsBtnClicked());
        rejectAllRequestsBtn.onClick.AddListener(() => RejectAllRequestsBtnClicked());
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
        acceptAllRequestsBtn.gameObject.SetActive(active);
        rejectAllRequestsBtn.gameObject.SetActive(active);
    }

    void SendData(string status)
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId) },
                { Db_Keys.status, status }
            };
        WebServices.Instance.MakeRequest<ResponseData>(
           ApiRoutes.manageLeagueReqAll,
           WebServices.HttpMethod.POST,
           OnSuccessAll,
           OnFailure,
           null,
           formData,
           null,
           true
           );
    }

    void OnSuccessAll(ResponseData response , long statusCode)
    {
        foreach (Transform item in leaguesReqContent.transform)
        {
            Destroy(item.gameObject);
        }
        ButtonsState(false);
        noLeaguesText.SetActive(true);
    }

    void OnSuccess(List<LeagueRequest> resp, long statusCode)
    {
        noLeaguesText.SetActive(false);
        ButtonsState(resp.Count > 0);
        foreach (Transform item in leaguesReqContent.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (LeagueRequest leagueReq in resp)
        {
            LeagueRequestPrefab leagueRequestObj = Instantiate(leagueRequestPrefab, leaguesReqContent);
            leagueRequestObj.leagueRequest = leagueReq;
        }
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        if (error != "League Requests Not Found.") MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
        if (error == "No pending league requests found.")
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<ModLeagueRequestScreen>();
            return;
        }
        if (leaguesReqContent.childCount <= 0) noLeaguesText.SetActive(true);
        else noLeaguesText.SetActive(false);
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
