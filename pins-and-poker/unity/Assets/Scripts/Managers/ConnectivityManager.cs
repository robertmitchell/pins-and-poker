using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ConnectivityManager : Singleton<ConnectivityManager>
{
    public GameObject errorPopup;

    public event Action OnInternetConnected;
    public event Action OnInternetDisconnected;

    private bool isConnected;
    private float checkInterval = 5f; // Check every 5 seconds
    private string testUrl = "https://www.google.com"; // URL to check connectivity

    //public override void Awake()
    //{
    //    base.Awake();
    //}
    public override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private void Start()
    {
        isConnected = Application.internetReachability != NetworkReachability.NotReachable;
        StartCoroutine(CheckInternetConnectivity());
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignCamera();
    }
    void AssignCamera()
    {
        Camera mainCamera = Camera.main;
        Canvas canvas = GetComponent<Canvas>();

        if (canvas != null && mainCamera != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = mainCamera;
        }
        else
        {
            Debug.LogWarning("Canvas or Main Camera not found!");
        }
    }
    private IEnumerator CheckInternetConnectivity()
    {
        while (true)
        {
            bool previousConnectionStatus = isConnected;
            yield return StartCoroutine(TestInternetConnection((connected) =>
            {
                isConnected = connected;
                if (isConnected && !previousConnectionStatus)
                {
                    OnInternetConnected?.Invoke();
                }
                else if (!isConnected && previousConnectionStatus)
                {
                    OnInternetDisconnected?.Invoke();
                }
            }));
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator TestInternetConnection(Action<bool> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(testUrl))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true);
                errorPopup.SetActive(false);

            }
            else
            {
                callback?.Invoke(false);
                errorPopup.SetActive(true);
            }
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }
}
