using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class NotificationScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public TMP_Text noNotificationsText;
    public ScrollRect notificationsScrollRect;
    public NotificationPrefab notificationPrefab;
    public Transform contentView;

    private void OnEnable()
    {
        notificationsScrollRect.verticalNormalizedPosition = 1;
        GetNotifications();
    }

    private void OnDisable()
    {
        if(noNotificationsText.gameObject.activeSelf) noNotificationsText.gameObject.SetActive(false);
        foreach (Transform item in contentView)
        {
            Destroy(item.gameObject);
        }
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
    }

    void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<HomeScreen>();
    }

    void GetNotifications()
    {
        WebServices.Instance.MakeRequest<List<NotificationData>>(
           ApiRoutes.getNotifications,
           WebServices.HttpMethod.GET,
           OnSuccess,
           OnFailure,
           null,
           null,
           null,
           true
           );
    }

    void OnSuccess(List<NotificationData> resp, long arg2)
    {
        if (resp != null && resp.Count > 0)
        {
            foreach (Transform item in contentView)
            {
                Destroy(item.gameObject);
            }
            foreach (var obj in resp)
            {
                NotificationPrefab gameObj = Instantiate(notificationPrefab, contentView).GetComponent<NotificationPrefab>();
                gameObj.notificationID = obj.Id;
                gameObj.isRead = obj.IsRead;
                gameObj.notificationTitleTxt.text = obj.Title;
                gameObj.notificationTxt.text = obj.Body;
                gameObj.timeTxt.text = obj.CreatedAt ?? "No Time Provided"; // Handle null time if not included
            }
            noNotificationsText.gameObject.SetActive(false);
        }
        else
        {
            noNotificationsText.gameObject.SetActive(true);
            Debug.LogWarning("No notifications available.");
        }
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        MessagePopUpScreen.Instance.ShowMessage(error, "", "OK", null, true, MessagePopUpScreen.Instance._wrongSprite);
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}
