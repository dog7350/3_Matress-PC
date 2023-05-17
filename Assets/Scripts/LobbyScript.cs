using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyScript : MonoBehaviourPunCallbacks
{
    public static LobbyScript instance = null;

    public GameObject BtnSound;
    public GameObject BGM;
    public AudioClip music;

    private void Awake()
    {
        instance = this;
        RoomPanel.SetActive(false);

        BtnSound = GameObject.Find("BtnSound");
        BGM = GameObject.Find("BGM");
        if (!BGM.GetComponent<AudioSource>().clip.name.Equals(music.name))
        {
            BGM.GetComponent<AudioSource>().clip = music;
            BGM.GetComponent<AudioSource>().Play();
        }
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

    public GameObject LoddingPanel;
    public Sprite Lodding1;
    public Sprite Lodding2;
    public Sprite Lodding3;
    public Sprite Lodding4;
    int lodingImage = 1;

    public bool normal = false;
    public bool rank = false;

    [Header("LobbyMenuButton")]
    public Text UserInfo;
    public Button RankingBtn;
    public Button NormalBtn;
    public Button ShopBtn;
    public Button FriendsBtn;
    public Button PayBtn;
    public Button LogoutBtn;

    public void Ranking()
    {
        rank = true;
        Hashtable ropt = new Hashtable() { { "Mode", "랭크" } };
        PhotonNetwork.JoinRandomRoom(ropt, 2);
        SceneManager.LoadScene("RoomScene");
    }
    public void Normal()
    {
        normal = true;
        Hashtable ropt = new Hashtable() { { "Mode", "일반" } };
        PhotonNetwork.JoinRandomRoom(ropt, 2);
        SceneManager.LoadScene("RoomScene");
    }
    public void Shop() => SceneManager.LoadScene("ItemShopScene");
    public void Friends()
    {
        PlayerInfoPanel.SetActive(false);
        SceneManager.LoadScene("FriendsScene");
    }
    public void Pay()
    {
        Application.OpenURL(GameManagerScript.webURL + "/userpay?id=" + LoginScript.instance.UserId + "&pw=" + LoginScript.instance.UserPw);
    }
    public void Logout()
    {
        PlayerInfoPanel.SetActive(false);
        Destroy(BtnSound);
        Destroy(BGM);
        LoginScript.instance.Logout();
    }

    /// <summary>
    /// 버튼 기능 관련
    /// </summary>
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

    [Header("UserListPanel")] // 유저 리스트 관련
    public GameObject ListPanel;
    public Text LobbyInfoText;
    public Text[] UserListId;
    public Text[] UserListMmr;
    public Button[] UserListBtn;
    public string[] UserTankList;

    public void TankList() => StartCoroutine(UserTankListSelect());
    IEnumerator UserTankListSelect()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", LoginScript.instance.UserId);
        WWW www = new WWW(GameManagerScript.serverURL + "/usertank.php", form);
        yield return www;

        UserTankList = www.text.Split('/');
    }

    IEnumerator UserListUpdate()
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
    
    /// <summary>
    /// 유저 목록 관련
    /// </summary>

    [Header("LobbyRoomPanel")]
    public GameObject RoomPanel;
    public GameObject MaxRoomPanel;
    public InputField RoomName;
    public Button RoomInputCancel;
    public Button RoomInputEnter;
    public Button CreateRoomBtn;
    public Button JoinRoomBtn;

    public void MakeRoom() => RoomPanel.SetActive(true);
    public void RoomCancel()
    {
        RoomName.text = "";
        RoomPanel.SetActive(false);
    }
    public void CreateRoom(int mode)
    {
        RoomOptions ropt = new RoomOptions();
        ropt.MaxPlayers = 2;
        if (mode == 1) // 사설
        {
            ropt.CustomRoomProperties = new Hashtable();
            ropt.CustomRoomPropertiesForLobby = new string[1];
            ropt.CustomRoomPropertiesForLobby[0] = "Mode";
            ropt.CustomRoomProperties.Add("Mode", "개인");
            PhotonNetwork.CreateRoom(RoomName.text == "" ? "Room" + Random.Range(0, 100) : RoomName.text, ropt);
        }
        else if (mode == 2) // 일반
        {
            RoomName.text = "Nor";
            ropt.CustomRoomProperties = new Hashtable();
            ropt.CustomRoomPropertiesForLobby = new string[1];
            ropt.CustomRoomPropertiesForLobby[0] = "Mode";
            ropt.CustomRoomProperties.Add("Mode", "일반");
            PhotonNetwork.CreateRoom(RoomName.text + Random.Range(0, 100), ropt);
        }
        else if (mode == 3) // 랭크
        {
            RoomName.text = "Ran";
            ropt.CustomRoomProperties = new Hashtable();
            ropt.CustomRoomPropertiesForLobby = new string[1];
            ropt.CustomRoomPropertiesForLobby[0] = "Mode";
            ropt.CustomRoomProperties.Add("Mode", "랭크");
            PhotonNetwork.CreateRoom(RoomName.text + Random.Range(0, 100), ropt);
        }
        SceneManager.LoadScene("RoomScene");
    }
    public void JRoom() => RoomPanel.SetActive(true);
    public void JoinRoom()
    {
        for (int i = 0; i < myList.Count; i++)
        {
            if(myList[i].Name.Equals(RoomName.text))
            {
                if (myList[i].PlayerCount >= 2)
                {
                    maxRoomflag = true;
                    MaxRoomPanel.SetActive(true);
                    break;
                }
                else
                {
                    PhotonNetwork.JoinRoom(RoomName.text);
                    MyListRenewal();
                    SceneManager.LoadScene("RoomScene");
                    break;
                }
            }
        }
    }
    /// <summary>
    /// 방 관련
    /// </summary>

    [Header("LobbyPanel")] // 방 목록 관련
    public GameObject LobbyPanel;
    public Button[] CellBtn;
    public Button PrevBtn;
    public Button NextBtn;
    public List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;
    bool maxRoomflag = false;

    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else
        {
            if (myList[multiple + num].PlayerCount >= 2)
            {
                maxRoomflag = true;
                MaxRoomPanel.SetActive(true);
            }
            else
            {
                PhotonNetwork.JoinRoom(myList[multiple + num].Name);
                MyListRenewal();
                SceneManager.LoadScene("RoomScene");
            }
        }
        MyListRenewal();
    }
    public void MyListRenewal()
    {
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        PrevBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * CellBtn.Length;
        for(int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;

        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i]))
                    if (roomList[i].CustomProperties["Mode"].Equals("개인"))
                        myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();

        ENBVar.roomList = myList;
    }

    public void MaxRoomPanelClear() => MaxRoomPanel.SetActive(false);

    public override void OnJoinedLobby()
    {
        RoomPanel.SetActive(false);
        ENBVar.roomList = null;
        myList.Clear();
        GameManagerScript.instance.userinfoupdate();
    }

    private void Start()
    {
        GameManagerScript.instance.enbInit();

        TankList();

        lodingImage = Random.Range(1, 5);
        if (lodingImage == 1) LoddingPanel.GetComponent<Image>().sprite = Lodding1;
        else if (lodingImage == 2) LoddingPanel.GetComponent<Image>().sprite = Lodding2;
        else if (lodingImage == 3) LoddingPanel.GetComponent<Image>().sprite = Lodding3;
        else if (lodingImage == 4) LoddingPanel.GetComponent<Image>().sprite = Lodding4;
    }

    void Update()
    {
        if (ENBVar.sceneChangeFlag == true)
        {
            myList = ENBVar.roomList;
            MyListRenewal();
            ENBVar.sceneChangeFlag = false;
        }

        if (PhotonNetwork.NetworkClientState.ToString().Equals("JoinedLobby")) LoddingPanel.SetActive(false);

        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";

        UserInfo.text = "ID : " + LoginScript.instance.UserId + " / MMR : " + LoginScript.instance.UserMmr + " / Money : "
             + LoginScript.instance.UserMoney + " / Cash : " + LoginScript.instance.UserCash;

        if (GameManagerScript.instance.update_time <= 0.05) StartCoroutine(UserListUpdate());
    }
}