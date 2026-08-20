using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class NetworkErrorPopUpScreen : Singleton<NetworkErrorPopUpScreen>
{
    [SerializeField] Image _fadeImg;
    [SerializeField] TMP_Text _errorTxt;
    [SerializeField] GameObject _networkErrorPanel;

    private void OnEnable()
    {
        StartConnectionCheck();
    }

    private void OnDisable()
    {
    }

    internal void ShowExceptionPanelText(string text)
    {
        _fadeImg.gameObject.SetActive(true);
        _fadeImg.DOFade(0.7f, 0.2f);
        _networkErrorPanel.SetActive(true);
        _errorTxt.text = text;
    }

    internal void CloseAuthenticationPanelText()
    {
        _networkErrorPanel.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _networkErrorPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            _networkErrorPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fadeImg.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    _networkErrorPanel.SetActive(false);
                    _fadeImg.gameObject.SetActive(false);
                });
            });
        });
    }

    #region CONNECTIVITY CHECK API

    internal bool isConnected;
    internal bool printDebugMessages;
    internal bool IsTestingConnectivity { get; private set; } = false;
    internal Coroutine connectionTestCoroutine;

    internal bool IsConnected
    {
        get
        {
            return isConnected;
        }
        private set
        {
            PrintDebugMessage($"Is Connected:: {isConnected}", MessageType.Verbose);
        }
    }

    internal void StartConnectionCheck()
    {
        if (connectionTestCoroutine == null)
        {
            connectionTestCoroutine = StartCoroutine(TestConnection());
            IsTestingConnectivity = true;
        }
        else
        {
            PrintDebugMessage("Connection check already started!", MessageType.Warning);
        }
    }

    void OnPhotonDisconnect()
    {
        if (isConnected == false) return;

        ShowExceptionPanelText("Failed to Connect to Server. Please try again later");
    }

    internal IEnumerator TestConnection()
    {
        while (true)
        {
            if (!string.IsNullOrEmpty("https://www.google.com/"))
            {
                UnityWebRequest webRequest = new UnityWebRequest("https://www.google.com/");
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {

                    //Debug.Log("Internet Connection Successfully created");
                    if (Application.isEditor)
                    {
                        IsConnected = true;
                        CloseAuthenticationPanelText();
                        //if (PlayerPrefs.GetString(ConstantVariables._UserLoginState) == "false" && PlayerPrefs.HasKey(ConstantVariables._PlayerID)) // need to check if playerid or accesstoken is required
                        //{
                        //    AutoLogin.Instance.TryLogin(true);
                        //}
                    }
                    else
                    {
                        IsConnected = true;
                        CloseAuthenticationPanelText();
                        //if (PlayerPrefs.GetString(ConstantVariables._UserLoginState) == "false" && PlayerPrefs.HasKey(ConstantVariables._PlayerID))
                        //{
                        //    AutoLogin.Instance.TryLogin(true);
                        //}
                    }
                }
                else if (webRequest.result != UnityWebRequest.Result.InProgress)
                {
                    IsConnected = false;
                    ShowExceptionPanelText("Network failure. Please check your network connection and try again.");
                    //PlayerPrefs.SetString(ConstantVariables._UserLoginState, "false");
                    PrintDebugMessage($"Connection Error::{webRequest.error}", MessageType.Warning);
                    //if (SceneManager.GetActiveScene().usergueNameTxt != SceneLoader.Scene.Authorization.ToString()) SceneLoader.Instance.LoadScene(SceneLoader.Scene.Authorization);
                    Debug.Log("Internet Connection Failed");
                }
            }
            else
            {
                IsConnected = false;
                ShowExceptionPanelText("Network failure. Please check your network connection and try again.");
                //PlayerPrefs.SetString(ConstantVariables._UserLoginState, "false");
                PrintDebugMessage($"Ping URL in Connectivity Manager ( Inspector ) is missing", MessageType.Error);
                //if (SceneManager.GetActiveScene().usergueNameTxt != SceneLoader.Scene.Authorization.ToString()) SceneLoader.Instance.LoadScene(SceneLoader.Scene.Authorization);
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private void PrintDebugMessage(string msg, MessageType msgType)
    {
        if (printDebugMessages)
        {
            switch (msgType)
            {
                case MessageType.Warning:
                    Debug.LogWarning($"ConnectivityManager:: {msg}");
                    break;
                case MessageType.Error:
                    Debug.LogError($"ConnectivityManager:: {msg}");
                    break;
                default:
                    Debug.Log($"ConnectivityManager:: {msg}");
                    break;
            }

        }
    }

    private enum MessageType
    {
        Verbose,
        Warning,
        Error
    }

    #endregion



}
