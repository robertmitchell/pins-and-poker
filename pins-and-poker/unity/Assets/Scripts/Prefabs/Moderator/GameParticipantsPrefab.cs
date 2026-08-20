using System.Collections;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class GameParticipantsPrefab : MonoBehaviour
{
    public RawImage profileImg;
    public TMP_Text playerNameTxt;
    public AnimatrixButton assignLaneBtn;
    public User userdata;

    private void OnEnable()
    {
        StartCoroutine(SetData());
    }

    void Start()
    {
        assignLaneBtn.onClick.AddListener(() => AssignedLaneBtnClicked());
    }

    IEnumerator SetData() 
    {
        yield return new WaitForSeconds(0.5f);
        playerNameTxt.text = userdata.Username;
        StartCoroutine(ImageCacheManager.Instance.DownloadMultipleImage(ApiRoutes.imageStartingPointURL + userdata.Image, SetImage));
    }

    void SetImage(Texture2D texture)
    {
        profileImg.texture = texture;
    }

    void AssignedLaneBtnClicked()
    {
        BGMusic.Instance.btn_audioSource.Play();
        AssignLanePopupScreen.Instance.profileImg.texture = profileImg.texture;       
        AssignLanePopupScreen.Instance.userdata = userdata;
        AssignLanePopupScreen.Instance.gameObject.SetActive(true);
    }
}
