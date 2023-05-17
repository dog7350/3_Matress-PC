using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomScript : MonoBehaviourPunCallbacks
{
    public static RoomScript instance = null;

    public GameObject BGM;
    public AudioClip roomMusic;
    public AudioClip shopMusic;

    private void Awake()
    {
        instance = this;

        BGM = GameObject.Find("BGM");
        BGM.GetComponent<AudioSource>().clip = roomMusic;
        BGM.GetComponent<AudioSource>().Play();
    }

    [Header("Option")]
    public Button OptionBtn;
    public GameObject OptionPanel;
    public Button CloseBtn;
    public Slider AudioSoundSlider;
    public Button AudioMuteBtn;

    public void OptionOpen() => OptionPanel.SetActive(true);
    public void OptionClose() => OptionPanel.SetActive(false);
    public void AudioMute()
    {
        GameManagerScript.instance.muteStatus = !GameManagerScript.instance.muteStatus;
        if (GameManagerScript.instance.muteStatus == true) BGM.GetComponent<AudioSource>().volume = 0f;
        else
        {
            AudioSoundSlider.value = GameManagerScript.instance.sound;
            BGM.GetComponent<AudioSource>().volume = AudioSoundSlider.value;
        }
    }
    public void AudioSound()
    {
        GameManagerScript.instance.sound = AudioSoundSlider.value;
        BGM.GetComponent<AudioSource>().volume = AudioSoundSlider.value;
    }

    [Header("RoomUserInfo")]
    public Text PlayerName1;
    public Text PlayerName2;
    public Text PlayerMmr1;
    public Text PlayerMmr2;
    public Text RoomInfoText;
    string roomStr;
    public Text UserInfo;

    public void LeaveRoom()
    {
        ready1 = false;
        ready2 = false;

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }
    public override void OnJoinedRoom()
    {
        LobbyScript.instance.normal = false;
        LobbyScript.instance.rank = false;
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    [Header("MatchHistory")]
    public GameObject PlayerInfoPanel;
    public Text PlayerIdText;
    public Text AllMHText;
    public Text PlayerTankText;
    public GameObject[] HistoryRowPanel;
    public Text[] HistoryDate;
    public Text[] HistoryMyId;
    public Text[] HistoryMyTank;
    public Text[] HistoryEnemyId;
    public Text[] HistoryEnemyTank;
    public Text[] HistoryResult;
    float panelColor = 190f;
    string[] uselist;

    // 유저 정보 조회
    public void ListInfomation(int num)
    {
        if (PhotonNetwork.IsMasterClient) { if (ready1 == true) return; }
        else { if (ready2 == true) return; }
        PlayerInfoPanel.SetActive(true);
        string[] str = uselist[num].Split(' ');
        string name = str[0];
        StartCoroutine(UserInfomationSelect(name));
    }
    public void CloseInfomation() => PlayerInfoPanel.SetActive(false);
    IEnumerator UserInfomationSelect(string name)
    {
        string searchId = name;
        string match = "SELECT * FROM matchhistory WHERE id='" + searchId + "' ORDER BY matchDate DESC;";
        WWWForm form = new WWWForm();
        form.AddField("query", match);
        form.AddField("mode", "1");
        WWW www = new WWW(GameManagerScript.serverURL + "/userMHselect.php", form);
        yield return www;

        string[] tempList;
        string temp = "";

        string[] matchList = www.text.Split('|'); //각 라인, 반복문 필요

        string cntTank = "SELECT mytank, count(*) cnt FROM matchhistory WHERE id = '" + searchId + "' GROUP by mytank ORDER BY cnt DESC;";
        form = new WWWForm();
        form.AddField("query", cntTank);
        form.AddField("mode", "2");
        www = new WWW(GameManagerScript.serverURL + "/userMHselect.php", form);
        yield return www;
        string[] tankList = www.text.Split('|');
        tankList = tankList[0].Split('/');

        string mchCnt = "SELECT result, count(*) cnt FROM matchhistory WHERE id='" + searchId + "' GROUP by result ORDER BY result ASC;";
        form = new WWWForm();
        form.AddField("query", mchCnt);
        form.AddField("mode", "3");
        www = new WWW(GameManagerScript.serverURL + "/userMHselect.php", form);
        yield return www;
        string[] mchList = www.text.Split('|');

        PlayerIdText.text = searchId;
        PlayerTankText.text = tankList[0];
        for (int i = 0; i < mchList.Length; i++)
        {
            tempList = mchList[i].Split('/');

            if (tempList[0].Equals("무")) temp = temp + tempList[1] + "무 ";
            else if (tempList[0].Equals("승")) temp = temp + tempList[1] + "승 ";
            else if (tempList[0].Equals("패")) temp = temp + tempList[1] + "패 ";
        }
        AllMHText.text = temp;

        for (int i = 0; i < matchList.Length; i++)
        {
            if (i >= 9) break;
            tempList = matchList[i].Split('/');
            HistoryDate[i].text = tempList[0];
            HistoryMyId[i].text = tempList[1];
            HistoryMyTank[i].text = tempList[2];
            HistoryEnemyId[i].text = tempList[3];
            HistoryEnemyTank[i].text = tempList[4];
            HistoryResult[i].text = tempList[5];
            if (tempList[5].Equals("승")) HistoryRowPanel[i].GetComponent<Image>().color = new Color(panelColor / 255, 255f / 255, panelColor / 255);
            if (tempList[5].Equals("무")) HistoryRowPanel[i].GetComponent<Image>().color = new Color(255f / 255, 255f / 255, panelColor / 255);
            if (tempList[5].Equals("패")) HistoryRowPanel[i].GetComponent<Image>().color = new Color(255f / 255, panelColor / 255, panelColor / 255);
        }
    }

    [Header("UserListPanel")]
    public Text LobbyInfoText;
    public Text[] UserListId;
    public Text[] UserListMmr;
    public Button[] UserListBtn;

    IEnumerator RoomUserListUpdate()
    {
        WWW www = new WWW(GameManagerScript.serverURL + "/userlist.php");
        yield return www;

        string fullstr = www.text;
        uselist = fullstr.Split('|');

        for (int i = 0; i < uselist.Length; i++)
        {
            string[] usr = uselist[i].Split(' ');
            if (uselist[i].Equals(""))
            {
                UserListBtn[i].interactable = false;
                UserListId[i].text = "";
                UserListMmr[i].text = "";
            }
            else
            {
                UserListBtn[i].interactable = true;
                UserListId[i].text = "ID : " + usr[0];
                UserListMmr[i].text = "MMR : " + usr[1];
            }
        }
    }

    [Header("Chatting")] // 채팅관련
    [Header("ETC")]
    public PhotonView PV;

    [Header("ChatScrollView")]
    public Text[] ChatText;
    public InputField ChatInput;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }

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

    Hashtable p1;
    Hashtable p2;

    public void RoomRenewal()
    {
        PlayerName1.text = "";
        PlayerName2.text = "";

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList.Length == 1)
            {
                p1 = PhotonNetwork.PlayerList[i].CustomProperties;

                PlayerName1.text = PhotonNetwork.PlayerList[i].NickName;
                PlayerMmr1.text = p1["Mmr"].ToString();
                PlayerMmr2.text = "";

                PV.RPC("rd2", RpcTarget.All, false);
            }
            else
            {
                p1 = PhotonNetwork.PlayerList[0].CustomProperties;
                p2 = PhotonNetwork.PlayerList[1].CustomProperties;

                PlayerName1.text = PhotonNetwork.PlayerList[0].NickName;
                PlayerName2.text = PhotonNetwork.PlayerList[1].NickName;
                PlayerMmr1.text = p1["Mmr"].ToString();
                PlayerMmr2.text = p2["Mmr"].ToString();

                PV.RPC("rd1", RpcTarget.All, false);
                PV.RPC("rd2", RpcTarget.All, false);
            }
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("일반") || PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("랭크"))
            roomStr = "현재 : " + PhotonNetwork.CurrentRoom.PlayerCount + " 명 / 최대 : " + PhotonNetwork.CurrentRoom.MaxPlayers + " 명 / 모드 : " + PhotonNetwork.CurrentRoom.CustomProperties["Mode"];
        else
            roomStr = "방 제목 : " + PhotonNetwork.CurrentRoom.Name + " / 현재 : " + PhotonNetwork.CurrentRoom.PlayerCount + " 명 / 최대 : " + PhotonNetwork.CurrentRoom.MaxPlayers + " 명 / 모드 : " + PhotonNetwork.CurrentRoom.CustomProperties["Mode"];

        RoomInfoText.text = roomStr;
    }

    [Header("StartGame")]
    public Button ReadyGame;
    public GameObject Ready1;
    public GameObject Ready2;
    public Button ExitRoom;

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

        if(i == 0)
        {
            if (ready1 == false) sw = true;
            else sw = false;
            PV.RPC("rd1", RpcTarget.All, sw);
        }
        else if(i == 1)
        {
            if (ready2 == false) sw = true;
            else sw = false;
            PV.RPC("rd2", RpcTarget.All, sw);
        }
    }

    [Header("ItemShop")]
    public GameObject ItemShopPanel;
    public Button ItemShop;
    public Button Slot1;
    public Button Slot2;
    public Button Item_Double;
    public Button Item_Power;
    public Button Item_Heist;
    public Button Item_Repair;
    public Button Item_Smoke;
    public Button Item_Invisible;
    public Button Item_Spy;
    public Image SelectItemImage;
    public Sprite ItemSlot;
    public Sprite ItemDoubleShot;
    public Sprite ItemPowerShot;
    public Sprite ItemHeist;
    public Sprite ItemRepair;
    public Sprite ItemSmoke;
    public Sprite ItemInvisible;
    public Sprite ItemSpy;
    Button ItemSelectTemp;
    public Text ItemTitle;
    public Text ItemContent;
    public Text ItemPriceInfo;
    public Text ItemPriceText;
    public GameObject AlarmPanel;
    public Text UserMoney;
    float selectColor = 125f;
    int slot1Price = 0;
    int slot2Price = 0;
    int selectItem = 0;
    int selectSlot = 0;
    string[] ItemInfo;

    public void closeAlarm() => AlarmPanel.gameObject.SetActive(false);
    public void openItemShop()
    {
        BGM.GetComponent<AudioSource>().clip = shopMusic;
        BGM.GetComponent<AudioSource>().Play();
        ItemShopPanel.gameObject.SetActive(true);
    }
    public void closeItemShop()
    {
        selectItem = 0;
        selectSlot = 0;
        SelectItemImage.sprite = ItemSlot;
        Slot1.GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255);
        Slot2.GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255);
        ItemTitle.text = "아이템 설명";
        ItemContent.text = "";
        ItemPriceText.text = "0";
        BGM.GetComponent<AudioSource>().clip = roomMusic;
        BGM.GetComponent<AudioSource>().Play();
        ItemShopPanel.gameObject.SetActive(false);

        if (ItemSelectTemp != null)
        {
            ItemSelectTemp.GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255);
            ItemSelectTemp = null;
        }
    }
    public void SelectSlot(int num)
    {
        if (num == 1)
        {
            Slot1.GetComponent<Image>().color = new Color(selectColor / 255, selectColor / 255, selectColor / 255);
            Slot2.GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255);
        }
        else if (num == 2)
        {
            Slot1.GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255);
            Slot2.GetComponent<Image>().color = new Color(selectColor / 255, selectColor / 255, selectColor / 255);
        }
        selectSlot = num;
    }
    void SelectItem(int num)
    {
        selectItem = num;
        switch (num)
        {
            case 1: ItemSelectTemp = Item_Double; Item_Double.GetComponent<Image>().color = new Color(selectColor / 255, selectColor / 255, selectColor / 255); break;
            case 2: ItemSelectTemp = Item_Power; Item_Power.GetComponent<Image>().color = new Color(selectColor / 255, selectColor / 255, selectColor / 255); break;
            case 3: ItemSelectTemp = Item_Heist; Item_Heist.GetComponent<Image>().color = new Color(selectColor / 255, selectColor / 255, selectColor / 255); break;
            case 4: ItemSelectTemp = Item_Repair; Item_Repair.GetComponent<Image>().color = new Color(selectColor / 255, selectColor / 255, selectColor / 255); break;
            case 5: ItemSelectTemp = Item_Smoke; Item_Smoke.GetComponent<Image>().color = new Color(selectColor / 255, selectColor / 255, selectColor / 255); break;
            case 6: ItemSelectTemp = Item_Invisible; Item_Invisible.GetComponent<Image>().color = new Color(selectColor / 255, selectColor / 255, selectColor / 255); break;
            case 7: ItemSelectTemp = Item_Spy; Item_Spy.GetComponent<Image>().color = new Color(selectColor / 255, selectColor / 255, selectColor / 255); break;
        }
        SelectItemImage.sprite = ItemSelectTemp.GetComponent<Image>().sprite;
    }
    public void SellItem()
    {
        int money = 0;
        if (selectSlot != 0)
        {
            if (selectSlot == 1)
            {
                if (slot1Price != 0)
                {
                    money = LoginScript.instance.UserMoney + (slot1Price / 2);
                    StartCoroutine(ItemDBUpdate(money));

                    if (PhotonNetwork.IsMasterClient) ENBVar.p1Item1 = 0;
                    else ENBVar.p2Item1 = 0;
                    slot1Price = 0;
                }
            }
            else if (selectSlot == 2)
            {
                if (slot2Price != 0)
                {
                    money = LoginScript.instance.UserMoney + (slot2Price / 2);
                    StartCoroutine(ItemDBUpdate(money));

                    if (PhotonNetwork.IsMasterClient) ENBVar.p1Item2 = 0;
                    else ENBVar.p2Item2 = 0;
                    slot2Price = 0;
                }
            }
        }
    }
    public void BuyItem(int num)
    {
        if(ItemSelectTemp != null) ItemSelectTemp.GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255);
        SelectItem(num);
        StartCoroutine(ItemDBSelect(num));
    }
    IEnumerator ItemDBSelect(int num)
    {
        WWWForm form = new WWWForm();
        form.AddField("ino", num);
        WWW www = new WWW(GameManagerScript.serverURL + "/selectitem.php", form);
        yield return www;

        string str = www.text;
        ItemInfo = str.Split('/');
        ItemTitle.text = ItemInfo[0];
        ItemContent.text = ItemInfo[1];
        ItemPriceText.text = ItemInfo[2];
    }
    public void BuyDBUpdate()
    {
        if (selectSlot != 0)
        {
            int money = LoginScript.instance.UserMoney;
            int price = int.Parse(ItemInfo[2]);
            if (money >= price)
            {
                money -= price;
                StartCoroutine(ItemDBUpdate(money));

                if (PhotonNetwork.IsMasterClient)
                {
                    if (selectSlot == 1) ENBVar.p1Item1 = selectItem;
                    else if (selectSlot == 2) ENBVar.p1Item2 = selectItem;
                }
                else
                {
                    if (selectSlot == 1) ENBVar.p2Item1 = selectItem;
                    else if (selectSlot == 2) ENBVar.p2Item2 = selectItem;
                }

                if (selectSlot == 1) slot1Price = price;
                else if (selectSlot == 2) slot2Price = price;
            }
            else AlarmPanel.gameObject.SetActive(true);
        }
    }
    IEnumerator ItemDBUpdate(int money)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", LoginScript.instance.UserId);
        form.AddField("money", money);
        form.AddField("cash", LoginScript.instance.UserCash);
        WWW www = new WWW(GameManagerScript.serverURL + "/sellandbuyitem.php", form);
        yield return www;

        GameManagerScript.instance.userinfoupdate();
    }

    public void StartGame()
    {
        if (ready1 == true) Ready1.SetActive(true);
        else Ready1.SetActive(false);

        if(ready2 == true) Ready2.SetActive(true);
        else Ready2.SetActive(false);

        if (ready1 == true && ready2 == true)
        {
            slot1Price = 0;
            slot2Price = 0;
            PhotonNetwork.AutomaticallySyncScene = true;
            SceneManager.LoadScene("SelectScene");
        }
    }

    void Start() => RoomRenewal();

    void Update()
    {
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";

        UserInfo.text = "ID : " + LoginScript.instance.UserId + " / MMR : " + LoginScript.instance.UserMmr + " / Money : "
             + LoginScript.instance.UserMoney + " / Cash : " + LoginScript.instance.UserCash;
        UserMoney.text = LoginScript.instance.UserMoney.ToString();

        if (PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("일반") || PhotonNetwork.CurrentRoom.CustomProperties["Mode"].Equals("랭크"))
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                ExitRoom.gameObject.SetActive(false);
            else
                ExitRoom.gameObject.SetActive(true);
        }
        else ExitRoom.gameObject.SetActive(true);

        if (ChatInput.isFocused == false)
            if (Input.GetKeyDown(KeyCode.Return))
                ChatInput.Select();

        StartGame();

        if (GameManagerScript.instance.update_time <= 0.05) StartCoroutine(RoomUserListUpdate());

        if (PhotonNetwork.IsMasterClient)
        {
            if (ready1 == true) ItemShop.gameObject.SetActive(false);
            else ItemShop.gameObject.SetActive(true);
        }
        else
        {
            if (ready2 == true) ItemShop.gameObject.SetActive(false);
            else ItemShop.gameObject.SetActive(true);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (ENBVar.p1Item1 == 0) Slot1.GetComponent<Image>().sprite = ItemSlot;
            else if (ENBVar.p1Item1 == 1) Slot1.GetComponent<Image>().sprite = ItemDoubleShot;
            else if (ENBVar.p1Item1 == 2) Slot1.GetComponent<Image>().sprite = ItemPowerShot;
            else if (ENBVar.p1Item1 == 3) Slot1.GetComponent<Image>().sprite = ItemHeist;
            else if (ENBVar.p1Item1 == 4) Slot1.GetComponent<Image>().sprite = ItemRepair;
            else if (ENBVar.p1Item1 == 5) Slot1.GetComponent<Image>().sprite = ItemSmoke;
            else if (ENBVar.p1Item1 == 6) Slot1.GetComponent<Image>().sprite = ItemInvisible;
            else if (ENBVar.p1Item1 == 7) Slot1.GetComponent<Image>().sprite = ItemSpy;

            if (ENBVar.p1Item2 == 0) Slot2.GetComponent<Image>().sprite = ItemSlot;
            else if (ENBVar.p1Item2 == 1) Slot2.GetComponent<Image>().sprite = ItemDoubleShot;
            else if (ENBVar.p1Item2 == 2) Slot2.GetComponent<Image>().sprite = ItemPowerShot;
            else if (ENBVar.p1Item2 == 3) Slot2.GetComponent<Image>().sprite = ItemHeist;
            else if (ENBVar.p1Item2 == 4) Slot2.GetComponent<Image>().sprite = ItemRepair;
            else if (ENBVar.p1Item2 == 5) Slot2.GetComponent<Image>().sprite = ItemSmoke;
            else if (ENBVar.p1Item2 == 6) Slot2.GetComponent<Image>().sprite = ItemInvisible;
            else if (ENBVar.p1Item2 == 7) Slot2.GetComponent<Image>().sprite = ItemSpy;
        }
        else
        {
            if (ENBVar.p2Item1 == 0) Slot1.GetComponent<Image>().sprite = ItemSlot;
            else if (ENBVar.p2Item1 == 1) Slot1.GetComponent<Image>().sprite = ItemDoubleShot;
            else if (ENBVar.p2Item1 == 2) Slot1.GetComponent<Image>().sprite = ItemPowerShot;
            else if (ENBVar.p2Item1 == 3) Slot1.GetComponent<Image>().sprite = ItemHeist;
            else if (ENBVar.p2Item1 == 4) Slot1.GetComponent<Image>().sprite = ItemRepair;
            else if (ENBVar.p2Item1 == 5) Slot1.GetComponent<Image>().sprite = ItemSmoke;
            else if (ENBVar.p2Item1 == 6) Slot1.GetComponent<Image>().sprite = ItemInvisible;
            else if (ENBVar.p2Item1 == 7) Slot1.GetComponent<Image>().sprite = ItemSpy;

            if (ENBVar.p2Item2 == 0) Slot2.GetComponent<Image>().sprite = ItemSlot;
            else if (ENBVar.p2Item2 == 1) Slot2.GetComponent<Image>().sprite = ItemDoubleShot;
            else if (ENBVar.p2Item2 == 2) Slot2.GetComponent<Image>().sprite = ItemPowerShot;
            else if (ENBVar.p2Item2 == 3) Slot2.GetComponent<Image>().sprite = ItemHeist;
            else if (ENBVar.p2Item2 == 4) Slot2.GetComponent<Image>().sprite = ItemRepair;
            else if (ENBVar.p2Item2 == 5) Slot2.GetComponent<Image>().sprite = ItemSmoke;
            else if (ENBVar.p2Item2 == 6) Slot2.GetComponent<Image>().sprite = ItemInvisible;
            else if (ENBVar.p2Item2 == 7) Slot2.GetComponent<Image>().sprite = ItemSpy;
        }
    }
}
