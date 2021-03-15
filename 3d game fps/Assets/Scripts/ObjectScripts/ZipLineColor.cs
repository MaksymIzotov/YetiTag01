using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipLineColor : MonoBehaviour
{
    public bool isGreen;

    // Start is called before the first frame update
    void Start()
    {
        isGreen = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isGreen)
            gameObject.GetComponent<Renderer>().material.color = Color.white;

        isGreen = false;
    }
}
