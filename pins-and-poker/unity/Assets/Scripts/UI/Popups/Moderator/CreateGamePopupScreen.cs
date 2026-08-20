using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class CreateGamePopupScreen : UIScreenBase
{
    [SerializeField] Image _fadeImg;
    [SerializeField] GameObject _creatGamePanel;
    [SerializeField] AnimatrixButton closeBtn;
    [SerializeField] AnimatrixButton createBtn;

    [SerializeField] Button AM_PM_Btn;
    [SerializeField] InputField gameNameInputfield;
    [SerializeField] InputField laneInputField;
    [SerializeField] InputField hoursTimeField;
    [SerializeField] InputField minutesTimeField;
    [SerializeField] Transform gamesContent;
    [SerializeField] internal LeagueGamePrefab leagueGamePrefab;

    TMP_Text AMPMText;
    Tweener _tweener;
    bool _closePopup;

    void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
    }

    void OnDisable()
    {
        ResetFields();
        CancelInvoke();
        _tweener.Kill();
    }

    void Start()
    {
        AMPMText = AM_PM_Btn.GetComponentInChildren<TMP_Text>();
        closeBtn.onClick.AddListener(() => CloseBtnClicked());
        AM_PM_Btn.onClick.AddListener(() => AMPMBtnClicked());
        createBtn.onClick.AddListener(() => CreateGameBtnClicked());
        hoursTimeField.onEndEdit.AddListener(delegate { FormatTimeInput(hoursTimeField, true); });
        minutesTimeField.onEndEdit.AddListener(delegate { FormatTimeInput(minutesTimeField, false); });
    }

    void CloseBtnClicked()
    {
        _closePopup = true;
        DisableGameObject();
    }

    void AMPMBtnClicked()
    {
        if (AMPMText.text == "AM")
        {
            AMPMText.text = "PM";
        }
        else
        {
            AMPMText.text = "AM";
        }
    }

    void CreateGameBtnClicked()
    {
        if (!string.IsNullOrEmpty(gameNameInputfield.text) && !string.IsNullOrEmpty(laneInputField.text) && !string.IsNullOrEmpty(hoursTimeField.text) && !string.IsNullOrEmpty(minutesTimeField.text))
        {
            UIManager.instance.GetScreen<LeagueScreen>().PauseApis();
            SendData();            
        }
        else
        {
            if (string.IsNullOrEmpty(gameNameInputfield.text)) ShowExceptionMessage("Please enter game name!", gameNameInputfield);
            if (string.IsNullOrEmpty(laneInputField.text)) ShowExceptionMessage("Please enter lane number!", laneInputField);
            if (string.IsNullOrEmpty(hoursTimeField.text)) ShowExceptionMessage("Enter hours!", hoursTimeField);
            if (string.IsNullOrEmpty(minutesTimeField.text)) ShowExceptionMessage("Enter mintues!", minutesTimeField);
            return;
        }
    }

    #region API CALL
    void SendData()
    {
        if (!string.IsNullOrEmpty(gameNameInputfield.text) && !string.IsNullOrEmpty(hoursTimeField.text) && !string.IsNullOrEmpty(minutesTimeField.text) && !string.IsNullOrEmpty(laneInputField.text))
        {
            string time = hoursTimeField.text + minutesTimeField.text + AMPMText.text;
            string gameName = gameNameInputfield.text;
            string laneName = laneInputField.text;
            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.gameName, gameName },
                { Db_Keys.laneName, laneName },
                { Db_Keys.start_time, time },
                { Db_Keys.leagueId, PlayerPrefs.GetString(Db_Keys.leagueId) }
            };

            WebServices.Instance.MakeRequest<CreateGame>(
                ApiRoutes.creatGame,
                WebServices.HttpMethod.POST,
                OnSuccess,
                OnFailure,
                null,
                formData,
                null,
                true
            );
        }        
    }

    void OnSuccess(CreateGame gameId, long statusCode)
    {
        DisableGameObject();
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite   );
    }
    #endregion

    #region TIME FORMAT CHECK
    void FormatTimeInput(InputField timeField, bool isHours)
    {
        if (isHours) // If it's the hours field
        {
            if (ValidateInputHours(timeField.text))
            {
                timeField.text = timeField.text.PadLeft(2, '0'); // Add leading zero if needed
                if (timeField.text == "00") timeField.text = "12";

            }
        }
        else // If it's the minutes field
        {
            if (ValidateInputMinutes(timeField.text))
            {
                timeField.text = timeField.text.PadLeft(2, '0'); // Add leading zero if needed

            }
        }
    }

    // Validation for hours input
    bool ValidateInputHours(string value)
    {
        if (int.TryParse(value, out int number) && number >= 0 && number <= 12)
        {
            Debug.Log("Valid hour input: " + number);
            return true;
        }
        else
        {
            Debug.Log("Invalid hour input: " + value);
            ShowExceptionMessage("12-hour format!", hoursTimeField);
            hoursTimeField.text = ""; // Clear invalid input
            return false;
        }
    }

    // Validation for minutes input
    bool ValidateInputMinutes(string value)
    {
        if (int.TryParse(value, out int number) && number >= 0 && number <= 59)
        {
            Debug.Log("Valid minute input: " + number);
            return true;
        }
        else
        {
            Debug.Log("Invalid minute input: " + value);
            ShowExceptionMessage("Invalid input!", minutesTimeField);
            minutesTimeField.text = ""; // Clear invalid input
            return false;
        }
    }
    #endregion

    #region EXCEPTION MESSAGE
    internal void ShowExceptionMessage(string msgTxt, InputField inputField, float duration = 1f)
    {
        inputField.placeholder.gameObject.SetActive(false);
        inputField.placeholder.gameObject.SetActive(true);
        ((Text)inputField.placeholder).text = msgTxt;
    }

    void DisableGameObject()
    {
        UIManager.instance.GetScreen<LeagueScreen>().ResumeApis();

        _tweener = _creatGamePanel.transform.DOScale(1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _creatGamePanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _creatGamePanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    if (_closePopup)
                    {
                        UIManager.instance.Hide();
                        UIManager.instance.Show<LeagueScreen>();
                    }
                    else
                    {
                        if (!gamesContent.gameObject.activeSelf) gamesContent.gameObject.SetActive(true);
                        UIManager.instance.Hide();
                        UIManager.instance.GetScreen<LeagueScreen>().Hide();
                        UIManager.instance.Show<LeagueScreen>();
                    }
                    
                });
            });
        });
    }
    #endregion

    void ResetFields()
    {
        ((Text)gameNameInputfield.placeholder).text = "Enter Game Name...";
        ((Text)laneInputField.placeholder).text = "Enter Lane Number...";
        ((Text)hoursTimeField.placeholder).text = "Hours";
        ((Text)minutesTimeField.placeholder).text = "Minutes";
        gameNameInputfield.text = string.Empty;
        laneInputField.text = string.Empty;
        hoursTimeField.text = string.Empty;
        minutesTimeField.text = string.Empty;
        _closePopup = false;
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
