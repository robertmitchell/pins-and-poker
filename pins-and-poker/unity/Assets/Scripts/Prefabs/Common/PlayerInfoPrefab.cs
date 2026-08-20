using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPrefab : MonoBehaviour
{
    public string playerName;
    public RawImage image;
    //public GameObject card;

    public User user;
    // Start is called before the first frame update

    public string imageStartingPointURL = "https://api.myprojectstaging.com/games/pinsandpoker/public/";
    void Start()
    {

        StartCoroutine(ImageCacheManager.Instance.DownloadMultipleImage(imageStartingPointURL + "       ", SetImage));
    }
    void SetImage(Texture2D texture)
    {
        image.texture = texture;
    }
}
