using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitingLoaderCanvas : Singleton<WaitingLoaderCanvas>
{
    public GameObject waitingLoader;
    public GameObject CardsLoader;

    public override void Awake()
    {
        base.Awake();
        AssignCamera();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        Hide();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignCamera();  
    }

    public void Show()
    {
        waitingLoader.SetActive(true);
    }

    public void Hide()
    {
        waitingLoader.SetActive(false);
    }

    public void ShowCardsLoader()
    {
        CardsLoader.SetActive(true);
    }

    public void HideCardsLoader()
    {
        CardsLoader.SetActive(false);
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
}
