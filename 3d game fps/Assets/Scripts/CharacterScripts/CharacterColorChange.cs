using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterColorChange : MonoBehaviour, IPunObservable
{
    int role = 0;
    int layer;

    bool valuesReceived = false;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(GameObject.Find("Spawner").GetComponent<PlayerSpawner>().role);
            stream.SendNext(gameObject.layer);
        }
        else
        {
            //Network player, receive data
            role = (int)stream.ReceiveNext();
            layer = (int)stream.ReceiveNext();

            valuesReceived = true;

            //Tak suka dont forget to pofixit' eto pusho fps tada gabella

            //if(gameObject.layer != role)
            //{
                GetComponent<PlayerController>().UpdateHnds();
                ChangeColor();
            //}
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
        {
            if(role == 10)
            {
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                gameObject.layer = role;
            }
            if(layer == 12)
            {
                gameObject.layer = layer;
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
            }
        }
    }
}
