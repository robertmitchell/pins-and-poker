using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class AssignLanePopupScreen : Singleton<AssignLanePopupScreen>
{
    [SerializeField] Image _fadeImg;
    [SerializeField] AnimatrixButton assignBtn;
    [SerializeField] AnimatrixButton closeBtn;
    [SerializeField] GameObject _assignLanePanel;
    [SerializeField] InputField assignLaneInputField;
    [SerializeField]  TMP_Text playerNameTxt;
    public RawImage profileImg;
    //public Texture texture;
    Tweener _tweener;

    public User userdata;
    private void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
        playerNameTxt.text = userdata.Username;
    }
   
   
    private void OnDisable()
    {
        ResetFields();
        CancelInvoke();
        _tweener.Kill();
    }

    private void Start()
    {
        assignBtn.onClick.AddListener(() => AssignBtnClicked());
        closeBtn.onClick.AddListener(() => DisableGameObject());
    }

    void AssignBtnClicked()
    {
        if (!string.IsNullOrEmpty(assignLaneInputField.text))
        {

            Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.playerID, userdata.PlayerId },
            { Db_Keys.gameId, PlayerPrefs.GetString(Db_Keys.gameId)  },
            { Db_Keys.assignedLane,  assignLaneInputField.text }
        };
            WebServices.Instance.MakeRequest<ResponseData>(ApiRoutes.assignLane, WebServices.HttpMethod.POST, OnSuccess, OnFail, null, formData, null, true);


        }
        else
        {
            if (string.IsNullOrEmpty(assignLaneInputField.text)) ShowExceptionMessage("Please assign lane!", assignLaneInputField);
            return;
        }
    }

    private void OnSuccess(ResponseData response, long arg2)
    {
        Debug.Log("OnSuccess Invoked");
        DisableGameObject();

    }



    private void OnFail(string obj)
    {
        Debug.Log("OnFail Invoked");
        MessagePopUpScreen.Instance.ShowMessage(obj, "Response", "OK", null, true);
        Debug.LogError("error: " + obj);
    }

    internal void ShowExceptionMessage(string msgTxt, InputField inputField, float duration = 1f)
    {
        inputField.placeholder.gameObject.SetActive(false);
        inputField.placeholder.gameObject.SetActive(true);
        ((Text)inputField.placeholder).text = msgTxt;
    }

    void DisableGameObject()
    {
        _tweener = _assignLanePanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _assignLanePanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _assignLanePanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
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
        assignLaneInputField.text = string.Empty;
        ((Text)assignLaneInputField.placeholder).text = "Enter Lane...";
    }
}
