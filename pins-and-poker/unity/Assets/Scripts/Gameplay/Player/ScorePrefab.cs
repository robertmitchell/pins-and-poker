using TMPro;
using UnityEngine;

public class ScorePrefab : MonoBehaviour
{
    public ScoreCell inputField1;
    public ScoreCell inputField2;
    public ScoreCell inputField3;
    public TMP_Text sumText;
    public int maxEntry=13;
    public bool isLastRoundExtraTurn = false;
    public bool isInputField2 = false;
    public bool isInputField3 = false;
    public Color defaultColor = Color.black;
    public Color highlightColor = Color.red;
    private ScoreManager scoreManager;

    
    public void DependencyInject(ScoreManager scoreManager)
    {
        this.scoreManager = scoreManager; 
    }

    public int ReturnMax()
    {
        if (isInputField3)
        {
            // We need to check if first and second cell have spare 
            // We also need to check have a strike
            // in both cases we need to treat third cell as fresh 
            int isSpare = int.Parse(inputField1.inputField.text) + int.Parse(inputField2.inputField.text);

            if (int.Parse(inputField2.inputField.text)==10|| isSpare==10)
            {
                return 10;
            }

            //it is tenth round
            maxEntry = 10 - int.Parse(inputField2.inputField.text);
            return maxEntry;
        }
        if (!string.IsNullOrEmpty(inputField1.inputField.text) && isInputField2)
        {
            Debug.LogWarning(" maxEntry " + maxEntry);
            maxEntry = 10 - int.Parse(inputField1.inputField.text);
            return maxEntry;
        }
        Debug.LogWarning(" maxEntry " + maxEntry);

        return 10;
    }

    void Start()
    {
        inputField1.createChatButton.onClick.AddListener(FirstInputField);
        inputField2.createChatButton.onClick.AddListener(SecondInputField);
        inputField3?.createChatButton.onClick.AddListener(ThirdInputField);
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found in parent hierarchy.");
            return;
        }
    }
    void FirstInputField()
    {
        isInputField3 = false;
        isInputField2 = false;
    }

    void SecondInputField()
    {
        isInputField3 = false;
        isInputField2 = true;
    }

    void ThirdInputField()
    {
        isInputField2 = false;
        isInputField3 = true;
    }
}