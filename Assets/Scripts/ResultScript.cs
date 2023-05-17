using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ResultScript : MonoBehaviourPunCallbacks
{
    public static ResultScript instance = null;

    public GameObject BGM;
    public AudioClip BgmWin;
    public AudioClip BgmLose;

    private void Awake()
    {
        instance = this;
        PhotonNetwork.AutomaticallySyncScene = false;

        BGM = GameObject.Find("BGM");
    }

    [Header("ResultUserInfo")]
    public Text PlayerName1;
    public Text PlayerName2;
    public Text PlayerMmr1;
    public Text PlayerMmr2;
    Hashtable p1;
    Hashtable p2;
    bool leaveFlag;
    string roomMode;

    public void ReturnRoom()
    {
        Win1.SetActive(false);
        Lose1.SetActive(false);
        Win2.SetActive(false);
        Lose2.SetActive(false);

        RoomScript.instance.ready1 = false;
        RoomScript.instance.ready2 = false;
        SelectScript.instance.ready1 = false;
        SelectScript.instance.ready2 = false;

        ENBVar.ResetENBVar();

        if (roomMode.Equals("일반") || roomMode.Equals("랭크"))
        {
            if (PlayerCount() == 2)
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("LobbyScene");
            }
            else SceneManager.LoadScene("LobbyScene");
        }
        else SceneManager.LoadScene("RoomScene");
    }

    [Header("Chatting")] // 채팅관련
    [Header("ETC")]
    public PhotonView PV;

    [Header("ChatScrollView")]
    public Text[] ChatText;
    public InputField ChatInput;

    public void Send()
    {
        if (!ChatInput.text.Equals(""))
        {
            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
            ChatInput.text = "";
        }
    }
    [PunRPC]
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }

    [Header("WinLose")]
    public GameObject Win1;
    public GameObject Lose1;
    public GameObject Win2;
    public GameObject Lose2;
    public GameObject Draw;
    float p1Hp = 0, p2Hp = 0;

    int win = 0;

    int moneyReault()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("랭크"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (win == 1) return Random.Range(GameManagerScript.rank_win_min, GameManagerScript.rank_win_max); // 승
                else if (win == 2) return Random.Range(GameManagerScript.rank_lose_min, GameManagerScript.rank_lose_max); // 패
                else return Random.Range(GameManagerScript.rank_draw_min, GameManagerScript.rank_draw_max); // 무승부
            }
            else
            {
                if (win == 1) return Random.Range(GameManagerScript.rank_lose_min, GameManagerScript.rank_lose_max); // 패
                else if (win == 2) return Random.Range(GameManagerScript.rank_win_min, GameManagerScript.rank_win_max); // 승
                else return Random.Range(GameManagerScript.rank_draw_min, GameManagerScript.rank_draw_max); // 무승부
            }
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("일반"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (win == 1) return Random.Range(GameManagerScript.nor_win_min, GameManagerScript.nor_win_max); // 승
                else if (win == 2) return Random.Range(GameManagerScript.nor_lose_min, GameManagerScript.nor_lose_max); // 패
                else return Random.Range(GameManagerScript.nor_draw_min, GameManagerScript.nor_draw_max); // 무승부
            }
            else
            {
                if (win == 1) return Random.Range(GameManagerScript.nor_lose_min, GameManagerScript.nor_lose_max); // 패
                else if (win == 2) return Random.Range(GameManagerScript.nor_win_min, GameManagerScript.nor_win_max); // 승
                else return Random.Range(GameManagerScript.nor_draw_min, GameManagerScript.nor_draw_max); // 무승부
            }
        }
        else return 0;
    }
    int mmrResult()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("랭크"))
        {
            int p1mmr = int.Parse(PlayerMmr1.text);
            int p2mmr = int.Parse(PlayerMmr2.text);
            int dif, result;

            if (win == 1) dif = p1mmr - p2mmr;
            else if (win == 2) dif = p2mmr - p1mmr;
            else dif = 0;
            result = ((10 - dif / 10) <= 0) ? 0 : 10 - dif / 10;

            if (PhotonNetwork.IsMasterClient)
            {
                if (win == 1) return result;
                else if (win == 2) return -result;
                else return 0;
            }
            else
            {
                if (win == 1) return -result;
                else if (win == 2) return result;
                else return 0;
            }
        }
        else return 0;
    }

    void Start()
    {
        roomMode = PhotonNetwork.CurrentRoom.CustomProperties["Mode"].ToString();

        leaveFlag = true;

        GameManagerScript.instance.userinfoupdate();
        GameManagerScript.instance.playerCustom();

        p1 = PhotonNetwork.PlayerList[0].CustomProperties;
        p2 = PhotonNetwork.PlayerList[1].CustomProperties;

        PlayerName1.text = PhotonNetwork.PlayerList[0].NickName;
        PlayerName2.text = PhotonNetwork.PlayerList[1].NickName;
        PlayerMmr1.text = p1["Mmr"].ToString();
        PlayerMmr2.text = p2["Mmr"].ToString();

        p1Hp = ENBVar.p1EndHP;
        p2Hp = ENBVar.p2EndHP;

        if (ENBVar.p1Hell == true && ENBVar.p2Hell == true)
            win = 3;
        else
            win = (p1Hp > p2Hp) ? 1 : 2;

        if (PhotonNetwork.IsMasterClient)
        {
            if (p1Hp > p2Hp)
                BGM.GetComponent<AudioSource>().clip = BgmWin;
            else
                BGM.GetComponent<AudioSource>().clip = BgmLose;
        }
        else
        {
            if (p2Hp > p1Hp)
                BGM.GetComponent<AudioSource>().clip = BgmWin;
            else
                BGM.GetComponent<AudioSource>().clip = BgmLose;
        }

        if (win == 3) BGM.GetComponent<AudioSource>().clip = BgmLose;

        BGM.GetComponent<AudioSource>().Play();

        resultUpdate();
    }

    void Update()
    {
        p1Hp = ENBVar.p1EndHP;
        p2Hp = ENBVar.p2EndHP;

        if (ENBVar.p1Hell == true && ENBVar.p2Hell == true)
            Draw.SetActive(true);
        else if (p1Hp <= 0)
        {
            Lose1.SetActive(true);
            Win2.SetActive(true);
        }
        else if (p2Hp <= 0)
        {
            Win1.SetActive(true);
            Lose2.SetActive(true);
        }

        if (ChatInput.isFocused == false)
            if (Input.GetKeyDown(KeyCode.Return))
                ChatInput.Select();

        if (leaveFlag == true && PlayerCount() != 2)
            if (PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("일반") || PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("랭크"))
            {
                leaveFlag = false;
                PhotonNetwork.LeaveRoom();
            }
    }
    int PlayerCount()
    {
        int count = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            count++;

        return count;
    }

    void resultUpdate()
    {
        StartCoroutine(MmrAndMoney());
        StartCoroutine(MatchHistory());
    }
    IEnumerator MmrAndMoney()
    {
        string id = LoginScript.instance.UserId;
        int mmr = LoginScript.instance.UserMmr + mmrResult();
        if (mmr < 0) mmr = 0;
        int money = LoginScript.instance.UserMoney + moneyReault();

        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("mmr", mmr);
        form.AddField("money", money);
        WWW www = new WWW(GameManagerScript.serverURL + "/mmrandmoney.php", form);
        yield return www;
    }
    IEnumerator MatchHistory()
    {
        string id = LoginScript.instance.UserId;
        string myTank;
        string enemy;
        string enemyTank;
        string result;
        if (PhotonNetwork.IsMasterClient)
        {
            enemy = PhotonNetwork.PlayerList[1].NickName;
            if (win == 1) result = "승";
            else if (win == 2) result = "패";
            else result = "무";
            myTank = ENBVar.tank1;
            enemyTank = ENBVar.tank2;
        }
        else
        {
            enemy = PhotonNetwork.PlayerList[0].NickName;
            if (win == 1) result = "패";
            else if (win == 2) result = "승";
            else result = "무";
            myTank = ENBVar.tank2;
            enemyTank = ENBVar.tank1;
        }

        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("mytank", myTank);
        form.AddField("enemy", enemy);
        form.AddField("enemytank", enemyTank);
        form.AddField("result", result);
        WWW www = new WWW(GameManagerScript.serverURL + "/matchhistory.php", form);
        yield return www;
    }
}
