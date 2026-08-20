using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using static Global;

public class LeagueChatScreen : UIScreenBase
{
    public AnimatrixButton backBtn;
    public AnimatrixButton sendMessageBtn;
    public AnimatrixButton resolveBtn;
    public Button uploadImageBtn;
    public TMP_Text headerTxt;
    public TMP_InputField messageInputField;
    public ScrollRect leagueChatScrollRect;
    public ScrollRect usersProfilePicScrollRect;
    public GameObject LocalChatPrefab;
    public GameObject LocalImageChatPrefab;
    public GameObject OtherChatPrefab;
    public GameObject OtherImageChatPrefab;
    public long currentGameID; 
    internal ChatInfo chatInfo;
    public GameObject failSafePrefab;
    public Texture2D defaultAvatar;
    Dictionary<string, Texture2D> chatAvatars = new();

    public string cellIndex;
    public string _groupID;

    public struct ChatInfo
    {
        public string groupID;
        public long moderatorID;
        public long disputedID;
        public long disputerID;
        public long gameID;
    }

    private void OnEnable()
    {
        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.user.ToString()) headerTxt.text = "League Chat";
        else headerTxt.text = "Dispute Chat";
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
        leagueChatScrollRect.verticalNormalizedPosition = 1;
    }

    void Start()
    {
        if (PlayerPrefs.GetString(Db_Keys.userType) == UserType.user.ToString()) resolveBtn.gameObject.SetActive(false);
        backBtn.onClick.AddListener(BackBtnClicked);
        sendMessageBtn.onClick.AddListener(SendMessage);
        resolveBtn.onClick.AddListener(SendRequestToResolveDispute);
        uploadImageBtn.onClick.AddListener(SelectImage);
    }

    void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<DisputeScreen>();
    }

    public void StartChat(string groupID, long moderatorID, long disputedID, long disputerID, long gameID)
    {
        ChatInfo info = new();
        info.groupID = groupID;
        info.moderatorID = moderatorID;
        info.disputedID = disputedID;
        info.disputerID = disputerID;
        info.gameID = currentGameID = gameID;
        chatInfo = info;

        chatAvatars.Clear();

        foreach(Transform child in leagueChatScrollRect.content)
            Destroy(child.gameObject);

        SocketManager.Instance.getAllMessages = true;
        SocketManager.Instance.OnGetAllMessages = OnGetAllMessages;
        SocketManager.Instance.OnGetMessage = OnGetMessage;
        SocketManager.Instance.EstablishConnection();
        WaitingLoaderCanvas.Instance.Show();
    }

    void OnGetAllMessages(GetMessages msgs)
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            StartCoroutine(GetAllMessagesCoroutine(msgs));
        });
    }

    IEnumerator GetAllMessagesCoroutine(GetMessages msgs)
    {
        Debug.Log("Received all messages");
        foreach (Transform item in leagueChatScrollRect.content)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in usersProfilePicScrollRect.content)
        {
            Destroy(item.gameObject);
        }

        ImageCacheManager.Instance.GetImage(ApiRoutes.imageStartingPointURL + msgs.RespondedAvatar, OnReceivedDisputedImage);
        ImageCacheManager.Instance.GetImage(ApiRoutes.imageStartingPointURL + msgs.DisputerAvatar, OnReceivedDisputerImage);
        ImageCacheManager.Instance.GetImage(ApiRoutes.imageStartingPointURL + msgs.ModeratorAvatar, OnReceivedModeratorImage);

        float avatarLoadStartTime = Time.time;
        yield return new WaitUntil(() => chatAvatars.Count >= 2 || Time.time - avatarLoadStartTime > 10f);
        WaitingLoaderCanvas.Instance.Hide();
        msgs.Data.ForEach(msg =>
        {
            Debug.Log("msgs Data ForEach");
            switch (msg.Type)
            {
                case "text":
                    CreateTextChatBox(msg);
                    break;
                case "image":
                    CreateImageChatBox(msg);
                    break;
            }
        });
    }

    void OnGetMessage(GetMessage msg)
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            Debug.Log("OnGetMessage Called");
            Debug.Log(msg.Data.DisputedAgainstId);
            switch (msg.Data.Type)
            {
                case "text":
                    CreateTextChatBox(msg.Data);
                    break;
                case "image":
                    CreateImageChatBox(msg.Data);
                    break;
            }
        });
    }
    void CreateTextChatBox(MessageData msg)
    {
        if (failSafePrefab != null)
            Instantiate(failSafePrefab, leagueChatScrollRect.content);
        string localPlayerID = PlayerPrefs.GetString(Db_Keys.playerID);
        Texture2D avatar = null;
        if (chatAvatars.ContainsKey(msg.SendedBy))
            avatar = chatAvatars[msg.SendedBy];
        else
            avatar = defaultAvatar;


        Debug.Log("Avatar" + avatar);
        if (localPlayerID == msg.SendedBy)
        {
            if (LocalChatPrefab == null) return;
            GameObject msgBox = Instantiate(LocalChatPrefab, leagueChatScrollRect.content);
            msgBox.GetComponent<UserChatPrefab>().SetData(msg.Message, $"{msg.CreatedAt.Hour}:{msg.CreatedAt.Minute}", avatar);
        }
        else
        {
            if (OtherChatPrefab == null) return;
            GameObject msgBox = Instantiate(OtherChatPrefab, leagueChatScrollRect.content);
            msgBox.GetComponent<UserChatPrefab>().SetData(msg.Message, $"{msg.CreatedAt.Hour}:{msg.CreatedAt.Minute}", avatar);
        } 
        DG.Tweening.DOTween.To(
            () => leagueChatScrollRect.verticalNormalizedPosition,
             val => leagueChatScrollRect.verticalNormalizedPosition = val ,
            0, 0.4f);


    }
    [ContextMenu("FirstPrefabSet")]
    public void FirstPrefabSet()
    {
        Debug.Log("FirstPrefabSet  :  " + leagueChatScrollRect.content.childCount);
        leagueChatScrollRect.gameObject.SetActive(true);

    }
    private IEnumerator delayChat()
    {
        Debug.Log("delayChat Called" + leagueChatScrollRect.content.childCount);
        leagueChatScrollRect.content.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        leagueChatScrollRect.content.gameObject.SetActive(true);
    }
    void CreateImageChatBox(MessageData msg)
    {
        string localPlayerID = PlayerPrefs.GetString(Db_Keys.playerID);
        Texture2D avatar = null;
        if (chatAvatars.ContainsKey(msg.SendedBy))
        {
            avatar = chatAvatars[msg.SendedBy];
            Debug.Log("chatAvatars Loaded");
        }
        else
            avatar = defaultAvatar;

        if (localPlayerID == msg.SendedBy)
        {
            if (LocalImageChatPrefab == null) return;
            GameObject msgBox = Instantiate(LocalImageChatPrefab, leagueChatScrollRect.content);
            msgBox.GetComponent<ImageChatPrefab>().SetImage(msg.Message, $"{msg.CreatedAt.Hour}:{msg.CreatedAt.Minute}", avatar);
        }
        else
        {
            if (OtherImageChatPrefab == null) return;
            GameObject msgBox = Instantiate(OtherImageChatPrefab, leagueChatScrollRect.content);
            msgBox.GetComponent<ImageChatPrefab>().SetImage(msg.Message, $"{msg.CreatedAt.Hour}:{msg.CreatedAt.Minute}", avatar);
        }
      /*  if (leagueChatScrollRect.content.childCount == 1)
        {
            leagueChatScrollRect.content.gameObject.SetActive(false);
            leagueChatScrollRect.content.gameObject.SetActive(true);
        }*/
        DG.Tweening.DOTween.To(() => leagueChatScrollRect.verticalNormalizedPosition,
            val => leagueChatScrollRect.verticalNormalizedPosition = val,
            0, 0.4f);
    }

    void SendMessage()
    {
        if (messageInputField.text == string.Empty)
            return;
        SocketManager.Instance.SendMessage(chatInfo.disputerID, chatInfo.disputedID, chatInfo.moderatorID, chatInfo.groupID.ToString(), long.Parse(PlayerPrefs.GetString(Db_Keys.playerID)), messageInputField.text, "text");
        messageInputField.text = "";
     }

    public void SelectImage()
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize: 1024, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                UploadImage(texture);
            }
        }, "Select an image", "image/*");
        Debug.Log("Permission result: " + permission);
    }

    public void UploadImage(Texture2D image)
    {
        Debug.Log("UploadImage Called");

        Dictionary<string, string> payload = new();
        WebServices.Instance.MakeRequest<ImagePathResponse>(ApiRoutes.uploadChatImage, WebServices.HttpMethod.POST, OnUploadImageSuccess, OnUploadImageFail, null, payload, image, false);
    }

    void OnUploadImageSuccess(ImagePathResponse resp, long code)
    {
        Debug.Log("Image successfully uploaded, path: " + resp.Image);
        SocketManager.Instance.SendMessage(chatInfo.disputerID, chatInfo.disputedID, chatInfo.moderatorID, chatInfo.groupID.ToString(), long.Parse(PlayerPrefs.GetString(Db_Keys.playerID)), resp.Image, "image");
    }

    void OnUploadImageFail(string resp)
    {
        Debug.Log("Error :  " + resp);
    }

    void SendRequestToResolveDispute()
    {
        Dictionary<string, string> payload = new()
        {
            { Db_Keys.gameId, currentGameID.ToString() },
            { Db_Keys.status, Status.resolved.ToString() },
            { Db_Keys.cell_index, cellIndex },
            { Db_Keys.disputeGroupID, _groupID },
        };
        WebServices.Instance.MakeRequest<GetDisputesResponse>(ApiRoutes.resolveDispute, WebServices.HttpMethod.POST, OnResolveSuccess, OnResolveFail, null, payload, null, true);
    }

    void OnResolveSuccess(GetDisputesResponse resp, long code)
    {
        MessagePopUpScreen.Instance.ShowMessage("Dispute has been marked as resolved", "Dispute");
        CharacterAnimationCanvas.Instance.PlayChrAnimation(AnimationNames.happyThumbsUp);
        UIManager.instance.Hide();
        UIManager.instance.Show<DisputeScreen>();
    }

    void OnResolveFail(string resp)
    {
        MessagePopUpScreen.Instance.ShowMessage(resp, "Failure",null,null, MessagePopUpScreen.Instance._wrongSprite);
    }

    void OnReceivedDisputerImage(Texture2D img)
    {
       if(!chatAvatars.ContainsKey(chatInfo.disputerID.ToString())) chatAvatars.Add(chatInfo.disputerID.ToString(), img);
    }

    void OnReceivedDisputedImage(Texture2D img)
    {
        if (!chatAvatars.ContainsKey(chatInfo.disputedID.ToString()))  chatAvatars.Add(chatInfo.disputedID.ToString(), img);
    }

    void OnReceivedModeratorImage(Texture2D img)
    {
        if (!chatAvatars.ContainsKey(chatInfo.moderatorID.ToString())) chatAvatars.Add(chatInfo.moderatorID.ToString(), img);
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}