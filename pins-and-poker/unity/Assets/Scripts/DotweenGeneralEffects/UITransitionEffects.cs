using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class UITransitionEffects : MonoBehaviour
{
    [SerializeField] EffectTypes _effectsType;

    [Space]
    [Header("Settings")]

    [SerializeField] float position;
    [SerializeField] float duration;
    [SerializeField] float delay;
    [SerializeField] Vector2 resetPosition;
    [SerializeField] Ease easingEffect;

    public UnityEvent callBack;

    bool isPlayingEffect;

    [System.Serializable]
    public enum EffectTypes
    {
        AnchorSimpleMove,
        AnchorEasinghMove,
        AnchorGroupMove
    }

    private void OnEnable()
    {
        PlayEffect();

    }


    private void OnDisable()
    {
        transform.GetComponent<RectTransform>().DOAnchorPos(resetPosition, 0f);
    }

    void PlayEffect()
    {
        if (isPlayingEffect) return;
        isPlayingEffect = true;
        switch (_effectsType)
        {
            case EffectTypes.AnchorSimpleMove:
                AnchorSimpleMove();
                break;
            case EffectTypes.AnchorEasinghMove:
                AnchorEasinghMove();
                break;
            case EffectTypes.AnchorGroupMove:
                AnchorGroupMove();
                break;
        }
    }

    #region TRANSITION EFFECTS

    void AnchorSimpleMove()
    {
        transform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), duration).OnComplete(() => OnCompleteEvent());
    }
    void AnchorEasinghMove()
    {
        transform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), duration).SetEase(easingEffect).SetDelay(delay).OnComplete(() => OnCompleteEvent());
    }

    void AnchorGroupMove()
    {
        foreach (var item in transform.gameObject.GetComponentsInChildren<GameObject>())
        {
            item.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), duration).SetEase(easingEffect).SetDelay(delay).OnComplete(() => OnCompleteEvent());
        }
    }

    void OnCompleteEvent() 
    {
        if (callBack != null)
            callBack.Invoke();

        isPlayingEffect = false;
    }
    #endregion
}
