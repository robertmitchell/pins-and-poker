using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class EditDisputePopup : Singleton<EditDisputePopup> , ISelectHandler, IDeselectHandler
{
    [SerializeField] Image _fadeImg;
    [SerializeField] GameObject _editDisputePanel;
    [SerializeField] Button _closeBtn;
    [SerializeField] Button _editButton;
    [SerializeField] Button _disputeButton;
    [SerializeField] internal Button _strikeButton;
    [SerializeField] internal Button _spareButton;
    [SerializeField] internal InputField _editInputField;
    [HideInInspector] internal TMP_InputField _selectedInputField;
    Tweener _tweener;
    public bool isPlayerCell = false;
    public int selectedCellIndex;

    void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
        if (Global.UserType.user.ToString() == PlayerPrefs.GetString(Db_Keys.userType.ToString()))
        {
            FieldsToShow();
        }
        SpareAndStrikeButton();
    }

    void OnDisable()
    {
        ResetFields();
        CancelInvoke();
        _tweener.Kill();
    }

    private void Start()
    {
        _closeBtn.onClick.AddListener(() => DisableGameObject());
        _editButton.onClick.AddListener(OnEditButtonClicked);
        _disputeButton.onClick.AddListener(OnDisputeButtonClicked);
        _strikeButton.onClick.AddListener(() => ChangeText("X"));
        _spareButton.onClick.AddListener(() => ChangeText("/"));
    }

    public void OnSelect(BaseEventData eventData) // reference in editor
    {
        _editInputField.contentType = InputField.ContentType.IntegerNumber;
        Debug.Log(this.gameObject.name + " was selected");
    }

    public void OnDeselect(BaseEventData eventData) // reference in editor
    {
        _editInputField.contentType = InputField.ContentType.Standard;
        Debug.Log(this.gameObject.name + " was selected");
    }

    void ChangeText(string text)
    {
        _editInputField.text = text;
    }

    void FieldsToShow()
    {      
        _editInputField.gameObject.SetActive(isPlayerCell);
        _editButton.gameObject.SetActive(isPlayerCell);
        _disputeButton.gameObject.SetActive(!isPlayerCell);  
    }

    void SpareAndStrikeButton()
    {

        if (selectedCellIndex == 19)
        {
            string eighteenthScoreCell = _selectedInputField.GetComponentInParent<ScorePrefab>().inputField1.symboltext.text;

            if (eighteenthScoreCell.Contains("X"))
            {
                _spareButton.gameObject.SetActive(false);
                _strikeButton.gameObject.SetActive(true);
                return;
            }
        }
        else if (selectedCellIndex == 20)
        {
            string nineteenthScoreCell = _selectedInputField.GetComponentInParent<ScorePrefab>().inputField2.symboltext.text;
            if (nineteenthScoreCell.Contains("X") || nineteenthScoreCell.Contains("/"))
            {
                _spareButton.gameObject.SetActive(false);
                _strikeButton.gameObject.SetActive(true);
                return;
            }
            else
            {
                _spareButton.gameObject.SetActive(true);
                _strikeButton.gameObject.SetActive(false);
                return;
            }
        }

        if (selectedCellIndex % 2 != 0)
        {
            Debug.Log("cellIndex " + selectedCellIndex);
            _spareButton.gameObject.SetActive(true);
            _strikeButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("cellIndex " + selectedCellIndex);
            _spareButton.gameObject.SetActive(false);
            _strikeButton.gameObject.SetActive(true);
        }
    }

    void OnEditButtonClicked()
    {
        Debug.Log("Edit Button Clicked");
        if (!string.IsNullOrEmpty(_editInputField.text) && _selectedInputField != null)
        {
            if (_editInputField.text.Contains("X")) _editInputField.text = "10";
            if (_editInputField.text.Contains("/"))
            {
                int scoreCell = selectedCellIndex == 20 ? int.Parse(_selectedInputField.GetComponentInParent<ScorePrefab>().inputField2.inputField.text) : int.Parse(_selectedInputField.GetComponentInParent<ScorePrefab>().inputField1.inputField.text);
                _editInputField.text = (10 - scoreCell).ToString();  
            }
            _selectedInputField.text = _editInputField.text;
            APIInvoker.Instance.RemoveApiRequest(UIManager.instance.GetScreen<TableDataManager>().SendRequestToGetScores);
            _selectedInputField.GetComponent<ScoreCell>().SaveRequest(selectedCellIndex);
            //DisableGameObject();
        }
        else
        {
            if (string.IsNullOrEmpty(_editInputField.text)) ShowExceptionMessage("Please enter your score!", _editInputField);
            return;
        }
    }

    void OnDisputeButtonClicked()
    {
        Debug.Log("Dispute Button Clicked");
        APIInvoker.Instance.RemoveApiRequest(UIManager.instance.GetScreen<TableDataManager>().SendRequestToGetScores);

        string disputeAgainstID = _selectedInputField.GetComponentInParent<ScoreManager>().playerScoreCardData.PlayerId;

        Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.gameId, PlayerPrefs.GetString(Db_Keys.gameId)  },
                { Db_Keys.moderatorId, PlayerPrefs.GetString(Db_Keys.moderatorId) },
                { Db_Keys.playerID, PlayerPrefs.GetString(Db_Keys.playerID)  },
                { Db_Keys.disputedgainstId, disputeAgainstID  },
                { Db_Keys.disputerId, PlayerPrefs.GetString(Db_Keys.playerID)    },
                { Db_Keys.cell_index, selectedCellIndex.ToString()  },
            };
        //Hit the update score request here
        WebServices.Instance.MakeRequest<CreateDisputeResponse>(
            ApiRoutes.disputeRequest,
            WebServices.HttpMethod.POST,
            OnDisputeRequestSuccess,
            OnDisputeRequestFail,
            null,
            formData,
            null,
            true);
    }

    void OnDisputeRequestSuccess(CreateDisputeResponse response, long code)
    {
        MessagePopUpScreen.Instance.ShowMessage("Dispute Created Successfully", "Dispute","OK", OkBtnClicked, true);
    }

    void OkBtnClicked()
    {
        APIInvoker.Instance.AddApiRequest(UIManager.instance.GetScreen<TableDataManager>().SendRequestToGetScores, 4f);
        DisableGameObject();
    }

    void OnDisputeRequestFail(string resp)
    {
        MessagePopUpScreen.Instance.ShowMessage("Failed to Create Dispute", "Failed",null,null, MessagePopUpScreen.Instance._wrongSprite);
    }

    internal void ShowExceptionMessage(string msgTxt, InputField inputField, float duration = 1f)
    {
        inputField.placeholder.gameObject.SetActive(false);
        inputField.placeholder.gameObject.SetActive(true);
        ((Text)inputField.placeholder).text = msgTxt;
    }

    internal void DisableGameObject()
    {
        _tweener = _editDisputePanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _editDisputePanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _editDisputePanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        });
    }

    void ResetFields()
    {
        //_selectedInputField = null;
        _editInputField.text = string.Empty;
        ((Text)_editInputField.placeholder).text = "Enter New Score...";
    }
}
