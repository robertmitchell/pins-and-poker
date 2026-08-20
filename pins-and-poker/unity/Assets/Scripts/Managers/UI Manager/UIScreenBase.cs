using System;
using UnityEngine;

public abstract class UIScreenBase : MonoBehaviour, IUIScreen
{

    public bool isDefaultScreen;

    public void Show(Action action = null) 
    {
        gameObject.SetActive(true);       
        action?.Invoke();
    }

    public void Hide(Action action = null)
    {
        //Debug.Log("Hide");
        gameObject.SetActive(false);
        action?.Invoke();
    }

    public abstract void UpdateScreen<T>(T data);

    bool IUIScreen.isDefaultScreen()
    {
        return isDefaultScreen;
    }
}
