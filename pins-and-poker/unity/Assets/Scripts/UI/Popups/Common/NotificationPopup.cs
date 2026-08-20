using DG.Tweening;
using TMPro;
using UnityEngine;

public class NotificationPopup : Singleton<NotificationPopup>
{
    public TMP_Text _titleTxt;
    public TMP_Text _messageTxt;
    public GameObject _iconImage;
    public GameObject _messagePanel;
    Tweener _tweener;

    private void OnEnable()
    {
        Invoke(nameof(DisableGameObject), 4f);
    }

    private void OnDisable()
    {
        CancelInvoke();
        _tweener.Kill();
    }

    public void SetData(string title, string message)
    {
        _titleTxt.text = title;
        _messageTxt.text = message;
    }


    void DisableGameObject()
    {
        _tweener =
            _messagePanel.transform.DOScale(0f, 0.3f).SetEase(Ease.OutQuart);
            _messagePanel.GetComponent<CanvasGroup>().DOFade(0f, 0.3f);
            _iconImage.transform.DOLocalMoveX(0f, 0.3f);
            _iconImage.transform.DOScale(1f, 0.3f).OnComplete(() =>
            {
                transform.DOLocalMoveY(330f, 0.4f);
                _iconImage.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
       
     }
}
