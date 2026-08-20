using UIAnimatrix;
using UnityEngine.UI;

public class ScoreApprovalScreen : UIScreenBase
{
    public AnimatrixButton BackBtn;
    public ScrollRect scoreApprovalScrollRect;

    private void OnEnable()
    {
        //FadeOutCanvas.Instance.PlayFadeOutEffect();
        scoreApprovalScrollRect.verticalNormalizedPosition = 1;
    }

    void Start()
    {
        BackBtn.onClick.AddListener(() => BackBtnClicked());
    }

    void BackBtnClicked()
    {
        UIManager.instance.Hide();
        UIManager.instance.Show<HomeScreen>();
    }

    public override void UpdateScreen<T>(T data)
    {
        throw new System.NotImplementedException();
    }
}