using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Kill_Sound_GoldKingZ;

public class Globals
{
    public static int Takefreezetime;
    public static Stopwatch Timers = new Stopwatch();
    public static bool First_Blood = false;
    public static Dictionary<ulong, int> Kill_Streak = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> Kill_StreakHS = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> Kill_Knife = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> Kill_Nade = new Dictionary<ulong, int>();
    public static Dictionary<ulong, int> Kill_Molly = new Dictionary<ulong, int>();
    public static Dictionary<ulong, DateTime> lastPlayTimes = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, DateTime> lastPlayTimesHS = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, DateTime> lastPlayTimesKnife = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, DateTime> lastPlayTimesNade = new Dictionary<ulong, DateTime>();
    public static Dictionary<ulong, DateTime> lastPlayTimesMolly = new Dictionary<ulong, DateTime>();
}