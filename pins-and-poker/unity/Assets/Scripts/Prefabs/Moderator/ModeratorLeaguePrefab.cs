using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ModeratorLeaguePrefab : MonoBehaviour
{
    [SerializeField] TMP_Text leagueNameTxt;
    [SerializeField] TMP_Text startTimeTxt;
    [SerializeField] TMP_Text participantsTxt;
    [SerializeField] TMP_Text pointsPoolTxt;
    [SerializeField] TMP_Text leaguesAndGamesCountTxt;
    [SerializeField] RawImage leagueImage;
    [SerializeField] AnimatrixButton openLeagueBtn;
    [SerializeField] GameObject leaguesAndGamesRequestsPnl;

    string leagueid;
    string player_id;
    List<Rule> rulesList;
    public GetLeaguesByModerator getLeaguesByModerator = new();


    void Start()
    {
        SetData();
        openLeagueBtn.onClick.AddListener(() => OpenLeagueBtnClicked());  
    }

    void OpenLeagueBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        PlayerPrefs.SetString(Db_Keys.leagueId, getLeaguesByModerator.leagueId);
        UIManager.instance.GetScreen<LeagueScreen>().leagueBackground.texture = leagueImage.texture;
        UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByModerator = getLeaguesByModerator;
        UIManager.instance.GetScreen<HomeScreen>().Hide();
        UIManager.instance.Show<LeagueScreen>();
    }

    void SetData()
    {
        leagueNameTxt.text = getLeaguesByModerator.leagueName.ToString();
       // participantsTxt.text = "Participants: <color=#FFFFFF> " + getLeaguesByModerator.participants.ToString() + "</color>";
        participantsTxt.text = "Participants: " + getLeaguesByModerator.participants.ToString();
        string formattedTime = getLeaguesByModerator.StartTime.Insert(2, ":");
        //startTimeTxt.text = "Start Time: <color=#FFFFFF> " + formattedTime + "</color>";
        startTimeTxt.text = "Start Time: " + formattedTime;
        leagueid = getLeaguesByModerator.leagueId;
        player_id = getLeaguesByModerator.playerId;
        rulesList = getLeaguesByModerator.rules;
        SetLeaguesAndGamesRequestCount();    
        DownloadImage();
    }

    public void SetLeaguesAndGamesRequestCount()
    {
        int leagueAndGameCount = int.Parse(getLeaguesByModerator.leagueInfo);
        if (leagueAndGameCount > 0)
        {
            leaguesAndGamesCountTxt.text = getLeaguesByModerator.leagueInfo;
            if (leaguesAndGamesRequestsPnl != null && !leaguesAndGamesRequestsPnl.activeSelf) leaguesAndGamesRequestsPnl.SetActive(true);
        }
        else
        {
            leaguesAndGamesCountTxt.text = string.Empty;
            if (leaguesAndGamesRequestsPnl != null && leaguesAndGamesRequestsPnl.activeSelf) leaguesAndGamesRequestsPnl.SetActive(false);
        }
    }

    void DownloadImage()
    {
        HomeScreen hm = UIManager.instance.GetScreen<HomeScreen>();
        if (hm.leagueImageCache.ContainsKey(getLeaguesByModerator.image))
            SetImage(hm.leagueImageCache[getLeaguesByModerator.image]);
        else
            StartCoroutine(ImageCacheManager.Instance.DownloadMultipleImage(ApiRoutes.imageStartingPointURL + getLeaguesByModerator.image, OnImageDownload));
    }

    void OnImageDownload(Texture2D texture)
    {
        UIManager.instance.GetScreen<HomeScreen>().leagueImageCache.Add(getLeaguesByModerator.image, texture);
        SetImage(texture);
    }

    void SetImage(Texture2D texture)
    {
        leagueImage.texture = texture;
    }
}
