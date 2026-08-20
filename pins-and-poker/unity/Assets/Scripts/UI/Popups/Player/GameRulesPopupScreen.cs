using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class GameRulesPopupScreen : Singleton<GameRulesPopupScreen>
{
    [SerializeField] Image _fadeImg;
    [SerializeField] GameObject _rulesPanel;
    [SerializeField] AnimatrixButton _continueBtn;
    [SerializeField] AnimatrixButton _closeBtn;
    [SerializeField] Transform _rulesContent;
    [SerializeField] GameObject _rulePrefab;
    Tweener _tweener;
    List<Rule> _rules = new();
    Action _OnContinueBtnPressed;

    private void Start()
    {
        _closeBtn.onClick.AddListener(() => DisableGameObject(true));
        _continueBtn.onClick.AddListener(() => DisableGameObject());
    }

    private void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
        SetRules();
    }

    private void OnDisable()
    {
        CancelInvoke();
        _tweener.Kill();
        foreach (Transform item in _rulesContent)
        {
            Destroy(item.gameObject);
        }
    }

    void SetRules()
    {
        Debug.Log("Rules Count: " + _rules.Count);
        foreach (var rule in _rules)
        {
            var obj = Instantiate(_rulePrefab, _rulesContent);
            obj.GetComponentInChildren<TMP_Text>().text = rule.description; 
        }
    }

    void DisableGameObject(bool isAction = false)
    {
        _tweener = _rulesPanel.transform.DOLocalMoveX(-840f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _rulesPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
            {
                if (!isAction)
                {
                    if (_OnContinueBtnPressed != null) { _OnContinueBtnPressed?.Invoke(); }
                }
                _OnContinueBtnPressed = null;
                gameObject.SetActive(false);
            });
        });
    }

    internal void ShowGameRulesPnl(Action onBtnPresed = null, List<Rule> rules = null)
    {
        CancelInvoke();
        if (rules != null) _rules = rules;
        gameObject.SetActive(true);
        if (onBtnPresed != null) _OnContinueBtnPressed = onBtnPresed;    
    }
}
