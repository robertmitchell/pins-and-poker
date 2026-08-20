using DG.Tweening;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class GeneralRulesPopupScreen : UIScreenBase
{
    [SerializeField] Image _fadeImg;
    [SerializeField] AnimatrixButton createBtn;
    [SerializeField] AnimatrixButton closeBtn;
    [SerializeField] GameObject _CreatePanel;
    public TMP_InputField generalRuleInputField;
    [HideInInspector]
    public SpecialRulesPrefab selectedGeneralRulePrefab;
    Tweener _tweener;

    private void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
    }

    private void OnDisable()
    {
        ResetFields();
        CancelInvoke();
        _tweener.Kill();
    }

    private void Start()
    {
        createBtn.onClick.AddListener(() => CreateBtnClicked());
        closeBtn.onClick.AddListener(() => DisableGameObject());
    }

    void CreateBtnClicked()
    {
        if (!string.IsNullOrEmpty(generalRuleInputField.text) && !string.IsNullOrWhiteSpace(generalRuleInputField.text))
        {
            ManageRulesScreen.leagueformData.Remove(Db_Keys.generalRules);

            //if (selectedGeneralRulePrefab != null)
               // Destroy(selectedGeneralRulePrefab);

            UIManager.instance.GetScreen<ManageRulesScreen>().generalRules = generalRuleInputField.text;
            /*UIManager.instance.GetScreen<ManageRulesScreen>().GeneralRulesBtn.gameObject.SetActive(false);*/
            //specialRulePrefab = Instantiate(UIManager.instance.GetScreen<ManageRulesScreen>().createGeneralRulePanelPrefab, UIManager.instance.GetScreen<ManageRulesScreen>().rulesContent);
            selectedGeneralRulePrefab.ruleTxt.text = generalRuleInputField.text;
            selectedGeneralRulePrefab.rule.description = generalRuleInputField.text;
            selectedGeneralRulePrefab.transform.SetSiblingIndex(0);

            DisableGameObject();
        }
        else
        {
            if (string.IsNullOrEmpty(generalRuleInputField.text)) ShowExceptionMessage("Text Field Required", generalRuleInputField);
            return;
        }
    }

    internal void ShowExceptionMessage(string msgTxt, TMP_InputField inputField, float duration = 1f)
    {
        inputField.placeholder.gameObject.SetActive(false);
        inputField.placeholder.gameObject.SetActive(true);
        ((TMP_Text)inputField.placeholder).text = msgTxt;
    }

    void DisableGameObject()
    {
        _tweener = _CreatePanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _CreatePanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _CreatePanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    UIManager.instance.GetScreen<GeneralRulesPopupScreen>().Hide();
                    //gameObject.SetActive(false);
                });
            });
        });
    }

    void ResetFields()
    {
        generalRuleInputField.text = string.Empty;
        ((TMP_Text)generalRuleInputField.placeholder).text = "Write special rule here...";
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
