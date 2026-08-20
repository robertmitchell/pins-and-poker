using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public IUIScreen currentScreen;

    public List<UIScreenBase> screensPool;

    public static UIManager instance;

    Dictionary<Type, IUIScreen> screensDic = new();

    private void Awake()
    {
        instance = this;
        RegisterScreens();
    }


    [ContextMenu("Register Screens")]
    private void RegisterScreens()
    {
        screensPool = FindObjectsOfType<UIScreenBase>(true).ToList();

        foreach (var screen in screensPool)
        {
            if (screen != null)
            {
                RegisterScreen(screen);
            }
        }
    }

    public void RegisterScreen(IUIScreen screen)
    {
        var type = screen.GetType();
        if (!screensDic.ContainsKey(type))
        {
            screensDic.Add(type, screen);
            if (screen.isDefaultScreen())
            {
                currentScreen = screen;
                currentScreen.Show();
            }
            Debug.Log($"{type.Name} registered.");
        }
        else
        {
            Debug.LogWarning($"{type.Name} is already registered.");
        }
    }


    public T GetScreen<T>() where T : IUIScreen
    {
        var type = typeof(T);
        if (screensDic.TryGetValue(type, out var newScreen))
        {
            return (T)newScreen;
        }
        return default(T);
    }

    public void Show<T>(Action action = null) where T : IUIScreen
    {
        var type = typeof(T);
        if (screensDic.TryGetValue(type, out var newScreen))
        {
                currentScreen = newScreen;
                currentScreen.Show(action);

            Debug.Log($"{type.Name} activated.");
        }
        else
        {
            Debug.LogError($"Screen of type {type.Name} not found.");
        }
    }

    public void Hide(Action action = null)
    {
        if (currentScreen != null)
        {
            currentScreen.Hide(action);
        }
    }
}
