using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class LeagueRequestPrefab : MonoBehaviour
{
    public RawImage profileImg;
    public TMP_Text playerNameTxt;
    public AnimatrixButton AcceptBtn;
    public AnimatrixButton RejectBtn;
    public LeagueRequest leagueRequest;

    void Start()
    {
        AcceptBtn.onClick.AddListener(() => AcceptBtnClicked());
        RejectBtn.onClick.AddListener(() => RejectBtnClicked());
        playerNameTxt.text = leagueRequest.userLeagueRequest.Username;
        StartCoroutine(ImageCacheManager.Instance.DownloadMultipleImage(ApiRoutes.imageStartingPointURL + leagueRequest.userLeagueRequest.Image, SetImage));
    }

    void SetImage(Texture2D texture)
    {
        profileImg.texture = texture;
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

    void SendData(string status)
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId) },
                { Db_Keys.playerID, leagueRequest.userLeagueRequest.PlayerId},
                { Db_Keys.status, status }
            };
        WebServices.Instance.MakeRequest<ResponseData>(
           ApiRoutes.manageLeagueReq,
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
        UIManager.instance.Show<ModLeagueRequestScreen>();
    }

    void OnFailure(string error)
    {
        //if(error == "League request not found") Destroy(gameObject);
        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
        if (error == "League request not found")
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<ModLeagueRequestScreen>();
        }
    }
}
