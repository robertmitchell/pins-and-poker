using System;
using TMPro;
using UIAnimatrix;
using UnityEngine;

public class DisputePrefab : MonoBehaviour
{
    public AnimatrixButton disputeBtn;
    public TMP_Text leagueTxt;
    public TMP_Text gameTxt;
    public TMP_Text disputeTxt;
    public TMP_Text timeTxt;
    public TMP_Text statusText;
    public string groupID;
    public long moderatorID;
    public long disputerID;
    public long disputedID;
    public long gameID;
    string _cellIndex;

    private void Start()
    {
       /* disputeBtn.onClick.AddListener(() => DisputeBtnClicked());*/
    }

    private void DisputeBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        UIManager.instance.GetScreen<DisputeScreen>().Hide();
        UIManager.instance.Show<LeagueChatScreen>();
        UIManager.instance.GetScreen<LeagueChatScreen>().cellIndex = _cellIndex;
        UIManager.instance.GetScreen<LeagueChatScreen>()._groupID = groupID;
        UIManager.instance.GetScreen<LeagueChatScreen>().StartChat(groupID, moderatorID, disputedID, disputerID, gameID);
    }

    public void Setup(string disputer, string disputed, string cellIndex, string league, string gameName, string time, string GroupID, long ModeratorID, long DisputerID, long DisputedID, long GameID, string status)
    {
        gameID = GameID;
        groupID = GroupID;
        disputerID = DisputerID;
        disputedID = DisputedID;
        moderatorID = ModeratorID;
        _cellIndex = cellIndex;
        leagueTxt.text = "League Name: <color=#AB0000>" + league + "</color>";
        gameTxt.text = "Game Name: <color=#AB0000>" + gameName + "</color>";
        timeTxt.text = time;
        disputeTxt.text = $"Disputed : {disputed} • Disputer : {disputer} • Lane : {cellIndex}";
        statusText.text = status;
        if (status == Global.Status.pending.ToString())
        disputeBtn.onClick.AddListener(DisputeBtnClicked);
    }
}