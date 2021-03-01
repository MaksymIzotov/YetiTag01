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
    [SerializeField] GameObject PlayerListItemPrefab;
    Transform playerListContent;
    GameObject startGameButton;
    GameObject increaseButton;
    GameObject decreaseButton;

    GameObject playerSpawner;

    GameObject yetiText;
    int yetiAmount;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
            return;

        playerSpawner = GameObject.Find("Spawner");
        yetiText = GameObject.Find("NumberYeti");
        yetiAmount = 1;
        startGameButton = GameObject.Find("StartGameButton");
        increaseButton = GameObject.Find("IncAmount");
        decreaseButton = GameObject.Find("DecAmount");
        playerListContent = GameObject.Find("PlayersList").transform;
        GameObject spawnParent = GameObject.Find("SpawnPoints");

        startGameButton.GetComponent<Button>().onClick.AddListener(delegate { StartGame(); });
        increaseButton.GetComponent<Button>().onClick.AddListener(delegate { IncreaseAmount(); });
        decreaseButton.GetComponent<Button>().onClick.AddListener(delegate { DecreaseAmount(); });

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
        increaseButton.SetActive(PhotonNetwork.IsMasterClient);
        decreaseButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGame()
    {
        playerSpawner.GetPhotonView().RPC("SpawnAllPlayers", RpcTarget.All, null);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        increaseButton.SetActive(PhotonNetwork.IsMasterClient);
        decreaseButton.SetActive(PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient)
            yetiAmount = int.Parse(yetiText.GetComponent<TextMeshProUGUI>().text);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PV.IsMine)
            return;

        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        playerSpawner.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void IncreaseAmount()
    {
        yetiAmount++;
        playerSpawner.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
    }

    public void DecreaseAmount()
    {
        yetiAmount--;
        playerSpawner.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
    }

    [PunRPC]
    void AssignYeti()
    {

    }

    [PunRPC]
    void AssignSci()
    {

    }

}