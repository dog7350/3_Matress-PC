using System.Collections.Generic;
using Photon.Realtime;

public static class ENBVar
{
    public static bool sceneChangeFlag;
    public static List<RoomInfo> roomList;

    public static int map = 1;
    public static int light = 1;
    public static int turn = 1;

    public static string tank1 = "";
    public static string tank2 = "";

    public static int p1Atk = 0;
    public static int p1Def = 0;
    public static float p1Speed = 0;
    public static int p2Atk = 0;
    public static int p2Def = 0;
    public static float p2Speed = 0;

    public static float shotPower = 0f;
    public static float p1Degree = 0f;
    public static float p2Degree = 0f;

    public static bool p1Hell = false;
    public static bool p2Hell = false;

    public static float p1EndHP = 0;
    public static float p2EndHP = 0;

    public static int p1Item1 = 0;
    public static int p1Item2 = 0;
    public static int p2Item1 = 0;
    public static int p2Item2 = 0;

    public static void ResetENBVar()
    {
        map = 1;
        light = 1;
        turn = 1;

        tank1 = "";
        tank2 = "";

        p1Atk = 0;
        p1Def = 0;
        p1Speed = 0;
        p2Atk = 0;
        p2Def = 0;
        p2Speed = 0;

        shotPower = 0f;
        p1Degree = 0f;
        p2Degree = 0f;

        p1Hell = false;
        p2Hell = false;

        p1EndHP = 0;
        p2EndHP = 0;

        p1Item1 = 0;
        p1Item2 = 0;
        p2Item1 = 0;
        p2Item2 = 0;
    }
}
