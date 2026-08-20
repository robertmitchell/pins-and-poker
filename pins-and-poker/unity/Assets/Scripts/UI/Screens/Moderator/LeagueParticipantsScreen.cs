using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class LeagueParticipantsScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public Transform userContent;
    public ScrollRect leagueParticipantsrScrollRect;
    public GameObject noLeagueParticipantsText;
    public LeagueParticipantsPrefab participantPrefab;

    void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
        leagueParticipantsrScrollRect.horizontalNormalizedPosition = 0;
        leagueParticipantsrScrollRect.verticalNormalizedPosition = 1;
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId)  }
        };
        WebServices.Instance.MakeRequest<List<User>>(ApiRoutes.getLeagueParticipants, WebServices.HttpMethod.GET, OnSuccess, OnFail, null, formData, null, true);
    }

    void OnDisable()
    {
        noLeagueParticipantsText.SetActive(false);
        foreach (Transform item in userContent)
        {
            Destroy(item.gameObject);
        }
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
    }

    void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<LeagueScreen>();
        UIManager.instance.GetScreen<LeagueScreen>().ResumeApis();
    }

    void OnSuccess(List<User> users, long arg2)
    {
        if (users.Count <= 0) 
        {
            noLeagueParticipantsText.SetActive(true); 
            return;
        }
        else noLeagueParticipantsText.SetActive(false);

        foreach (Transform item in userContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var user in users)
        {
            LeagueParticipantsPrefab UsersObj = Instantiate(participantPrefab, userContent);
            UsersObj.userdata = user;
        }
    }

    void OnFail(string error)
    {
        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "Response", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
        if (userContent.childCount <= 0) noLeagueParticipantsText.SetActive(true);
        else noLeagueParticipantsText.SetActive(false);
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
