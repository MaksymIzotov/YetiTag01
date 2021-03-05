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
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject menuCam;
    [SerializeField] GameObject yetiText;
    [SerializeField] GameObject pingText;
    [SerializeField] GameObject roleText;

    public bool isGameStarted;

    float gameTime;
    [SerializeField] GameObject timerText;
    GameObject player;

    public int yetiAmount;

    public int role;

    private void Awake()
    {
        isGameStarted = false;
        GameObject spawnParent = GameObject.Find("SpawnPoints");
        spawnPoints = spawnParent.GetComponentsInChildren<Transform>();
        gameTime = 180;
    }

    private void FixedUpdate()
    {
        pingText.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.GetPing().ToString();
    }

    [PunRPC]
    public void SetYeti() => role = 10;

    [PunRPC]
    public void SetSci() => role = 9;

    public void GetYeti(int num)
    {
        yetiAmount = num;
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
        isGameStarted = true;
        gameMenu.SetActive(false);
        inGameMenu.SetActive(true);
        menuCam.SetActive(false);
        UpdateRoleText();
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        player  = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoints[spawnIndex].position, Quaternion.identity);
        player.layer = role;
    }

    private void Update()
    {
        if (!isGameStarted || !PhotonNetwork.IsMasterClient)
            return;

        gameTime -= Time.deltaTime;
        gameObject.GetPhotonView().RPC("UpdateTimer", RpcTarget.All, gameTime);
        if (gameTime <= 0 || yetiAmount >= PhotonNetwork.PlayerList.Count())
        {
            StopGame();
        }
    }

    public void UpdateRoleText() => roleText.GetComponent<TextMeshProUGUI>().text = (role == 10) ? "Yeti" : "Scientist";

    void StopGame()
    {
        //TODO: Stop game and return to settings menu
        gameTime = 180;
        gameObject.GetPhotonView().RPC("DestroyPlayer", RpcTarget.All);
    }

    [PunRPC]
    void DestroyPlayer()
    {
        isGameStarted = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameMenu.SetActive(true);
        inGameMenu.SetActive(false);
        menuCam.SetActive(true);
        PhotonNetwork.Destroy(player);
    }

    [PunRPC]
    void UpdateYetiText(int num) => yetiText.GetComponent<TextMeshProUGUI>().text = num.ToString();

    [PunRPC]
    void UpdateTimer(float time) => timerText.GetComponent<TextMeshProUGUI>().text = (Mathf.Round(time * 100f) / 100f).ToString();
}
