using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerScript : MonoBehaviourPunCallbacks
{
    public static PlayerScript instance = null;

    private void Awake()
    {
        instance = this;
    }

    public PhotonView PV;
    int initCount = 1;
    public bool camera = true;
    bool dest = true;
    int destCnt = 0;
    int destFlag = -10;
    int destPlayer = -30;

    public AudioSource BGM;
    public AudioClip ShotBGM;
    public AudioClip MoveBGM;

    [Header("PlayerStat")]
    float maxHp, maxAtk, maxDef, moveSpeed, moveCount;
    int shotCount;

    public void Hit()
    {
        float damage = 0;
        DirectorScript.player enemy;
        if (PhotonNetwork.IsMasterClient)
        {
            enemy = DirectorScript.instance.player2;

            if (DirectorScript.instance.player1.def < enemy.atk)
                damage = Mathf.Abs(DirectorScript.instance.player1.def - enemy.atk);
            else damage = 0;
            PV.RPC("HP1sync", RpcTarget.All, damage);
        }
        else
        {
            enemy = DirectorScript.instance.player1;

            if (DirectorScript.instance.player2.def < enemy.atk)
                damage = Mathf.Abs(DirectorScript.instance.player2.def - enemy.atk);
            else damage = 0;
            PV.RPC("HP2sync", RpcTarget.All, damage);
        }

        damage = damage * 0.01f;
        DirectorScript.instance.HP.fillAmount -= damage;
    }
    [PunRPC] void HP1sync(float damage) => DirectorScript.instance.player1.hp -= damage;
    [PunRPC] void HP2sync(float damage) => DirectorScript.instance.player2.hp -= damage;

    [Header("PlayerInfo")]
    string[] tNameTemp;
    string[] eTNameTemp;
    public string tankName;
    public string enemyTank;
    public bool turn;
    public bool turnInit = true;
    GameObject UserTank;
    public int userNumber;
    bool itemFlag;

    public GameObject turret;
    float turretSpeed = 0.2f;
    public GameObject barrel;
    float barrelSpeed = 0.03f;
    public float degree = 0;
    public Transform firePos;
    public string cannon;

    float h, v;
    public Rigidbody rb;
    public Transform tr;
    [PunRPC] void MovePlayer()
    {
        DirectorScript.instance.MOVE.fillAmount -= (0.1f * Time.deltaTime);
        if (PhotonNetwork.IsMasterClient) DirectorScript.instance.player1.moveCount -= (0.1f * Time.deltaTime);
        else DirectorScript.instance.player2.moveCount -= (0.1f * Time.deltaTime);

        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        if (userNumber == 1) moveSpeed = DirectorScript.instance.player1.move;
        else moveSpeed = DirectorScript.instance.player2.move;

        if (turn == true)
        {
            tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
            tr.Rotate(Vector3.up * h * 60 * Time.deltaTime);
        }

        if (BGM.isPlaying) return;
        else BGM.PlayOneShot(MoveBGM);
    }
    [PunRPC] void ShotBoom()
    {
        BGM.PlayOneShot(ShotBGM);

        shotCount = 0;
        if (PhotonNetwork.IsMasterClient) DirectorScript.instance.player1.shotCount = 0;
        else DirectorScript.instance.player2.shotCount = 0;
    }
    [PunRPC] void ShotPower()
    {
        ENBVar.shotPower += (0.3f * Time.deltaTime);

        if (ENBVar.shotPower >= 1) ENBVar.shotPower = 1;
    }
    [PunRPC] void BarrelMove(string input)
    {
        if (input.Equals("W"))
            if (degree >= -85f)
            {
                degree -= barrelSpeed;

                if (PhotonNetwork.IsMasterClient && turn == true) ENBVar.p1Degree -= barrelSpeed;
                else if (PhotonNetwork.IsMasterClient && turn == false) ENBVar.p2Degree -= barrelSpeed;
                else if (!PhotonNetwork.IsMasterClient && turn == true) ENBVar.p2Degree -= barrelSpeed;
                else if (!PhotonNetwork.IsMasterClient && turn == false) ENBVar.p1Degree -= barrelSpeed;

                barrel.transform.Rotate(-barrelSpeed, 0, 0);
            }

        if (input.Equals("S"))
            if (degree <= 10f)
            {
                degree += barrelSpeed;

                if (PhotonNetwork.IsMasterClient && turn == true) ENBVar.p1Degree += barrelSpeed;
                else if (PhotonNetwork.IsMasterClient && turn == false) ENBVar.p2Degree += barrelSpeed;
                else if (!PhotonNetwork.IsMasterClient && turn == true) ENBVar.p2Degree += barrelSpeed;
                else if (!PhotonNetwork.IsMasterClient && turn == false) ENBVar.p1Degree += barrelSpeed;

                barrel.transform.Rotate(barrelSpeed, 0, 0);
            }
    }
    [PunRPC] void TurretMove(string input)
    {
        if (turn == false && enemyTank.Equals("SmallTank"))
        {
            if (input.Equals("A"))
                turret.transform.Rotate(0, turretSpeed, 0);

            if (input.Equals("D"))
                turret.transform.Rotate(0, -turretSpeed, 0);
        }
        else if (turn == false && tankName.Equals("SmallTank"))
        {
            if (input.Equals("A"))
                turret.transform.Rotate(0, 0, turretSpeed);

            if (input.Equals("D"))
                turret.transform.Rotate(0, 0, -turretSpeed);
        }
        else if (turn == true && tankName.Equals("SmallTank"))
        {
            if (input.Equals("A"))
                turret.transform.Rotate(0, turretSpeed, 0);

            if (input.Equals("D"))
                turret.transform.Rotate(0, -turretSpeed, 0);
        }
        else if (!tankName.Equals("T95"))
        {
            if (input.Equals("A"))
                turret.transform.Rotate(0, 0, turretSpeed);

            if (input.Equals("D"))
                turret.transform.Rotate(0, 0, -turretSpeed);
        }
    }
    [PunRPC]
    void PlayerHellFlag()
    {
        destCnt++;
        if (gameObject.transform.position.y <= destPlayer)
        {
            if (PhotonNetwork.IsMasterClient) ENBVar.p1Hell = true;
            else ENBVar.p2Hell = true;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient) ENBVar.p2Hell = true;
            else ENBVar.p1Hell = true;
        }

        if (destCnt == 2)
        {
            ENBVar.p1Hell = true;
            ENBVar.p2Hell = true;
        }
    }
    [PunRPC] void PlayerDestroy()
    {
        if (gameObject.transform.position.y <= destPlayer)
        {
            if (PhotonNetwork.IsMasterClient) DirectorScript.instance.player1.hp = 0;
            else DirectorScript.instance.player2.hp = 0;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient) DirectorScript.instance.player2.hp = 0;
            else DirectorScript.instance.player1.hp = 0;
        }

        ENBVar.p1EndHP = DirectorScript.instance.player1.hp;
        ENBVar.p2EndHP = DirectorScript.instance.player2.hp;

        Destroy(gameObject);
    }

    void Start()
    {
        initCount = 1;
        dest = true;
        destCnt = 0;
    }

    void Update()
    {
        PlayerTurnValue();

        if (turn == false)
        {
            moveSpeed = 0;
        }

        if (DirectorScript.instance.PlayerTank != null && initCount == 1)
        {
            PlayerUserInit();
            initCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)) DirectorScript.instance.CameraChange(camera = !camera);

        DirectorScript.instance.AtkCount.text = shotCount.ToString();

        PlayerController();

        if (gameObject.transform.position.y <= destFlag && dest)
        {
            dest = false;
            PV.RPC("PlayerHellFlag", RpcTarget.AllBuffered);
        }
        if (gameObject.transform.position.y <= destPlayer) PV.RPC("PlayerDestroy", RpcTarget.AllBuffered);
    }

    void PlayerController()
    {
        if (PV.IsMine == false) return;
        if (PhotonNetwork.IsMasterClient) if (DirectorScript.instance.p1Turn == false) return;
        if (!PhotonNetwork.IsMasterClient) if (DirectorScript.instance.p2Turn == false) return;
        if (DirectorScript.instance.stayEnemyFlag == true) return;

        if (DirectorScript.instance.ChatInput.isFocused == false)
        {
            if (shotCount == 1)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    DirectorScript.instance.POWER.fillAmount += (0.3f * Time.deltaTime);
                    PV.RPC("ShotPower", RpcTarget.All);
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    DirectorScript.instance.PrevPOWER.fillAmount = DirectorScript.instance.POWER.fillAmount;
                    PV.RPC("ShotBoom", RpcTarget.All);
                    PhotonNetwork.Instantiate(cannon, firePos.position, firePos.rotation);
                }
            }

            if (moveCount > 0)
            {
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) ||
                    Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                {
                    PV.RPC("MovePlayer", RpcTarget.All);
                }
            }

            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) ||
                Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
                BGM.Stop();

            if (Input.GetKey(KeyCode.W)) PV.RPC("BarrelMove", RpcTarget.All, "W");
            if (Input.GetKey(KeyCode.S)) PV.RPC("BarrelMove", RpcTarget.All, "S");
            if (Input.GetKey(KeyCode.A)) PV.RPC("TurretMove", RpcTarget.All, "D");
            if (Input.GetKey(KeyCode.D)) PV.RPC("TurretMove", RpcTarget.All, "A");

            if (Input.GetKey(KeyCode.Alpha1))
            {
                itemFlag = true;
                if (userNumber == 1)
                    if (ENBVar.p1Item1 != 0)
                    {
                        if (ENBVar.p1Item1 == 1)
                        {
                            if (shotCount == 0)
                                DirectorScript.instance.doubleShot(userNumber);
                            else if (shotCount == 1)
                                itemFlag = false;
                        }
                        if (ENBVar.p1Item1 == 2) DirectorScript.instance.powerShot(userNumber);
                        if (ENBVar.p1Item1 == 3) DirectorScript.instance.haste(userNumber);
                        if (ENBVar.p1Item1 == 4) DirectorScript.instance.repair(userNumber);
                        if (ENBVar.p1Item1 == 5) DirectorScript.instance.smoke();
                        if (ENBVar.p1Item1 == 6) DirectorScript.instance.invisible();
                        if (ENBVar.p1Item1 == 7) DirectorScript.instance.spy();
                    }

                if (userNumber == 2)
                    if (ENBVar.p2Item1 != 0)
                    {
                        if (ENBVar.p2Item1 == 1)
                        {
                            if (shotCount == 0)
                                DirectorScript.instance.doubleShot(userNumber);
                            else if (shotCount == 1)
                                itemFlag = false;
                        }
                        if (ENBVar.p2Item1 == 2) DirectorScript.instance.powerShot(userNumber);
                        if (ENBVar.p2Item1 == 3) DirectorScript.instance.haste(userNumber);
                        if (ENBVar.p2Item1 == 4) DirectorScript.instance.repair(userNumber);
                        if (ENBVar.p2Item1 == 5) DirectorScript.instance.smoke();
                        if (ENBVar.p2Item1 == 6) DirectorScript.instance.invisible();
                        if (ENBVar.p2Item1 == 7) DirectorScript.instance.spy();
                    }

                if (itemFlag == true)
                    DirectorScript.instance.deleteItem(1);

                itemFlag = false;
            }

            if (Input.GetKey(KeyCode.Alpha2))
            {
                itemFlag = true;
                if (userNumber == 1)
                    if (ENBVar.p1Item2 != 0)
                    {
                        if (ENBVar.p1Item2 == 1)
                        {
                            if (shotCount == 0)
                                DirectorScript.instance.doubleShot(userNumber);
                            else if (shotCount == 1)
                                itemFlag = false;
                        }
                        if (ENBVar.p1Item2 == 2) DirectorScript.instance.powerShot(userNumber);
                        if (ENBVar.p1Item2 == 3) DirectorScript.instance.haste(userNumber);
                        if (ENBVar.p1Item2 == 4) DirectorScript.instance.repair(userNumber);
                        if (ENBVar.p1Item2 == 5) DirectorScript.instance.smoke();
                        if (ENBVar.p1Item2 == 6) DirectorScript.instance.invisible();
                        if (ENBVar.p1Item2 == 7) DirectorScript.instance.spy();
                    }

                if (userNumber == 2)
                    if (ENBVar.p2Item2 != 0)
                    {
                        if (ENBVar.p2Item2 == 1)
                        {
                            if (shotCount == 0)
                                DirectorScript.instance.doubleShot(userNumber);
                            else if (shotCount == 1)
                                itemFlag = false;
                        }
                        if (ENBVar.p2Item2 == 2) DirectorScript.instance.powerShot(userNumber);
                        if (ENBVar.p2Item2 == 3) DirectorScript.instance.haste(userNumber);
                        if (ENBVar.p2Item2 == 4) DirectorScript.instance.repair(userNumber);
                        if (ENBVar.p2Item2 == 5) DirectorScript.instance.smoke();
                        if (ENBVar.p2Item2 == 6) DirectorScript.instance.invisible();
                        if (ENBVar.p2Item2 == 7) DirectorScript.instance.spy();
                    }

                if (itemFlag == true)
                    DirectorScript.instance.deleteItem(2);

                itemFlag = false;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                turn = false;
                if (PhotonNetwork.IsMasterClient) DirectorScript.instance.PassTurn(1);
                else DirectorScript.instance.PassTurn(2);
            }
        }
    }

    void PlayerTurnValue()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            turn = DirectorScript.instance.p1Turn;
            shotCount = DirectorScript.instance.player1.shotCount;
            moveCount = DirectorScript.instance.player1.moveCount;
            moveSpeed = DirectorScript.instance.player1.move;

            DirectorScript.instance.Degree.text = (ENBVar.p1Degree * -1).ToString("F1") + "˚";
            DirectorScript.instance.ENEMYHP.fillAmount = DirectorScript.instance.player2.hp * 0.01f;
        }
        else
        {
            turn = DirectorScript.instance.p2Turn;
            shotCount = DirectorScript.instance.player2.shotCount;
            moveCount = DirectorScript.instance.player2.moveCount;
            moveSpeed = DirectorScript.instance.player2.move;

            DirectorScript.instance.Degree.text = (ENBVar.p2Degree * -1).ToString("F1") + "˚";
            DirectorScript.instance.ENEMYHP.fillAmount = DirectorScript.instance.player1.hp * 0.01f;
        }
    }

    void PlayerUserInit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            maxHp = DirectorScript.instance.player1.hp;
            maxAtk = DirectorScript.instance.player1.atk;
            maxDef = DirectorScript.instance.player1.def;
            tNameTemp = ENBVar.tank1.Split(' ');
            eTNameTemp = ENBVar.tank2.Split(' ');
            userNumber = 1;
        }
        else
        {
            maxHp = DirectorScript.instance.player2.hp;
            maxAtk = DirectorScript.instance.player2.atk;
            maxDef = DirectorScript.instance.player2.def;
            tNameTemp = ENBVar.tank2.Split(' ');
            eTNameTemp = ENBVar.tank1.Split(' ');
            userNumber = 2;
        }

        UserTank = DirectorScript.instance.PlayerTank;
        tankName = tNameTemp[0];
        enemyTank = eTNameTemp[0];

        if (tankName.Equals("T95")) cannon = "Boom30";
        else if (tankName.Equals("Panzer4")) cannon = "Boom6";
        else if (tankName.Equals("M4A3")) cannon = "Boom5";
        else cannon = "Boom25";

        firePos = GameObject.Find("barrel").transform.Find("FirePos").transform;

        PV = UserTank.GetComponent<PhotonView>();
        BGM = UserTank.GetComponent<AudioSource>();
        rb = UserTank.GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1.5f, 0);
        tr = UserTank.GetComponent<Transform>();
        SmoothFollow.instance.target = GameObject.Find(UserTank.name).transform.Find("turret").transform;
    }
}