using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using CounterStrikeSharp.API.Modules.Entities;
using System.Text.RegularExpressions;
using System.Text.Json;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace Kill_Sound_GoldKingZ;

public class Helper
{
    public static Dictionary<string, Dictionary<string, string>> LoadJsonFromFile(string filePath)
    {
        using (StreamReader r = new StreamReader(filePath))
        {
            string json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json)!;
        }
    }

    public static void AdvancedPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                player.PrintToChat(" " + messages);
            }
        }else
        {
            player.PrintToChat(message);
        }
    }
    public static void AdvancedPrintToServer(string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                Server.PrintToChatAll(" " + messages);
            }
        }else
        {
            Server.PrintToChatAll(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        var excludedGroups = groups.Split(',');
        foreach (var group in excludedGroups)
        {
            if (group.StartsWith("#"))
            {
                if (AdminManager.PlayerInGroup(player, group))
                    return true;
            }
            else if (group.StartsWith("@"))
            {
                if (AdminManager.PlayerHasPermissions(player, group))
                    return true;
            }
        }
        return false;
    }
    public static List<CCSPlayerController> GetCounterTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.CounterTerrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.Terrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetAllController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected).ToList();
        return playerList;
    }
    public static int GetCounterTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.CounterTerrorist);
    }
    public static int GetTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.Terrorist);
    }
    public static int GetAllCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected);
    }
    public static void ClearVariables()
    {
        Globals.Kill_Streak.Clear();
        Globals.Kill_StreakHS.Clear();
        Globals.Kill_Knife.Clear();
        Globals.Kill_Nade.Clear();
        Globals.Kill_Molly.Clear();
        Globals.lastPlayTimes.Clear();
        Globals.lastPlayTimesHS.Clear();
        Globals.lastPlayTimesKnife.Clear();
        Globals.lastPlayTimesNade.Clear();
        Globals.lastPlayTimesMolly.Clear();
    }
    
    public static string ReplaceMessages(string Message, string date, string time, string PlayerName, string SteamId, string ipAddress, string reason)
    {
        var replacedMessage = Message
                                    .Replace("{TIME}", time)
                                    .Replace("{DATE}", date)
                                    .Replace("{PLAYERNAME}", PlayerName.ToString())
                                    .Replace("{STEAMID}", SteamId.ToString())
                                    .Replace("{IP}", ipAddress.ToString())
                                    .Replace("{REASON}", reason);
        return replacedMessage;
    }
    public static void CreateDefaultWeaponsJson(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            var configData = new Dictionary<string, Dictionary<string, object>>
            {
                ["HeadShot_1"] = new Dictionary<string, object>
                {
                    { "Path", "sounds/GoldKingZ/Quake/headshot.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["HeadShot_2"] = new Dictionary<string, object>
                {
                    { "Path", "sounds/GoldKingZ/Quake/headshot.vsnd_c" }
                },
                ["HeadShot_5"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/headhunter.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_4"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/dominating.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_6"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/rampage.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_8"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/killingspree.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_10"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/monsterkill.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_14"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/unstoppable.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_16"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/ultrakill.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_18"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/godlike.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_20"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/wickedsick.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_24"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/ludicrouskill.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_26"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/holyshit.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["KnifeKill"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/humiliation.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["GrenadeKill"] = new Dictionary<string, object>
                {
                    { "Announcement", false },
                    { "Path", "sounds/GoldKingZ/Quake/perfect.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["MollyKill"] = new Dictionary<string, object>
                {
                    { "Announcement", false },
                    { "Path", "sounds/GoldKingZ/Quake/impressive.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["SelfKill"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/haha.vsnd_c" }
                },
                ["TeamKill"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "Path", "sounds/GoldKingZ/Quake/teamkiller.vsnd_c" }
                },
                ["FirstBlood"] = new Dictionary<string, object>
                {
                    { "Path", "sounds/GoldKingZ/Quake/firstblood.vsnd_c" }
                },
                ["RoundPrepare"] = new Dictionary<string, object>
                {
                    { "Path", "sounds/GoldKingZ/Quake/prepare.vsnd_c" }
                },
                ["RoundStart"] = new Dictionary<string, object>
                {
                    { "Path", "sounds/GoldKingZ/Quake/play.vsnd_c" }
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(configData, options);

            json = "// Note: To Use These You Need To Enable KS_EnableQuakeSounds First In config.json \n// Then Download https://github.com/Source2ZE/MultiAddonManager  With Gold KingZ WorkShop \n// https://steamcommunity.com/sharedfiles/filedetails/?id=3230015783\n// mm_extra_addons 3230015783 \n\n" + json;

            File.WriteAllText(jsonFilePath, json);
        }
    }
    public static string RemoveLeadingSpaces(string content)
    {
        string[] lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].TrimStart();
        }
        return string.Join("\n", lines);
    }
    private static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch
        {
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }
}