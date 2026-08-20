using System.Collections;
using TMPro;
using UnityEngine;

public class DefaultLoaderCanvas : Singleton<DefaultLoaderCanvas>
{
    [SerializeField] TMP_Text _loadingTxt;

    private void Start()
    {
        gameObject.SetActive(false);
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
