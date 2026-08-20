using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UserChatPrefab : MonoBehaviour
{
    public RawImage profilePic;
    public TMP_Text chatTxt;
    public TMP_Text timeTxt;
    //private void OnEnable()
    //{
    //   gameObject.transform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f) ,0.1f);
    //}

    internal void SetData(string chat, string time, Texture2D picture)
    {
        chatTxt.text = chat;
        timeTxt.text = time;
        profilePic.texture = picture;
    }
}
