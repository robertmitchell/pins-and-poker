using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ImageCacheManager : Singleton<ImageCacheManager>
{
    private string cachePath;
    public override void Awake()
    {
        base.Awake();
        cachePath = Path.Combine(Application.persistentDataPath, "ImageCache");
        if (!Directory.Exists(cachePath))
        {
            Directory.CreateDirectory(cachePath);
        }
    }

    public void GetImage(string url, System.Action<Texture2D> onComplete)
    {
        if (string.IsNullOrEmpty(url))
        {

            Debug.LogError("URL is null or empty.");
            onComplete?.Invoke(null);
            return;
        }

        // Encode the URL to ensure it's properly formatted
        string encodedUrl = Uri.EscapeUriString(url);
        string filename = GetFileNameFromUrl(encodedUrl);
        string filePath = Path.Combine(cachePath, filename);

        if (File.Exists(filePath))
        {
            StartCoroutine(LoadImageFromCache(filePath, onComplete));
        }
        else
        {
            StartCoroutine(DownloadImage(encodedUrl, filePath, onComplete));
        }
    }


    private string GetFileNameFromUrl(string url)
    {
        return Path.GetFileName(url);
    }

    private IEnumerator LoadImageFromCache(string filePath, System.Action<Texture2D> onComplete)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        yield return null;
        onComplete?.Invoke(texture);
    }

    private IEnumerator DownloadImage(string url, string filePath, System.Action<Texture2D> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download image: {request.error}");
                onComplete?.Invoke(null);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                File.WriteAllBytes(filePath, texture.EncodeToPNG());
                onComplete?.Invoke(texture);
            }
        }
    }

    public IEnumerator DownloadMultipleImage(string url, System.Action<Texture2D> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            request.SetRequestHeader("User-Agent", "UnityWebRequest");

            //Debug.Log("URL : "+ url);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download image: {request.error}");
                onComplete?.Invoke(null);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                onComplete?.Invoke(texture);
            }
        }
    }
}
