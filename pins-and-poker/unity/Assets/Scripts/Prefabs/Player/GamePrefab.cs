using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;

public class GamePrefab : MonoBehaviour
{
    public TMP_Text gameNameTxt;
    public TMP_Text laneTxt;
    public TMP_Text timeTxt;
    public TMP_Text participantsTxt;
    public AnimatrixButton gameStatusBtn;
    public Games gamesForUser;

    void Start()
    {
        gameStatusBtn.onClick.AddListener(GameStatusBtnClicked);
        SetGameInfo();
    }

    public void SetGameInfo()
    {
        gameNameTxt.text = "Game Name: <color=#92222F>" + gamesForUser.Name + "</color>";
        laneTxt.text = "Lane: <color=#92222F>" + gamesForUser.Lane + "</color>";
        string time = gamesForUser.startTime.Insert(2, ":");
        timeTxt.text = "Time: <color=#92222F>" + time + "</color>";
        participantsTxt.text = "Participants: <color=#92222F>" + gamesForUser.Participants + "</color>";

        if (gamesForUser.GameRequests != null && gamesForUser.GameRequests.Count != 0)
        {
            Debug.Log("Request Not Null");

            //gameStatusBtn.enabled = false;
            foreach (var item in gamesForUser.GameRequests)
            {
                if (item.User.PlayerId == PlayerPrefs.GetString(Db_Keys.playerID))
                {
                    if (item.Status == Global.Status.pending.ToString())
                    {
                        // openGameBtn.enabled = false;
                        gameStatusBtn.GetComponentInChildren<TMP_Text>().text = "Cancel Request";
                    }
                    else if (item.Status == Global.Status.accepted.ToString())
                    {
                        //gameStatusBtn.enabled = true;
                        gameStatusBtn.GetComponentInChildren<TMP_Text>().text = "Open Game";
                    }
                }
                else
                {

                    gameStatusBtn.GetComponentInChildren<TMP_Text>().text = "Send Request";

                }
            }
        }
        else
        {
            //Send Request
            gameStatusBtn.GetComponentInChildren<TMP_Text>().text = "Send Request";
        }
    }

    void GameStatusBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.leagueId,  PlayerPrefs.GetString(Db_Keys.leagueId) },
            { Db_Keys.gameId, gamesForUser.Id  }
        };

        if (gameStatusBtn.GetComponentInChildren<TMP_Text>().text == "Send Request")
        {

            //if first time send game request 
            Debug.Log("Join Game Request Clicked");

            WebServices.Instance.MakeRequest<Games>(
                ApiRoutes.joinGame,
                WebServices.HttpMethod.POST,
                OnSuccessJoin,
                OnFailure,
                null,
                formData,
                null,
                true
                );
        }

        // else if (gamesForUser.GameRequests[0].Status == Global.Status.pending.ToString())
        else if (gameStatusBtn.GetComponentInChildren<TMP_Text>().text == "Cancel Request")
        {
            Debug.Log("Request Cancel Clicked");
            MessagePopUpScreen.Instance.ShowMessage("Are you sure that you want to cancel your game\r\nrequest?.",
                                                   "", "Continue", CancelRequest);

            void CancelRequest()
            {
                //if pending Cancel Request
                WebServices.Instance.MakeRequest<Games>(
                ApiRoutes.cancelGame,
                WebServices.HttpMethod.POST,
                OnSuccessCancel,
                OnFailure,
                null,
                formData,
                null,
                true
                );
            }

        }
       else if(gameStatusBtn.GetComponentInChildren<TMP_Text>().text == "Open Game")
        {
            //if Accepted Open Game
            PlayerPrefs.SetString(Db_Keys.gameId, gamesForUser.Id);

            //UIManager.instance.GetScreen<MyAssignedCardsScreen>().backBtn.gameObject.SetActive(true);
            //UIManager.instance.GetScreen<LeagueScreen>().Hide();
            UIManager.instance.GetScreen<LeagueScreen>().PauseApis();
            UIManager.instance.Show<TableGraphScreen>();
        }
    }

    void OnSuccessJoin(Games requestData, long statusCode)
    {
        gameStatusBtn.GetComponentInChildren<TMP_Text>().text = "Pending";
        gameStatusBtn.interactable = false;
        MessagePopUpScreen.Instance.ShowMessage("Request to Join the Game has been sent.", "", "OK", RefreshLeagueScreen, true);
        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.thumbsBall);

    }

    void OnSuccessCancel(Games requestData, long statusCode)
    {
        // gameStatusBtn.GetComponentInChildren<TMP_Text>().text = "Cancel Request";
        MessagePopUpScreen.Instance.ShowMessage("Request to Cancel Game has been sent.", "", "OK", RefreshLeagueScreen, true);
        gameStatusBtn.interactable = false;
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
    }

    private void RefreshLeagueScreen()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<LeagueScreen>();
    }
}
