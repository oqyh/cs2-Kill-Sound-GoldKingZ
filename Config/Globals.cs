using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Kill_Sound_GoldKingZ;

public class Globals
{
    public static int Takefreezetime;
    public static Stopwatch Timers = new Stopwatch();
    public static bool First_Blood = false;
    public static bool Round_Start = false;
    public static Dictionary<ulong, int> Kill_Streak = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> Kill_StreakHS = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> Kill_Knife = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> Kill_Nade = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> Kill_Molly = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> Kill_Taser = new Dictionary<ulong, int>();
    public static Dictionary<ulong, DateTime> lastPlayTimes = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, DateTime> lastPlayTimesHS = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, DateTime> lastPlayTimesKnife = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, DateTime> lastPlayTimesNade = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, DateTime> lastPlayTimesMolly = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, DateTime> lastPlayTimesTaser = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, bool> allow_groups = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> menuon = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, int> currentIndexDict = new Dictionary<ulong, int>();
    public static Dictionary<ulong, bool> buttonPressed = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, string> ShowHud_Kill = new Dictionary<ulong, string>();
    public static string ShowHud_Kill_Name = "";
    public static string ShowHud_Kill_Name2 = "";
    public static int ShowHud_Kill_int = 0;
}