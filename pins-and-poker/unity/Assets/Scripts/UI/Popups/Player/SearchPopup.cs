using DG.Tweening;
using System.Collections.Generic;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class SearchPopup : UIScreenBase
{
    [SerializeField] Image _fadeImg;
    [SerializeField] AnimatrixButton searchBtn;
    [SerializeField] AnimatrixButton closeBtn;
    [SerializeField] GameObject _searchPanel;
    [SerializeField] InputField SearchInputField;
    Tweener _tweener;

    private void OnEnable()
    {
        _fadeImg.DOFade(0.7f, 0.2f);
    }

    private void OnDisable()
    {
        ResetFields();
        CancelInvoke();
        _tweener.Kill();
    }

    private void Start()
    {
        searchBtn.onClick.AddListener(() => SearchBtnClicked());
        closeBtn.onClick.AddListener(() => DisableGameObject());
    }

    void SearchBtnClicked()
    {
        if (!string.IsNullOrEmpty(SearchInputField.text))
        {
            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { Db_Keys.searchTerm, SearchInputField.text}
            };
            WebServices.Instance.MakeRequest<SearchResult>(
               ApiRoutes.userSearch,
               WebServices.HttpMethod.GET,
               OnSuccess,
               OnFailure,
               null,
               formData,
               null,
               true
               );

        }
        else
        {
            if (string.IsNullOrEmpty(SearchInputField.text)) ShowExceptionMessage("Search Is Empty", SearchInputField);
            return;
        }
    }
    void OnSuccess(SearchResult searchObj, long statusCode)
    {
        if (searchObj == null || searchObj.Leagues == null)
        {
            Debug.LogError("Notifications or Leagues is null.");
            MessagePopUpScreen.Instance?.ShowMessage("Failed to retrieve league information.", "Error", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
            UIManager.instance.GetScreen<SearchPopup>().Hide();
            return;
        }
        if (searchObj.Leagues.Count != 0)
        {
            UIManager.instance.GetScreen<SearchPopup>().Hide();

            UIManager.instance.GetScreen<SearchScreen>().searchObj = searchObj;
            UIManager.instance.GetScreen<HomeScreen>().Hide();
        }
        else
        {
            MessagePopUpScreen.Instance.ShowMessage("No leagues found matching your search.", "Result", "OK", ShowSearchScreenWithDelay, true, MessagePopUpScreen.Instance._wrongSprite);
              return;
        }

        UIManager.instance.Hide();
        UIManager.instance.Show<SearchScreen>();
    }

    private void ShowSearchScreenWithDelay()
    {
        UIManager.instance.GetScreen<SearchPopup>().Hide();
        UIManager.instance.Hide();
        UIManager.instance.Show<SearchScreen>();
    }


    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
    }

    internal void ShowExceptionMessage(string msgTxt, InputField inputField, float duration = 1f)
    {
        inputField.placeholder.gameObject.SetActive(false);
        inputField.placeholder.gameObject.SetActive(true);
        ((Text)inputField.placeholder).text = msgTxt;
    }

    void DisableGameObject()
    {



        _tweener = _searchPanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _searchPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _searchPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
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
        SearchInputField.text = string.Empty;
        ((Text)SearchInputField.placeholder).text = "Search...";
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
