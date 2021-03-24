using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    // Start is called before the first frame update
    public int avgFrameRate;

    public void Update()
    {
        float current = 0;
        current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        GetComponent<TextMeshProUGUI>().text = avgFrameRate.ToString() + " FPS";
    }
}
