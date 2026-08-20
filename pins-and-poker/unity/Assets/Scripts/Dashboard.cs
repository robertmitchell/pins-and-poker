using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : UIScreenBase
{
    public Text nameText;
    public Text emailText;
    public Image image;

    public PlayerData playerData;
    public override void UpdateScreen<T>(T data)
    {
        if (data is PlayerData profileData)
        {
            if (nameText) nameText.text = profileData.Username;
            if (emailText) emailText.text = profileData.Email;
            ImageCacheManager.Instance.GetImage(profileData.Image, OnImageLoaded);
        }
        else
        {
            Debug.LogError("Invalid data type for " + gameObject.name);
        }
    }

    private void OnImageLoaded(Texture2D texture)
    {
        if (texture != null)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            if (image) image.sprite = sprite;
        }
        else
        {
            Debug.LogError("Failed to load image.");
        }
    }
}
