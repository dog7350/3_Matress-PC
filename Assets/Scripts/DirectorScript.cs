using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class DirectorScript : MonoBehaviourPunCallbacks
{
    public static DirectorScript instance = null;

    public GameObject BGM;
    public AudioClip BgmCity;
    public AudioClip BgmMountain;
    public AudioClip BgmPlain;

    private void Awake()
    {
        instance = this;
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = true;

        BGM = GameObject.Find("BGM");
        if (ENBVar.map == 1) BGM.GetComponent<AudioSource>().clip = BgmCity;
        if (ENBVar.map == 2) BGM.GetComponent<AudioSource>().clip = BgmMountain;
        if (ENBVar.map == 3) BGM.GetComponent<AudioSource>().clip = BgmPlain;
        BGM.GetComponent<AudioSource>().Play();
    }

    public PhotonView PV;
    
    [Header("ENBObject")]
    public Light LightObject;
    bool exit = false;
    public AudioSource Sound;
    public Canvas canvas;
    public Canvas cameraCanvas;
    public Text Degree;
    public Text AtkInfo;
    public Text MoveInfo;
    public Text ChatInfo;
    int maxitemPoint;
    int maxItemCount;
    public GameObject itemGenPoint;
    public Sprite ItemSlot;
    public Sprite ItemDoubleShot;
    public Sprite ItemPowerShot;
    public Sprite ItemHeist;
    public Sprite ItemRepair;
    public Sprite ItemSmoke;
    public Sprite ItemInvisible;
    public Sprite ItemSpy;
    public Image Smoke;
    int smokeFlag = 0;
    int smokeTime = 0;
    public bool invisibleFlag = false;
    public int invisibleCount = 0;
    public bool spyFlag = false;
    public int spyCount = 0;
    int cameraSpyFlag = 0;
    public bool gamePlay = true;

    [Header("HellObject")]
    public GameObject hellPoint;
    public GameObject hellObject;
    public GameObject[] planeHell;
    public GameObject[] planeInfo;
    public int cityHell = 15;
    public int mountainHell = 21;
    public int plainHell = 9;
    bool hellAlarm = false;

    [Header("PlayerUserObject")]
    public GameObject StartPoint1;
    public GameObject StartPoint2;
    public GameObject PlayerTank;
    public Image HP, ENEMYHP, POWER, PrevPOWER, MOVE;
    public Image Item1, Item2;
    public Text AtkCount;
    public float p1Atk, p1Def, p1Move, p2Atk, p2Def, p2Move;

    public struct player
    {
        public float hp, atk, def, move, moveCount;
        public int shotCount;

        public player(float hp, float atk, float def, float move, int shotCount, float moveCount)
        {
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.move = move;
            this.shotCount = shotCount;
            this.moveCount = moveCount;
        }
    }
    public player player1;
    public player player2;

    [Header("ChangeTurn")]
    public GameObject TurnPanel;
    public AudioClip TurnPass;
    public bool p1Turn;
    public bool p2Turn;
    public float turnTime = 30.0f;
    public Text TurnStatus;
    public Text TimeStatus;
    public int turnCount = 0;
    public Text LostTurn;

    [PunRPC] public void ChangeTurn(string status)
    {
        p1Turn = false;
        p2Turn = false;

        PlayerScript.instance.turnInit = true;

        turnTime = 30.0f;

        MOVE.fillAmount = 0;
        POWER.fillAmount = 0;
        ENBVar.shotPower = 0f;

        Sound.PlayOneShot(TurnPass);

        if (status.Equals("Player1 Turn"))
        {
            TurnPanel.GetComponent<Image>().color = new Color(255/255f, 255/255f, 150/255f, 255/255f);

            p1Turn = true;
            p2Turn = false;

            player1.moveCount = 1;
            player1.shotCount = 1;
            player2.moveCount = 0;
            player2.moveCount = 0;
            if (PhotonNetwork.IsMasterClient) MOVE.fillAmount = 1;
        }
        else if(status.Equals("Player2 Turn"))
        {
            TurnPanel.GetComponent<Image>().color = new Color(150/255f, 255/255f, 255/255f, 255/255f);

            p1Turn = false;
            p2Turn = true;

            player1.moveCount = 0;
            player1.shotCount = 0;
            player2.moveCount = 1;
            player2.shotCount = 1;
            if (!PhotonNetwork.IsMasterClient) MOVE.fillAmount = 1;
        }

        resetStat();
        
        if(invisibleFlag == true)
        {
            if(invisibleCount > 2)
            {
                invisibleFlag = false;
                invisibleCount = 0;
            }
            else
            {
                invisibleCount++;
                if(PlayerScript.instance.userNumber == 1) player1.def = GameManagerScript.item_invisible_max;
                else if(PlayerScript.instance.userNumber == 2) player2.def = GameManagerScript.item_invisible_max;
            }
        }

        if (spyFlag == true)
        {
            if (turnCount >= 5)
                if (spyCount > GameManagerScript.item_spy_max)
                {
                    spyFlag = false;
                    spyCount = GameManagerScript.item_spy_max;
                }
                else
                {
                    spyCount++;
                }
        }
        else if (spyFlag == false) planeInfoDeActive();

        itemPointer();

        hellAlarm = true;

        turnCount++;
        LostTurn.text = turnCount.ToString();
    }
    public void PassTurn(int userNumber)
    {
        if (userNumber == 1) PV.RPC("ChangeTurn", RpcTarget.AllBuffered, "Player2 Turn");
        else if (userNumber == 2) PV.RPC("ChangeTurn", RpcTarget.AllBuffered, "Player1 Turn");
    }

    [PunRPC] public void itemGenerator()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int num = Random.Range(1, maxItemCount);
            int point = 0;

            for (int i = 0; i < num; i++)
            {
                point = Random.Range(1, maxitemPoint);
                itemGenPoint = GameObject.Find("ItemObject").transform.GetChild(point).gameObject;
                PhotonNetwork.Instantiate("AmmoBox", itemGenPoint.transform.position, Quaternion.identity);
            }
        }
    }
    public void itemPointer() => PV.RPC("itemGenerator", RpcTarget.All);
    // 아이템 사용
    public void doubleShot(int user)
    {
        if (user == 1) player1.shotCount = 1;
        else if (user == 2) player2.shotCount = 1;

        spyChat(PhotonNetwork.NickName + "님이 '더블샷'아이템을 사용하였습니다.");
    }
    public void powerShot(int user)
    {
        if (user == 1) PV.RPC("atk1Add", RpcTarget.All);
        else if (user == 2) PV.RPC("atk2Add", RpcTarget.All);

        spyChat(PhotonNetwork.NickName + "님이 '파워샷'아이템을 사용하였습니다.");
    }
    [PunRPC] void atk1Add() => player1.atk *= GameManagerScript.item_powershot_max;
    [PunRPC] void atk2Add() => player2.atk *= GameManagerScript.item_powershot_max;
    public void haste(int user)
    {
        if (user == 1) PV.RPC("move1Add", RpcTarget.All);
        else if (user == 2) PV.RPC("move2Add", RpcTarget.All);

        spyChat(PhotonNetwork.NickName + "님이 '헤이스트'아이템을 사용하였습니다.");
    }
    [PunRPC] void move1Add() => player1.move *= GameManagerScript.item_haste_max;
    [PunRPC] void move2Add() => player2.move *= GameManagerScript.item_haste_max;
    public void repair(int user)
    {
        float addHp = GameManagerScript.item_repair_max;

        if (user == 1)
            PV.RPC("HP1Add", RpcTarget.All, addHp);
        else if (user == 2)
            PV.RPC("HP2Add", RpcTarget.All, addHp);

        addHp = addHp * 0.01f;
        HP.fillAmount += addHp;

        spyChat(PhotonNetwork.NickName + "님이 '수리도구'아이템을 사용하였습니다.");
    }
    [PunRPC]
    void HP1Add(float addHp)
    {
        DirectorScript.instance.player1.hp += addHp;
        if (DirectorScript.instance.player1.hp > 100) DirectorScript.instance.player1.hp = 100;
    }
    [PunRPC]
    void HP2Add(float addHp)
    {
        DirectorScript.instance.player2.hp += addHp;
        if (DirectorScript.instance.player2.hp > 100) DirectorScript.instance.player2.hp = 100;
    }
    public void smoke()
    {
        PV.RPC("playerSmoke", RpcTarget.All);
        Smoke.fillAmount = 0;
        smokeFlag = 0;

        spyChat(PhotonNetwork.NickName + "님이 '연막탄'아이템을 사용하였습니다.");
    }
    [PunRPC]
    void playerSmoke()
    {
        Smoke.fillAmount = 1;
        smokeFlag = turnCount;
        if (PlayerScript.instance.camera == true)
            Smoke.fillAmount = 0;
    }
    [PunRPC] public void deleteItem(int item)
    {
        if (item == 1)
        {
            if (PlayerScript.instance.userNumber == 1) ENBVar.p1Item1 = 0;
            else ENBVar.p2Item1 = 0;
        }
        else if (item == 2)
        {
            if (PlayerScript.instance.userNumber == 1) ENBVar.p1Item2 = 0;
            else ENBVar.p2Item2 = 0;
        }
    }
    public void invisible()
    {
        PV.RPC("invisibleActive", RpcTarget.All);
        spyChat(PhotonNetwork.NickName + "님이 '중장갑'아이템을 사용하였습니다.");
    }
    [PunRPC]
    void invisibleActive()
    {
        if (PlayerScript.instance.turn == true)
        {
            invisibleFlag = true;
            invisibleCount += 1;
        }
    }
    public void spy()
    {
        PV.RPC("spyActive", RpcTarget.All);
        spyChat(PhotonNetwork.NickName + "님이 '스파이'아이템을 사용하였습니다.");
    }
    [PunRPC]
    void spyActive()
    {
        if (PlayerScript.instance.turn == true)
        {
            spyFlag = true;
            spyCount -= 3;
        }
    }
    public void spyChat(string msg) => PV.RPC("spyChatActive", RpcTarget.All, "<color=yellow>[시스템] " + msg + "</color>");
    [PunRPC]
    void spyChatActive(string msg)
    {
        if (spyFlag == true)
        {
            bool isInput = false;
            chatTime = 5.0f;
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
    }

    void planeInfoDeActive()
    {
        cameraSpyFlag = 0;
        for (int i = 0; i < mountainHell; i++)
        {
            if (planeInfo[i] != null)
                planeInfo[i].SetActive(false);
        }
    }
    // 아이템 끝

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
        chatTime = 5.0f;
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
    public void ChatActive()
    {
        chatTime = 5.0f;

        canvas.transform.Find("ChatScrollView").gameObject.SetActive(true);
        ChatInput.Select();
    }

    string[] tInfo;
    public bool stayEnemyFlag = true;
    void Start()
    {
        //플레이어별 스텟(체, 공, 방, 기) 부여 (SC, MC)
        player1 = new player(100, ENBVar.p1Atk, ENBVar.p1Def, ENBVar.p1Speed, 0, 0f);
        player2 = new player(100, ENBVar.p2Atk, ENBVar.p2Def, ENBVar.p2Speed, 0, 0f);
        p1Atk = player1.atk;
        p1Def = player1.def;
        p1Move = player1.move;
        p2Atk = player2.atk;
        p2Def = player2.def;
        p2Move = player2.move;

        if (ENBVar.light == 1) LightObject.transform.rotation = Quaternion.Euler(30, 0, 0);
        else if(ENBVar.light == 2) LightObject.transform.rotation = Quaternion.Euler(60, 0, 0);
        else if (ENBVar.light == 3) LightObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        else if (ENBVar.light == 4) LightObject.transform.rotation = Quaternion.Euler(120, 0, 0);
        else if (ENBVar.light == 5) LightObject.transform.rotation = Quaternion.Euler(150, 0, 0);

        if (PhotonNetwork.IsMasterClient)
            PlayerTank = PhotonNetwork.Instantiate(ENBVar.tank1, StartPoint1.transform.position, Quaternion.identity);
        else
            PlayerTank = PhotonNetwork.Instantiate(ENBVar.tank2, StartPoint2.transform.position, Quaternion.Euler(0, -90, 0));

        if (ENBVar.turn == 1) PV.RPC("ChangeTurn", RpcTarget.AllBuffered, "Player1 Turn");
        else if(ENBVar.turn == 2) PV.RPC("ChangeTurn", RpcTarget.AllBuffered, "Player2 Turn");

        turnCount--;

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("HellObject", hellPoint.transform.position, hellPoint.transform.rotation);

        hellObject = GameObject.Find("HellObject(Clone)").gameObject;

        if (ENBVar.map == 1) // city
        {
            maxItemCount = 7;
            maxitemPoint = 72;
        }
        else if (ENBVar.map == 2) // mountain
        {
            maxItemCount = 12;
            maxitemPoint = 106;
        }
        else if (ENBVar.map == 3) // plain
        {
            maxItemCount = 4;
            maxitemPoint = 56;
        }

        cameraSpyFlag = 1;

        rcx = SubCamera.transform.position.x;
        rcy = SubCamera.transform.position.y;
        rcz = SubCamera.transform.position.z;

        gamePlay = true;
    }

    float goneTime = 3.0f;
    float chatTime = 5.0f;
    void Update()
    {
        StartInit();

        turnTime -= Time.deltaTime;
        TimeStatus.text = Mathf.Round(turnTime).ToString();
        if (turnTime <= 0.01)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (p1Turn == true) PassTurn(1);
                else if (p2Turn == true) PassTurn(2);
            }
        }

        if (PlayerCount() != 2)
        {
            goneTime -= Time.deltaTime;
            if (goneTime <= 0.01)
            {
                gamePlay = false;
                PhotonNetwork.LeaveRoom();
                canvas.transform.Find("ExitPanel").gameObject.SetActive(true);
            }
        }
        else goneTime = 0.5f;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exit == false)
            {
                exit = true;
                canvas.transform.Find("QuitPanel").gameObject.SetActive(true);
            }
            else
            {
                exit = false;
                canvas.transform.Find("QuitPanel").gameObject.SetActive(false);
            }
        }

        if (turnCount == 1)
        {
            spyFlag = true;
            spyCount = 4;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            AtkInfo.text = player1.atk.ToString();
            MoveInfo.text = player1.move.ToString();

            if (ENBVar.p1Item1 == 0) Item1.sprite = ItemSlot;
            else if (ENBVar.p1Item1 == 1) Item1.sprite = ItemDoubleShot;
            else if (ENBVar.p1Item1 == 2) Item1.sprite = ItemPowerShot;
            else if (ENBVar.p1Item1 == 3) Item1.sprite = ItemHeist;
            else if (ENBVar.p1Item1 == 4) Item1.sprite = ItemRepair;
            else if (ENBVar.p1Item1 == 5) Item1.sprite = ItemSmoke;
            else if (ENBVar.p1Item1 == 6) Item1.sprite = ItemInvisible;
            else if (ENBVar.p1Item1 == 7) Item1.sprite = ItemSpy;

            if (ENBVar.p1Item2 == 0) Item2.sprite = ItemSlot;
            else if (ENBVar.p1Item2 == 1) Item2.sprite = ItemDoubleShot;
            else if (ENBVar.p1Item2 == 2) Item2.sprite = ItemPowerShot;
            else if (ENBVar.p1Item2 == 3) Item2.sprite = ItemHeist;
            else if (ENBVar.p1Item2 == 4) Item2.sprite = ItemRepair;
            else if (ENBVar.p1Item2 == 5) Item2.sprite = ItemSmoke;
            else if (ENBVar.p1Item2 == 6) Item2.sprite = ItemInvisible;
            else if (ENBVar.p1Item2 == 7) Item2.sprite = ItemSpy;
        }
        else
        {
            AtkInfo.text = player2.atk.ToString();
            MoveInfo.text = player2.move.ToString();

            if (ENBVar.p2Item1 == 0) Item1.sprite = ItemSlot;
            else if (ENBVar.p2Item1 == 1) Item1.sprite = ItemDoubleShot;
            else if (ENBVar.p2Item1 == 2) Item1.sprite = ItemPowerShot;
            else if (ENBVar.p2Item1 == 3) Item1.sprite = ItemHeist;
            else if (ENBVar.p2Item1 == 4) Item1.sprite = ItemRepair;
            else if (ENBVar.p2Item1 == 5) Item1.sprite = ItemSmoke;
            else if (ENBVar.p2Item1 == 6) Item1.sprite = ItemInvisible;
            else if (ENBVar.p2Item1 == 7) Item1.sprite = ItemSpy;

            if (ENBVar.p2Item2 == 0) Item2.sprite = ItemSlot;
            else if (ENBVar.p2Item2 == 1) Item2.sprite = ItemDoubleShot;
            else if (ENBVar.p2Item2 == 2) Item2.sprite = ItemPowerShot;
            else if (ENBVar.p2Item2 == 3) Item2.sprite = ItemHeist;
            else if (ENBVar.p2Item2 == 4) Item2.sprite = ItemRepair;
            else if (ENBVar.p2Item2 == 5) Item2.sprite = ItemSmoke;
            else if (ENBVar.p2Item2 == 6) Item2.sprite = ItemInvisible;
            else if (ENBVar.p2Item2 == 7) Item2.sprite = ItemSpy;
        }

        if (smokeFlag != 0)
        {
            smokeTime = turnCount - smokeFlag;
            if(smokeTime >= GameManagerScript.item_smoke_max)
            {
                Smoke.fillAmount = 0;
                smokeTime = 0;
                smokeFlag = 0;
            }
        }

        if (PlayerScript.instance.camera == true)
            if (spyFlag == true)
                if (cameraSpyFlag == 1)
                    planeInfoDeActive();

        if (turnCount % 5 == 4)
            if (hellAlarm == true)
                if (PhotonNetwork.IsMasterClient)
                {
                    hellAlarm = false;
                    spyChat("1턴 후 타일이 없어집니다. 크큭...");
                }

        if (Input.GetKeyDown(KeyCode.Return)) ChatActive();

        CameraMove();

        if (ChatInput.isFocused == true) ChatInfo.text = "주의! 채팅이 활성화된 상태!";
        else ChatInfo.text = "";

        if (chatTime >= 0.01)
        {
            chatTime -= Time.deltaTime;
            canvas.transform.Find("ChatScrollView").gameObject.SetActive(true);
        }
        else if (chatTime < 0.01)
        {
            chatTime = 0;
            canvas.transform.Find("ChatScrollView").gameObject.SetActive(false);
        }

        if (p1Turn == true) TurnStatus.text = PhotonNetwork.PlayerList[0].NickName;
        else if (p2Turn == true) TurnStatus.text = PhotonNetwork.PlayerList[1].NickName;

        if (player1.hp <= 0 || player2.hp <= 0)
        {
            ENBVar.p1EndHP = player1.hp;
            ENBVar.p2EndHP = player2.hp;

            PV.RPC("EndGame", RpcTarget.AllBuffered);
        }
    }

    [PunRPC] public void EndGame()
    {
        gamePlay = false;
        Destroy(PlayerTank);
        SceneManager.LoadScene("ResultScene");
    }

    public void ExitBtn()
    {
        ENBVar.ResetENBVar();

        RoomScript.instance.ready1 = false;
        RoomScript.instance.ready2 = false;
        SelectScript.instance.ready1 = false;
        SelectScript.instance.ready2 = false;

        if (gamePlay == true)
        {
            gamePlay = false;

            int p1Mmr = int.Parse(PhotonNetwork.PlayerList[0].CustomProperties["Mmr"].ToString());
            int p2Mmr = int.Parse(PhotonNetwork.PlayerList[1].CustomProperties["Mmr"].ToString());

            StartCoroutine(exitPlayer(p1Mmr, p2Mmr));
        }

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }
    public void CancleBtn() => canvas.transform.Find("QuitPanel").gameObject.SetActive(false);

    int PlayerCount()
    {
        int count = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            count++;
        }
        return count;
    }

    void resetStat()
    {
        player1.atk = p1Atk;
        player1.def = p1Def;
        player1.move = p1Move;
        player2.atk = p2Atk;
        player2.def = p2Def;
        player2.move = p2Move;
    }

    IEnumerator exitPlayer(int p1Mmr, int p2Mmr)
    {
        int resultMmr = 0, dif = 0;

        if (PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("랭크"))
        {
            if (PhotonNetwork.IsMasterClient) dif = p2Mmr - p1Mmr;
            else dif = p1Mmr - p2Mmr;

            resultMmr = ((10 - dif / 10) <= 0) ? 0 : 10 - dif / 10;
        }

        string id = LoginScript.instance.UserId;
        int mmr = LoginScript.instance.UserMmr - resultMmr;
        if (mmr < 0) mmr = 0;
        int money = LoginScript.instance.UserMoney;
        if (money < 0) money = 0;

        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("mmr", mmr);
        form.AddField("money", money);
        WWW www = new WWW(GameManagerScript.serverURL + "/mmrandmoney.php", form);
        yield return www;


        string myTank, enemy, enemyTank, result;
        if(PhotonNetwork.IsMasterClient)
        {
            enemy = PhotonNetwork.PlayerList[1].NickName;
            myTank = ENBVar.tank1;
            enemyTank = ENBVar.tank2;
        }
        else
        {
            enemy = PhotonNetwork.PlayerList[0].NickName;
            myTank = ENBVar.tank2;
            enemyTank = ENBVar.tank1;
        }
        result = "패";

        WWWForm form2 = new WWWForm();
        form2.AddField("id", id);
        form2.AddField("mytank", myTank);
        form2.AddField("enemy", enemy);
        form2.AddField("enemytank", enemyTank);
        form2.AddField("result", result);
        WWW www2 = new WWW(GameManagerScript.serverURL + "/matchhistory.php", form);
        yield return www2;
    }

    void StartInit()
    {
        if (stayEnemyFlag == true)
        {
            string enemy;
            if (PhotonNetwork.IsMasterClient) enemy = ENBVar.tank2 + "(Clone)";
            else enemy = ENBVar.tank1 + "(Clone)";

            if (GameObject.Find(enemy).name.Equals(enemy)) stayEnemyFlag = false;
        }

        if (rcx == 0 || rcy == 0 || rcz == 0)
        {
            rcx = SubCamera.transform.position.x;
            rcy = SubCamera.transform.position.y;
            rcz = SubCamera.transform.position.z;
        }
    }

    [Header("CameraObject")]
    public Camera MainCamera;
    public Camera SubCamera;
    float rcx, rcy, rcz;
    public void CameraChange(bool camera)
    {
        if (camera == true)
        {
            MainCamera.enabled = true;
            SubCamera.enabled = false;
            Smoke.fillAmount = 0;

            if (spyFlag == true) planeInfoDeActive();
        }
        else if(camera == false)
        {
            MainCamera.enabled = false;
            SubCamera.enabled = true;
            if (smokeFlag != 0)
                Smoke.fillAmount = 1;

            cameraSpyFlag = 1;
            if (spyFlag == true)
                for (int i = 0; i < mountainHell; i++)
                    if (planeInfo[i] != null)
                        planeInfo[i].SetActive(true);
        }
    }
    public void CameraMove()
    {
        if (Input.GetKey(KeyCode.I)) SubCamera.transform.Translate(new Vector3(0, 0.2f, 0));
        if (Input.GetKey(KeyCode.K)) SubCamera.transform.Translate(new Vector3(0, -0.2f, 0));
        if (Input.GetKey(KeyCode.J)) SubCamera.transform.Translate(new Vector3(-0.2f, 0, 0));
        if (Input.GetKey(KeyCode.L)) SubCamera.transform.Translate(new Vector3(0.2f, 0, 0));

        if (Input.GetKey(KeyCode.U)) SubCamera.transform.Translate(new Vector3(0, 0, -0.2f));
        if (Input.GetKey(KeyCode.O)) SubCamera.transform.Translate(new Vector3(0, 0, 0.2f));

        if (Input.GetKey(KeyCode.P)) SubCamera.transform.position = new Vector3(rcx, rcy, rcz);

        if (ChatInput.isFocused == false)
        {
            if (Input.GetKey(KeyCode.Z)) MainCamera.GetComponent<SmoothFollow>().distance += 0.01f;
            if (Input.GetKey(KeyCode.X)) MainCamera.GetComponent<SmoothFollow>().distance -= 0.01f;
            if (Input.GetKey(KeyCode.C)) MainCamera.GetComponent<SmoothFollow>().height += 0.01f;
            if (Input.GetKey(KeyCode.V)) MainCamera.GetComponent<SmoothFollow>().height -= 0.01f;
            if (Input.GetKey(KeyCode.B))
            {
                MainCamera.GetComponent<SmoothFollow>().distance = 4.2f;
                MainCamera.GetComponent<SmoothFollow>().height = 2.0f;
            }
        }
    }
}
