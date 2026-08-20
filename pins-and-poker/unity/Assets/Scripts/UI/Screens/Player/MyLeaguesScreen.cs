using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class MyLeaguesScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton reloadBtn;   // Refreshes the leagues list from the server
    public TMP_Text noLeaguesText;
    public TMP_Text descriptionText;
    public GameObject leaguesRequestScrollViewShadows;
    public ScrollRect leaguesRequestScrollrect;
    public Transform leaguesContent;
    public MyLeaguePrefab myleaguePrefab;

    private const string DESCRIPTION =
        "My Leagues\n\n" +
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. A league is a group of players " +
        "who compete together over a set schedule. You can join an existing league by searching for " +
        "it on the home screen, or ask a league moderator for an invite.\n\n" +
        "To create your own league, you must be registered as a moderator.";

    private void OnEnable()
    {
        leaguesRequestScrollrect.horizontalNormalizedPosition = -0f;
        if (descriptionText) descriptionText.text = DESCRIPTION;
        WebServices.Instance.MakeRequest<List<GetLeaguesByUser>>(ApiRoutes.getUserLeagues, WebServices.HttpMethod.GET, OnSuccess, OnFail, null, null, null, true);
    }

    private void OnDisable()
    {
        noLeaguesText.gameObject.SetActive(false);
        leaguesRequestScrollViewShadows.SetActive(false);
        foreach (Transform item in leaguesContent.transform)
        {
            Destroy(item.gameObject);
        }
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
        reloadBtn.onClick.AddListener(() => ReloadBtnClicked());
    }

    void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<HomeScreen>();
    }

    void ReloadBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<MyLeaguesScreen>();
    }

    private void OnSuccess(List<GetLeaguesByUser> leagues, long ResCode)
    {
        noLeaguesText.gameObject.SetActive(false);
        foreach (Transform item in leaguesContent.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (var league in leagues)
        {
            MyLeaguePrefab myLeagueObj = Instantiate(myleaguePrefab.gameObject, leaguesContent).GetComponent<MyLeaguePrefab>();
            myLeagueObj.getLeaguesByUser = league;
        }
        leaguesRequestScrollViewShadows.SetActive(true);
    }

    private void OnFail(string obj)
    {
        Debug.LogError("Request failed: " + obj);
        if (leaguesContent.childCount <= 0) noLeaguesText.gameObject.SetActive(true);
        else noLeaguesText.gameObject.SetActive(false);
        //MessagePopUpScreen.Instance.ShowMessage(obj, "Response", "OK", null, true);
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
