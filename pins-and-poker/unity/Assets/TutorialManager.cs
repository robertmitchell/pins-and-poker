using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIAnimatrix;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class TutorialManager : Singleton<TutorialManager>
{
    [System.Serializable]
    public class TutorialStep
    {
        public string message;
        public GameObject highlightObject;
        public GameObject objectPosition;
        public bool isClickable;
        public bool isTutorialEnd;
    }
    public List<TutorialStep> steps;
    public TMP_Text tutorialText;
    public GameObject tutorialPanel;
    private int currentStep = 0;
    public List<GameObject> disableObjects;
    [SerializeField] private Button NextBtn;

    void Start()
    {
        if (IsFirstTimeOpening() && PlayerPrefs.GetString(Db_Keys.userType) == Global.UserType.moderator.ToString())
        {
            UnityEngine.Debug.Log("ShowTutorial chla");
            ShowTutorial();
          /*  foreach (var btn in UIManager.instance.GetScreen<HomeScreen>().buttonDetails)
            {
                btn.gameObject.SetActive(true);
            }*/
            PlayerPrefs.SetInt(Db_Keys.isFirstTime, 1);
            PlayerPrefs.Save();
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    void ShowTutorial()
    {
        Global.ShowTutorial = true;
        tutorialPanel.SetActive(true);

        foreach (var item in disableObjects)
        {
            UnityEngine.Debug.Log("Disable Objects------------------");
            item.SetActive(false);
        }

        foreach (var item in UIManager.instance.GetScreen<HomeScreen>().buttonDetails)
        {
            item.SetActive(false);
        }
        NextBtn.onClick.AddListener(() => NextBtnClicked());
        NextBtnClicked();
    }

    public void NextBtnClicked()
    {
        SetCanvasLayer(steps[currentStep].highlightObject, steps[currentStep].isClickable);
    }

    void SetCanvasLayer(GameObject obj, bool clickable)
    {
        if (currentStep > 0)
        {
            RemoveCanvasLayer(steps[currentStep - 1].highlightObject);
        }
        if (obj != null)
        {
            if (clickable && obj.GetComponent<GraphicRaycaster>() == null)
            {
                obj.AddComponent<GraphicRaycaster>();
                Canvas canvas = obj.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.overrideSorting = true;
                canvas.sortingLayerName = "Tutorial";
                canvas.sortingOrder = 0;
                NextBtn.gameObject.SetActive(false);

                obj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    tutorialPanel.SetActive(false);
                    if (steps[currentStep - 1].isTutorialEnd == true)
                    { 
                        EndTutorial();
                    }
                });
            }

            if (obj.GetComponent<Canvas>() == null)
            {
                Canvas canvas = obj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.overrideSorting = true;
                canvas.sortingLayerName = "Tutorial";
                canvas.sortingOrder = 0;
            }
        }

        ShowMessage(currentStep);
        currentStep++;
    }

    void RemoveCanvasLayer(GameObject obj)
    {
        if (obj != null)
        {
            GraphicRaycaster raycaster = obj.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
            {
                Destroy(raycaster);
            }
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                Destroy(canvas);
            }
        }
    }

    void NextMessage(string msg, GameObject targetObject)
    {
        Transform parent = tutorialText.transform.parent; 

        if (parent != null)
        {
            parent.gameObject.SetActive(false); 
            tutorialText.text = msg;
            parent.gameObject.SetActive(true);  
            parent.position = targetObject.transform.position; 
        }
        else
        {
            Debug.LogWarning("tutorialText has no parent!");
        }
    }

    void ShowMessage(int stepIndex)
    {
        /* if (stepIndex >= steps.Count)
         {

             EndTutorial();
             return;
         }*/
        NextMessage(steps[stepIndex].message, steps[stepIndex].objectPosition);
    }

    public void EndTutorial()
    {
        foreach (var btn in UIManager.instance.GetScreen<HomeScreen>().buttonDetails)
        {
            btn.gameObject.SetActive(true);
        }
        RemoveCanvasLayer(steps[currentStep - 1].highlightObject);
        ResetDisableObjects();
        Global.ShowTutorial = false;
        tutorialPanel?.SetActive(false);
    }

    void ResetDisableObjects()
    {
        foreach (var obj in disableObjects)
        {
            obj?.SetActive(true);
        }
    }

    private bool IsFirstTimeOpening()
    {
        return PlayerPrefs.GetInt(Db_Keys.isFirstTime, 0) == 0;
    }
}
