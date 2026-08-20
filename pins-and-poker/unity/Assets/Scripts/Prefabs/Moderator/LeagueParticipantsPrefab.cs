using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class LeagueParticipantsPrefab : MonoBehaviour
{
    public RawImage profileImg;
    public TMP_Text playerNameTxt;
    public AnimatrixButton removeBtn;
    public User userdata;

    void Start()
    {
        playerNameTxt.text = userdata.Username;
        removeBtn.onClick.AddListener(() => RemoveBtnClicked());
        StartCoroutine(ImageCacheManager.Instance.DownloadMultipleImage(ApiRoutes.imageStartingPointURL + userdata.Image, SetImage));
    }

    void SetImage(Texture2D texture)
    {
        profileImg.texture = texture;
    }

    void RemoveBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId)  },
            { Db_Keys.playerID, userdata.PlayerId },
            { Db_Keys.removefrom, "league" }   // league   ||   game
        };
        WebServices.Instance.MakeRequest<ResponseData>(ApiRoutes.removeParticipant, WebServices.HttpMethod.POST, OnSuccess, OnFail, null, formData, null, true);
    }

    void OnSuccess(ResponseData response, long arg2)
    {
        Debug.Log("OnSuccess Invoked");
        Destroy(gameObject);        
    }

    void OnFail(string obj)
    {
        MessagePopUpScreen.Instance.ShowMessage(obj, "Response", "OK", null, true);
        Debug.LogError("error: " + obj);
    }
}
