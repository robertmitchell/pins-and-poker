using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GalleryImageUploader : Singleton<GalleryImageUploader>
{
    public GameObject cameraIconImage;

    private void OnDisable()
    {
        cameraIconImage.SetActive(true);
    }

    public void PickImage(RawImage image = null)
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                // Load the image from the path
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize: 1024, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                Scene currentScene = SceneManager.GetActiveScene();

                image.texture = texture;

                cameraIconImage.SetActive(false);
            }
        }, "Select an image", "image/*");

        Debug.Log("Permission result: " + permission);
    }
}
