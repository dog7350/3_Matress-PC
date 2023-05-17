using System.Collections;
using UnityEngine;
using Photon.Pun;

public class HellScript : MonoBehaviourPunCallbacks
{
    public static HellScript instance = null;

    private void Awake()
    {
        instance = this;
    }

    public PhotonView PV;
    int prevHellTurn;
    int nowHellTurn;

    int i = 0;
    int hellTurn = 5;
    public int planeNum;
    public int maxPlane = 0;
    public int maxHellCount = 0;

    void Start()
    {
        if (ENBVar.map == 1) // city
        {
            maxHellCount = 15;
            maxPlane = 100;
        }
        else if (ENBVar.map == 2) // mountain
        {
            maxHellCount = 21;
            maxPlane = 144;
        }
        else if (ENBVar.map == 3) // plain
        {
            maxHellCount = 9;
            maxPlane = 64;
        }

        nowHellTurn = DirectorScript.instance.turnCount;
        prevHellTurn = nowHellTurn;
    }


    void Update()
    {
        nowHellTurn = DirectorScript.instance.turnCount;

        if (nowHellTurn != prevHellTurn)
        {
            prevHellTurn = nowHellTurn;

            switch (nowHellTurn)
            {
                case 1: prevPlaneHell(); break;
                case 6: prevPlaneHell(); break;
                case 11: prevPlaneHell(); break;
                case 16: prevPlaneHell(); break;
                case 21: prevPlaneHell(); break;
            }
        }

        if (nowHellTurn < 30)
            if (nowHellTurn % hellTurn == 0)
                for (int i = 0; i < maxHellCount; i++)
                {
                    if (DirectorScript.instance.planeHell[i] != null)
                    {
                        PhotonNetwork.Instantiate("Explosion_Flame1", DirectorScript.instance.planeHell[i].transform.position, Quaternion.identity);
                        Destroy(DirectorScript.instance.planeHell[i]);
                        DirectorScript.instance.planeHell[i] = null;
                    }
                    if (DirectorScript.instance.planeInfo[i] != null)
                    {
                        Destroy(DirectorScript.instance.planeInfo[i]);
                        DirectorScript.instance.planeInfo[i] = null;
                    }
                }
    }

    void prevPlaneHell()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for(i = 0; i < maxHellCount; i++)
            {
                switch(i)
                {
                    case 0: PV.RPC("i0", RpcTarget.All); break;
                    case 1: PV.RPC("i1", RpcTarget.All); break;
                    case 2: PV.RPC("i2", RpcTarget.All); break;
                    case 3: PV.RPC("i3", RpcTarget.All); break;
                    case 4: PV.RPC("i4", RpcTarget.All); break;
                    case 5: PV.RPC("i5", RpcTarget.All); break;
                    case 6: PV.RPC("i6", RpcTarget.All); break;
                    case 7: PV.RPC("i7", RpcTarget.All); break;
                    case 8: PV.RPC("i8", RpcTarget.All); break;
                    case 9: PV.RPC("i9", RpcTarget.All); break;
                    case 10: PV.RPC("i10", RpcTarget.All); break;
                    case 11: PV.RPC("i11", RpcTarget.All); break;
                    case 12: PV.RPC("i12", RpcTarget.All); break;
                    case 13: PV.RPC("i13", RpcTarget.All); break;
                    case 14: PV.RPC("i14", RpcTarget.All); break;
                    case 15: PV.RPC("i15", RpcTarget.All); break;
                    case 16: PV.RPC("i16", RpcTarget.All); break;
                    case 17: PV.RPC("i17", RpcTarget.All); break;
                    case 18: PV.RPC("i18", RpcTarget.All); break;
                    case 19: PV.RPC("i19", RpcTarget.All); break;
                    case 20: PV.RPC("i20", RpcTarget.All); break;
                    case 21: PV.RPC("i21", RpcTarget.All); break;
                }
                planeNum = Random.Range(0, maxPlane);
                switch(planeNum)
                {
                    case 0: PV.RPC("plane0", RpcTarget.All); break;
                    case 1: PV.RPC("plane1", RpcTarget.All); break;
                    case 2: PV.RPC("plane2", RpcTarget.All); break;
                    case 3: PV.RPC("plane3", RpcTarget.All); break;
                    case 4: PV.RPC("plane4", RpcTarget.All); break;
                    case 5: PV.RPC("plane5", RpcTarget.All); break;
                    case 6: PV.RPC("plane6", RpcTarget.All); break;
                    case 7: PV.RPC("plane7", RpcTarget.All); break;
                    case 8: PV.RPC("plane8", RpcTarget.All); break;
                    case 9: PV.RPC("plane9", RpcTarget.All); break;
                    case 10: PV.RPC("plane10", RpcTarget.All); break;
                    case 11: PV.RPC("plane11", RpcTarget.All); break;
                    case 12: PV.RPC("plane12", RpcTarget.All); break;
                    case 13: PV.RPC("plane13", RpcTarget.All); break;
                    case 14: PV.RPC("plane14", RpcTarget.All); break;
                    case 15: PV.RPC("plane15", RpcTarget.All); break;
                    case 16: PV.RPC("plane16", RpcTarget.All); break;
                    case 17: PV.RPC("plane17", RpcTarget.All); break;
                    case 18: PV.RPC("plane18", RpcTarget.All); break;
                    case 19: PV.RPC("plane19", RpcTarget.All); break;
                    case 20: PV.RPC("plane20", RpcTarget.All); break;
                    case 21: PV.RPC("plane21", RpcTarget.All); break;
                    case 22: PV.RPC("plane22", RpcTarget.All); break;
                    case 23: PV.RPC("plane23", RpcTarget.All); break;
                    case 24: PV.RPC("plane24", RpcTarget.All); break;
                    case 25: PV.RPC("plane25", RpcTarget.All); break;
                    case 26: PV.RPC("plane26", RpcTarget.All); break;
                    case 27: PV.RPC("plane27", RpcTarget.All); break;
                    case 28: PV.RPC("plane28", RpcTarget.All); break;
                    case 29: PV.RPC("plane29", RpcTarget.All); break;
                    case 30: PV.RPC("plane30", RpcTarget.All); break;
                    case 31: PV.RPC("plane31", RpcTarget.All); break;
                    case 32: PV.RPC("plane32", RpcTarget.All); break;
                    case 33: PV.RPC("plane33", RpcTarget.All); break;
                    case 34: PV.RPC("plane34", RpcTarget.All); break;
                    case 35: PV.RPC("plane35", RpcTarget.All); break;
                    case 36: PV.RPC("plane36", RpcTarget.All); break;
                    case 37: PV.RPC("plane37", RpcTarget.All); break;
                    case 38: PV.RPC("plane38", RpcTarget.All); break;
                    case 39: PV.RPC("plane39", RpcTarget.All); break;
                    case 40: PV.RPC("plane40", RpcTarget.All); break;
                    case 41: PV.RPC("plane41", RpcTarget.All); break;
                    case 42: PV.RPC("plane42", RpcTarget.All); break;
                    case 43: PV.RPC("plane43", RpcTarget.All); break;
                    case 44: PV.RPC("plane44", RpcTarget.All); break;
                    case 45: PV.RPC("plane45", RpcTarget.All); break;
                    case 46: PV.RPC("plane46", RpcTarget.All); break;
                    case 47: PV.RPC("plane47", RpcTarget.All); break;
                    case 48: PV.RPC("plane48", RpcTarget.All); break;
                    case 49: PV.RPC("plane49", RpcTarget.All); break;
                    case 50: PV.RPC("plane50", RpcTarget.All); break;
                    case 51: PV.RPC("plane51", RpcTarget.All); break;
                    case 52: PV.RPC("plane52", RpcTarget.All); break;
                    case 53: PV.RPC("plane53", RpcTarget.All); break;
                    case 54: PV.RPC("plane54", RpcTarget.All); break;
                    case 55: PV.RPC("plane55", RpcTarget.All); break;
                    case 56: PV.RPC("plane56", RpcTarget.All); break;
                    case 57: PV.RPC("plane57", RpcTarget.All); break;
                    case 58: PV.RPC("plane58", RpcTarget.All); break;
                    case 59: PV.RPC("plane59", RpcTarget.All); break;
                    case 60: PV.RPC("plane60", RpcTarget.All); break;
                    case 61: PV.RPC("plane61", RpcTarget.All); break;
                    case 62: PV.RPC("plane62", RpcTarget.All); break;
                    case 63: PV.RPC("plane63", RpcTarget.All); break;
                    case 64: PV.RPC("plane64", RpcTarget.All); break;
                    case 65: PV.RPC("plane65", RpcTarget.All); break;
                    case 66: PV.RPC("plane66", RpcTarget.All); break;
                    case 67: PV.RPC("plane67", RpcTarget.All); break;
                    case 68: PV.RPC("plane68", RpcTarget.All); break;
                    case 69: PV.RPC("plane69", RpcTarget.All); break;
                    case 70: PV.RPC("plane70", RpcTarget.All); break;
                    case 71: PV.RPC("plane71", RpcTarget.All); break;
                    case 72: PV.RPC("plane72", RpcTarget.All); break;
                    case 73: PV.RPC("plane73", RpcTarget.All); break;
                    case 74: PV.RPC("plane74", RpcTarget.All); break;
                    case 75: PV.RPC("plane75", RpcTarget.All); break;
                    case 76: PV.RPC("plane76", RpcTarget.All); break;
                    case 77: PV.RPC("plane77", RpcTarget.All); break;
                    case 78: PV.RPC("plane78", RpcTarget.All); break;
                    case 79: PV.RPC("plane79", RpcTarget.All); break;
                    case 80: PV.RPC("plane80", RpcTarget.All); break;
                    case 81: PV.RPC("plane81", RpcTarget.All); break;
                    case 82: PV.RPC("plane82", RpcTarget.All); break;
                    case 83: PV.RPC("plane83", RpcTarget.All); break;
                    case 84: PV.RPC("plane84", RpcTarget.All); break;
                    case 85: PV.RPC("plane85", RpcTarget.All); break;
                    case 86: PV.RPC("plane86", RpcTarget.All); break;
                    case 87: PV.RPC("plane87", RpcTarget.All); break;
                    case 88: PV.RPC("plane88", RpcTarget.All); break;
                    case 89: PV.RPC("plane89", RpcTarget.All); break;
                    case 90: PV.RPC("plane90", RpcTarget.All); break;
                    case 91: PV.RPC("plane91", RpcTarget.All); break;
                    case 92: PV.RPC("plane92", RpcTarget.All); break;
                    case 93: PV.RPC("plane93", RpcTarget.All); break;
                    case 94: PV.RPC("plane94", RpcTarget.All); break;
                    case 95: PV.RPC("plane95", RpcTarget.All); break;
                    case 96: PV.RPC("plane96", RpcTarget.All); break;
                    case 97: PV.RPC("plane97", RpcTarget.All); break;
                    case 98: PV.RPC("plane98", RpcTarget.All); break;
                    case 99: PV.RPC("plane99", RpcTarget.All); break;
                    case 100: PV.RPC("plane100", RpcTarget.All); break;
                    case 101: PV.RPC("plane101", RpcTarget.All); break;
                    case 102: PV.RPC("plane102", RpcTarget.All); break;
                    case 103: PV.RPC("plane103", RpcTarget.All); break;
                    case 104: PV.RPC("plane104", RpcTarget.All); break;
                    case 105: PV.RPC("plane105", RpcTarget.All); break;
                    case 106: PV.RPC("plane106", RpcTarget.All); break;
                    case 107: PV.RPC("plane107", RpcTarget.All); break;
                    case 108: PV.RPC("plane108", RpcTarget.All); break;
                    case 109: PV.RPC("plane109", RpcTarget.All); break;
                    case 110: PV.RPC("plane110", RpcTarget.All); break;
                    case 111: PV.RPC("plane111", RpcTarget.All); break;
                    case 112: PV.RPC("plane112", RpcTarget.All); break;
                    case 113: PV.RPC("plane113", RpcTarget.All); break;
                    case 114: PV.RPC("plane114", RpcTarget.All); break;
                    case 115: PV.RPC("plane115", RpcTarget.All); break;
                    case 116: PV.RPC("plane116", RpcTarget.All); break;
                    case 117: PV.RPC("plane117", RpcTarget.All); break;
                    case 118: PV.RPC("plane118", RpcTarget.All); break;
                    case 119: PV.RPC("plane119", RpcTarget.All); break;
                    case 120: PV.RPC("plane120", RpcTarget.All); break;
                    case 121: PV.RPC("plane121", RpcTarget.All); break;
                    case 122: PV.RPC("plane122", RpcTarget.All); break;
                    case 123: PV.RPC("plane123", RpcTarget.All); break;
                    case 124: PV.RPC("plane124", RpcTarget.All); break;
                    case 125: PV.RPC("plane125", RpcTarget.All); break;
                    case 126: PV.RPC("plane126", RpcTarget.All); break;
                    case 127: PV.RPC("plane127", RpcTarget.All); break;
                    case 128: PV.RPC("plane128", RpcTarget.All); break;
                    case 129: PV.RPC("plane129", RpcTarget.All); break;
                    case 130: PV.RPC("plane130", RpcTarget.All); break;
                    case 131: PV.RPC("plane131", RpcTarget.All); break;
                    case 132: PV.RPC("plane132", RpcTarget.All); break;
                    case 133: PV.RPC("plane133", RpcTarget.All); break;
                    case 134: PV.RPC("plane134", RpcTarget.All); break;
                    case 135: PV.RPC("plane135", RpcTarget.All); break;
                    case 136: PV.RPC("plane136", RpcTarget.All); break;
                    case 137: PV.RPC("plane137", RpcTarget.All); break;
                    case 138: PV.RPC("plane138", RpcTarget.All); break;
                    case 139: PV.RPC("plane139", RpcTarget.All); break;
                    case 140: PV.RPC("plane140", RpcTarget.All); break;
                    case 141: PV.RPC("plane141", RpcTarget.All); break;
                    case 142: PV.RPC("plane142", RpcTarget.All); break;
                    case 143: PV.RPC("plane143", RpcTarget.All); break;
                    case 144: PV.RPC("plane144", RpcTarget.All); break;
                }

                PV.RPC("planeObjectArray", RpcTarget.All);
            }
        }

        maxPlane -= maxHellCount;
    }

    [PunRPC]
    void planeObjectArray()
    {
        DirectorScript.instance.planeHell[i] = GameObject.Find("PlaneObject").transform.GetChild(planeNum).gameObject;
        DirectorScript.instance.planeInfo[i] = GameObject.Find("PlaneInfoObject").transform.GetChild(planeNum).gameObject;
    }

    [PunRPC]
    void i0() => i = 0;
    [PunRPC]
    void i1() => i = 1;
    [PunRPC]
    void i2() => i = 2;
    [PunRPC]
    void i3() => i = 3;
    [PunRPC]
    void i4() => i = 4;
    [PunRPC]
    void i5() => i = 5;
    [PunRPC]
    void i6() => i = 6;
    [PunRPC]
    void i7() => i = 7;
    [PunRPC]
    void i8() => i = 8;
    [PunRPC]
    void i9() => i = 9;
    [PunRPC]
    void i10() => i = 10;
    [PunRPC]
    void i11() => i = 11;
    [PunRPC]
    void i12() => i = 12;
    [PunRPC]
    void i13() => i = 13;
    [PunRPC]
    void i14() => i = 14;
    [PunRPC]
    void i15() => i = 15;
    [PunRPC]
    void i16() => i = 16;
    [PunRPC]
    void i17() => i = 17;
    [PunRPC]
    void i18() => i = 18;
    [PunRPC]
    void i19() => i = 19;
    [PunRPC]
    void i20() => i = 20;
    [PunRPC]
    void i21() => i = 21;

    [PunRPC]
    void plane0() => planeNum = 0;
    [PunRPC]
    void plane1() => planeNum = 1;
    [PunRPC]
    void plane2() => planeNum = 2;
    [PunRPC]
    void plane3() => planeNum = 3;
    [PunRPC]
    void plane4() => planeNum = 4;
    [PunRPC]
    void plane5() => planeNum = 5;
    [PunRPC]
    void plane6() => planeNum = 6;
    [PunRPC]
    void plane7() => planeNum = 7;
    [PunRPC]
    void plane8() => planeNum = 8;
    [PunRPC]
    void plane9() => planeNum = 9;
    [PunRPC]
    void plane10() => planeNum = 10;
    [PunRPC]
    void plane11() => planeNum = 11;
    [PunRPC]
    void plane12() => planeNum = 12;
    [PunRPC]
    void plane13() => planeNum = 13;
    [PunRPC]
    void plane14() => planeNum = 14;
    [PunRPC]
    void plane15() => planeNum = 15;
    [PunRPC]
    void plane16() => planeNum = 16;
    [PunRPC]
    void plane17() => planeNum = 17;
    [PunRPC]
    void plane18() => planeNum = 18;
    [PunRPC]
    void plane19() => planeNum = 19;
    [PunRPC]
    void plane20() => planeNum = 20;
    [PunRPC]
    void plane21() => planeNum = 21;
    [PunRPC]
    void plane22() => planeNum = 22;
    [PunRPC]
    void plane23() => planeNum = 23;
    [PunRPC]
    void plane24() => planeNum = 24;
    [PunRPC]
    void plane25() => planeNum = 25;
    [PunRPC]
    void plane26() => planeNum = 26;
    [PunRPC]
    void plane27() => planeNum = 27;
    [PunRPC]
    void plane28() => planeNum = 28;
    [PunRPC]
    void plane29() => planeNum = 29;
    [PunRPC]
    void plane30() => planeNum = 30;
    [PunRPC]
    void plane31() => planeNum = 31;
    [PunRPC]
    void plane32() => planeNum = 32;
    [PunRPC]
    void plane33() => planeNum = 33;
    [PunRPC]
    void plane34() => planeNum = 34;
    [PunRPC]
    void plane35() => planeNum = 35;
    [PunRPC]
    void plane36() => planeNum = 36;
    [PunRPC]
    void plane37() => planeNum = 37;
    [PunRPC]
    void plane38() => planeNum = 38;
    [PunRPC]
    void plane39() => planeNum = 39;
    [PunRPC]
    void plane40() => planeNum = 40;
    [PunRPC]
    void plane41() => planeNum = 41;
    [PunRPC]
    void plane42() => planeNum = 42;
    [PunRPC]
    void plane43() => planeNum = 43;
    [PunRPC]
    void plane44() => planeNum = 44;
    [PunRPC]
    void plane45() => planeNum = 45;
    [PunRPC]
    void plane46() => planeNum = 46;
    [PunRPC]
    void plane47() => planeNum = 47;
    [PunRPC]
    void plane48() => planeNum = 48;
    [PunRPC]
    void plane49() => planeNum = 49;
    [PunRPC]
    void plane50() => planeNum = 50;
    [PunRPC]
    void plane51() => planeNum = 51;
    [PunRPC]
    void plane52() => planeNum = 52;
    [PunRPC]
    void plane53() => planeNum = 53;
    [PunRPC]
    void plane54() => planeNum = 54;
    [PunRPC]
    void plane55() => planeNum = 55;
    [PunRPC]
    void plane56() => planeNum = 56;
    [PunRPC]
    void plane57() => planeNum = 57;
    [PunRPC]
    void plane58() => planeNum = 58;
    [PunRPC]
    void plane59() => planeNum = 59;
    [PunRPC]
    void plane60() => planeNum = 60;
    [PunRPC]
    void plane61() => planeNum = 61;
    [PunRPC]
    void plane62() => planeNum = 62;
    [PunRPC]
    void plane63() => planeNum = 63;
    [PunRPC]
    void plane64() => planeNum = 64;
    [PunRPC]
    void plane65() => planeNum = 65;
    [PunRPC]
    void plane66() => planeNum = 66;
    [PunRPC]
    void plane67() => planeNum = 67;
    [PunRPC]
    void plane68() => planeNum = 68;
    [PunRPC]
    void plane69() => planeNum = 69;
    [PunRPC]
    void plane70() => planeNum = 70;
    [PunRPC]
    void plane71() => planeNum = 71;
    [PunRPC]
    void plane72() => planeNum = 72;
    [PunRPC]
    void plane73() => planeNum = 73;
    [PunRPC]
    void plane74() => planeNum = 74;
    [PunRPC]
    void plane75() => planeNum = 75;
    [PunRPC]
    void plane76() => planeNum = 76;
    [PunRPC]
    void plane77() => planeNum = 77;
    [PunRPC]
    void plane78() => planeNum = 78;
    [PunRPC]
    void plane79() => planeNum = 79;
    [PunRPC]
    void plane80() => planeNum = 80;
    [PunRPC]
    void plane81() => planeNum = 81;
    [PunRPC]
    void plane82() => planeNum = 82;
    [PunRPC]
    void plane83() => planeNum = 83;
    [PunRPC]
    void plane84() => planeNum = 84;
    [PunRPC]
    void plane85() => planeNum = 85;
    [PunRPC]
    void plane86() => planeNum = 86;
    [PunRPC]
    void plane87() => planeNum = 87;
    [PunRPC]
    void plane88() => planeNum = 88;
    [PunRPC]
    void plane89() => planeNum = 89;
    [PunRPC]
    void plane90() => planeNum = 90;
    [PunRPC]
    void plane91() => planeNum = 91;
    [PunRPC]
    void plane92() => planeNum = 92;
    [PunRPC]
    void plane93() => planeNum = 93;
    [PunRPC]
    void plane94() => planeNum = 94;
    [PunRPC]
    void plane95() => planeNum = 95;
    [PunRPC]
    void plane96() => planeNum = 96;
    [PunRPC]
    void plane97() => planeNum = 97;
    [PunRPC]
    void plane98() => planeNum = 98;
    [PunRPC]
    void plane99() => planeNum = 99;
    [PunRPC]
    void plane100() => planeNum = 100;
    [PunRPC]
    void plane101() => planeNum = 101;
    [PunRPC]
    void plane102() => planeNum = 102;
    [PunRPC]
    void plane103() => planeNum = 103;
    [PunRPC]
    void plane104() => planeNum = 104;
    [PunRPC]
    void plane105() => planeNum = 105;
    [PunRPC]
    void plane106() => planeNum = 106;
    [PunRPC]
    void plane107() => planeNum = 107;
    [PunRPC]
    void plane108() => planeNum = 108;
    [PunRPC]
    void plane109() => planeNum = 109;
    [PunRPC]
    void plane110() => planeNum = 110;
    [PunRPC]
    void plane111() => planeNum = 111;
    [PunRPC]
    void plane112() => planeNum = 112;
    [PunRPC]
    void plane113() => planeNum = 113;
    [PunRPC]
    void plane114() => planeNum = 114;
    [PunRPC]
    void plane115() => planeNum = 115;
    [PunRPC]
    void plane116() => planeNum = 116;
    [PunRPC]
    void plane117() => planeNum = 117;
    [PunRPC]
    void plane118() => planeNum = 118;
    [PunRPC]
    void plane119() => planeNum = 119;
    [PunRPC]
    void plane120() => planeNum = 120;
    [PunRPC]
    void plane121() => planeNum = 121;
    [PunRPC]
    void plane122() => planeNum = 122;
    [PunRPC]
    void plane123() => planeNum = 123;
    [PunRPC]
    void plane124() => planeNum = 124;
    [PunRPC]
    void plane125() => planeNum = 125;
    [PunRPC]
    void plane126() => planeNum = 126;
    [PunRPC]
    void plane127() => planeNum = 127;
    [PunRPC]
    void plane128() => planeNum = 128;
    [PunRPC]
    void plane129() => planeNum = 129;
    [PunRPC]
    void plane130() => planeNum = 130;
    [PunRPC]
    void plane131() => planeNum = 131;
    [PunRPC]
    void plane132() => planeNum = 132;
    [PunRPC]
    void plane133() => planeNum = 133;
    [PunRPC]
    void plane134() => planeNum = 134;
    [PunRPC]
    void plane135() => planeNum = 135;
    [PunRPC]
    void plane136() => planeNum = 136;
    [PunRPC]
    void plane137() => planeNum = 137;
    [PunRPC]
    void plane138() => planeNum = 138;
    [PunRPC]
    void plane139() => planeNum = 139;
    [PunRPC]
    void plane140() => planeNum = 140;
    [PunRPC]
    void plane141() => planeNum = 141;
    [PunRPC]
    void plane142() => planeNum = 142;
    [PunRPC]
    void plane143() => planeNum = 143;
    [PunRPC]
    void plane144() => planeNum = 144;
}
