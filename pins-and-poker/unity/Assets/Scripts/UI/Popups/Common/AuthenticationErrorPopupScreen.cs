using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthenticationErrorPopupScreen : Singleton<AuthenticationErrorPopupScreen>
{
    [SerializeField] Image _fadeImg;
    [SerializeField] TMP_Text _errorTxt;
    [SerializeField] GameObject _authenticationErrorPanel;

    private void Start()
    {
        EnableDisableScreen(false);
    }

    private void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
    }

    internal void ShowAuthenticationPanelText(string text)
    {
        gameObject.SetActive(true);
        _errorTxt.text = text;
        //OnUnathunticatedError();
    }

    public void CloseAuthenticationPanelText()
    {
        _authenticationErrorPanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _authenticationErrorPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _authenticationErrorPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        });
    }

    //void OnUnathunticatedError()
    //{
    //    Invoke(nameof(UnathunticatedError), 4f);
    //}

    //void UnathunticatedError()
    //{
    //    if (SceneManager.GetActiveScene().buildIndex != 0)
    //    {
    //        SceneLoader.Instance.LoadScene(SceneLoader.Scene.UI, 0f);
    //        if (SceneLoader.Instance.asyncLoad.isDone) Logout.Instance.OnSuccess("Logout");
    //    }
    //    else Logout.Instance.OnSuccess("Logout");
    //    gameObject.SetActive(false);
    //}
}
