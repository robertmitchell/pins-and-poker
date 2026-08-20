using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class NotificationPrefab : MonoBehaviour
{
    Button notificationBtn;
    public RawImage profilePic;
    public TMP_Text timeTxt;
    public TMP_Text countTxt;
    public TMP_Text notificationTitleTxt;
    public TMP_Text notificationTxt;
    public long notificationID;
    public string isRead;

    private void Start()
    {
        notificationBtn = GetComponent<Button>();  
        if (isRead == "1") notificationBtn.interactable = false;
        else notificationBtn.onClick.AddListener(() => OnNotificationBtnClicked());
    }

    private void OnNotificationBtnClicked()
    {
        Dictionary<string, string> formData = new Dictionary<string, string>
        {
            { Db_Keys.notifyID,  notificationID.ToString() },
            { Db_Keys.isRead,  "1" }
        };

        WebServices.Instance.MakeRequest<NotificationSeenData> (
           ApiRoutes.notificationSeen,
           WebServices.HttpMethod.POST,
           OnSuccess,
           OnFailure,
           null,
           formData,
           null,
           false
           );
    }

    void OnSuccess(NotificationSeenData resp, long arg2)
    {
        Debug.Log("Notificaiton Seen:  " + resp);
        notificationBtn.interactable = false;
    }

    void OnFailure(string error)
    {
        Debug.LogError("Request failed: " + error);
        return;
    }
}
