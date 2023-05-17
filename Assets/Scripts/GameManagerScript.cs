using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManagerScript : MonoBehaviourPunCallbacks
{
    public static GameManagerScript instance = null;

    void Awake()
    {
        Screen.SetResolution(1280, 720, false);
        instance = this;
    }

    public PhotonView PV;
    public float sound = 0.2f;
    public bool muteStatus = false;

    // AWS : 13.125.77.214/maetress
    // PI : 172.30.1.30/maetress
    public static string serverURL;
    public static string webURL;

    public float update_time = 2.0f;

    string id = LoginScript.instance.UserId;
    int mmr = LoginScript.instance.UserMmr;
    int money = LoginScript.instance.UserMoney;
    int cash = LoginScript.instance.UserCash;

    public Text StatusText;

    public InputField roomInput;

    void Update()
    {
        update_time -= Time.deltaTime;
        if (update_time <= 0) update_time = 3.0f;

        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void userinfoupdate() => StartCoroutine(UserInfoUpdate());
    IEnumerator UserInfoUpdate()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", LoginScript.instance.UserId);
        WWW www = new WWW(serverURL + "/userinfoupdate.php", form);
        yield return www;

        string str = www.text;

        string[] spstr = str.Split('/');
        LoginScript.instance.UserId = spstr[0];
        LoginScript.instance.UserMmr = int.Parse(spstr[1]);
        LoginScript.instance.UserMoney = int.Parse(spstr[2]);
        LoginScript.instance.UserCash = int.Parse(spstr[3]);
    }
    
    public void playerCustom()
    {
        PhotonNetwork.LocalPlayer.CustomProperties["Mmr"] = LoginScript.instance.UserMmr.ToString();
        PhotonNetwork.LocalPlayer.CustomProperties["Money"] = LoginScript.instance.UserMoney.ToString();
        PhotonNetwork.LocalPlayer.CustomProperties["Cash"] = LoginScript.instance.UserCash.ToString();
    }

    public void Connected() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = id;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "Mmr", mmr } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "Money", money } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "Cash", cash } });
        PhotonNetwork.JoinLobby();
    }

    void gamePlayStatus()
    {
        if (DirectorScript.instance.gamePlay == true)
        {
            DirectorScript.instance.gamePlay = false;

            int p1Mmr = int.Parse(PhotonNetwork.PlayerList[0].CustomProperties["Mmr"].ToString());
            int p2Mmr = int.Parse(PhotonNetwork.PlayerList[1].CustomProperties["Mmr"].ToString());

            StartCoroutine(exitPlayer(p1Mmr, p2Mmr));
        }
    }
    IEnumerator exitPlayer(int p1Mmr, int p2Mmr)
    {
        int resultMmr = 0, dif = 0;

        if (PhotonNetwork.IsMasterClient) dif = p2Mmr - p1Mmr;
        else dif = p1Mmr - p2Mmr;

        resultMmr = ((10 - dif / 10) <= 0) ? 0 : 10 - dif / 10;

        string id = LoginScript.instance.UserId;
        int mmr = LoginScript.instance.UserMmr - resultMmr;
        if (mmr < 0) mmr = 0;
        int money = LoginScript.instance.UserMoney - 500;
        if (money < 0) money = 0;

        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("mmr", mmr);
        form.AddField("money", money);
        WWW www = new WWW(GameManagerScript.serverURL + "/mmrandmoney.php", form);
        yield return www;
    }

    void OnApplicationPause(bool pause)
    {
        gamePlayStatus();

        SceneManager.LoadScene("LoginScene");
        StartCoroutine(Logout());
        PhotonNetwork.Disconnect();
    }
    void OnApplicationQuit()
    {
        gamePlayStatus();

        StartCoroutine(Logout());
        PhotonNetwork.Disconnect();
    }
    public void Disconnect()
    {
        StartCoroutine(Logout());
        PhotonNetwork.Disconnect();
    }
    IEnumerator Logout()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        WWW www = new WWW(serverURL + "/logout.php", form);
        yield return www;
    }
    public override void OnDisconnected(DisconnectCause cause) {
        Disconnect();
    }

    public void JoinOrCreateRoom() => PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 }, null);
    
    public override void OnCreatedRoom() { }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        LobbyScript.instance.RoomName.text = "";
        LobbyScript.instance.CreateRoom(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message) { }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if(LobbyScript.instance.normal == true)
        {
            LobbyScript.instance.normal = false;
            LobbyScript.instance.CreateRoom(2);
        }
        else if (LobbyScript.instance.rank == true)
        {
            LobbyScript.instance.rank = false;
            LobbyScript.instance.CreateRoom(3);
        }
    }

    [ContextMenu("정보")]
    void info()
    {
        if(PhotonNetwork.InRoom) // 방에 있을 때만 표시
        {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대 인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
        else // 로비나 마스터에 있을 때 표시
        {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결 됐는지? : " + PhotonNetwork.IsConnected);
        }
    }

    [Header("환경변수")]
    public static float stat_max_atk;
    public static float stat_max_def;
    public static float stat_max_speed;
    public static int rank_win_min;
    public static int rank_win_max;
    public static int rank_lose_min;
    public static int rank_lose_max;
    public static int rank_draw_min;
    public static int rank_draw_max;
    public static int nor_win_min;
    public static int nor_win_max;
    public static int nor_lose_min;
    public static int nor_lose_max;
    public static int nor_draw_min;
    public static int nor_draw_max;
    public static float item_powershot_max;
    public static float item_haste_max;
    public static float item_repair_max;
    public static float item_invisible_max;
    public static int item_smoke_max;
    public static int item_spy_max;

    public void enbInit() => StartCoroutine(enbInitActive());
    IEnumerator enbInitActive()
    {
        WWW www = new WWW(serverURL + "/enbtable.php");
        yield return www;

        string fullstr = www.text;
        string[] enblist = fullstr.Split('|');

        for (int i = 0; i < enblist.Length; i++)
        {
            string[] enb = enblist[i].Split(' ');

            if (enb[0].Equals("stat_max_atk")) stat_max_atk = float.Parse(enb[1]);
            if (enb[0].Equals("stat_max_def")) stat_max_def = float.Parse(enb[1]);
            if (enb[0].Equals("stat_max_speed")) stat_max_speed = float.Parse(enb[1]);
            if (enb[0].Equals("rank_win_min")) rank_win_min = int.Parse(enb[1]);
            if (enb[0].Equals("rank_win_max")) rank_win_max = int.Parse(enb[1]);
            if (enb[0].Equals("rank_lose_min")) rank_lose_min = int.Parse(enb[1]);
            if (enb[0].Equals("rank_lose_max")) rank_lose_max = int.Parse(enb[1]);
            if (enb[0].Equals("rank_draw_min")) rank_draw_min = int.Parse(enb[1]);
            if (enb[0].Equals("rank_draw_max")) rank_draw_max = int.Parse(enb[1]);
            if (enb[0].Equals("nor_win_min")) nor_win_min = int.Parse(enb[1]);
            if (enb[0].Equals("nor_win_max")) nor_win_max = int.Parse(enb[1]);
            if (enb[0].Equals("nor_lose_min")) nor_lose_min = int.Parse(enb[1]);
            if (enb[0].Equals("nor_lose_max")) nor_lose_max = int.Parse(enb[1]);
            if (enb[0].Equals("nor_draw_min")) nor_draw_min = int.Parse(enb[1]);
            if (enb[0].Equals("nor_draw_max")) nor_draw_max = int.Parse(enb[1]);
            if (enb[0].Equals("item_powershot_max")) item_powershot_max = int.Parse(enb[1]);
            if (enb[0].Equals("item_haste_max")) item_haste_max = int.Parse(enb[1]);
            if (enb[0].Equals("item_repair_max")) item_repair_max = int.Parse(enb[1]);
            if (enb[0].Equals("item_invisible_max")) item_invisible_max = int.Parse(enb[1]);
            if (enb[0].Equals("item_smoke_max")) item_smoke_max = int.Parse(enb[1]);
            if (enb[0].Equals("item_spy_max")) item_spy_max = int.Parse(enb[1]);
        }
    }

    void Start() =>  BtnSound = GameObject.Find("BtnSound");

    public GameObject BtnSound;
    public void ButtonSound() => BtnSound.GetComponent<AudioSource>().Play();
}
