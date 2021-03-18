using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{

    [SerializeField] Transform[] spawnPointsSCI;
    [SerializeField] Transform[] spawnPointsYETI;
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject menuCam;
    [SerializeField] GameObject yetiText;
    [SerializeField] GameObject pingText;
    [SerializeField] GameObject roleText;
    [SerializeField] GameObject sensText;
    Transform playerListContent;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject increaseButton;
    [SerializeField] GameObject decreaseButton;
    [SerializeField] GameObject leaveButton;
    [SerializeField] GameObject PlayerListItemPrefab;

    bool hasLeft;
    public int yetiAmount;


    [SerializeField] Slider sensSlider;

    public bool isGameStarted;

    float gameTime;
    [SerializeField] GameObject timerText;
    GameObject player;

    public int YetiNum;

    public int role;

    public bool isPaused;

    private void Awake()
    {
        isGameStarted = false;
        gameTime = 180;
    }

    private void FixedUpdate()
    {
        pingText.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.GetPing().ToString();
    }
    private void Start()
    {
        spawnPointsSCI = GameObject.Find("SpawnPointsSCI").transform.GetComponentsInChildren<Transform>();
        spawnPointsYETI = GameObject.Find("SpawnPointsYETI").transform.GetComponentsInChildren<Transform>();

        isPaused = false;
        hasLeft = false;
        sensSlider.value = PlayerPrefs.GetFloat("Sens");
        ValueChangeCheck();
        sensSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        yetiAmount = 1;
        playerListContent = GameObject.Find("PlayersList").transform;

        startGameButton.GetComponent<Button>().onClick.AddListener(delegate { GetYeti(); gameObject.GetPhotonView().RPC("SpawnAllPlayers", RpcTarget.All); });
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

    public void ValueChangeCheck()
    {
        sensText.GetComponent<TextMeshProUGUI>().text = "Sensitivity: " + sensSlider.value;
        PlayerPrefs.SetFloat("Sens",sensSlider.value);
    }

    [PunRPC]
    public void SetYeti() => role = 10;

    [PunRPC]
    public void SetSci() => role = 9;

    public void GetYeti()
    {
        List<Photon.Realtime.Player> shuffledList = PhotonNetwork.PlayerList.OrderBy(x => Random.value).ToList();
        for(int i = 0; i< shuffledList.Count; i++)
        {
            if (i < yetiAmount)
                gameObject.GetPhotonView().RPC("SetYeti", shuffledList[i]);
            if(i >= yetiAmount)
                gameObject.GetPhotonView().RPC("SetSci", shuffledList[i]);
        }
    }

    public void IncreaseAmount()
    {
        if (yetiAmount < PhotonNetwork.PlayerList.Length - 1)
        {
            yetiAmount++;
            gameObject.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
        }
    }

    public void DecreaseAmount()
    {
        if (yetiAmount > 1)
        {
            yetiAmount--;
            gameObject.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
        }
    }

    public void LeaveRoom()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        if (PhotonNetwork.IsMasterClient)
            gameObject.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        increaseButton.SetActive(PhotonNetwork.IsMasterClient);
        decreaseButton.SetActive(PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient && !isGameStarted)
            yetiAmount = int.Parse(yetiText.GetComponent<TextMeshProUGUI>().text);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!isGameStarted)
        {
            UpdatePlayerList();
            if (yetiAmount == PhotonNetwork.PlayerList.Length && PhotonNetwork.IsMasterClient)
            {
                if (yetiAmount == 1)
                    return;

                yetiAmount--;
                gameObject.GetPhotonView().RPC("UpdateYetiText", RpcTarget.All, yetiAmount);
            }
        }
        else
        {
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);
            hasLeft = true;
        }
    }
    void UpdatePlayerList()
    {
        foreach (Transform trans in playerListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(PhotonNetwork.PlayerList[i]);
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
        if(role == 10)
        {
            int spawnIndex = Random.Range(0, spawnPointsYETI.Length);
            player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPointsYETI[spawnIndex].position, Quaternion.identity);
        }
        else
        {
            int spawnIndex = Random.Range(0, spawnPointsSCI.Length);
            player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPointsSCI[spawnIndex].position, Quaternion.identity);
        }
        

        player.layer = role;
        player.GetComponent<Look>().sensitivity = sensSlider.value;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isGameStarted)
        {
            gameMenu.SetActive(!gameMenu.activeSelf);
            increaseButton.SetActive(!increaseButton.activeSelf);
            decreaseButton.SetActive(!decreaseButton.activeSelf);
            startGameButton.SetActive(!startGameButton.activeSelf);
            isPaused = !isPaused;
            Cursor.visible = !Cursor.visible;
            if(Cursor.visible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }

        if (!isGameStarted || !PhotonNetwork.IsMasterClient)
            return;

        gameTime -= Time.deltaTime;
        gameObject.GetPhotonView().RPC("UpdateTimer", RpcTarget.All, gameTime);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject n in players)
        {
            if (n.layer == 10 || n.layer == 12)
                YetiNum++;
        }

        if(PhotonNetwork.PlayerList.Count() != 1)
        {
            if (gameTime <= 0 || yetiAmount >= PhotonNetwork.PlayerList.Count())
            {
                StopGame();
            }
        }
        YetiNum = 0;
    }

    public void UpdateRoleText() => roleText.GetComponent<TextMeshProUGUI>().text = (role == 10) ? "Yeti" : "Scientist";

    void StopGame()
    {
        //TODO: Stop game and return to settings menu
        if(hasLeft)
            UpdatePlayerList();

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
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        increaseButton.SetActive(PhotonNetwork.IsMasterClient);
        decreaseButton.SetActive(PhotonNetwork.IsMasterClient);
        menuCam.SetActive(true);
        PhotonNetwork.Destroy(player);
    }

    [PunRPC]
    void UpdateYetiText(int num) => yetiText.GetComponent<TextMeshProUGUI>().text = num.ToString();

    [PunRPC]
    void UpdateTimer(float time) => timerText.GetComponent<TextMeshProUGUI>().text = (Mathf.Round(time * 100f) / 100f).ToString();
}
