using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class FriendsScript : MonoBehaviourPunCallbacks
{
    public Button BackBtn;
    public void BackLobby()
    {
        for(int i = 0; i < HistoryRowPanel.Length; i++) HistoryRowPanel[i].GetComponent<Image>().color = new Color(255f / 255, 255f / 255, 255f / 255);

        ENBVar.sceneChangeFlag = true;
        FriendStatus.text = "";
        SceneManager.LoadScene("LobbyScene");
    }

    [Header("FriendsPanel")] //친구 신청창
    public InputField FriendId;
    public Button InsertBtn;
    public Button DeleteBtn;
    public Button InfoBtn;
    public Text FriendStatus;

    public void InsertFriend() => StartCoroutine(FriendUpdate("insert"));
    public void DeleteFriend() => StartCoroutine(FriendUpdate("delete"));
    IEnumerator FriendUpdate(string mod)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", LoginScript.instance.UserId);
        form.AddField("friendid", FriendId.text);
        form.AddField("mod", mod);
        WWW www = new WWW(GameManagerScript.serverURL + "/friendsupdate.php", form);
        yield return www;

        string str = www.text;

        if(str.Equals("Not exist ID")) FriendStatus.text = "없는 ID 입니다.";
        else if(str.Equals("Insert Complete")) FriendStatus.text = FriendId.text + "님을 추가하였습니다.";
        else if(str.Equals("Delete Complete")) FriendStatus.text = FriendId.text + "님을 삭제하였습니다.";
    }

    [Header("FriendsListPanel")] // 친구 목록 창
    public Text LobbyInfoText;
    public Button[] UserListBtn;
    public Text[] UserListId;
    public Text[] UserListMmr;
    string[] uselist;

    IEnumerator FriendsListUpdate()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", LoginScript.instance.UserId);
        WWW www = new WWW(GameManagerScript.serverURL + "/friendslist.php", form);
        yield return www;

        string fullstr = www.text;
        uselist = fullstr.Split('|');

        if (fullstr.Equals("No Friend"))
        {
            UserListId[0].text = "친구가 없습니다.";
            UserListMmr[0].text = "ㅠㅠ";
        }
        else
        {
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
                    UserListId[i].text = "ID : " + usr[0];
                    UserListMmr[i].text = "MMR : " + usr[1];
                    UserListBtn[i].interactable = (usr[2].Equals("o")) ? true : false;
                }
            }
        }
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

    // 유저 정보 조회
    public void ListInfomation(int num)
    {
        PlayerInfoPanel.SetActive(true);
        string[] str = uselist[num].Split(' ');
        string name = str[0];
        StartCoroutine(UserInfomationSelect(name));
    }
    public void UserInfomation()
    {
        PlayerInfoPanel.SetActive(true);
        if (!FriendId.Equals(""))
        {
            StartCoroutine(UserInfomationSelect(FriendId.text));
        }
        else FriendStatus.text = "아이디를 입력하세요.";
    }
    IEnumerator UserInfomationSelect(string name)
    {
        string searchId = name;
        string match = "SELECT * FROM matchhistory WHERE id='" + searchId + "' ORDER BY matchDate DESC;";
        WWWForm form = new WWWForm();
        form.AddField("query", match);
        form.AddField("mode", "1");
        WWW www = new WWW(GameManagerScript.serverURL + "/userMHselect.php", form);
        yield return www;

        string err = www.text;
        if (err.Equals("No ID"))
        {
            FriendStatus.text = "유저가 없거나 랭킹전을 하지 않았습니다.";
            yield break;
        }

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
            if (i >= 4) break;
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

    [Header("UserInfoPanel")]
    public Text UserInfo;

    void Update()
    {
        UserInfo.text = "ID : " + LoginScript.instance.UserId + " / MMR : " + LoginScript.instance.UserMmr + " / Money : "
             + LoginScript.instance.UserMoney + " / Cash : " + LoginScript.instance.UserCash;

        if(GameManagerScript.instance.update_time <= 0.05) StartCoroutine(FriendsListUpdate());
    }
}
