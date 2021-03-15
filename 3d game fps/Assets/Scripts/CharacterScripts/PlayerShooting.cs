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
            SingleShoot();

        Debug.DrawRay(cam.transform.position, cam.transform.forward * 2.5f, Color.red);
    }

    void SingleShoot()
    {
        RaycastHit hit;

        // rayDistance is the look distance, 
        // transform.forward is forward relative to this object's transform
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2.5f))
        {

            // collision detected
            if (hit.collider.gameObject.layer == 9 && gameObject.layer == 10)
            {
                hit.collider.transform.root.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
                Debug.Log("PLAYERHIT");
            }

            if (hit.collider.gameObject.layer == 12 && gameObject.layer == 9)
            {
                hit.collider.transform.root.gameObject.GetPhotonView().RPC("Unfreeze", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
                Debug.Log("PLAYERHIT");
            }
        }
        else
        {
            // no collision at all
        }
    }

    [PunRPC]
    private void TakeDamage(int p_actor)
    {
        GetComponent<PlayerHealth>().TakeDamage(p_actor);
    }

    [PunRPC]
    private void Unfreeze(int p_actor)
    {
        GetComponent<PlayerHealth>().Unfreeze(p_actor);
    }
}
