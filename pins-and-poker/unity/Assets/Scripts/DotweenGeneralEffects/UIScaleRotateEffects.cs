using DG.Tweening;
using UnityEngine;

public class UIScaleRotateEffects : MonoBehaviour
{
    [SerializeField] EffectTypes _effectType;

    [Space]
    [Header("Settings")]

    [SerializeField] Vector3 endValue;
    [SerializeField] float duration;
    [SerializeField] float delay;
    [SerializeField] int loopCount;
    [SerializeField] LoopType loopType;
    [SerializeField] Ease easingEffect;
    [SerializeField] float vibrato;
    [SerializeField] float elasticity;

    Tweener tweener;
    bool isPlayingEffect;

    [System.Serializable]
    public enum EffectTypes
    {
        RotateEffect,
        PunchScaleEffect,
        GroupScaleEffect
    }

    private void OnEnable()
    {
        PlayEffect();
    }

    private void OnDisable()
    {
        foreach (Transform item in transform)
        {
            item.DOScale(0f, 0f);
        }
    }

    void PlayEffect()
    {
        if (isPlayingEffect) return;
        isPlayingEffect = true;
        switch (_effectType)
        {
            case EffectTypes.RotateEffect:
                RotateEffect();
                break;
            case EffectTypes.PunchScaleEffect:
                PunchScaleEffect();
                break;
            case EffectTypes.GroupScaleEffect:
                GroupScaleEffect();
                break;
        }
    }

    #region TRANSITION EFFECTS

    void RotateEffect()
    {
        tweener = transform.DORotate(endValue, duration).SetLoops(loopCount, loopType);
        tweener.Play();
    }

    void PunchScaleEffect()
    {
        transform.DOPunchScale(endValue, duration).OnComplete(() => isPlayingEffect = false);
        Invoke(nameof(PunchScaleEffect), delay);
    }

    void GroupScaleEffect()
    {  
        foreach (Transform item in transform)
        {
           //if(item.localScale.magnitude == 1)  item.DOScale(0f, 0f);
            item.DOScale(endValue, duration).SetEase(easingEffect).SetDelay(delay).OnComplete(() => isPlayingEffect = false);
        }
    }
    #endregion
}
