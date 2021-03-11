﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterColorChange : MonoBehaviour, IPunObservable
{
    int role = 0;

    bool valuesReceived = false;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(GameObject.Find("Spawner").GetComponent<PlayerSpawner>().role);
        }
        else
        {
            //Network player, receive data
            role = (int)stream.ReceiveNext();

            valuesReceived = true;
            ChangeColor();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ChangeColor()
    {       
        if (!GetComponent<PhotonView>().IsMine && valuesReceived)
            if (role == 10)
            {
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                gameObject.layer = role;
            }
    }
}