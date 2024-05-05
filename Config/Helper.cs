using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text.Json;
using Kill_Sound_GoldKingZ.Config;

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
        Globals.Kill_Taser.Clear();
        Globals.lastPlayTimes.Clear();
        Globals.lastPlayTimesHS.Clear();
        Globals.lastPlayTimesKnife.Clear();
        Globals.lastPlayTimesNade.Clear();
        Globals.lastPlayTimesMolly.Clear();
        Globals.lastPlayTimesTaser.Clear();
        Globals.allow_groups.Clear();
        Globals.menuon.Clear();
        Globals.currentIndexDict.Clear();
        Globals.buttonPressed.Clear();
        Globals.ShowHud_Kill.Clear();
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
                    { "Announcement", false },
                    { "ShowChat", false },
                    { "ShowCenter", false },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/headshot.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["HeadShot_2"] = new Dictionary<string, object>
                {
                    { "Announcement", false },
                    { "ShowChat", false },
                    { "ShowCenter", false },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/headshot.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["HeadShot_5"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/headhunter.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_4"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/dominating.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_6"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/rampage.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_8"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/killingspree.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_10"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/monsterkill.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_14"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/unstoppable.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_16"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/ultrakill.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_18"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/godlike.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_20"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/wickedsick.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_24"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/ludicrouskill.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["Kill_26"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 15 },
                    { "Path", "sounds/GoldKingZ/Quake/holyshit.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["KnifeKill"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/humiliation.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["TaserKill"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/humiliation.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["GrenadeKill"] = new Dictionary<string, object>
                {
                    { "Announcement", false },
                    { "ShowChat", true },
                    { "ShowCenter", false },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/perfect.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["MollyKill"] = new Dictionary<string, object>
                {
                    { "Announcement", false },
                    { "ShowChat", true },
                    { "ShowCenter", false },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/impressive.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["SelfKill"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", false },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/haha.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["TeamKill"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", false },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/teamkiller.vsnd_c" },
                    { "Interval_InSecs", 5 }
                },
                ["FirstBlood"] = new Dictionary<string, object>
                {
                    { "Announcement", true },
                    { "ShowChat", true },
                    { "ShowCenter", false },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/firstblood.vsnd_c" }
                },
                ["RoundPrepare"] = new Dictionary<string, object>
                {
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/prepare.vsnd_c" }
                },
                ["RoundStart"] = new Dictionary<string, object>
                {
                    { "ShowChat", true },
                    { "ShowCenter", true },
                    { "ShowCenter_InSecs", 10 },
                    { "Path", "sounds/GoldKingZ/Quake/play.vsnd_c" }
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(configData, options);

            json = "// Note: To Use These You Need To Enable KS_EnableQuakeSounds First In config.json \n// Then Download https://github.com/Source2ZE/MultiAddonManager  With Gold KingZ WorkShop \n// https://steamcommunity.com/sharedfiles/filedetails/?id=3230015783\n// mm_extra_addons 3230015783\n// You Can Find WorkShop Path Sound In  https://github.com/oqyh/cs2-Kill-Sound-GoldKingZ/blob/main/sounds/Gold%20KingZ%20WorkShop%20Sounds.txt \n\n" + json;

            File.WriteAllText(jsonFilePath, json);
        }
    }

    public static void CreateDefaultWeaponsJson2(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            var configData = new Dictionary<string, object>
            {
                {"MySqlHost", "your_mysql_host"},
                {"MySqlDatabase", "your_mysql_database"},
                {"MySqlUsername", "your_mysql_username"},
                {"MySqlPassword", "your_mysql_password"},
                {"MySqlPort", 3306}
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(configData, options);

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

    public class PersonData
    {
        public ulong PlayerSteamID { get; set; }
        public bool freezemenu { get; set; }
        public bool headshotkill { get; set; }
        public bool headshothit { get; set; }
        public bool bodyshotkill { get; set; }
        public bool bodyshothit { get; set; }
        public bool quakesounds { get; set; }
        public bool quakehmessages { get; set; }
        public bool quakecmessages { get; set; }
        public DateTime DateAndTime { get; set; }
    }
    public static void SaveToJsonFile(ulong PlayerSteamID, bool freezemenu, bool headshotkill, bool headshothit, bool bodyshotkill, bool bodyshothit, bool quakesounds, bool quakehmessages, bool quakecmessages, DateTime DateAndTime)
    {
        string Fpath = Path.Combine(Configs.Shared.CookiesModule!, "../../plugins/Kill-Sound-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(Configs.Shared.CookiesModule!, "../../plugins/Kill-Sound-GoldKingZ/Cookies/Kill_Sound_Cookies.json");
        try
        {
            if (!Directory.Exists(Fpath))
            {
                Directory.CreateDirectory(Fpath);
            }

            if (!File.Exists(Fpathc))
            {
                File.WriteAllText(Fpathc, "[]");
            }

            List<PersonData> allPersonsData;
            string jsonData = File.ReadAllText(Fpathc);
            allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

            PersonData existingPerson = allPersonsData.Find(p => p.PlayerSteamID == PlayerSteamID)!;

            if (existingPerson != null)
            {
                if (existingPerson.freezemenu != freezemenu)
                    existingPerson.freezemenu = freezemenu;

                if (existingPerson.headshotkill != headshotkill)
                    existingPerson.headshotkill = headshotkill;

                if (existingPerson.headshothit != headshothit)
                    existingPerson.headshothit = headshothit;

                if (existingPerson.bodyshotkill != bodyshotkill)
                    existingPerson.bodyshotkill = bodyshotkill;

                if (existingPerson.bodyshothit != bodyshothit)
                    existingPerson.bodyshothit = bodyshothit;

                if (existingPerson.quakesounds != quakesounds)
                    existingPerson.quakesounds = quakesounds;

                if (existingPerson.quakehmessages != quakehmessages)
                    existingPerson.quakehmessages = quakehmessages;

                if (existingPerson.quakecmessages != quakecmessages)
                    existingPerson.quakecmessages = quakecmessages;

                existingPerson.DateAndTime = DateAndTime;
            }
            else
            {
                PersonData newPerson = new PersonData
                {
                    PlayerSteamID = PlayerSteamID,
                    freezemenu = freezemenu,
                    headshotkill = headshotkill,
                    headshothit = headshothit,
                    bodyshotkill = bodyshotkill,
                    bodyshothit = bodyshothit,
                    quakesounds = quakesounds,
                    quakehmessages = quakehmessages,
                    quakecmessages = quakecmessages,
                    DateAndTime = DateAndTime
                };
                allPersonsData.Add(newPerson);
            }

            
            allPersonsData.RemoveAll(p => (DateTime.Now - p.DateAndTime).TotalDays > Configs.GetConfigData().KS_AutoRemovePlayerCookieOlderThanXDays);
            
            

            string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
            try
            {
                File.WriteAllText(Fpathc, updatedJsonData);
            }
            catch
            {
                // Handle exception
            }
        }
        catch
        {
            // Handle exception
        }
    }

    public static PersonData RetrievePersonDataById(ulong targetId)
    {
        string Fpath = Path.Combine(Configs.Shared.CookiesModule!, "../../plugins/Kill-Sound-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(Configs.Shared.CookiesModule!, "../../plugins/Kill-Sound-GoldKingZ/Cookies/Kill_Sound_Cookies.json");
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                PersonData targetPerson = allPersonsData.Find(p => p.PlayerSteamID == targetId)!;

               
                if (targetPerson != null && (DateTime.Now - targetPerson.DateAndTime<= TimeSpan.FromDays(Configs.GetConfigData().KS_AutoRemovePlayerCookieOlderThanXDays)))
                {
                    return targetPerson;
                }
                else if (targetPerson != null)
                {
                    allPersonsData.Remove(targetPerson);
                    string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                    try
                    {
                        File.WriteAllText(Fpathc, updatedJsonData);
                    }
                    catch
                    {
                        // Handle exception
                    }
                }
                
                
            }
        }
        catch
        {
            // Handle exception
        }
        return new PersonData();
    }

    /* public void loaddefault()
    {
        try
        {
            string _json = Path.Combine(Configs.Shared.CookiesModule!, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
            var json = Helper.LoadJsonFromFile(_json);

            string kill_pattern = @"^Kill(_\d+)?$";
            string headshot_pattern = @"^HeadShot(_\d+)?$";

            Regex kill_regex = new Regex(kill_pattern);
            Regex headshot_regex = new Regex(headshot_pattern);

            string kill_matchingKey = json.Keys.FirstOrDefault(key => kill_regex.IsMatch(key))!;
            string headshot_matchingKey = json.Keys.FirstOrDefault(key => headshot_regex.IsMatch(key))!;

            if (kill_matchingKey != null)
            {
                Globals.Kill_Quake = "Kill_Quake";
            }
            if (headshot_matchingKey != null)
            {
                Globals.HeadShot_Quake = "HeadShot_Quake";
            }
        }catch{}
    } */
}