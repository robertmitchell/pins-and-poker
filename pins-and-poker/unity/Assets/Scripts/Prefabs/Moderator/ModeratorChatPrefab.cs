using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModeratorChatPrefab : MonoBehaviour
{
    public RawImage profilePic;
    public TMP_Text chatTxt;
    public TMP_Text timeTxt;
    public string playerID;

    internal void SetData(string chat, string time/*, Texture2D picture*/)
    {
        chatTxt.text = chat;
        timeTxt.text = time;
       /* profilePic.texture = picture;*/
    }
}
