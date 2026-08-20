using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIFadeEffects : MonoBehaviour
{
    [SerializeField] EffectTypes _effectType;

    [Space]
    [Header("Settings")]

    [SerializeField] float endValue;
    [SerializeField] float duration;
    [SerializeField] float delay;
    [SerializeField] float resetCanvasAlpha;

    bool isPlayingEffect;

    [System.Serializable]
    public enum EffectTypes
    {
        SimpleFade,
        GroupFade
    }

    private void Awake()
    {
        if (gameObject.GetComponent<CanvasGroup>() == null) gameObject.AddComponent<CanvasGroup>().alpha = 0;
    }

    private void OnEnable()
    {
        PlayEffect();
    }

    private void OnDisable()
    {
        gameObject.GetComponent<CanvasGroup>().alpha = resetCanvasAlpha;
    }

    void PlayEffect()
    {
        if (isPlayingEffect) return;
        isPlayingEffect = true;
        switch (_effectType)
        {
            case EffectTypes.SimpleFade:
                SimpleFade();
                break;
            case EffectTypes.GroupFade:
                GroupFade();
                break;
        }
    }

    #region TRANSITION EFFECTS

    void SimpleFade()
    {
        //if (gameObject.GetComponent<CanvasGroup>() == null) gameObject.AddComponent<CanvasGroup>();
        gameObject.GetComponent<CanvasGroup>().DOFade(endValue, duration).SetDelay(delay).OnComplete(() => isPlayingEffect = false);
    }

    void GroupFade()
    {
        
    }

    #endregion
}
