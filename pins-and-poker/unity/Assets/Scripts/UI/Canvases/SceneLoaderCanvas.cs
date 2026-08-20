using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoaderCanvas : Singleton<SceneLoaderCanvas>
{
    [SerializeField] Text _loadingTxt;

    private void Start()
    {
        EnableDisableScreen(false);
    }

    private void OnEnable()
    {
        StartCoroutine(ChangeText());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator ChangeText()
    {
        yield return new WaitForSeconds(0.5f);
        _loadingTxt.text = "LOADING PLEASE WAIT";
        yield return new WaitForSeconds(0.5f);
        _loadingTxt.text = "LOADING PLEASE WAIT.";
        yield return new WaitForSeconds(0.5f);
        _loadingTxt.text = "LOADING PLEASE WAIT..";
        yield return new WaitForSeconds(0.5f);
        _loadingTxt.text = "LOADING PLEASE WAIT...";
        StartCoroutine(ChangeText());
    }
}
