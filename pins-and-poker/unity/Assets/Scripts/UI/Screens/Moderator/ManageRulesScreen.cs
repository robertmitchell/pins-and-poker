using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class ManageRulesScreen : UIScreenBase
{
    public Button backBtn;
    public AnimatrixButton createEditBtn;
    public AnimatrixButton GeneralRulesBtn;
    public Transform rulesContent;
    public ScrollRect manageRulesScrollRect;
    public ModeratorLeaguePrefab leaguePrefab;
    public SpecialRulesPrefab specialRulesPrefab;
    public GameObject createGeneralRulePanelPrefab;
    public List<int> specialRules = new List<int>();
    public static Dictionary<string, string> leagueformData;
    public string generalRules = "";
    public bool isUpdate = false;


    void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
        manageRulesScrollRect.verticalNormalizedPosition = 1;
        WebServices.Instance.MakeRequest<List<Rule>>(ApiRoutes.getAdminRules, WebServices.HttpMethod.GET, SuccessForRules, OnFailure, null, null, null, true);
        MessagePopUpScreen.Instance?.ShowMessage("You can edit the general rule and select or deselect the special rules as desired.", "Message", "OK", null, false);
    }

    void OnDisable()
    {
        leagueformData.Clear();
        specialRules.Clear();
        leagueformData.Remove(Db_Keys.generalRules);
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
        createEditBtn.onClick.AddListener(() => CreateEditBtnClicked());
        GeneralRulesBtn.onClick.AddListener(() => GeneralRulesBtnClicked());
        //DebugDictionary(leagueformData);
    }

    void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<CreateEditLeagueScreen>();
    }

    void GeneralRulesBtnClicked()
    {
        UIManager.instance.GetScreen<GeneralRulesPopupScreen>().Show();
    }

    void SuccessForRules(List<Rule> rules, long arg2)
    {
        foreach (Transform item in rulesContent)
        {
            Destroy(item.gameObject);
        }
        if (rules.Count == 0) return; 
        foreach (var rule in rules)
        {
            SpecialRulesPrefab specialRuleObj = Instantiate(specialRulesPrefab.gameObject, rulesContent).GetComponent<SpecialRulesPrefab>();
            specialRuleObj.rule = rule;
            if (rule.type == "general") generalRules = rule.description;            
        }
    }

    void CreateEditBtnClicked()
    {
        if(UIManager.instance.GetScreen<CreateEditLeagueScreen>().isEdit)
        {
            SaveData(ApiRoutes.updateLeague);
        }
        else
        {
            SaveData(ApiRoutes.createLeague);
        }
    }

    void SaveData(string apiRoute)
    {
        if (specialRules.Contains(3) || specialRules.Contains(4))
        {
            Debug.Log("|||||||||||||||||||||||||||SpecialRules Rules: " + specialRules.ToString());
        }
        else
        {
            Debug.Log("|||||||||||||||||||||||||||SpecialRules Rules: not contains 3 or 4 " + specialRules.ToString());
            MessagePopUpScreen.Instance.ShowMessage("Rule 3 or 4 is Mandetory", "", "OK", null, true);
            return;
        }
        var json = Serializer.ToJson(specialRules);

        Texture2D leagueImage = UIManager.instance.GetScreen<CreateEditLeagueScreen>().leagueRawImage.texture as Texture2D;

        leagueformData.Add(Db_Keys.generalRules, generalRules);
        leagueformData.Add(Db_Keys.special_rules, json);

        if (UIManager.instance.GetScreen<CreateEditLeagueScreen>().isEdit)
        {
               leagueformData.Add(Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId));
               WebServices.Instance.MakeRequest<UpdateLeague>(
               apiRoute,
               WebServices.HttpMethod.POST,
               OnSuccessUpdate,
               OnFailure,
               null,
               leagueformData,
               leagueImage,
               true
               );
        }

        else
        {
            WebServices.Instance.MakeRequest<GetLeaguesByModerator>(
            apiRoute,
            WebServices.HttpMethod.POST,
            OnSuccess,
            OnFailure,
            null,
            leagueformData,
            leagueImage,
            true
            );
        }
    }

    public void DebugDictionary(Dictionary<string, string> dictionary)
    {
        foreach (KeyValuePair<string, string> kvp in dictionary)
        {
            Debug.Log("Key: " + kvp.Key + ", Value: " + kvp.Value);
        }
    }

    void OnSuccess(GetLeaguesByModerator response, long statusCode)
    {
        Debug.Log("League ID: " + response.leagueId);
        PlayerPrefs.SetString(Db_Keys.leagueId, response.leagueId);
        
        UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByModerator.leagueName = leagueformData[Db_Keys.leagueName];
        UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByModerator.PrizePool = leagueformData[Db_Keys.prizePool];
        UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByModerator.StartTime = leagueformData[Db_Keys.start_time];
        UIManager.instance.GetScreen<LeagueScreen>().leagueBackground.texture = UIManager.instance.GetScreen<CreateEditLeagueScreen>().leagueRawImage.texture;
        UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByModerator.participants = "0";
        UIManager.instance.GetScreen<CreateEditLeagueScreen>().isEdit = false;
        UIManager.instance.GetScreen<CreateEditLeagueScreen>().ResetFields();
        UIManager.instance.Hide();
        UIManager.instance.Show<LeagueScreen>();
    }

    void OnSuccessUpdate(UpdateLeague response, long statusCode)
    {
        UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByModerator.leagueName = leagueformData[Db_Keys.leagueName];
        UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByModerator.PrizePool = leagueformData[Db_Keys.prizePool];
        UIManager.instance.GetScreen<LeagueScreen>().getLeaguesByModerator.StartTime = leagueformData[Db_Keys.start_time];
        UIManager.instance.GetScreen<LeagueScreen>().leagueBackground.texture = UIManager.instance.GetScreen<CreateEditLeagueScreen>().leagueRawImage.texture;
        UIManager.instance.GetScreen<CreateEditLeagueScreen>().ResetFields();
        UIManager.instance.Hide();
        UIManager.instance.GetScreen<LeagueScreen>().Hide();
        UIManager.instance.Show<LeagueScreen>();
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        specialRules.Clear();
        leagueformData.Remove(Db_Keys.generalRules);
        leagueformData.Remove(Db_Keys.special_rules);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", ResetAPICall, true, MessagePopUpScreen.Instance._wrongSprite);
        //DebugDictionary(leagueformData);
    }

    void  ResetAPICall()
    {
        manageRulesScrollRect.verticalNormalizedPosition = 1;
        WebServices.Instance.MakeRequest<List<Rule>>(ApiRoutes.getAdminRules, WebServices.HttpMethod.GET, SuccessForRules, OnFailure, null, null, null, true);
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}

