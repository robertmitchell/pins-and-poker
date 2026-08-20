using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text playerNameTxt;
    public TMP_Text TotalScoreTxt;
    public Image highlightImg;
    public List<ScorePrefab> scorePrefabs;
    //public ScorePrefab ScorePrefab;
    //public GameObject PointToInstantiate;

    public PlayerScoreCardData playerScoreCardData;
    public List<ScoreCell> rollsTextList = new List<ScoreCell>();
    public List<TMP_Text> scoreTextList = new List<TMP_Text>();

    //public Scrollbar scrollbar;
    [HideInInspector]
    public int TotalScore = 0;
    public bool isPlayerRow = false;

    private void OnEnable()
    {
        Invoke(nameof(SetPlayerCell),2f);
    }

    void SetPlayerCell()
    {
        Debug.Log(" ScoreManagerDebug  isPlayerRow  :  " + isPlayerRow);
        foreach (var item in rollsTextList)
        {
            item.isPlayerCell = isPlayerRow;
        }
        for (int i = 0; i < rollsTextList.Count; i++)
        {
            rollsTextList[i].cellIndex = i;
        }
    } 
    
    public void PickColumns()
    {
        foreach (var scorePrefab in scorePrefabs)
        {
            scorePrefab.DependencyInject(this);
            rollsTextList.Add(scorePrefab.inputField1);
            rollsTextList.Add(scorePrefab.inputField2);
            if (scorePrefab.inputField3!=null)
            {
                rollsTextList.Add(scorePrefab.inputField3);
            }
            scoreTextList.Add(scorePrefab.sumText);
        }
    }

    public void InitializeData(PlayerScoreCardData player)
    {
        this.playerScoreCardData = player;
    }

    public void SetData(PlayerScoreCardData playerScoreCardData) 
    {
        //UpdateFinalScore();
        this.playerNameTxt.text = playerScoreCardData.Username;
        if (playerScoreCardData.Rolls != null && playerScoreCardData.CellScores != null)
        {
            //to save my current cell id as per my rolls
            if (playerScoreCardData.PlayerId== PlayerPrefs.GetString(Db_Keys.playerID))
            {
                Global.myCurrentCellIndex = playerScoreCardData.Rolls.Count;
                if (playerScoreCardData.ExchangeCards == true && TableGraphScreen.canShowPopup == true)
                {

                    if (PlayerPrefs.GetString(Db_Keys.userType) == Global.UserType.user.ToString())
                        MessagePopUpScreen.Instance.ShowMessage("Please select a card to replace", "Card Limit Exceeded", "OK", ChangeScreen, true);
                }
            }

            for (int i = 0; i < playerScoreCardData.Rolls.Count; i++)
            {
                rollsTextList[i].inputField.text = playerScoreCardData.Rolls[i].ToString();

                int totalScore = 0; // Declare totalScore outside of conditions so it's always available

                // Handle Strike (first roll of a frame, with 10 pins knocked down)
                if (playerScoreCardData.Rolls[i] == 10) // Strike case
                {
                    rollsTextList[i].symboltext.gameObject.SetActive(true);
                    rollsTextList[i].symboltext.text = "X";
                    rollsTextList[i].inputField.transform.GetChild(0).gameObject.SetActive(false); // Input Field in 1st child
                    if (i >= 18)
                    {
                        rollsTextList[20].createChatButton.gameObject.SetActive(true); // Enabled the last input field entry 


                    }
                }
                else
                {
                    // Hide strike symbol
                    rollsTextList[i].symboltext?.gameObject.SetActive(false);
                    rollsTextList[i].symboltext.text = "";
                    if (i==20)
                    {

                        totalScore = playerScoreCardData.Rolls[i - 1] + playerScoreCardData.Rolls[i];
                        Debug.Log("rollsTextList    :      :    " +  rollsTextList[i].gameObject.name);
                        if (totalScore == 10) // Spare case
                        {
                            rollsTextList[i].symboltext.gameObject.SetActive(true);
                            rollsTextList[i].symboltext.text = "/";
                            rollsTextList[i].inputField.transform.GetChild(0).gameObject.SetActive(false); // Input Field in 1st child
                            if (playerScoreCardData.Rolls[i] == 0)
                            {
                                rollsTextList[i].symboltext?.gameObject.SetActive(false);
                                rollsTextList[i].symboltext.text = "";
                                rollsTextList[i].inputField.transform.GetChild(0).gameObject.SetActive(true); // Input Field in 1st child
                            }
                          

                        }
                    }
                    // Handle Spare (second roll of a frame, knocking total 10 pins)
                    else if (i % 2 != 0) // Ensure this is the second roll in a frame
                    {
                        totalScore = playerScoreCardData.Rolls[i - 1] + playerScoreCardData.Rolls[i];
                     
                        if (totalScore == 10) // Spare case
                        {
                            rollsTextList[i].symboltext.gameObject.SetActive(true);
                            rollsTextList[i].symboltext.text = "/";
                            rollsTextList[i].inputField.transform.GetChild(0).gameObject.SetActive(false); // Input Field in 1st child
                            if (playerScoreCardData.Rolls[i] == 0)
                            {
                                rollsTextList[i].symboltext?.gameObject.SetActive(false);
                                rollsTextList[i].symboltext.text = "";
                                rollsTextList[i].inputField.transform.GetChild(0).gameObject.SetActive(true); // Input Field in 1st child
                            }
                            if (i > 18)
                            {
                                rollsTextList[20].createChatButton.gameObject.SetActive(true); // Enabled the last input field entry 
                            }

                        }
                        else if(totalScore < 10)
                        {
                            if (i > 18)
                            {

                                rollsTextList[i].symboltext?.gameObject.SetActive(false);
                                rollsTextList[20].inputField.text = "";
                                rollsTextList[20].createChatButton.gameObject.SetActive(false); // Disabled the last input field entry 
                            }
                        }
                    }

                    // Show input field if no strike or spare
                    if (i % 2 == 0 || (i % 2 != 0 && totalScore < 10))
                    {
                        if (i!=20)
                        {
                        rollsTextList[i].inputField.transform.GetChild(0).gameObject.SetActive(true);

                        }
                    }
                }

                if (i >=18)
                {
                    if (playerScoreCardData.Rolls[i]==10 || totalScore>=10)
                    {
                        Debug.Log("----------------" + playerScoreCardData.Rolls[i]);

                    }

                }
            }



            // for (int i = 0; i < playerScoreCardData.CellScores.Count-1; i++)   //temp
            if (playerScoreCardData.CellScores.Count<10)
            {
                for (int i = 0; i < playerScoreCardData.CellScores.Count; i++)
                {
                    scoreTextList[i].text = playerScoreCardData.CellScores[i].ToString();
                    
                }
            }
            else if (playerScoreCardData.CellScores.Count >= 10)
            {
                for (int i = 0; i < playerScoreCardData.CellScores.Count-1; i++)
                {
                    scoreTextList[i].text = playerScoreCardData.CellScores[i].ToString();
                    if (i==9)
                    {
                    Debug.Log("^^^^^^^^^^^^^^^^^^" + playerScoreCardData.CellScores.Count);
                    scoreTextList[i].text = playerScoreCardData.CellScores[i].ToString();
                    scoreTextList[i].text = playerScoreCardData.CellScores[i+1].ToString();
                    }
                }
            }
            /*  for (int i = 0; i < playerScoreCardData.CellScores.Count; i++)
              {
                  scoreTextList[i].text = playerScoreCardData.CellScores[i].ToString();
              }*/
            if (playerScoreCardData.CellScores.Count > 0)
            {
                TotalScoreTxt.text = playerScoreCardData.CellScores[^1].ToString(); // Using ^1 to get the last value
            }
            else
            {
                TotalScoreTxt.text = "0"; // Default value if the list is empty
            }
        }
    }

    void ChangeScreen()
    {
        /*UIManager.instance.getsc.Hide();*/
        UIManager.instance.GetScreen<MyAssignedCardsScreen>().Show();
        
    }

    [ContextMenu("Test Total Score")]
    public void UpdateFinalScore()
    {
        TotalScore = 0;
        foreach (var item in scoreTextList)
        {
            TotalScore=(int.Parse(item.text))+TotalScore;
            Debug.Log("scoreTextList"+item.text);
            Debug.Log("TotaScore"+ TotalScore);
            TotalScoreTxt.text = TotalScore.ToString();
        }
    }
}
