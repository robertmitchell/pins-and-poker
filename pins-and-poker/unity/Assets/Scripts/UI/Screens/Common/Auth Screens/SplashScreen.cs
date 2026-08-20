using System;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreen : UIScreenBase
{
    public Image loaderImage;
    public Image bgImage;
    public float animationTime = 5f;

    private void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
    }

    void Start()
    {
        loaderImage.fillAmount = 0f;
        FillLoader();
    }

    void FillLoader()
    {
        LeanTween.value(gameObject, UpdateLoaderFill, 0f, 1f, animationTime)
            .setEase(LeanTweenType.linear)
            .setOnComplete(OnFillComplete);
    }

    void UpdateLoaderFill(float fillAmount)
    {
        loaderImage.fillAmount = fillAmount;
    }

    void OnFillComplete()
    {              
        if (PlayerPrefs.HasKey(Db_Keys.islogedIn))
        {
            CharacterAnimationCanvas.Instance?.PlayChrAnimation(AnimationNames.thumbsBall);
            SceneLoader.Instance.LoadScene(SceneLoader.Scene.Gameplay);
        }
        else
        {
            UIManager.instance.Hide();
            UIManager.instance.Show<RoleSelectionScreen>();
            bgImage.gameObject.SetActive(true);
        }
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new NotImplementedException();
    }
}
