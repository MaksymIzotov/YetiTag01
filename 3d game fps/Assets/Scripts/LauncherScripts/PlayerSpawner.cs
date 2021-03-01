using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;

public class PlayerSpawner : MonoBehaviour
{

    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject menuCam;
    [SerializeField] GameObject yetiText;

    private void Awake()
    {
        GameObject spawnParent = GameObject.Find("SpawnPoints");
        spawnPoints = spawnParent.GetComponentsInChildren<Transform>();
    }


    [PunRPC]
    public void SpawnAllPlayers()
    {
        gameMenu.SetActive(false);
        menuCam.SetActive(false);
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoints[spawnIndex].position, Quaternion.identity);
    }

    [PunRPC]
    void UpdateYetiText(int num) => yetiText.GetComponent<TextMeshProUGUI>().text = num.ToString();
}
