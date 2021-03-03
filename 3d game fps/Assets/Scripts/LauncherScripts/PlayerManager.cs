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
    GameObject leaveButton;

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

        leaveButton = GameObject.Find("LeaveRoomButton");
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
        leaveButton.GetComponent<Button>().onClick.AddListener(delegate { LeaveRoom(); });

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
        if (PhotonNetwork.IsMasterClient)
            playerSpawner.GetComponent<PlayerSpawner>().GetYeti(yetiAmount);

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
        if (PhotonNetwork.IsMasterClient)
            playerSpawner.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
    }

    public void LeaveRoom()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (Transform trans in playerListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(PhotonNetwork.PlayerList[i]);
        }
        if (yetiAmount == PhotonNetwork.PlayerList.Length && PhotonNetwork.IsMasterClient)
        {
            if (yetiAmount == 1)
                return;

            yetiAmount--;
            playerSpawner.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
        }
    }

    public void IncreaseAmount()
    {
        if(yetiAmount < PhotonNetwork.PlayerList.Length - 1)
        {
            yetiAmount++;
            playerSpawner.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
        }
    }

    public void DecreaseAmount()
    {
        if(yetiAmount > 1)
        {
            yetiAmount--;
            playerSpawner.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
        }
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