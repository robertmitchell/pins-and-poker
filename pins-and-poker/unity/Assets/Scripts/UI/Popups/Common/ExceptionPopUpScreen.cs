using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExceptionPopUpScreen : Singleton<ExceptionPopUpScreen>    
{
    [SerializeField] Image _fadeImg;
    [SerializeField] Button _closeBtn;
    [SerializeField] TMP_Text _exceptionTxt;
    [SerializeField] GameObject _exceptionPanel;

  
    private void Start()
    {
        _closeBtn.onClick.AddListener(() => OnCloseBtnClick());
    }

    private void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
    }

    void OnCloseBtnClick()
    {
        _exceptionPanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(()=>
        {
            _exceptionPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _exceptionPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        });
    }

    internal void ShowExceptionPanelText(string text)
    {
        gameObject.SetActive(true);
        _exceptionTxt.text = text;
    }
}
