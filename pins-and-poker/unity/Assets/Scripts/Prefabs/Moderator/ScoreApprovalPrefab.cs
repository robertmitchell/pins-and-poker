using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class ScoreApprovalPrefab : MonoBehaviour
{
    public RawImage profileImg;
    public TMP_Text leagueNameTxt;
    public TMP_Text startTimeTxt;
    public TMP_Text participantNameTxt;
    public TMP_Text scoreTxt;
    public TMP_Text assignedLaneTxt;
    public AnimatrixButton ApproveBtn;
    public AnimatrixButton RejectBtn;

    void Start()
    {
        ApproveBtn.onClick.AddListener(() => ApproveBtnClicked());
        RejectBtn.onClick.AddListener(() => RejectBtnClicked());
    }

    void ApproveBtnClicked()
    {

    }

    void RejectBtnClicked()
    {

    }

    internal void SetData(string leagueName, string startTime, string participantName, string score, string assignedLane, Texture2D picture)
    {
        leagueNameTxt.text = leagueName;
        startTimeTxt.text = startTime;
        participantNameTxt.text = participantName;
        scoreTxt.text = score;
        assignedLaneTxt.text = assignedLane;
        profileImg.texture = picture;
    }
}
