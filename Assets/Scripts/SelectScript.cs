using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SelectScript : MonoBehaviourPunCallbacks
{
    public static SelectScript instance = null;

    public GameObject BGM;
    public AudioClip music;

    private void Awake()
    {
        instance = this;

        BGM = GameObject.Find("BGM");
    }
    public PhotonView PV;

    [Header("StartGame")]
    public Button ReadyGame;
    public GameObject Ready1;
    public GameObject Ready2;

    public bool ready1 = false;
    [PunRPC] void rd1(bool sw) => ready1 = sw;
    public bool ready2 = false;
    [PunRPC] void rd2(bool sw) => ready2 = sw;

    public void ReadyBtn()
    {
        int i;
        bool sw;
        for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i].NickName.Equals(LoginScript.instance.UserId))
                break;

        if (i == 0)
        {
            if (ready1 == false) sw = true;
            else sw = false;
            PV.RPC("rd1", RpcTarget.All, sw);

            PV.RPC("MapInit", RpcTarget.All, map);
            PV.RPC("TurnInit", RpcTarget.All, turn);
            PV.RPC("LightInit", RpcTarget.All, light);
        }
        else if (i == 1)
        {
            if (ready2 == false) sw = true;
            else sw = false;
            PV.RPC("rd2", RpcTarget.All, sw);
        }
    }

    [Header("TankSelect")]
    int btnX = 65;
    int btnY = -50;
    public Button[] TankButton;
    public string p1Tank = "";
    string p1prevTank = "";
    [PunRPC] void player1Tank(string name)
    {
        p1Tank = name;
        GameObject.Find("TankObject").transform.Find("P1_" + name).gameObject.SetActive(true);
        GameObject.Find(name + " Btn").GetComponent<Button>().interactable = false;

        if (!p1prevTank.Equals(""))
        {
            GameObject.Find("TankObject").transform.Find("P1_" + p1prevTank).gameObject.SetActive(false);
            GameObject.Find(p1prevTank + " Btn").GetComponent<Button>().interactable = true;
        }

        p1prevTank = name;
    }

    public string p2Tank = "";
    string p2prevTank = "";
    [PunRPC]  void player2Tank(string name)
    {
        p2Tank = name;
        GameObject.Find("TankObject").transform.Find("P2_" + name).gameObject.SetActive(true);
        GameObject.Find(name + " Btn").GetComponent<Button>().interactable = false;

        if (!p2prevTank.Equals(""))
        {
            GameObject.Find("TankObject").transform.Find("P2_" + p2prevTank).gameObject.SetActive(false);
            GameObject.Find(p2prevTank + " Btn").GetComponent<Button>().interactable = true;
        }

        p2prevTank = name;
    }

    public void TankSelectBtn(string name)
    {
        int i;
        for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i].NickName.Equals(LoginScript.instance.UserId))
                break;

        if(i == 0)
        {
            ReadyGame.interactable = true;
            PV.RPC("player1Tank", RpcTarget.All, name);
        }
        else if(i == 1)
        {
            ReadyGame.interactable = true;
            PV.RPC("player2Tank", RpcTarget.All, name);
        }
    }
    public void TankSelectBtnActive() // x : 65 -> 150+, y : -50 -> -120+
    {
        for (int i = 0; i < LobbyScript.instance.UserTankList.Length; i++)
        {
            switch (LobbyScript.instance.UserTankList[i])
            {
                case "1" : selectBtnXY(0); break;
                case "2" : selectBtnXY(1); break;
                case "3" : selectBtnXY(2); break;
                case "4" : selectBtnXY(3); break;
                case "5" : selectBtnXY(4); break;
                case "6" : selectBtnXY(5); break;
                case "7" : selectBtnXY(6); break;
                case "8" : selectBtnXY(7); break;
                case "9" : selectBtnXY(8); break;
                case "10" : selectBtnXY(9); break;
            }
        }
    }
    void selectBtnXY(int num)
    {
        TankButton[num].GetComponent<RectTransform>().anchoredPosition = new Vector2(btnX, btnY);
        btnX += 150;
        if (btnX > 815)
        {
            btnX = 65;
            btnY += -120;
        }
    }

    [Header("TankStatus")]
    string[] tInfo;
    public Text TankNameText;
    public Image AtkImage;
    public Text AtkText;
    public Image DefImage;
    public Text DefText;
    public Image SpeedImage;
    public Text SpeedText;
    public Text WeightText;
    public Text BoomText;
    public Text TankText;
    public void TankStatus(int num) => StartCoroutine(TankStatusSelect(num));
    IEnumerator TankStatusSelect(int num)
    {
        WWWForm form = new WWWForm();
        form.AddField("tno", num);
        WWW www = new WWW(GameManagerScript.serverURL + "/tankstatus.php", form);
        yield return www;

        string str = www.text;
        tInfo = str.Split('/');
        TankNameText.text = tInfo[0];
        WeightText.text = tInfo[1];
        BoomText.text = tInfo[2];
        TankText.text = tInfo[3];
        AtkText.text = tInfo[4];
        AtkImage.fillAmount = float.Parse(tInfo[4]) / GameManagerScript.stat_max_atk;
        DefText.text = tInfo[5];
        DefImage.fillAmount = float.Parse(tInfo[5]) / GameManagerScript.stat_max_def;
        SpeedText.text = tInfo[6];
        SpeedImage.fillAmount = float.Parse(tInfo[6]) / GameManagerScript.stat_max_speed;
    }
    public void ResetTankStatus()
    {
        TankNameText.text = "탱크 이름";
        AtkImage.fillAmount = 0;
        AtkText.text = "0";
        DefImage.fillAmount = 0;
        DefText.text = "0";
        SpeedImage.fillAmount = 0;
        SpeedText.text = "0";
        WeightText.text = "";
        BoomText.text = "";
        TankText.text = "";
    }

    [PunRPC] void MapInit(int num) => ENBVar.map = num;
    [PunRPC] void TurnInit(int num) => ENBVar.turn = num;
    [PunRPC] void LightInit(int num) => ENBVar.light = num;

    string[] p1Str;
    string[] p2Str;
    public void StartGame()
    {
        if (ready1 == true) Ready1.SetActive(true);
        else Ready1.SetActive(false);

        if (ready2 == true) Ready2.SetActive(true);
        else Ready2.SetActive(false);

        if (ready1 == true && ready2 == true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("MapInit", RpcTarget.All, map);
                PV.RPC("TurnInit", RpcTarget.All, turn);
                PV.RPC("LightInit", RpcTarget.All, light);
            }

            ResetTankStatus();

            ENBVar.tank1 = p1Tank;
            ENBVar.tank2 = p2Tank;

            playerTankStat(playerTankNum(p1Tank), 1);
            ENBVar.p1Atk = int.Parse(p1Str[4]);
            ENBVar.p1Def = int.Parse(p1Str[5]);
            ENBVar.p1Speed = float.Parse(p1Str[6]);
            playerTankStat(playerTankNum(p2Tank), 2);
            ENBVar.p2Atk = int.Parse(p2Str[4]);
            ENBVar.p2Def = int.Parse(p2Str[5]);
            ENBVar.p2Speed = float.Parse(p2Str[6]);

            if (ENBVar.map == 1) SceneManager.LoadScene("G_CityScene");
            else if(ENBVar.map == 2) SceneManager.LoadScene("G_MountainScene");
            else if(ENBVar.map == 3) SceneManager.LoadScene("G_PlainScene");
        }
    }
    public int playerTankNum(string name)
    {
        int num = 0;
        switch (name)
        {
            case "T95 (H)": num = 1; break;
            case "T95 (M)": num = 2; break;
            case "T95 (L)": num = 3; break;
            case "Panzer4 (H)": num = 4; break;
            case "Panzer4 (M)": num = 5; break;
            case "Panzer4 (L)": num = 6; break;
            case "M4A3 (H)": num = 7; break;
            case "M4A3 (M)": num = 8; break;
            case "M4A3 (L)": num = 9; break;
            case "SmallTank": num = 10; break;
        }
        return num;
    }
    public void playerTankStat(int num, int user) => StartCoroutine(TankStatSelect(num, user));
    IEnumerator TankStatSelect(int num, int user)
    {
        WWWForm form = new WWWForm();
        form.AddField("tno", num);
        WWW www = new WWW(GameManagerScript.serverURL + "/tankstatus.php", form);
        yield return www;

        if (user == 1) p1Str = www.text.Split('/');
        else if (user == 2) p2Str = www.text.Split('/');
    }

    [Header("Chatting")] // 채팅관련
    [Header("ETC")]

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
    [PunRPC] void ChatRPC(string msg)
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

    [Header("RoomUserInfo")]
    public Text PlayerName1;
    public Text PlayerName2;

    void Start()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PlayerName1.text = PhotonNetwork.PlayerList[0].NickName;
            PlayerName2.text = PhotonNetwork.PlayerList[1].NickName;
        }
        initcount = 1;

        if(!BGM.name.Equals(music.name))
        {
            BGM.GetComponent<AudioSource>().clip = music;
            BGM.GetComponent<AudioSource>().Play();
        }

        TankSelectBtnActive();
    }

    public int initcount = 1;
    float goneTime = 3.0f;
    int map = 1;
    int turn = 1;
    int light = 1;

    public Text UserInfo;
    void Update()
    {
        UserInfo.text = "ID : " + LoginScript.instance.UserId + " / MMR : " + LoginScript.instance.UserMmr;

        if (ChatInput.isFocused == false)
            if (Input.GetKeyDown(KeyCode.Return))
                ChatInput.Select();

        if(initcount == 1)
        {
            map = Random.Range(1, 4);
            light = Random.Range(1, 6);
            turn = Random.Range(1, 3);
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("MapInit", RpcTarget.All, map);
                PV.RPC("TurnInit", RpcTarget.All, turn);
                PV.RPC("LightInit", RpcTarget.All, light);
            }
            initcount--;
        }

        StartGame();

        if (PlayerCount() != 2)
        {
            goneTime -= Time.deltaTime;

            if (goneTime <= 0.01)
            {
                if (!p1prevTank.Equals(""))
                {
                    GameObject.Find("TankObject").transform.Find("P1_" + p1prevTank).gameObject.SetActive(false);
                    GameObject.Find(p1prevTank + " Btn").GetComponent<Button>().interactable = true;
                }
                if (!p2prevTank.Equals(""))
                {
                    GameObject.Find("TankObject").transform.Find("P2_" + p2prevTank).gameObject.SetActive(false);
                    GameObject.Find(p2prevTank + " Btn").GetComponent<Button>().interactable = true;
                }
                GameObject.Find("Canvas").transform.Find("ExitPanel").gameObject.SetActive(true);
            }
        }
        else goneTime = 0.5f;
    }

    public void ExitBtn()
    {
        RoomScript.instance.ready1 = false;
        RoomScript.instance.ready2 = false;
        ready1 = false;
        ready2 = false;

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }

    int PlayerCount()
    {
        int count = 0;
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            count++;

        return count;
    }
}
