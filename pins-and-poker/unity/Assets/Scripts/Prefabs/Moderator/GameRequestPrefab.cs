using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;

public class GameRequestPrefab : MonoBehaviour
{
    public TMP_Text participantNameTxt;
    public TMP_Text laneTxt;
    public AnimatrixButton AcceptBtn;
    public AnimatrixButton RejectBtn;
    public Request request;
    public string lane;

    void Start()
    {
        AcceptBtn.onClick.AddListener(() => AcceptBtnClicked());
        RejectBtn.onClick.AddListener(() => RejectBtnClicked());
        participantNameTxt.text = "<color=#EAD188>  " + request.User.Username + "</color>";
        laneTxt.text ="Lane : <color=#EAD188> " + lane + "</color>";
    }

    void AcceptBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        SendData(Global.Status.accepted.ToString());
    }

    void RejectBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        SendData(Global.Status.declined.ToString());
    }

    void SendData(string _status)
    {
        Dictionary<string, string> formData = new()
        {
            { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId.ToString()) },
            { Db_Keys.gameId, PlayerPrefs.GetString(Db_Keys.gameId.ToString()) },
            { Db_Keys.playerID, request.User.PlayerId },
            { Db_Keys.assignedLane, lane },
            { Db_Keys.status, _status }
        };
       
        WebServices.Instance.MakeRequest<ResponseData>(
           ApiRoutes.manageGameReq,
           WebServices.HttpMethod.POST,
           OnSuccess,
           OnFailure,
           null,
           formData,
           null,
           true
           );
    }

    void OnSuccess(ResponseData response, long statusCode)
    {
        Destroy(gameObject);
        UIManager.instance.Hide();
        UIManager.instance.Show<GameRequestScreen>();
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
        if (error == "Game request not found")
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<GameRequestScreen>();
        }
    }
}
