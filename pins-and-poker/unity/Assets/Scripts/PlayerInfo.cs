using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public string playerName;
    public RawImage image;
    public Image winnerImage;
    public BowlingScoreCardData cardData;
    public PlayerScoreCardData user;
    public TMP_Text userNameField;
    Button playerBtn;
    bool imageDownload;

    void OnEnable()
    {
        StartCoroutine(ImageCacheManager.Instance.DownloadMultipleImage(ApiRoutes.imageStartingPointURL + user.Image , SetImage));
        userNameField.text = user.Username;
        StartCoroutine(DisplayWinner());
    }

    void OnDisable()
    {
        imageDownload = false;
        winnerImage.gameObject.SetActive(false);  
        StopAllCoroutines();
    }

    void Start()
    {
        playerBtn = GetComponent<Button>();
        playerBtn.onClick.AddListener(DisplayWiner);
    }

    void DisplayWiner()
    {
        //Debug.Log("IS WINNER (PlayerInfo Btn): " + user.IsWinner);
        UIManager.instance.GetScreen<WinnerPopup>().user = user;
        UIManager.instance.GetScreen<WinnerPopup>().imageTexture = image.texture;
        UIManager.instance.GetScreen<WinnerPopup>().Show();
    }

    IEnumerator DisplayWinner()
    {
        //yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => imageDownload == true);
        //Debug.Log("IS WINNER (PlayerInfo Co): " + user.IsWinner);

        if (user.IsWinner == true)
        {         
            winnerImage.gameObject.SetActive(true);
            UIManager.instance.GetScreen<WinnerPopup>().user = user;
            UIManager.instance.GetScreen<WinnerPopup>().imageTexture = image.texture;
            UIManager.instance.GetScreen<WinnerPopup>().Show();
        }
    }

    void SetImage(Texture2D texture)
    {
        image.texture = texture;
        imageDownload = true;
    }
}
