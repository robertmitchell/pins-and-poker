using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterAnimationCanvas : Singleton<CharacterAnimationCanvas>
{
    public Animator animator;
    public GameObject characterSprite;

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
        characterSprite.SetActive(true);
    }

    public void Hide()
    {
        characterSprite.SetActive(false);
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

    public void PlayChrAnimation(AnimationNames triggerName)
    {
        Show();
        animator.SetTrigger(triggerName.ToString());
    }
}


public enum AnimationNames
{
    happy, happyThumbsUp, thumbsUp, singleThumbsUp, smillingShy, shocking, thumbsBall, waving
}

