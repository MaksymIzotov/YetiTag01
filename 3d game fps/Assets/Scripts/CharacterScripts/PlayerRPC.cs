using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerRPC : MonoBehaviour
{
    PhotonView pv;


    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum Teams
    {
        Yeti = 0,
        Sci = 1
    }
}
