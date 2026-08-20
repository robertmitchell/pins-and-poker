using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageSaveLoad : Singleton<ImageSaveLoad>
{
    public string profileName = "User";

    private void SaveImageToPlayerPrefs(Texture2D texture)
    {
        Texture2D uncompressedImage= ConvertToUncompressed(texture);

        byte[] imageData = uncompressedImage.EncodeToPNG(); 
        string encodedImageData = Convert.ToBase64String(imageData);
        PlayerPrefs.SetString(Db_Keys.userImage, encodedImageData);
        PlayerPrefs.Save();
    }

    public void SaveRawImage(RawImage rawImage)
    {
        if (rawImage != null && rawImage.texture != null)
        {
            Debug.Log("---------------Image Saveing Processin PlayerPrefs2");
            Texture2D texture = rawImage.texture as Texture2D;
            SaveImageToPlayerPrefs(texture);
            Debug.Log("---------------Image Saveing Processin PlayerPrefs");
        }
    }

    public void LoadImageFromPlayerPrefs(RawImage rawImage)
    {

        if (PlayerPrefs.HasKey(Db_Keys.userImage))
        {
            string encodedImageData = PlayerPrefs.GetString(Db_Keys.userImage);
            byte[] imageData = Convert.FromBase64String(encodedImageData);
            Texture2D loadedTexture = new Texture2D(2, 2);
            loadedTexture.LoadImage(imageData);
            rawImage.texture = loadedTexture;
        }
        else
        {
            Debug.Log("No image data found in PlayerPrefs.");
        }
    }
    private Texture2D ConvertToUncompressed(Texture2D texture)
    {
        // Create a new imageTexture with the same dimensions and uncompressed format
        Texture2D uncompressedTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        // Copy pixel data from the original imageTexture to the new uncompressed imageTexture
        uncompressedTexture.SetPixels(texture.GetPixels());
        uncompressedTexture.Apply();

        return uncompressedTexture;
    }
}
