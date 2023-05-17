using UnityEngine;
using Photon.Pun;

public class ItemScript : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    int turn = 0;
    int sub = 0;
    int itemNumber = 1;

    void Start() => turn = DirectorScript.instance.turnCount;


    void Update()
    {
        if(turn != DirectorScript.instance.turnCount)
        {
            turn = DirectorScript.instance.turnCount;
            sub++;
        }

        if (sub >= 4)
            Destroy(gameObject);

        if (gameObject.transform.position.y <= -30) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        itemNumber = Random.Range(1, 8);

        if (col.gameObject.tag == "Player")
        {
            string name = col.gameObject.name;
            name = name.Replace("(Clone)", "");

            if (ENBVar.tank1.Equals(name))
            {
                if (ENBVar.p1Item1 == 0) ENBVar.p1Item1 = itemNumber;
                else if (ENBVar.p1Item2 == 0) ENBVar.p1Item2 = itemNumber;
            }
            else if (ENBVar.tank2.Equals(name))
            {
                if (ENBVar.p2Item1 == 0) ENBVar.p2Item1 = itemNumber;
                else if (ENBVar.p2Item2 == 0) ENBVar.p2Item2 = itemNumber;
            }

            Destroy(gameObject);
        }
    }
}
