using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] internal string currentScene;
    internal AsyncOperation asyncLoad;
    public enum Scene
    {
        Splash = 0,
        Gameplay = 1,
        Moderator = 2
    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }

    public void LoadScene(Scene sceneName, float delay = 0.5f, bool state = true)
    {
        StartCoroutine(_LoadScene(sceneName, delay, state));
    }

    public IEnumerator _LoadScene(Scene sceneName, float delay, bool state = true)
    {
        currentScene = SceneManager.GetActiveScene().name;
        WaitingLoaderCanvas.Instance.Show();
        //SceneLoaderCanvas.Instance.EnableDisableScreen(state);
        asyncLoad = SceneManager.LoadSceneAsync((int)sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f) // Check if the scene is almost loaded
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }


        //Debug.Log(sceneName + " was Loaded asyncly!");
        currentScene = sceneName.ToString();
        WaitingLoaderCanvas.Instance.Hide();
        //SceneLoaderCanvas.Instance.EnableDisableScreen(false);
        //FadeOutCanvas.Instance.PlayFadeOutEffect(1f);

    }

    public IEnumerator Unloadscene(string oldSceneName)
    {
        //SceneLoaderCanvas.Instance.EnableDisableScreen(true);

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(oldSceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        if (asyncUnload.isDone)
        {
            Debug.Log(oldSceneName + " was unloaded asyncly!");
            //SceneLoaderCanvas.Instance.EnableDisableScreen(false);
        }
    }
}
