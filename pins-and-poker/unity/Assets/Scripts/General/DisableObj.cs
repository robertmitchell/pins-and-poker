using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObj : MonoBehaviour
{
    public float disableTime = 3f;
    private void OnEnable()
    {
        StartCoroutine(DisableObject());

    }

    // Update is called once per frame
    IEnumerator DisableObject()
    {
        yield return new WaitForSeconds(disableTime);
        gameObject.SetActive(false);
    }
}
