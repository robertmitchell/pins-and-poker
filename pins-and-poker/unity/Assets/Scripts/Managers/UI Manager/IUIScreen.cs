using System;

public interface IUIScreen
{
    bool isDefaultScreen();

    void Show(Action action = null);
    void Hide(Action action = null);
    void UpdateScreen<T>(T data);
}
