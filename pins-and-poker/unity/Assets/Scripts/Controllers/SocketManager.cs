using Newtonsoft.Json;
using SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


    public class SocketManager : Singleton<SocketManager>
    {
        public State _socketState;
        [SerializeField] string url; // port 4004
        private SocketIOUnity socket;

        public Action<GetMessages> OnGetAllMessages;
        public Action<GetMessage> OnGetMessage;
        public Action OnSocketConnected;

        public bool getAllMessages = true; 

    public enum State
        {
            Connected,
            Disconnected,
            Reconnecting
        }


        #region Monobehaviour and Socket Connection
        // Start is called before the first frame update
        void Start()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
            // Initialize the socket
            var uri = new System.Uri(url); // Change this to your server's URI
            socket = new SocketIOUnity(uri, new SocketIOOptions
            {
                
                Transport = SocketIOClient.Transport.TransportProtocol.Polling,
                Reconnection = true, // Auto reconnect if the connection drops
                AutoUpgrade = false,
                   
            });

            /*socket.Options.AutoUpgrade = false;*/

            // Setup event handlers
            socket.OnConnected += (sender, e) =>
            {
                Debug.Log("Connected to the server.");
                if(getAllMessages)
                {
                    GetAllMessages(UIManager.instance.GetScreen<LeagueChatScreen>().chatInfo.groupID);
                    getAllMessages = false;
                }
                _socketState = State.Connected;
                
            };

            socket.OnDisconnected += (sender, e) =>
            {
                Debug.Log("Disconnected from the server.");
                _socketState = State.Disconnected;
            };

            socket.OnReconnectAttempt += (sender, e) =>
            {
                Debug.Log("Attempting to reconnect...");
                _socketState = State.Reconnecting;
            };

            // Add your custom event handlers here
            socket.On("response", response =>
            {
                Debug.Log(response.ToString());
                SocketResponse resp = Deserialzer.FromJson<SocketResponse[]>(response.ToString())[0];
                switch (resp.ObjectType)
                {
                    case "get_messages":
                        GetMessages msgs = Deserialzer.FromJson<GetMessages[]>(response.ToString())[0];
                        OnGetAllMessages.Invoke(msgs);
                        Debug.Log("All messages received successfully");
                        break;
                    case "get_message":
                        GetMessage msg = Deserialzer.FromJson<GetMessage[]>(response.ToString())[0];
                        OnGetMessage.Invoke(msg);
                        Debug.Log("Message received");
                        break;
                }
            });

        socket.On("error", response =>
        {
            MessagePopUpScreen.Instance.ShowMessage("Error", response.ToString(),null, null, MessagePopUpScreen.Instance._wrongSprite);
        });
        }

        // Update is called once per frame
        void Update()
        {
            // You can handle any per-frame logic here if needed          
        }

        private void OnApplicationQuit()
        {
            // Clean up the socket connection when the application quits
            if (socket != null)
            {
                socket.Disconnect();
            }
        }
    #endregion

    #region Socket Methods

    struct MessagePayload
    {
        public long disputer_id;
        public long disputed_against_id;
        public long moderator_id;
        public string group_id;
        public long sended_by;
        public string message;
        public string type;
    }

    public void EstablishConnection()
    {
        if (_socketState == State.Connected)
        {
            if (getAllMessages)
            {
                GetAllMessages(UIManager.instance.GetScreen<LeagueChatScreen>().chatInfo.groupID);
                getAllMessages = false;
            }
        }
        else
        {
            socket.Connect();
        }
    }
    public void BreakConnection()
    {
        socket.Disconnect();
    }

    struct GroupIDInJson
    {
        public string group_id;
    }

    public void GetAllMessages(string groupID)
    {
        GroupIDInJson gj = new();
        gj.group_id = groupID;
        string idInJson = JsonConvert.SerializeObject(gj);
        socket.EmitStringAsJSON("get_messages", idInJson);
        Debug.Log(idInJson);
        Debug.Log("Get all messages emitted");
    }

    public void SendMessage(long disputerID, long disputedID, long moderatorID, string groupID, long sentBy, string message, string type)
    {
        MessagePayload p = new();
        p.disputer_id = disputerID;
        p.disputed_against_id = disputedID;
        p.moderator_id = moderatorID;
        p.group_id = groupID;
        p.sended_by = sentBy;
        p.message = message;
        p.type = type;
        

        string payload = JsonConvert.SerializeObject(p);
        Debug.Log(payload);
        socket.EmitStringAsJSON("send_message", payload);
        Debug.Log("Send message emitted");

    }

    #endregion

    #region Socket Validation

    public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
        {
            // Certificate is valid
            return true;
        }

        // You can log the error and optionally choose to bypass the error (for dev purposes)
        Debug.LogWarning($"SSL Certificate error: {sslPolicyErrors}");

        // For testing or development environments, you might return true to bypass SSL errors
        return true;  // This bypasses SSL validation (not recommended in production)
    }

    #endregion
}