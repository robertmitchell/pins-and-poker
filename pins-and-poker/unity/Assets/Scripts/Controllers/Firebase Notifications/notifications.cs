using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using System;

public class notifications : Singleton<notifications>
{
    public override void Awake()
    {
        // Firebase Topic Subscription (Optional: For Both iOS and Android)
        Firebase.Messaging.FirebaseMessaging.SubscribeAsync("all").ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Subscribed to 'all' topic successfully.");
            }
        });
    }

    void Start()
    {
        // Firebase Messaging Event Handlers
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

        // Check Firebase Dependencies
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                var app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("Firebase is ready to use.");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
            }
        }); 
    }
     

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
        Dictionary<string, string> formData = new Dictionary<string, string>
            { 
                { Db_Keys.fcmToken, token.Token }, 
            };

        WebServices.Instance.MakeRequest<PlayerData>(
            ApiRoutes.updateProfile,
            WebServices.HttpMethod.POST,
            OnSuccess,
            OnFailure,
            null,
            formData,
            null,
            true
        );
    }

    private void OnFailure(string obj)
    {
        Debug.Log(" fcmToken Registration OnFailure: " + obj);

    }

    private void OnSuccess(PlayerData arg1, long arg2)
    {
        Debug.Log("fcmToken Registration Token: OnSuccess");

    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log($"Received a new message from: {e.Message.From}");
        if (e.Message.Notification != null)
        {
            string title = e.Message.Notification.Title;
            string body = e.Message.Notification.Body;
            //call method
            NotificationPopup.Instance.SetData(title, body);
            NotificationPopup.Instance.gameObject.SetActive(true);
            Debug.Log($"Notification Received - Title: {title}, Body: {body}");
          
          /*  if (e.Message.Data.ContainsKey("target")) // open a screen
            {
                string targetObjectName = e.Message.Data["target"];
                Debug.Log($"Target GameObject: {targetObjectName}"); 
            }*/
        }
    }
}
