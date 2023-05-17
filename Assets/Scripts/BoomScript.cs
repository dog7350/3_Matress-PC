using UnityEngine;
using Photon.Pun;

public class BoomScript : MonoBehaviourPunCallbacks
{
    public static BoomScript instance = null;

    private void Awake()
    {
        instance = this;
    }

    public PhotonView PV;

    int createCount = 1;

    public float speed = 2000f;
    public float power;
    private Transform tr;

    public AudioClip ShortBoom;
    public AudioClip LargeBoom;
    public AudioClip DoubleBoom;

    public AudioClip BoomSound;

    string bName;

    [PunRPC]
    void BoomDestroy()
    {
        DirectorScript.instance.POWER.fillAmount = 0;
        ENBVar.shotPower = 0;

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        if (!PV.IsMine && col.gameObject.tag == "Player" && col.gameObject.GetComponent<PhotonView>().IsMine)
        {
            col.gameObject.GetComponent<PlayerScript>().Hit();

            if (bName.Equals("Boom5")) col.gameObject.GetComponent<PlayerScript>().Hit();

            if (PlayerScript.instance.turn == true)
                if (createCount == 1)
                {
                    if (bName.Equals("Boom30")) PhotonNetwork.Instantiate("Explosion_Flame2", tr.position, tr.rotation); // 큰 범위
                    else if (bName.Equals("Boom6")) PhotonNetwork.Instantiate("Explosion_Flame1", tr.position, tr.rotation); // 작은 범위
                    else if (bName.Equals("Boom5")) PhotonNetwork.Instantiate("Explosion_Flame1", tr.position, tr.rotation);
                    else PhotonNetwork.Instantiate("Explosion_Flame2", tr.position, tr.rotation); // 큰 범위
                    createCount = 0;
                }
        }
        else
        {
            if (PlayerScript.instance.turn == true)
                if (createCount == 1)
                {
                    if (bName.Equals("Boom30")) PhotonNetwork.Instantiate("Explosion_Flame2", tr.position, tr.rotation); // 큰 범위
                    else if (bName.Equals("Boom6")) PhotonNetwork.Instantiate("Explosion_Flame1", tr.position, tr.rotation); // 작은 범위
                    else if (bName.Equals("Boom5")) PhotonNetwork.Instantiate("Explosion_Flame1", tr.position, tr.rotation);
                    else PhotonNetwork.Instantiate("Explosion_Flame2", tr.position, tr.rotation); // 큰 범위
                    createCount = 0;
                }
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (bName.Equals("Boom5")) PhotonNetwork.Instantiate("Explosion_Flame1", tr.position, tr.rotation);
    }

    void OnCollisionExit(Collision col) => PV.RPC("BoomDestroy", RpcTarget.AllBuffered);

    void Start()
    {
        bName = gameObject.name;
        bName = bName.Replace("(Clone)", "");

        tr = GetComponent<Transform>();

        power = ENBVar.shotPower * 3f;

        createCount = 1;

        if (bName.Equals("Boom6")) BoomSound = ShortBoom;
        else if (bName.Equals("Boom5")) BoomSound = DoubleBoom;
        else if (bName.Equals("Boom30")) BoomSound = LargeBoom;
        else if (bName.Equals("Boom25")) BoomSound = LargeBoom;
        else BoomSound = ShortBoom;
    }

    void Update()
    {
        tr.Translate(Vector3.forward * speed * power * Time.deltaTime);

        if (tr.transform.position.y <= -20) PV.RPC("BoomDestroy", RpcTarget.AllBuffered);
    }
}
