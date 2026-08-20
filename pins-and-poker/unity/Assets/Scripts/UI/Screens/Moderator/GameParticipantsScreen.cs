using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class GameParticipantsScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public Transform userContent;
    public ScrollRect gameParticipantsScrollRect;
    public GameObject noGameParticipantsText;
    public GameParticipantsPrefab participantPrefab;


    private void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
        gameParticipantsScrollRect.verticalNormalizedPosition = 1;
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId)  },
            { Db_Keys.gameId, PlayerPrefs.GetString(Db_Keys.gameId)  }
        };
        WebServices.Instance.MakeRequest<List<User>>(ApiRoutes.getGameParticiants, WebServices.HttpMethod.GET, OnSuccess, OnFail, null, formData, null, true);
    }

    private void OnDisable()
    {
        noGameParticipantsText.SetActive(false);
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

    private void OnSuccess(List<User> users, long arg2)
    {
        if (users.Count <= 0)
        {
            noGameParticipantsText.SetActive(true);
            return;
        }
        else noGameParticipantsText.SetActive(false);

        foreach (Transform item in userContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var user in users)
        {
            GameParticipantsPrefab UsersObj = Instantiate(participantPrefab, userContent);
            UsersObj.userdata=user;
        }
    }

    private void OnFail(string error)
    {
        Debug.LogError("Request failed: " + error);
        if (userContent.childCount <= 0) noGameParticipantsText.SetActive(true);
        else noGameParticipantsText.SetActive(false);
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
