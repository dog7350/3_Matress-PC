using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LoginScript : MonoBehaviourPunCallbacks
{
    public static LoginScript instance = null;
    public InputField IPInput;

    public GameObject BtnSound;
    public GameObject BGM;
    public AudioClip music;

    void Awake()
    {
        instance = this;

        BtnSound = GameObject.Find("BtnSound");
        BGM.GetComponent<AudioSource>().clip = music;
        BGM.GetComponent<AudioSource>().Play();
        DontDestroyOnLoad(BGM);
        DontDestroyOnLoad(BtnSound);
    }

    public void OpenHomePage() => Application.OpenURL(GameManagerScript.webURL + "/");

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

    [Header("LoginUserInfo")]
    public string UserId = "";
    public string UserPw = "";
    public int UserMmr = 0;
    public int UserMoney = 0;
    public int UserCash = 0;

    public InputField IdField;
    public InputField PwField;

    public Button Login;

    public Text ErrText;

    public void LoginBtn() => StartCoroutine(LoginCheck());

    IEnumerator LoginCheck()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", IdField.text);
        form.AddField("pw", PwField.text);
        WWW www = new WWW(GameManagerScript.serverURL + "/login.php", form);
        yield return www;

        string str = www.text;

        if (str.Equals("ID does not exist")) ErrText.text = "ID를 확인하세요.";
        else if (str.Equals("PW does not Match")) ErrText.text = "PW를 확인하세요.";
        else if (str.Equals("Already connecting")) ErrText.text = "이미 접속중인 ID 입니다.";
        else
        {
            string[] spstr = str.Split('/');
            UserId = spstr[0];
            UserPw = PwField.text;
            UserMmr = int.Parse(spstr[1]);
            UserMoney = int.Parse(spstr[2]);
            UserCash = int.Parse(spstr[3]);

            ErrText.text = "";

            OptionPanel.SetActive(false);
            GameManagerScript.instance.Connected();
            SceneManager.LoadScene("LobbyScene");
        }
    }

    public void AccountBtn() => Application.OpenURL(GameManagerScript.webURL + "/join");

    public void SearchBtn() => Application.OpenURL(GameManagerScript.webURL + "/find?F_State=idFind");

    public void Logout()
    {
        GameManagerScript.instance.Disconnect();

        UserId = null;
        UserMmr = 0;
        UserMoney = 0;
        UserCash = 0;

        SceneManager.LoadScene("LoginScene");
    }

    void Update()
    {
        GameManagerScript.serverURL = "http://" + IPInput.text + "/maetress";
        GameManagerScript.webURL = "http://" + IPInput.text + ":8080";

        if (IdField.isFocused == false)
            if (Input.GetKeyDown(KeyCode.Tab))
                IdField.Select();

        if (IdField.isFocused == true)
            if (Input.GetKeyDown(KeyCode.Tab))
                PwField.Select();

        if (PwField.isFocused == true)
            if (Input.GetKeyDown(KeyCode.Tab))
                IdField.Select();

        if (Input.GetKeyDown(KeyCode.Return)) LoginBtn();
    }
}
