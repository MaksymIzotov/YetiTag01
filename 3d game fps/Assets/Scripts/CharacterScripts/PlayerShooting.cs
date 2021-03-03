using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShooting : MonoBehaviour
{

    int damage = 10;

    PhotonView pv;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();

        if (!pv.IsMine)
            return;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pv.IsMine)
            return;

        if (Input.GetMouseButtonDown(0))
            pv.RPC("SingleShoot", RpcTarget.All);

        Debug.DrawRay(cam.transform.position, cam.transform.forward * 1.5f, Color.red);
    }

    [PunRPC]
    void SingleShoot()
    {
        RaycastHit hit;

        // rayDistance is the look distance, 
        // transform.forward is forward relative to this object's transform
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1.5f))
        {

            // collision detected
            if (hit.collider.tag == "Player")
            {
                hit.collider.transform.root.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, damage, PhotonNetwork.LocalPlayer.ActorNumber);
                Debug.Log("PLAYERHIT");
            }
            else
            {

            }
        }
        else
        {
            // no collision at all
        }
    }

    [PunRPC]
    private void TakeDamage(int p_damage, int p_actor)
    {
        GetComponent<PlayerHealth>().TakeDamage(p_damage, p_actor);
    }
}
