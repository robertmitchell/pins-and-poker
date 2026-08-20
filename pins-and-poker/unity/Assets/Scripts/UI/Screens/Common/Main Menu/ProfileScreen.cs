using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreen : UIScreenBase
{
    public TMP_Text userNameText;
    public TMP_Text emailText;
    public RawImage profilePic;
    public AnimatrixButton backBtn;
    public AnimatrixButton EditProfileBtn;

    [Header("Stats")]
    public TMP_Text gamesCountText;
    public TMP_Text winRecordText;
    public TMP_Text pointsText;
    public TMP_Text moneyEarnedText;

    [Header("QR Code")]
    public RawImage qrCodeImage;

    private void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
        if (PlayerPrefs.HasKey(Db_Keys.userImage)) ImageSaveLoad.Instance.LoadImageFromPlayerPrefs(profilePic);
        else Debug.Log("UserImage Not Available PlayerPrefs");
        userNameText.text = PlayerPrefs.GetString(Db_Keys.userName);
        emailText.text = PlayerPrefs.GetString(Db_Keys.userEmail);

        if (gamesCountText) gamesCountText.text = "Games Played: " + PlayerPrefs.GetInt(Db_Keys.gamesPlayed, 0);
        if (winRecordText) winRecordText.text = "Win Record: " + PlayerPrefs.GetInt(Db_Keys.gamesWon, 0) + "W / " + PlayerPrefs.GetInt(Db_Keys.gamesLost, 0) + "L";
        if (pointsText) pointsText.text = "Points: " + PlayerPrefs.GetInt(Db_Keys.pointsAccumulated, 0);
        if (moneyEarnedText) moneyEarnedText.text = "Money Earned: $" + PlayerPrefs.GetFloat(Db_Keys.moneyEarned, 0f).ToString("F2");
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnClicked());
        EditProfileBtn.onClick.AddListener(() => EditProfileBtnClicked());
    }

    public void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<SettingScreen>();
    }

    public void EditProfileBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<EditProfileScreen>();
    }

    //public override void UpdateScreen<T>(T data)
    //{
    //    if (data is PlayerData profileData)
    //    {
    //        if (userNameText) userNameText.text = profileData.Username;
    //       // if (emailText) emailText.text = profileData.playerEmail;
    //        Debug.LogError(" profileData.playerName_________________ " + profileData.Username+ "profileData.playerEmail"+ profileData.Email);

    //        ImageCacheManager.Instance.GetImage(profileData.Image , OnImageLoaded);
    //    }
    //    else
    //    {
    //        Debug.LogError("Invalid data type for " + gameObject.name);
    //    }

    //}

    private void OnImageLoaded(Texture2D texture)
    {
        if (texture != null)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
           // if(profileImage) profileImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("Failed to load image.");
        }
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}