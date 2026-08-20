using TMPro;
using UIAnimatrix;
using UnityEngine;

public class LeagueGamePrefab : MonoBehaviour
{
    public TMP_Text gameNameTxt;
    public TMP_Text laneTxt;
    public TMP_Text timeTxt;
    public TMP_Text gameRequestsTxt;
    public TMP_Text participantsTxt;
    public AnimatrixButton participantsBtn;
    public AnimatrixButton openGameBtn;
    public AnimatrixButton applicantsBtn;
    public GameObject gameRequestsCountPnl;
    public Games getgamesByModerator;

    void Start()
    {
        SetGameInfo();
        SetGamesRequestCount();
        participantsBtn.onClick.AddListener(() => ParticipantsBtnClicked());
        openGameBtn.onClick.AddListener(() => OpenGameBtnClicked());
        applicantsBtn.onClick.AddListener(() => ApplicantsBtnClicked());
    }

    public void SetGameInfo()
    {
        gameNameTxt.text = "Game Name: <color=#Ab0000>" + getgamesByModerator.Name + "</color>";
        laneTxt.text = "Lane: <color=#Ab0000>" + getgamesByModerator.Lane + "</color>";
        string time = getgamesByModerator.startTime.Insert(2, ":");
        timeTxt.text = "Time: <color=#Ab0000>" + time + "</color>";
        participantsTxt.text = "Participants: <color=#Ab0000>" + getgamesByModerator.Participants + "</color>";
    }

    public void SetGamesRequestCount()
    {
        int gameCount = int.Parse(getgamesByModerator.gameInfo);
        if (gameCount > 0)
        {
            gameRequestsTxt.text = getgamesByModerator.gameInfo;
            if (gameRequestsCountPnl != null && !gameRequestsCountPnl.activeSelf) gameRequestsCountPnl.SetActive(true);
        }
        else
        {
            gameRequestsTxt.text = string.Empty;
            if (gameRequestsCountPnl != null && gameRequestsCountPnl.activeSelf) gameRequestsCountPnl.SetActive(false);
        }
    }

    void ParticipantsBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        PlayerPrefs.SetString(Db_Keys.gameId, getgamesByModerator.Id);
        //UIManager.instance.GetScreen<LeagueScreen>().Hide();
        UIManager.instance.GetScreen<LeagueScreen>().PauseApis();
        UIManager.instance.Show<GameParticipantsScreen>();
    }

    void OpenGameBtnClicked()
    {
        int participantsCount = int.Parse(getgamesByModerator.Participants);
        if (participantsCount >= 2)
        {
            BGMusic.Instance.btn_audioSource.Play();
            PlayerPrefs.SetString(Db_Keys.gameId, getgamesByModerator.Id);

            //UIManager.instance.GetScreen<LeagueScreen>().Hide();
            UIManager.instance.GetScreen<LeagueScreen>().PauseApis();
            UIManager.instance.Show<TableGraphScreen>();
        }
        else
        {
            MessagePopUpScreen.Instance.ShowMessage("Participants must be 2 or more.", "Message", "OK", null, false, MessagePopUpScreen.Instance._wrongSprite);        
        }       
    }

    void ApplicantsBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        PlayerPrefs.SetString(Db_Keys.gameId, getgamesByModerator.Id);
        UIManager.instance.GetScreen<GameRequestScreen>().lane  = getgamesByModerator.Lane;
        //UIManager.instance.GetScreen<LeagueScreen>().Hide();
        UIManager.instance.GetScreen<LeagueScreen>().PauseApis();
        UIManager.instance.Show<GameRequestScreen>();
    }
}
