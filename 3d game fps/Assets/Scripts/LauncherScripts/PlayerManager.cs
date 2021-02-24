using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;
using System.Linq;
using System;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    static public PlayerManager Instance;

    PhotonView PV;
    [SerializeField] Transform[] spawnPoints;
    GameObject menuCam;
    GameObject gameMenu;
    [SerializeField] GameObject PlayerListItemPrefab;
    Transform playerListContent;
    GameObject startGameButton;
    GameObject IncreaseButton;
    GameObject DecreaseButton;

    GameObject yetiText;
    int yetiAmount;

    void Awake()
    {
        Instance = this;


        yetiText = GameObject.Find("NumberYeti");
        yetiAmount = 1;
        startGameButton = GameObject.Find("StartGameButton");
        IncreaseButton = GameObject.Find("IncAmount");
        DecreaseButton = GameObject.Find("DecAmount");
        playerListContent = GameObject.Find("PlayersList").transform;
        menuCam = GameObject.Find("MenuCamera");
        gameMenu = GameObject.Find("PreGameMenu");
        GameObject spawnParent = GameObject.Find("SpawnPoints");
        spawnPoints = spawnParent.GetComponentsInChildren<Transform>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        startGameButton.GetComponent<Button>().onClick.AddListener(delegate { StartGame(); });
        IncreaseButton.GetComponent<Button>().onClick.AddListener(delegate { IncreaseAmount(); });
        DecreaseButton.GetComponent<Button>().onClick.AddListener(delegate { DecreaseAmount(); });

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGame()
    {
        Debug.Log("PIVAS");
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient)
        {
            yetiAmount = int.Parse(yetiText.GetComponent<TextMeshPro>().text);

        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void CreateController()
    {
        gameMenu.SetActive(false);
        menuCam.SetActive(false);
        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoints[spawnIndex].position, Quaternion.identity);
    }

    [PunRPC]
    public void IncreaseAmount()
    {
        yetiAmount++;
        UpdateYetiText();
        Instance.photonView.RPC("IncreaseAmount", RpcTarget.All, null);
    }

    [PunRPC]
    public void DecreaseAmount()
    {

        yetiAmount--;
        UpdateYetiText();
        Instance.photonView.RPC("DecreaseAmount", RpcTarget.All, null);
    }

    void UpdateYetiText()
    {
        yetiText.GetComponent<TextMeshPro>().text = yetiAmount.ToString();
    }
}