using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ItemShopScript : MonoBehaviourPunCallbacks
{
    public static ItemShopScript instance = null;

    public GameObject BGM;
    public AudioClip music;

    private void Awake()
    {
        instance = this;

        BGM = GameObject.Find("BGM");
    }

    public void BackLobby()
    {
        PlayerInfoPanel.SetActive(false);
        ResetTankStatus();
        ENBVar.sceneChangeFlag = true;
        SceneManager.LoadScene("LobbyScene");
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

    [Header("TankSelect")]
    public Button[] TankButton;
    string prevTank = "";
    public void itemTankSelect(string name)
    {
        GameObject.Find("TankObject").transform.Find(name).gameObject.SetActive(true);
        GameObject.Find(name + " Btn").GetComponent<Button>().interactable = false;

        if (!prevTank.Equals(""))
        {
            GameObject.Find("TankObject").transform.Find(prevTank).gameObject.SetActive(false);
            GameObject.Find(prevTank + " Btn").GetComponent<Button>().interactable = true;
        }

        prevTank = name;
    }
    public void resetTankSelect(int num)
    {
        GameObject.Find("TankObject").transform.Find(prevTank).gameObject.SetActive(false);
        if (num == 1)
            GameObject.Find(prevTank + " Btn").GetComponent<Button>().interactable = false;
        else
            GameObject.Find(prevTank + " Btn").GetComponent<Button>().interactable = true;
        prevTank = "";
        ResetTankStatus();
    }

    [Header("TankStatus")]
    int tankNumber;
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
    public Text MoneyText;
    public Text CashText;
    public void TankStatus(int num)
    {
        tankNumber = num;
        StartCoroutine(TankStatusSelect(num));
    }
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
        MoneyText.text = tInfo[7];
        CashText.text = tInfo[8];
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
        MoneyText.text = "0";
        CashText.text = "0";
    }
    public void TankSelectBtnActive()
    {
        for (int i = 0; i < LobbyScript.instance.UserTankList.Length; i++)
        {
            switch (LobbyScript.instance.UserTankList[i])
            {
                case "1": TankButton[0].interactable = false; break;
                case "2": TankButton[1].interactable = false; break;
                case "3": TankButton[2].interactable = false; break;
                case "4": TankButton[3].interactable = false; break;
                case "5": TankButton[4].interactable = false; break;
                case "7": TankButton[5].interactable = false; break;
                case "8": TankButton[6].interactable = false; break;
                case "10": TankButton[7].interactable = false; break;
            }
        }
    }

    int price;
    public void BuyCheck()
    {
        if (MoneyText.Equals("0"))
        {
            if (!CashText.Equals("0"))
            {
                if (LoginScript.instance.UserCash >= int.Parse(CashText.text))
                {
                    price = LoginScript.instance.UserCash - int.Parse(CashText.text);
                    BuyItem(LoginScript.instance.UserMoney, price);
                }
                else
                {
                    resetTankSelect(0);
                    AlarmPanel.SetActive(true);
                    AlarmText.text = "캐시가 부족합니다.";
                }
            }
            else Debug.Log("가격 오류");
        }
        else
        {
            if (LoginScript.instance.UserMoney >= int.Parse(MoneyText.text))
            {
                price = LoginScript.instance.UserMoney - int.Parse(MoneyText.text);
                BuyItem(price, LoginScript.instance.UserCash);
            }
            else
            {
                resetTankSelect(0);
                AlarmPanel.SetActive(true);
                AlarmText.text = "게임 머니가 부족합니다.";
            }
        }
    }

    void BuyItem(int money, int cash)
    {
        StartCoroutine(Buy(money, cash));
        GameManagerScript.instance.userinfoupdate();
        TankList();
        resetTankSelect(1);
        AlarmPanel.SetActive(true);
        AlarmText.text = "구매가 완료되었습니다.";
        GameManagerScript.instance.userinfoupdate();
    }
    IEnumerator Buy(int money, int cash)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", LoginScript.instance.UserId);
        form.AddField("money", money);
        form.AddField("cash", cash);
        WWW www = new WWW(GameManagerScript.serverURL + "/sellandbuyitem.php", form);
        yield return www;

        form = new WWWForm();
        form.AddField("id", LoginScript.instance.UserId);
        form.AddField("tno", tankNumber);
        www = new WWW(GameManagerScript.serverURL + "/usertankupdate.php", form);
        yield return www;

        GameManagerScript.instance.userinfoupdate();
    }
    public void TankList() => StartCoroutine(UserTankListSelect());
    IEnumerator UserTankListSelect()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", LoginScript.instance.UserId);
        WWW www = new WWW(GameManagerScript.serverURL + "/usertank.php", form);
        yield return www;

        LobbyScript.instance.UserTankList = www.text.Split('/');
    }

    [Header("AlarmPanel")]
    public GameObject AlarmPanel;
    public Text AlarmText;
    public Button AlarmButton;
    public void AlarmClose()
    {
        AlarmText.text = "";
        AlarmPanel.SetActive(false);
    }

    void Start()
    {
        if (!BGM.name.Equals(music.name))
        {
            BGM.GetComponent<AudioSource>().clip = music;
            BGM.GetComponent<AudioSource>().Play();
        }
        TankSelectBtnActive();
    }

    public Text UserInfo;
    void Update()
    {
        UserInfo.text = "ID : " + LoginScript.instance.UserId + " / MMR : " + LoginScript.instance.UserMmr + " / Money : "
                + LoginScript.instance.UserMoney + " / Cash : " + LoginScript.instance.UserCash;

        if (GameManagerScript.instance.update_time <= 0.05) StartCoroutine(UserListUpdate());
    }
}
