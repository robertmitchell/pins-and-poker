using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeOutCanvas : Singleton<FadeOutCanvas>
{
    public void PlayFadeOutEffect(float duaration = 0.4f)
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            if (TryGetComponent(out Image image))
            {
                image.GetComponent<Image>().DOFade(0f, duaration).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    image.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
                });
            }
        }
    }
}
