using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Global;

public class ScoreCell : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text symboltext;
    public Button createChatButton;
    public bool isPlayerCell = false;
    public int cellIndex;

    private void OnEnable()
    {
        StartCoroutine(HighlightPlayerCell());
    }

    IEnumerator HighlightPlayerCell()
    {
        yield return new WaitForSeconds(1f);
        if (isPlayerCell)
        {
            if (cellIndex == myCurrentCellIndex)
            {
                SetTransparency(30);
            }
            else
            {
                SetTransparency(0);
            }
        }
        StartCoroutine(HighlightPlayerCell());

    }

    void Start()
    {
        createChatButton.onClick.AddListener(OpenEditDispute);
    }

    public void IsPlayerCell( bool _isPlayerCell)
    {
        isPlayerCell = _isPlayerCell;
        Debug.Log("Is Player Cell  :  "+ _isPlayerCell);
    }

    private void OpenEditDispute()
    {
        EditDisputePopup.Instance.isPlayerCell = isPlayerCell;
        EditDisputePopup.Instance.selectedCellIndex = cellIndex;
    
        if (isPlayerCell)
        {
            if (cellIndex == myCurrentCellIndex)
            {
                //Global.myCurrentCellIndex++;  increment on success score update
                EditDisputePopup.Instance.isPlayerCell = isPlayerCell;
                SetTransparency(30);
                //createChatButton.GetComponent<Image>().color = Color.grey;
            }
            else if (cellIndex <= myCurrentCellIndex)
            {
                EditDisputePopup.Instance.isPlayerCell = false;
                if (SystemInfo.supportsVibration)
                {
                    Handheld.Vibrate();
                }
                SetTransparency(0);               
            }
            else 
            {
                EditDisputePopup.Instance.isPlayerCell = false;
                MessagePopUpScreen.Instance.ShowMessage("Unable to Edit Selected Score", "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
                if (SystemInfo.supportsVibration)
                {
                    Handheld.Vibrate();
                }
                SetTransparency(0);
                return;
            }
        }       
        EditDisputePopup.Instance._selectedInputField = inputField;
        EditDisputePopup.Instance.gameObject.SetActive(true);
    }

    public void SetTransparency(float transparencyPercent)
    {
        float alpha = Mathf.Clamp01(transparencyPercent / 100f); 
        if (createChatButton != null)
        {
            Color color = createChatButton.GetComponent<Image>().color;
            color.a = alpha;
            createChatButton.GetComponent<Image>().color = color;
        }
    }

    public void SaveRequest(int selectedIndex)
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            Debug.LogError("Please Enter a value");
            return;
        }
        if (ValidateInput(inputField.text, GetComponentInParent<ScorePrefab>().ReturnMax(), selectedIndex ) )
        {

            ScoreManager sm = GetComponentInParent<ScoreManager>();
            List<int> scores = new();
            sm.scorePrefabs.ForEach(sp => 
            {
                int total=0;
                if (sp.inputField1.inputField.text != null && sp.inputField1.inputField.text != "")
                {
                    Debug.Log("Cell inputField : " + sp.inputField1.inputField.text);
                    total = int.Parse(sp.inputField1.inputField.text);
                    scores.Add(int.Parse(sp.inputField1.inputField.text));
                }
                if (sp.inputField2.inputField.text != null&& sp.inputField2.inputField.text !="")
                {
                  
                    Debug.Log("Cell inputField : " + sp.inputField2.inputField.text);
                    total = total + int.Parse(sp.inputField2.inputField.text);
                    scores.Add(int.Parse(sp.inputField2.inputField.text));

                }
                if (sp.inputField3 != null)
                {
                    if (total >= 10)
                    {
                        if (sp.inputField3.inputField.text != null && sp.inputField3.inputField.text != "")
                        {
                            scores.Add(int.Parse(sp.inputField3.inputField.text));
                        }
                    }
                    else
                    {
                        sp.inputField3.inputField.text = "";
                    }
                   
                }
            });
            string serializedData = Newtonsoft.Json.JsonConvert.SerializeObject(scores.ToArray());
            Debug.Log($"serialized data: {serializedData}");

            string player= EditDisputePopup.Instance._selectedInputField.GetComponentInParent<ScoreManager>().playerScoreCardData.PlayerId;

            Debug.Log("player ID : "+player);
          
            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.leagueId,  PlayerPrefs.GetString(Db_Keys.leagueId) },
                { Db_Keys.gameId, PlayerPrefs.GetString(Db_Keys.gameId)  },
                { Db_Keys.playerID,  player },
                { Db_Keys.rolls,  serializedData} 
            };
            //Hit the update score request here
            WebServices.Instance.MakeRequest<BowlingScoreCardData>(
                ApiRoutes.updateGameScore,
                WebServices.HttpMethod.POST,
                OnSuccess,
                OnFail, 
                null, 
                formData,
                null, 
                true);
        }
    }

    private void OnSuccess(BowlingScoreCardData data, long code)
    {
        if (Response.Check(code))
        {
            if (data != null) 
            {
                inputField.interactable = false;
                createChatButton.gameObject.SetActive(true);
                TableDataManager tb = GetComponentInParent<TableDataManager>();
                tb.bowlingScoreCardData.score = data.score;
                tb.UpdateScoreCard(tb.bowlingScoreCardData);
                APIInvoker.Instance.AddApiRequest(UIManager.instance.GetScreen<TableDataManager>().SendRequestToGetScores, 4f);
                EditDisputePopup.Instance.DisableGameObject();
            }
            else
            {
                OnFail("Data is null");
            }
        }
    }

    private void OnFail(string errorMsg)
    {
        SetTransparency(0);
        EditDisputePopup.Instance._selectedInputField.text = "";
        inputField.text = "";
        Debug.LogError("Request failed: " + errorMsg);
        MessagePopUpScreen.Instance.ShowMessage(errorMsg, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
        APIInvoker.Instance.AddApiRequest(UIManager.instance.GetScreen<TableDataManager>().SendRequestToGetScores, 4f);
    }

    bool ValidateInput(string value, int maxEntry, int selectedIndex)
    {
        int number;
        bool isNumber = int.TryParse(value, out number);
        Debug.LogError("maxEntry   :  " + maxEntry);
        Debug.LogError("selectedIndex   :  " + selectedIndex);

        //if (isNumber && number >= 0 && number <= 10)
        if (isNumber && number >= 0 && number <= maxEntry)
        {
            Debug.Log("Input is valid: " + number);
          /*GetComponentInParent<ScorePrefab>().maxEntry = GetComponentInParent<ScorePrefab>().maxEntry - number;
            if (GetComponentInParent<ScorePrefab>().inputField2)*/
             return true;
        }
        if (selectedIndex==19 && isNumber && number <=10)
        {

            Debug.Log("Input is valid: " + number);
            return true;
        }
        else
        {
            Debug.Log("Input is invalid: " + value);
            MessagePopUpScreen.Instance.ShowMessage("Input is invalid. \n Please a write value less then ten", "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
            inputField.text = "";
            return false;
        }
    }
}
