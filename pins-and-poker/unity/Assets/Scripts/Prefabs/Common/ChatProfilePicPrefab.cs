using UnityEngine;
using UnityEngine.UI;

public class ChatProfilePicPrefab : MonoBehaviour
{
    public RawImage profilePic;

    internal void SetData(Texture2D picture)
    {
        profilePic.texture = picture;
    }
}
