using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DisputeScreen : UIScreenBase
{
    public Button backBtn;
    public TMP_Text noDisputesText;
    public ScrollRect disputesScrollRect;
    public DisputePrefab Disputeprefab;

    private void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
        disputesScrollRect.verticalNormalizedPosition = 1;
        SendRequestToGetDisputes();
        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.thumbsBall);
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
    }

    void BackBtnClicked()
    {
        if (UIManager.instance.GetScreen<TableGraphScreen>().gameObject.activeSelf)
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<TableGraphScreen>();
        }
        else
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<HomeScreen>();
        }
    }

    void SendRequestToGetDisputes()
    {
        foreach (Transform child in disputesScrollRect.content)
            Destroy(child.gameObject);
        WebServices.Instance.MakeRequest<List<GetDisputesResponse>>(ApiRoutes.getDisputeRequest, WebServices.HttpMethod.GET, OnGetDisputesSuccess, OnGetDisputesFail, null, null, null, true);
    }

    void OnGetDisputesSuccess(List<GetDisputesResponse> response, long code)
    {
        noDisputesText.gameObject.SetActive(false);
        response.ForEach(dispute => 
        {
            DisputePrefab obj = Instantiate(Disputeprefab, disputesScrollRect.content);
            obj.Setup(dispute.DisputerName, dispute.DisputedAgainstName, dispute.CellIndex, dispute.LeagueName, dispute.GameName, dispute.CreatedAt, dispute.groupID, long.Parse(dispute.ModeratorId), long.Parse(dispute.DisputerId), long.Parse(dispute.DisputedAgainstId), long.Parse(dispute.GameId), dispute.Status);
        });
    }

    void OnGetDisputesFail(string response)
    {
        //MessagePopUpScreen.Instance.ShowMessage("Failed to fetch Disputes. Please try again", "Failure");
        if (disputesScrollRect.content.transform.childCount <= 0) noDisputesText.gameObject.SetActive(true);
        else noDisputesText.gameObject.SetActive(false);
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
