using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialRulesPrefab : MonoBehaviour
{
    Button Btn;
    public int SpecialRuleId;
    public TMP_Text ruleTxt;
    public Rule rule;
    bool hasBeenAdded = false;
    public Image generalImage;
    public Image specialImage;
   // public Button generalBtn;

    private void OnEnable()
    {
        ruleTxt = GetComponentInChildren<TMP_Text>();
        Btn = GetComponent<Button>();
        Btn.interactable = true;
    }
    void Start()
    {
        if (rule.type==Global.RuleType.general.ToString())
        {
            gameObject.transform.SetSiblingIndex(0);
            generalImage.gameObject.SetActive(true);
            specialImage.gameObject.SetActive(false);
            Btn.GetComponent<Image>().color = new Color32(200, 60, 80, 255);
        }
        else
        {

            generalImage.gameObject.SetActive(false);
            specialImage.gameObject.SetActive(true);
        }
        Btn.onClick.AddListener(() => BtnClicked());
        SpecialRuleId = int.Parse(rule.id);
        ruleTxt.text = rule.description;
    }

    void BtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        if (rule.type == Global.RuleType.general.ToString())
        {
            UIManager.instance.GetScreen<GeneralRulesPopupScreen>().generalRuleInputField.text=rule.description;
            UIManager.instance.GetScreen<GeneralRulesPopupScreen>().selectedGeneralRulePrefab=this;
            UIManager.instance.GetScreen<GeneralRulesPopupScreen>().Show();
            return;
        }

        if (!hasBeenAdded)
        {
            UIManager.instance.GetScreen<ManageRulesScreen>().specialRules.Add(SpecialRuleId);
            Btn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            hasBeenAdded = true;
        }
        else
        {
            UIManager.instance.GetScreen<ManageRulesScreen>().specialRules.Remove(SpecialRuleId);
            Btn.GetComponent<Image>().color = new Color32(100, 100, 100, 220);
            hasBeenAdded = false;
        }
        //gameObject.SetActive(false);
        // REMAINING : rule select and Deselect pending
    }
    
}
