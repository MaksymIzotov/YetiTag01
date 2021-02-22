using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    [SerializeField] Transform[] spawnPoints;
    GameObject menuCam;

    void Awake()
    {
        menuCam = GameObject.Find("MenuCamera");
        GameObject spawnParent = GameObject.Find("SpawnPoints");
        spawnPoints = spawnParent.GetComponentsInChildren<Transform>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        //if (PV.IsMine)
        //{
        //    CreateController();
        //}
    }

    public void CreateController()
    {
        menuCam.SetActive(false);
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoints[spawnIndex].position, Quaternion.identity);
    }
}