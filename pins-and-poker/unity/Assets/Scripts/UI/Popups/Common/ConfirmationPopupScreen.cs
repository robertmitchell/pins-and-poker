using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopupScreen : Singleton<ConfirmationPopupScreen>
{
    [SerializeField] Image _fadeImg;
    [SerializeField] TMP_Text _messageTxt;
    [SerializeField] TMP_Text _titleTxt;
    [SerializeField] GameObject _confirmationPanel;
    [SerializeField] Image _iconImg;
    [SerializeField] AnimatrixButton _yesBtn;
    [SerializeField] AnimatrixButton _noBtn;
    [SerializeField] List<Sprite> _iconslist;
    Tweener _tweener;
    Action _OnDefaultBtnPressed;

    private void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
    }

    private void OnDisable()
    {
        CancelInvoke();
        _tweener.Kill();
        ResetObjects();
    }


    public void OnUserClickYesNo(int choice)
    {
        if (choice == 1)
        {
            if (_OnDefaultBtnPressed != null) { _OnDefaultBtnPressed?.Invoke(); }
            _OnDefaultBtnPressed = null;
            DisableGameObject();
        }
        else DisableGameObject();
    }

    void DisableGameObject()
    {
        _tweener = _confirmationPanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _confirmationPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _confirmationPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
                {      
                    _OnDefaultBtnPressed = null;
                    gameObject.SetActive(false);
                });
            });
        });
    }

    internal void ShowConfirmationMessage(string msgTxt, string titlTxt, string nobtnTxt = null, string yesbtnTxt = null, Action onBtnPresed = null, int iconIndex = 0)
    {
        CancelInvoke();
        gameObject.SetActive(true);
        _messageTxt.text = msgTxt;
        _titleTxt.text = titlTxt;
        if (nobtnTxt != null) _noBtn.GetComponentInChildren<TMP_Text>().text = nobtnTxt;
        if (yesbtnTxt != null) _yesBtn.GetComponentInChildren<TMP_Text>().text = yesbtnTxt;
        if (iconIndex <= 2) _iconImg.sprite = _iconslist[iconIndex];
        else Debug.Log("Index out of range");
        if (onBtnPresed != null)
        {
            _OnDefaultBtnPressed = onBtnPresed;
        }
    }

    private void ResetObjects()
    {
        _titleTxt.text = string.Empty;
        _iconImg.sprite = _iconslist[0];
        _noBtn.GetComponentInChildren<TMP_Text>().text = "No";
        _yesBtn.GetComponentInChildren<TMP_Text>().text = "Yes";
    }
}
