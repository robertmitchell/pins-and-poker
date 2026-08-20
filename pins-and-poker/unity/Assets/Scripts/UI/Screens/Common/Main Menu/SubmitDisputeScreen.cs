using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class SubmitDisputeScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton submitBtn;
    public InputField subjectField;
    public InputField descriptionField;
    public TMP_Text statusText;

    private void OnEnable()
    {
        if (statusText) statusText.gameObject.SetActive(false);
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
        submitBtn.onClick.AddListener(() => SubmitBtnClicked());
    }

    void SubmitBtnClicked()
    {
        if (string.IsNullOrEmpty(subjectField.text))
        {
            ShowStatus("Please enter a subject.", false);
            return;
        }
        if (string.IsNullOrEmpty(descriptionField.text))
        {
            ShowStatus("Please describe your dispute.", false);
            return;
        }

        submitBtn.interactable = false;

        Dictionary<string, string> formData = new()
        {
            { "subject", subjectField.text },
            { "description", descriptionField.text },
            { Db_Keys.token, PlayerPrefs.GetString(Db_Keys.token) }
        };

        WebServices.Instance.MakeRequest<ResponseData>(
            ApiRoutes.disputeRequest,
            WebServices.HttpMethod.POST,
            OnSuccess,
            OnFailure,
            null,
            formData,
            null,
            true
        );
    }

    void OnSuccess(ResponseData response, long statusCode)
    {
        subjectField.text = string.Empty;
        descriptionField.text = string.Empty;
        submitBtn.interactable = true;
        ShowStatus("Your dispute has been submitted successfully.", true);
    }

    void OnFailure(string error)
    {
        Debug.LogError("Dispute submit failed: " + error);
        submitBtn.interactable = true;
        ShowStatus(error, false);
    }

    void ShowStatus(string message, bool success)
    {
        if (!statusText) return;
        statusText.text = message;
        statusText.color = success ? Color.green : Color.red;
        statusText.gameObject.SetActive(true);
    }

    void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<SettingScreen>();
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
