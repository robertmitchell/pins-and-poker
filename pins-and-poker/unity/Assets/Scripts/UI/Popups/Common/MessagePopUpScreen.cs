using DG.Tweening;
using System;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopUpScreen : Singleton<MessagePopUpScreen>
{
    [SerializeField] Image _fadeImg;
    //[SerializeField] Image wrongSprite;
    public Sprite _wrongSprite;
    [SerializeField] Sprite _defaultSprite;
    [SerializeField] Image _iconImage;
    [SerializeField] TMP_Text _messageTxt;
    [SerializeField] GameObject _titlePanel;
    [SerializeField] GameObject _messagePanel;
    [SerializeField] AnimatrixButton _defaultBtn;
    [SerializeField] float _duration = 4f;
    Tweener _tweener;
    Action _OnDefaultBtnPressed;

    public override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    private void Start()
    {   
        _defaultBtn.onClick.AddListener(() => DisableGameObject());
    }

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

    void DisableGameObject()
    {
        BGMusic.Instance.btn_audioSource.Play();
        _tweener = _messagePanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _messagePanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _messagePanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    if (_OnDefaultBtnPressed != null) { _OnDefaultBtnPressed?.Invoke(); }
                    _OnDefaultBtnPressed = null;
                    gameObject.SetActive(false);
                });
            });
        });
    }

    internal void ShowMessage(string msgTxt, string titlTxt, string btnTxt = null, Action onBtnPresed = null, bool isTitlePnlOff = false, Sprite wrongSprite=null)
    {
        CancelInvoke();
        gameObject.SetActive(true);
        _messageTxt.text = msgTxt;
        _titlePanel.GetComponentInChildren<TMP_Text>().text = titlTxt;
        if (wrongSprite!=null)
        {
            _iconImage.sprite = wrongSprite;
        }
        if (btnTxt != null) _defaultBtn.GetComponentInChildren<TMP_Text>().text = btnTxt;
        if(isTitlePnlOff) _titlePanel.SetActive(false);
        if (onBtnPresed != null) 
        {
            _OnDefaultBtnPressed = onBtnPresed;        
        }
    }

    private void ResetObjects()
    {
        _iconImage.sprite = _defaultSprite;
        _titlePanel.SetActive(true);
        _defaultBtn.gameObject.SetActive(true);
    }
}
