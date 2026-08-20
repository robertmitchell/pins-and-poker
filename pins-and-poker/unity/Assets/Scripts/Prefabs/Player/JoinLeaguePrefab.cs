using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class JoinLeaguePrefab : Singleton<JoinLeaguePrefab>
{
    public TMP_Text leagueNameTxt;
    public TMP_Text participantsTxt;
    public TMP_Text prizePoolTxt;
    public TMP_Text startTimeTxt;
    public RawImage leagueImage;
    public AnimatrixButton joinLeagueBtn;

    public string leagueid;
    public string moderatorId;
    public List<Rule> gameRulesList;
    public GetLeaguesByUser getLeaguesByUser = new();
    public bool isDestroyable = true;

    void Start()
    {
        joinLeagueBtn.onClick.AddListener(() => JoinLeagueBtnClicked());
        SetData();
    }

    void JoinLeagueBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        UIManager.instance.GetScreen<LeagueScreen>().leagueBackground.texture = leagueImage.texture;
        GameRulesPopupScreen.Instance.ShowGameRulesPnl(SendLeagueRequest, gameRulesList);
    }

    void SendLeagueRequest()
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.leagueId, leagueid  }
        };
        WebServices.Instance.MakeRequest<ResponseData>(ApiRoutes.joinLeague, WebServices.HttpMethod.POST, OnSuccess, OnFail, null, formData, null, true);
    }

    void OnSuccess(ResponseData resp, long arg2)
    {
        Debug.Log("Message : " + resp);
        UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByUser = getLeaguesByUser;
        ShowPanel();
    }

    void OnFail(string error)
    {
        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "Response", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
    }

    void ShowPanel()
    {
        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.thumbsBall);
        MessagePopUpScreen.Instance.ShowMessage("Request to Join league has\r\nbeen successfully sent. Please wait\r\nfor moderator approval.",
                                                "", "OK", ChangeScreen, true);

    }

    void ChangeScreen()
    {
        if (UIManager.instance.GetScreen<SearchScreen>().gameObject.activeSelf)
            UIManager.instance.GetScreen<SearchScreen>().Hide();
        else
            UIManager.instance.GetScreen<HomeScreen>().Hide();

        //joinLeagueBtn.GetComponentInChildren<TMP_Text>().text = "Joined";
        joinLeagueBtn.interactable = false;
        UIManager.instance.Show<MyLeaguesScreen>();
    }

    /*    void SearchResultSetData()
        {
            leagueNameTxt.text = getLeaguesByUser.leagueName.ToString();
            startTimeTxt.text = "Start Time: " + getLeaguesByUser.start_time.ToString();
            leagueid = getLeaguesByUser.leagueId;
            moderatorId = getLeaguesByUser.moderator_Id;
            PlayerPrefs.SetString(Db_Keys.moderatorId, moderatorId);

            gameRulesList = getLeaguesByUser.rules;
            Debug.Log("Image Url : : : " + imageStartingPointURL + getLeaguesByUser.image);
            StartCoroutine(ImageCacheManager.Instance.DownloadMultipleImage(imageStartingPointURL + getLeaguesByUser.image, SetImage));
            if (getLeaguesByUser.leagueRequests != null && getLeaguesByUser.leagueRequests.Count != 0)
            {
                if (getLeaguesByUser.leagueRequests[0].Status == Global.Status.pending.ToString())
                {
                    joinLeagueBtn.GetComponentInChildren<TMP_Text>().text = "Request Pending";
                    joinLeagueBtn.interactable = false;
                }
                else if (getLeaguesByUser.leagueRequests[0].Status == Global.Status.accepted.ToString())
                {
                    gameObject.SetActive(false);
                }
            }
        }
    */

    void SetData()
    {
        leagueNameTxt.text = getLeaguesByUser.leagueName;
        participantsTxt.text = "Participants: " + getLeaguesByUser.participants;
        prizePoolTxt.text = "Points Pool: " + getLeaguesByUser.prize_pool;
        startTimeTxt.text = "Start Time: " + getLeaguesByUser.start_time.Insert(2, ":");
        leagueid = getLeaguesByUser.leagueId;
        moderatorId = getLeaguesByUser.moderator_Id;
        gameRulesList = getLeaguesByUser.rules;
        DownloadImage();
        //if (getLeaguesByUser.leagueRequests != null && getLeaguesByUser.leagueRequests.Count != 0)
        //{
        //    foreach (var item in getLeaguesByUser.leagueRequests)
        //    {
        //        if (item.User.PlayerId == PlayerPrefs.GetString(Db_Keys.playerID))
        //        {
        //            if (item.Status == Global.Status.pending.ToString())
        //            {
        //                joinLeagueBtn.GetComponentInChildren<TMP_Text>().text = "Request Pending";
        //                joinLeagueBtn.interactable = false;
        //            }
        //            else if (item.Status == Global.Status.accepted.ToString())
        //            {
        //                if (isDestroyable)
        //                {
        //                    gameObject.SetActive(false);
        //                }
        //                else
        //                {
        //                    joinLeagueBtn.GetComponentInChildren<TMP_Text>().text = "Joined";
        //                    joinLeagueBtn.interactable = false;
        //                }
        //            }
        //        }
        //    }
        //}
    }

    void DownloadImage()
    {
        HomeScreen home = UIManager.instance.GetScreen<HomeScreen>();
        if (home.leagueImageCache.ContainsKey(getLeaguesByUser.image))
            SetImage(home.leagueImageCache[getLeaguesByUser.image]);
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
}
