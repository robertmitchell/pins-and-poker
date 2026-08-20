using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebServices : Singleton<WebServices>
{
    private const int RequestTimeoutSeconds = 30;

    private Dictionary<string, string> _headers = new();
    private string _accessToken;

    public enum HttpMethod { GET, POST, PUT, DELETE }
    public enum ContentType { JSON, FORM }
    public enum LoginType { guest,social,connect}

    public ApiRoutes ApiRoutes;

    public override void Awake()
    {
        base.Awake();
        SetAccessToken();
    }

    public void SetAccessToken()
    {
        string accessToken = PlayerPrefs.GetString(Db_Keys.token);
        if (!string.IsNullOrEmpty(accessToken))
        {
            _headers["Authorization"] = "Bearer " + accessToken;
            _headers["Accept"] = "application/json";
            Debug.Log("Bearer: " + accessToken);
        }
    }

    public void MakeRequest<T>(string url, HttpMethod method, Action<T, long> onSuccess, Action<string> onFailure, string jsonData = null, Dictionary<string, string> formData = null, Texture2D image = null, bool showLoader = true)
    {
        StartCoroutine(HandleRequest<T>(url, method, onSuccess, onFailure, jsonData, formData, image, showLoader));
    }

    private void HandleGetRequest(UnityWebRequest request, Dictionary<string, string> queryParameters)
    {
        if (queryParameters != null && queryParameters.Count > 0)
        {
            string query = "?";
            foreach (var item in queryParameters)
            {
                query += UnityWebRequest.EscapeURL(item.Key) + "=" + UnityWebRequest.EscapeURL(item.Value) + "&";
            }
            // Remove trailing '&'
            query = query.TrimEnd('&');
            request.url += query;
        }
        request.downloadHandler = new DownloadHandlerBuffer();
    }

    private UnityWebRequest CreatePostRequest(string url, WWWForm form)
    {
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.downloadHandler = new DownloadHandlerBuffer();
        return request;
    }

    private IEnumerator HandleRequest<T>(string url, HttpMethod method, Action<T, long> onSuccess, Action<string> onFailure, string jsonData, Dictionary<string, string> formData, Texture2D image, bool showLoader = true)
    {
        if (showLoader)
        {
            WaitingLoaderCanvas.Instance?.Show();
        }

        Debug.Log("URL : " + ApiRoutes.baseUrl + url);
        UnityWebRequest request = null;

        switch (method)
        {
            case HttpMethod.GET:
                request = new UnityWebRequest(ApiRoutes.baseUrl + url, UnityWebRequest.kHttpVerbGET);
                HandleGetRequest(request, formData);

                break;

            case HttpMethod.POST:
                if (!string.IsNullOrEmpty(jsonData))
                {
                    // JSON PlayerData
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                    request = new UnityWebRequest(ApiRoutes.baseUrl + url, UnityWebRequest.kHttpVerbPOST)
                    {
                        uploadHandler = new UploadHandlerRaw(bodyRaw),
                        downloadHandler = new DownloadHandlerBuffer()
                    };
                    request.SetRequestHeader("Content-Type", "application/json");
                }
                else if (formData != null)
                {
                    // Form PlayerData
                    WWWForm form = new WWWForm();
                    foreach (KeyValuePair<string, string> field in formData)
                    {
                        form.AddField(field.Key, field.Value);
                        //Debug.Log("field.Key == " + field.Key + " field.Value == " + field.Value);
                    }

                    if (image != null)
                    {
                        Texture2D uncompressedImage = ConvertToUncompressed(image);
                        byte[] imageBytes = uncompressedImage.EncodeToPNG();
                        form.AddBinaryData("image", imageBytes, "image.png", "image/png");
                        Debug.Log($"Image size: {imageBytes.Length} bytes");
                    }
                    //Debug.Log("URL : " + ApiRoutes.baseUrl + url);
                    //request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

                    request = CreatePostRequest(ApiRoutes.baseUrl + url, form);
                    //request.uploadHandler = new UploadHandlerRaw(form.data);
                    request.downloadHandler = new DownloadHandlerBuffer();
                }
                break;

            case HttpMethod.PUT:
                // Handle PUT request similarly to POST
                request = new UnityWebRequest(ApiRoutes.baseUrl + url, UnityWebRequest.kHttpVerbPUT);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                }
                else if (formData != null)
                {
                    WWWForm form = new WWWForm();
                    foreach (KeyValuePair<string, string> field in formData)
                    {
                        form.AddField(field.Key, field.Value);
                    }

                    if (image != null)
                    {
                        byte[] imageBytes = image.EncodeToPNG();
                        form.AddBinaryData("image", imageBytes, "image.png", "image/png");
                    }

                    request.uploadHandler = CreatePostRequest(ApiRoutes.baseUrl + url, form).uploadHandler;
                    request.downloadHandler = new DownloadHandlerBuffer();
                }
                break;

            case HttpMethod.DELETE:
                request = new UnityWebRequest(ApiRoutes.baseUrl + url, UnityWebRequest.kHttpVerbDELETE)
                {
                    downloadHandler = new DownloadHandlerBuffer()
                };
                break;
        }

        foreach (var header in _headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        request.timeout = RequestTimeoutSeconds;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(request.downloadHandler?.text);
            T res = JsonConvert.DeserializeObject<T>(Serializer.ToJson(response.data));
            onSuccess?.Invoke(res, request.responseCode);
            SetAccessToken();
            Debug.Log("Response Code: " + request.responseCode);
            Debug.Log("Response: " + request.downloadHandler?.text);
        }
        else
        {
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(request.downloadHandler?.text);
            string errorMsg = response?.message ?? request.error ?? "Unknown error";
            onFailure?.Invoke(errorMsg);
            Debug.Log("Error Response Code: " + request.responseCode);
            Debug.Log("Error Response: " + request.downloadHandler?.text);
        }

        WaitingLoaderCanvas.Instance?.Hide();
    }
    private Texture2D ConvertToUncompressed(Texture2D texture)
    {
        Texture2D uncompressedTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        uncompressedTexture.SetPixels(texture.GetPixels());
        uncompressedTexture.Apply();
        return uncompressedTexture;
    }
}


[Serializable]
public class ApiRoutes
{
    public static string baseUrl= "https://api.myprojectstaging.com/games/pinsandpoker/api/v1";
    public static string imageStartingPointURL = "https://api.myprojectstaging.com/games/pinsandpoker/public/";

    [Space]
    [Header("Api Calls")]
    //public const string signUp;
    public const string logIn = "/auth/login";
    public const string modlogin = "/auth/moderator/login";
    public const string logOut = "/auth/logout";
    public const string deleteAccount = "/profile/delete-account";
    public const string getProfile ="";
    public const string updateProfile = "/auth/profile/update";
    public const string getNotifications = "/notifications";
    public const string notificationSeen = "/notification/seen";
    public const string leaderboard ="";
    public const string purchase ="";


    // USER API ROUTES       
    public const string userSearch = "/user/search";
    public static string joinLeague = "/user/league/join";
    public static string cancelLeague = "/user/league/cancel";
    public static string joinGame = "/user/game/join";
    public static string cancelGame = "/user/game/cancel";
    public static string getLeaguesForUser = "/user/league/all";
    public static string getUserLeagues = "/user/league";
    public static string getUserLeagueGames = "/user/game";
    public static string getGameParticiants = "/game/participants";
    public static string getScores = "/game/score";
    public static string cardExchange = "/user/card/exchange";
    public const string updateGameScore = "/game/score/update";
    public const string manageGame = "/moderator/game/status";


    // MODERATOR API ROUTES
    public static string getAdminRules = "/moderator/rules";
    public static string createLeague = "/moderator/league/create";
    public static string updateLeague = "/moderator/league/update";
    public static string getLeaguesForModerator = "/moderator/league";
    public static string getLeagueParticipants= "/league/participants";
    public static string getLeagueReqModerator = "/moderator/league/requests";
    public static string manageLeagueReq = "/moderator/league/manage-request";
    public static string manageLeagueReqAll = "/moderator/league/manage-request-all";
    public static string removeParticipant = "/moderator/participant/remove";

    public static string creatGame = "/moderator/game/create";
    public static string assignLane = "/moderator/game/assign-lane";
    public static string getModeratorLeagueGames = "/moderator/game";
    public static string getGameRequests = "/moderator/game/requests";
    public static string manageGameReq = "/moderator/game/manage-request";
    public static string manageAllGameReq = "/moderator/game/manage-request-all";
    public static string getLeagueAndGamesRequestsCount = "/moderator/league/info";
    // public static string getGameRequests = "/moderator/game";

    public static string getDisputeRequest = "/dispute";
    public static string disputeRequest = "/dispute/create";
    public static string resolveDispute = "/moderator/dispute/status";
    public static string uploadChatImage = "/dispute/upload-image";

}


public static class Response
{
    public static bool Check(long responseCode)
    {
        return responseCode.Equals(200) || responseCode.Equals(201);
    }
}

