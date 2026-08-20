using UnityEngine;
using UnityEngine.UI;

public class ImageChatPrefab : MonoBehaviour
{
    [SerializeField] Button scaleImageBtn;
    [SerializeField] RawImage chatImg;
    [SerializeField] RawImage profileImg;
    [SerializeField] TMPro.TMP_Text timeText;

    private void Start()
    {      
        scaleImageBtn.onClick.AddListener(() => ImageZoomDrag.onImageClick?.Invoke(true, chatImg.texture));
    }

    public void SetImage(string path, string time, Texture2D img)
    {
        timeText.text = time;
        profileImg.texture = img;
        Debug.Log(ApiRoutes.imageStartingPointURL + path);
        ImageCacheManager.Instance.GetImage(ApiRoutes.imageStartingPointURL + path, OnImageReceived);
    }

    void OnImageReceived(Texture2D image)
    {
        chatImg.texture = image;
    }
}
