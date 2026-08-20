using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Global;

public class MyLeaguePrefab : MonoBehaviour
{
    public TMP_Text leagueNameTxt;
    public TMP_Text participantsTxt;
    public TMP_Text prizePoolTxt;
    public TMP_Text startTimeTxt;
    public TMP_Text startDateTxt;
    public TMP_Text statusTxt;
    public RawImage leagueImage;
    public AnimatrixButton openOrCancelLeagueBtn;

    public string leagueid;
    public string moderatorId;
    public List<Rule> rules = new();
    public GetLeaguesByUser getLeaguesByUser;
    string status;

    void Start()
    {
        openOrCancelLeagueBtn.onClick.AddListener(() => OpenOrCancelLeagueBtnClicked());
        SetData();
    }

    void OpenOrCancelLeagueBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        if (status == Status.pending.ToString())
        {
            CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.shocking);
            ConfirmationPopupScreen.Instance.ShowConfirmationMessage("Are you sure you want to cancel\r\nyour request?", "Cancel Request", null, null, CancelRequest, 2);
        }
        else
        {
            PlayerPrefs.SetString(Db_Keys.leagueId, leagueid);
            UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByUser = getLeaguesByUser;
            UIManager.instance.GetScreen<LeagueScreen>().leagueBackground.texture = leagueImage.texture;
            if (UIManager.instance.GetScreen<MyLeaguesScreen>().gameObject.activeSelf) UIManager.instance.GetScreen<MyLeaguesScreen>().Hide();
            else UIManager.instance.GetScreen<SearchScreen>().Hide();
            UIManager.instance.Show<LeagueScreen>();
        }
    }

    void SetData()
    {
        leagueNameTxt.text = getLeaguesByUser.leagueName;
        participantsTxt.text = "Participants: " + getLeaguesByUser.participants;
        prizePoolTxt.text = "Points Pool: " + getLeaguesByUser.prize_pool;
        startTimeTxt.text = "Start Time: " + getLeaguesByUser.start_time.Insert(2, ":");
        if (startDateTxt) startDateTxt.text = "Start Date: " + (getLeaguesByUser.created_at?.Split('T')[0] ?? "N/A"); ;
        var matchingItem = getLeaguesByUser.leagueRequests.Find(item => item.User.PlayerId == PlayerPrefs.GetString(Db_Keys.playerID));
        if (matchingItem != null) statusTxt.text = "Status: " + matchingItem.Status.FirstCharacterToUpper();
            
        leagueid = getLeaguesByUser.leagueId;
        moderatorId = getLeaguesByUser.moderator_Id;
        rules = getLeaguesByUser.rules;
        PlayerPrefs.SetString(Db_Keys.moderatorId,moderatorId);
        DownloadImage();
        if (getLeaguesByUser.leagueRequests.Count < 1)
            return;

        //status = getLeaguesByUser.leagueRequests[0].Status;
        getLeaguesByUser.leagueRequests.ForEach(status =>
        {
            this.status = status.Status;
            Debug.Log(status);
            if (this.status == Status.pending.ToString())
            {
                openOrCancelLeagueBtn.GetComponentInChildren<TMP_Text>().text = "Cancel Request";
            }
            else
            {
                openOrCancelLeagueBtn.GetComponentInChildren<TMP_Text>().text = "Open";
            }
        });
    }

    void DownloadImage()
    {
        HomeScreen hm = UIManager.instance.GetScreen<HomeScreen>();
        if (hm.leagueImageCache.ContainsKey(getLeaguesByUser.image))
            SetImage(hm.leagueImageCache[getLeaguesByUser.image]);
        else
            StartCoroutine(ImageCacheManager.Instance.DownloadMultipleImage(ApiRoutes.imageStartingPointURL + getLeaguesByUser.image, OnImageDownload));
    }

    void OnImageDownload(Texture2D texture)
    {
        UIManager.instance.GetScreen<HomeScreen>().leagueImageCache.Add(getLeaguesByUser.image, texture);
        SetImage(texture);
    }

    void SetImage(Texture2D texture)
    {
        leagueImage.texture = texture;
    }

    void CancelRequest()
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.leagueId, leagueid }
            };
        WebServices.Instance.MakeRequest<ResponseData>(
           ApiRoutes.cancelLeague,
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
        Debug.Log("League Request SuccessFully Canceled : " + response);
        Destroy(gameObject);
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        if (error != "No league records found.") MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
        if (error == "Sorry, we couldn't find your request in the league." || error == "You cannot cancel your request as it has already been accepted into the league.")
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<MyLeaguesScreen>();
            return;
        }
    }
}
