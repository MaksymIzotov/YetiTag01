using Photon.Pun;
using UnityEngine;

public class CharacterControllerSync : MonoBehaviour, IPunObservable
{
    CharacterController cc;

    Vector3 latestPos;
    Quaternion latestRot;
    float height;
    Vector3 center;


    bool valuesReceived = false;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(cc.height);
            stream.SendNext(cc.center);
        }
        else
        {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            height = (float)stream.ReceiveNext();
            center = (Vector3)stream.ReceiveNext();

            valuesReceived = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine && valuesReceived)
        {
            //Update Object position and Rigidbody parameters
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
            cc.height = height;
            cc.center = center;
        }
    }

    void OnCollisionEnter(Collision contact)
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            Transform collisionObjectRoot = contact.transform.root;
            if (collisionObjectRoot.CompareTag("Player"))
            {
                //Transfer PhotonView of Rigidbody to our local player
                GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }
    }
}
