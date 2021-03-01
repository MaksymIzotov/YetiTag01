using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviour
{


    GameObject manager;
    PhotonView pv;
    int health;
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        pv = GetComponent<PhotonView>();
        manager = GameObject.Find("PlayerManager(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int p_damage, int p_actor)
    {
        if (pv.IsMine)
        {
            health -= p_damage;
            RefreshHealthBar();

            if (health <= 0)
            {
                //manager.GetComponent<PlayerManager>().CreateController();

                PhotonNetwork.Destroy(gameObject);
            }
            Debug.Log(health);
        }
    }

    public void Respawn()
    {

    }

    void RefreshHealthBar()
    {
        //TODO: Refresh it
    }
}
