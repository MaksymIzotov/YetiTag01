using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;
using System.Linq;

public class PlayerSpawner : MonoBehaviour
{

    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject menuCam;
    [SerializeField] GameObject yetiText;
    [SerializeField] GameObject debugText;

    int role;

    private void Awake()
    {
        GameObject spawnParent = GameObject.Find("SpawnPoints");
        spawnPoints = spawnParent.GetComponentsInChildren<Transform>();
    }

    [PunRPC]
    public void SetYeti() => role = 1;

    [PunRPC]
    public void SetSci() => role = 0;

    public void GetYeti(int num)
    {
        List<Photon.Realtime.Player> shuffledList = PhotonNetwork.PlayerList.OrderBy(x => Random.value).ToList();
        for(int i = 0; i< shuffledList.Count; i++)
        {
            if (i < num)
                gameObject.GetPhotonView().RPC("SetYeti", shuffledList[i]);
            if(i >= num)
                gameObject.GetPhotonView().RPC("SetSci", shuffledList[i]);
        }
    }


    [PunRPC]
    public void SpawnAllPlayers()
    {
        gameMenu.SetActive(false);
        menuCam.SetActive(false);
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoints[spawnIndex].position, Quaternion.identity);
        debugText.GetComponent<TextMeshProUGUI>().text = role.ToString();
    }

    [PunRPC]
    void UpdateYetiText(int num) => yetiText.GetComponent<TextMeshProUGUI>().text = num.ToString();
}
