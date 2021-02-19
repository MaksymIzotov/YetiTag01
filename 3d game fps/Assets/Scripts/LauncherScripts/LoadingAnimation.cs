using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingAnimation : MonoBehaviour
{
    string[] loadingCondition = {"Loading", "Loading.", "Loading..", "Loading..." };
    int loadingState;

    IEnumerator Loading()
    {
        while (true)
        {
            GetComponent<TMP_Text>().text = loadingCondition[loadingState];
            loadingState++;
            if (loadingState == 4)
                loadingState = 0;
            yield return new WaitForSeconds(.5f);
        }
    }

    private void OnEnable()
    {
        loadingState = 0;
        StartCoroutine("Loading");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
