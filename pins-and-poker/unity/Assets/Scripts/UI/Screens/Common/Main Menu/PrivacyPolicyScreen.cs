using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class PrivacyPolicyScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public TMP_Text contentTxt;
    public ScrollRect contentScrollRect;
    public Toggle agreeToggle;
    public AnimatrixButton agreeBtn;

    private void OnEnable()
    {
        contentScrollRect.verticalNormalizedPosition = 1;
        if (contentTxt) contentTxt.alignment = TextAlignmentOptions.Left;
        if (agreeToggle) agreeToggle.isOn = false;
        UpdateAgreeButton();
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
        if (agreeToggle) agreeToggle.onValueChanged.AddListener((_) => UpdateAgreeButton());
        if (agreeBtn) agreeBtn.onClick.AddListener(() => AgreeBtnClicked());
    }

    void UpdateAgreeButton()
    {
        if (agreeBtn) agreeBtn.interactable = agreeToggle != null && agreeToggle.isOn;
    }

    void AgreeBtnClicked()
    {
        PlayerPrefs.SetInt("privacy_policy_agreed", 1);
        BackBtnClicked();
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
