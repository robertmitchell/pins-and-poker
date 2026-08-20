using DG.Tweening;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class CreateEditLeagueScreen : UIScreenBase
{
    public Button backBtn;
    public Button AM_PM_Btn;
    public AnimatrixButton uploadBtn;
    public AnimatrixButton continueBtn;

    public InputField leagueNameField;
    public InputField hoursTimeField;
    public InputField minutesTimeField;
    public InputField prizePoolField;
    public InputField startDateField;
    public TMP_Text headerTxt;
    public GameObject uploadPnl;
    public RawImage leagueRawImage;
    public Texture2D defaultPic;
    public bool isEdit;
    TMP_Text AMPMText;

    // Frequency options: "Once", "Every week", "Every two weeks", "Monthly"
    public TMPro.TMP_Dropdown frequencyDropdown;
    private readonly string[] frequencyOptions = { "Once", "Every week", "Every two weeks", "Monthly" };


    void OnEnable()
    {
        ChangeReferences();
        UIManager.instance.GetScreen<ManageRulesScreen>().specialRules.Clear();
    }

    void OnDisable()
    {       
        CancelInvoke();        
    }

    void Start()
    {
        AMPMText = AM_PM_Btn.GetComponentInChildren<TMP_Text>();
        backBtn.onClick.AddListener(() => BackBtnClicked());
        uploadBtn.onClick.AddListener(() => UploadBtnClicked());
        AM_PM_Btn.onClick.AddListener(() => AMPMBtnClicked());
        continueBtn.onClick.AddListener(() => ContinueBtnClicked());
        hoursTimeField.onEndEdit.AddListener(delegate { FormatTimeInput(hoursTimeField, true); });
        minutesTimeField.onEndEdit.AddListener(delegate { FormatTimeInput(minutesTimeField, false); });
        if (frequencyDropdown != null)
        {
            frequencyDropdown.ClearOptions();
            frequencyDropdown.AddOptions(new System.Collections.Generic.List<string>(frequencyOptions));
            frequencyDropdown.value = 1; // Default to "Every week"
        }
    }

    void BackBtnClicked()
    {
        if (isEdit)
        {
            ResetFields();
            UIManager.instance.Hide();
            UIManager.instance.Show<LeagueScreen>();
            UIManager.instance.GetScreen<LeagueScreen>().ResumeApis();
        }
        else
        {
            ResetFields();
            UIManager.instance.Hide();
            UIManager.instance.Show<HomeScreen>();
        }
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

    #region CREATE LEAGUE DATA
    void UploadBtnClicked()
    {
        PickImage();
    }

    void ContinueBtnClicked()
    {
        SaveData();
    }

    void PickImage()
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize: 1024, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                //LeagueImage.sprite = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f));
                leagueRawImage.texture = texture;
                leagueRawImage.gameObject.SetActive(true);
                uploadBtn.GetComponent<Image>().enabled = false;
                uploadPnl.SetActive(false);
            }
        }, "Select an image", "image/*");

        Debug.Log("Permission result: " + permission);
    }

    void SaveData()
    {
        bool hasDate = startDateField == null || !string.IsNullOrEmpty(startDateField.text);
        if (!string.IsNullOrEmpty(leagueNameField.text) && !string.IsNullOrEmpty(hoursTimeField.text) && !string.IsNullOrEmpty(minutesTimeField.text) && !string.IsNullOrEmpty(prizePoolField.text) && hasDate)
        {
            string leagueName = leagueNameField.text;
            string prizePool = prizePoolField.text;
            string time = hoursTimeField.text + minutesTimeField.text + AMPMText.text;
            string frequency = frequencyDropdown != null ? frequencyOptions[frequencyDropdown.value] : "Once";
            string startDate = startDateField != null ? startDateField.text : "";

            ManageRulesScreen.leagueformData = new()
            {
                { Db_Keys.leagueName, leagueName },
                { Db_Keys.prizePool, prizePool },
                { Db_Keys.start_time, time },
                { Db_Keys.start_date, startDate },
                { Db_Keys.frequency, frequency },
            };

            UIManager.instance.Hide();
            UIManager.instance.Show<ManageRulesScreen>();
        }
        else
        {
            if (string.IsNullOrEmpty(leagueNameField.text)) ShowExceptionMessage("Please enter league name!", leagueNameField);
            if (string.IsNullOrEmpty(hoursTimeField.text)) ShowExceptionMessage("Enter hours!", hoursTimeField);
            if (string.IsNullOrEmpty(minutesTimeField.text)) ShowExceptionMessage("Enter mintues!", minutesTimeField);
            if (string.IsNullOrEmpty(prizePoolField.text)) ShowExceptionMessage("Please enter prize pool!", prizePoolField);
            if (startDateField != null && string.IsNullOrEmpty(startDateField.text)) ShowExceptionMessage("Please enter start date!", startDateField);
            return;
        }
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

    #region EXCEPTION MESSGAGE
    internal void ShowExceptionMessage(string msgTxt, InputField inputField)
    {
        inputField.placeholder.gameObject.SetActive(false);
        inputField.placeholder.gameObject.SetActive(true);
        ((Text)inputField.placeholder).text = msgTxt;
    }
    #endregion

    void ChangeReferences()
    {
        if (isEdit) headerTxt.text = "Edit League";
        else headerTxt.text = "Create League";
    }

    internal void ResetFields()
    {
        ((Text)leagueNameField.placeholder).text = "Enter League Name...";
        ((Text)prizePoolField.placeholder).text = "Enter Prize Pool...";
        ((Text)hoursTimeField.placeholder).text = "Hours";
        ((Text)minutesTimeField.placeholder).text = "Minutes";
        leagueNameField.text = string.Empty;
        hoursTimeField.text = string.Empty;
        prizePoolField.text = string.Empty;
        if (startDateField != null) { startDateField.text = string.Empty; ((Text)startDateField.placeholder).text = "Start Date (MM/DD/YYYY)..."; }
        if (frequencyDropdown != null) frequencyDropdown.value = 1;
        leagueRawImage.texture = defaultPic;
        uploadPnl.SetActive(true);
        leagueRawImage.gameObject.SetActive(false);
        uploadBtn.GetComponent<Image>().enabled = true;
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
