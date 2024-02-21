using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API;
using System.Text;
using Newtonsoft.Json;

namespace Kill_Sound;

[MinimumApiVersion(164)]
public class KillSoundConfig : BasePluginConfig
{
    [JsonPropertyName("HeadShotKillSoundPath")] public string HeadShotKillSoundPath { get; set; } = "sounds/training/bell_normal.vsnd_c";
    [JsonPropertyName("BodyKillSoundPath")] public string BodyKillSoundPath { get; set; } = "sounds/training/timer_bell.vsnd_c";
    

    [JsonPropertyName("HeadShotHitSoundPath")] public string HeadShotHitSoundPath { get; set; } = "sounds/training/bell_impact.vsnd_c";
    [JsonPropertyName("BodyHitSoundPath")] public string BodyHitSoundPath { get; set; } = "sounds/training/timer_bell.vsnd_c";

    [JsonPropertyName("SoundDisableCommandsMenu")] public string SoundDisableCommandsMenu { get; set; } = "!soundmenu,!soundsmenu,!soundsetting,!soundsettings";
    [JsonPropertyName("SoundDisableCommands")] public string SoundDisableCommands { get; set; } = "!stopsound,!stopsounds";
    [JsonPropertyName("RemovePlayerCookieOlderThanXDays")] public int RemovePlayerCookieOlderThanXDays { get; set; } = 7;
}

public class KillSound : BasePlugin, IPluginConfig<KillSoundConfig>
{
    public override string ModuleName => "Kill Sound";
    public override string ModuleVersion => "1.0.2";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Sound On , Kill , HeadShot , Body";
    public KillSoundConfig Config { get; set; } = new KillSoundConfig();
    private Dictionary<int, bool> menuon = new Dictionary<int, bool>();
    public void OnConfigParsed(KillSoundConfig config)
    {
        Config = config;
    }
    
    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
        AddCommandListener("say", OnPlayerSayPublic, HookMode.Post);
        AddCommandListener("say_team", OnPlayerSayTeam, HookMode.Post);
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
    }
    public void OnTick()
    {
        var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
        foreach (var player in playerEntities)
        {
            if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) continue;
            
            if (menuon.ContainsKey(player.UserId!.Value))
            {
                var playerid = player.SteamID;
                string Close = "</font>";
                string Red = "<font color='Red'>";
                string Cyan = "<font color='cyan'>";
                string Blue = "<font color='blue'>";
                string DarkBlue = "<font color='darkblue'>";
                string LightBlue = "<font color='lightblue'>";
                string Purple = "<font color='purple'>";
                string Yellow = "<font color='yellow'>";
                string Lime = "<font color='lime'>";
                string Magenta = "<font color='magenta'>";
                string Pink = "<font color='pink'>";
                string Grey = "<font color='grey'>";
                string Green = "<font color='green'>";
                string Orange = "<font color='orange'>";
                
                PersonData personData = RetrievePersonDataById((int)playerid);
                StringBuilder builder = new StringBuilder();
                bool test = personData.BoolValue3;
                if (!string.IsNullOrEmpty(Config.HeadShotKillSoundPath))
                {
                    if(personData.BoolValue1)
                    {
                        if (!string.IsNullOrEmpty(Localizer["Menu.HeadShotKillOff"]))
                        {
                            builder.AppendFormat(Localizer["Menu.HeadShotKillOff"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange);
                            builder.AppendLine("<br>");
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Menu.HeadShotKillOn"]))
                        {
                            builder.AppendFormat(Localizer["Menu.HeadShotKillOn"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange);
                            builder.AppendLine("<br>");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Config.HeadShotHitSoundPath))
                {
                    if(personData.BoolValue2)
                    {
                        if (!string.IsNullOrEmpty(Localizer["Menu.HeadShotHitOff"]))
                        {
                            builder.AppendFormat(Localizer["Menu.HeadShotHitOff"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange);
                            builder.AppendLine("<br>");
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Menu.HeadShotHitOn"]))
                        {
                            builder.AppendFormat(Localizer["Menu.HeadShotHitOn"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange);
                            builder.AppendLine("<br>");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Config.BodyKillSoundPath))
                {
                    if(personData.BoolValue3)
                    {
                        if (!string.IsNullOrEmpty(Localizer["Menu.BodyKillOff"]))
                        {
                            builder.AppendFormat(Localizer["Menu.BodyKillOff"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange);
                            builder.AppendLine("<br>");
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Menu.BodyKillOn"]))
                        {
                            builder.AppendFormat(Localizer["Menu.BodyKillOn"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange);
                            builder.AppendLine("<br>");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Config.BodyHitSoundPath))
                {
                    if(personData.BoolValue4)
                    {
                        if (!string.IsNullOrEmpty(Localizer["Menu.BodyHitOff"]))
                        {
                            builder.AppendFormat(Localizer["Menu.BodyHitOff"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange);
                            builder.AppendLine("<br>");
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Menu.BodyHitOn"]))
                        {
                            builder.AppendFormat(Localizer["Menu.BodyHitOn"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange);
                            builder.AppendLine("<br>");
                        }
                        
                    }
                }
                builder.AppendFormat(Localizer["Menu.Exit"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange);
                var centerhtml = builder.ToString();
                player?.PrintToCenterHtml(centerhtml);
            }
        }
    }
    private HookResult OnPlayerSayPublic(CCSPlayerController? player, CommandInfo info)
	{
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return HookResult.Continue;
        string Fpath = Path.Combine(ModuleDirectory,"../../plugins/Kill_Sound/");
        string Fpathc = Path.Combine(ModuleDirectory,"../../plugins/Kill_Sound/Newtonsoft.Json.dll");
        if(Directory.Exists(Fpath))
        {

            if (!File.Exists(Fpathc))
            {
                Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
                Console.WriteLine("[Kill_Sound] Could Not Found Newtonsoft.Json.dll inside (plugins/Kill_Sound/)");
                Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
            }
        }
        var playerid = player.SteamID;
        var message = info.GetArg(1);
        if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
        string trimmedMessage1 = message.TrimStart();
        string trimmedMessage = trimmedMessage1.TrimEnd();
        PersonData personData = RetrievePersonDataById((int)playerid);
        
        string[] disableCommandsmenu = Config.SoundDisableCommandsMenu.Split(',');
        string[] disableCommands = Config.SoundDisableCommands.Split(',');
        
        if (!string.IsNullOrEmpty(Config.SoundDisableCommandsMenu) && disableCommandsmenu.Any(cmd => cmd.Equals(trimmedMessage, StringComparison.OrdinalIgnoreCase)))
        {
            if (menuon.ContainsKey(player.UserId!.Value))return HookResult.Continue;
            menuon.Add(player.UserId.Value, true);
        }

        if (!string.IsNullOrEmpty(Config.SoundDisableCommands) && disableCommands.Any(cmd => cmd.Equals(trimmedMessage, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            bool boolValue1 = personData.BoolValue1;
            bool boolValue2 = personData.BoolValue2;
            bool boolValue3 = personData.BoolValue3;
            bool boolValue4 = personData.BoolValue4;

            boolValue1 = true;
            boolValue2 = true;
            boolValue3 = true;
            boolValue4 = true;

            if (!string.IsNullOrEmpty(Localizer["Chat.AllSoundsOff"]))
            {
                player.PrintToChat(Localizer["Chat.AllSoundsOff"]);
            }

            SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
        }


        if (menuon.ContainsKey(player.UserId!.Value))
        {
            if(!string.IsNullOrEmpty(Config.HeadShotKillSoundPath) && trimmedMessage.Contains("!1"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = !personData.BoolValue1;
                bool boolValue2 = personData.BoolValue2;
                bool boolValue3 = personData.BoolValue3;
                bool boolValue4 = personData.BoolValue4;
                if(personData.BoolValue1)
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.HeadShotKillOn"]))
                    {
                        player.PrintToChat(Localizer["Chat.HeadShotKillOn"]);
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.HeadShotKillOff"]))
                    {
                        player.PrintToChat(Localizer["Chat.HeadShotKillOff"]);
                    }
                }
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
            }
            if(!string.IsNullOrEmpty(Config.HeadShotHitSoundPath) && trimmedMessage.Contains("!2"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = personData.BoolValue1;
                bool boolValue2 = !personData.BoolValue2;
                bool boolValue3 = personData.BoolValue3;
                bool boolValue4 = personData.BoolValue4;
                if(personData.BoolValue2)
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.HeadShotHitOn"]))
                    {
                        player.PrintToChat(Localizer["Chat.HeadShotHitOn"]);
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.HeadShotHitOff"]))
                    {
                        player.PrintToChat(Localizer["Chat.HeadShotHitOff"]);
                    }
                }
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
            }
            if(!string.IsNullOrEmpty(Config.BodyKillSoundPath) && trimmedMessage.Contains("!3"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = personData.BoolValue1;
                bool boolValue2 = personData.BoolValue2;
                bool boolValue3 = !personData.BoolValue3;
                bool boolValue4 = personData.BoolValue4;
                if(personData.BoolValue3)
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.BodyKillOn"]))
                    {
                        player.PrintToChat(Localizer["Chat.BodyKillOn"]);
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.BodyKillOff"]))
                    {
                        player.PrintToChat(Localizer["Chat.BodyKillOff"]);
                    }
                }
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
            }
            if(!string.IsNullOrEmpty(Config.BodyHitSoundPath) && trimmedMessage.Contains("!4"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = personData.BoolValue1;
                bool boolValue2 = personData.BoolValue2;
                bool boolValue3 = personData.BoolValue3;
                bool boolValue4 = !personData.BoolValue4;
                if(personData.BoolValue4)
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.BodyHitOn"]))
                    {
                        player.PrintToChat(Localizer["Chat.BodyHitOn"]);
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.BodyHitOff"]))
                    {
                        player.PrintToChat(Localizer["Chat.BodyHitOff"]);
                    }
                }
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
            }
            
            if(trimmedMessage.Contains("!5"))
            {
                menuon.Remove(player.UserId.Value);
            }
        }
        
        return HookResult.Continue;
    }
    private HookResult OnPlayerSayTeam(CCSPlayerController? player, CommandInfo info)
	{
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return HookResult.Continue;
        string Fpath = Path.Combine(ModuleDirectory,"../../plugins/Kill_Sound/");
        string Fpathc = Path.Combine(ModuleDirectory,"../../plugins/Kill_Sound/Newtonsoft.Json.dll");
        if(Directory.Exists(Fpath))
        {

            if (!File.Exists(Fpathc))
            {
                Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
                Console.WriteLine("[Kill_Sound] Could Not Found Newtonsoft.Json.dll inside (plugins/Kill_Sound/)");
                Console.WriteLine("|||||||||||||||||||||||||||||| E R R O R ||||||||||||||||||||||||||||||");
            }
        }
        var playerid = player.SteamID;
        var message = info.GetArg(1);
        if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
        string trimmedMessage1 = message.TrimStart();
        string trimmedMessage = trimmedMessage1.TrimEnd();
        PersonData personData = RetrievePersonDataById((int)playerid);
        
        string[] disableCommandsmenu = Config.SoundDisableCommandsMenu.Split(',');
        string[] disableCommands = Config.SoundDisableCommands.Split(',');
        
        if (!string.IsNullOrEmpty(Config.SoundDisableCommandsMenu) && disableCommandsmenu.Any(cmd => cmd.Equals(trimmedMessage, StringComparison.OrdinalIgnoreCase)))
        {
            if (menuon.ContainsKey(player.UserId!.Value))return HookResult.Continue;
            menuon.Add(player.UserId.Value, true);
        }

        if (!string.IsNullOrEmpty(Config.SoundDisableCommands) && disableCommands.Any(cmd => cmd.Equals(trimmedMessage, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            bool boolValue1 = personData.BoolValue1;
            bool boolValue2 = personData.BoolValue2;
            bool boolValue3 = personData.BoolValue3;
            bool boolValue4 = personData.BoolValue4;

            boolValue1 = true;
            boolValue2 = true;
            boolValue3 = true;
            boolValue4 = true;

            if (!string.IsNullOrEmpty(Localizer["Chat.AllSoundsOff"]))
            {
                player.PrintToChat(Localizer["Chat.AllSoundsOff"]);
            }

            SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
        }


        if (menuon.ContainsKey(player.UserId!.Value))
        {
            if(!string.IsNullOrEmpty(Config.HeadShotKillSoundPath) && trimmedMessage.Contains("!1"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = !personData.BoolValue1;
                bool boolValue2 = personData.BoolValue2;
                bool boolValue3 = personData.BoolValue3;
                bool boolValue4 = personData.BoolValue4;
                if(personData.BoolValue1)
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.HeadShotKillOn"]))
                    {
                        player.PrintToChat(Localizer["Chat.HeadShotKillOn"]);
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.HeadShotKillOff"]))
                    {
                        player.PrintToChat(Localizer["Chat.HeadShotKillOff"]);
                    }
                }
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
            }
            if(!string.IsNullOrEmpty(Config.HeadShotHitSoundPath) && trimmedMessage.Contains("!2"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = personData.BoolValue1;
                bool boolValue2 = !personData.BoolValue2;
                bool boolValue3 = personData.BoolValue3;
                bool boolValue4 = personData.BoolValue4;
                if(personData.BoolValue2)
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.HeadShotHitOn"]))
                    {
                        player.PrintToChat(Localizer["Chat.HeadShotHitOn"]);
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.HeadShotHitOff"]))
                    {
                        player.PrintToChat(Localizer["Chat.HeadShotHitOff"]);
                    }
                }
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
            }
            if(!string.IsNullOrEmpty(Config.BodyKillSoundPath) && trimmedMessage.Contains("!3"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = personData.BoolValue1;
                bool boolValue2 = personData.BoolValue2;
                bool boolValue3 = !personData.BoolValue3;
                bool boolValue4 = personData.BoolValue4;
                if(personData.BoolValue3)
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.BodyKillOn"]))
                    {
                        player.PrintToChat(Localizer["Chat.BodyKillOn"]);
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.BodyKillOff"]))
                    {
                        player.PrintToChat(Localizer["Chat.BodyKillOff"]);
                    }
                }
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
            }
            if(!string.IsNullOrEmpty(Config.BodyHitSoundPath) && trimmedMessage.Contains("!4"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = personData.BoolValue1;
                bool boolValue2 = personData.BoolValue2;
                bool boolValue3 = personData.BoolValue3;
                bool boolValue4 = !personData.BoolValue4;
                if(personData.BoolValue4)
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.BodyHitOn"]))
                    {
                        player.PrintToChat(Localizer["Chat.BodyHitOn"]);
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(Localizer["Chat.BodyHitOff"]))
                    {
                        player.PrintToChat(Localizer["Chat.BodyHitOff"]);
                    }
                }
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, personDate);
            }
            
            if(trimmedMessage.Contains("!5"))
            {
                menuon.Remove(player.UserId.Value);
            }
        }
        
        return HookResult.Continue;
    }
    
    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo _)
    {
        var attacker = @event.Attacker;
        var attackerteam = attacker.TeamNum;
        var victim = @event.Userid;
        var victimteam = victim.TeamNum;
        var headshot = @event.Headshot;
        var playerid = attacker.SteamID;
        PersonData personData = RetrievePersonDataById((int)playerid);

        if (attacker.IsValid && victim.IsValid && (attacker != victim) && !attacker.IsBot)
        {
            if (headshot)
            {
                if (!string.IsNullOrEmpty(Config.HeadShotKillSoundPath))
                {
                    if (personData.BoolValue1)
                    {
                        //SKIP SOUNDS
                    }else
                    {
                        if(ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false)
                        {
                            if(attackerteam != victimteam)
                            {
                                attacker.ExecuteClientCommand("play " + Config.HeadShotKillSoundPath);
                            }
                        }else
                        {
                            attacker.ExecuteClientCommand("play " + Config.HeadShotKillSoundPath);
                        }
                        
                    }
                    
                }
            }else
            {
                if (!string.IsNullOrEmpty(Config.BodyKillSoundPath))
                {
                   
                    if (personData.BoolValue3)
                    {
                        //SKIP SOUNDS
                    }else
                    {
                        if(ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false)
                        {
                            if(attackerteam != victimteam)
                            {
                                attacker.ExecuteClientCommand("play " + Config.BodyKillSoundPath);
                            }
                        }else
                        {
                            attacker.ExecuteClientCommand("play " + Config.BodyKillSoundPath);
                        }
                        
                    }
                }
            }
        }
        return HookResult.Continue;
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo _)
    {
        var attacker = @event.Attacker;
        var attackerteam = attacker.TeamNum;
        var victim = @event.Userid;
        var victimteam = victim.TeamNum;
        var hitgroup = @event.Hitgroup;
        var playerid = attacker.SteamID;
        PersonData personData = RetrievePersonDataById((int)playerid);

        if (attacker.IsValid && victim.IsValid && (attacker != victim) && !attacker.IsBot)
        {
            if (hitgroup == 1)
            {
                if (!string.IsNullOrEmpty(Config.HeadShotHitSoundPath))
                {
                    if (personData.BoolValue2)
                    {
                        //SKIP SOUNDS
                    }else
                    {
                        if(ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false)
                        {
                            if(attackerteam != victimteam)
                            {
                                attacker.ExecuteClientCommand("play " + Config.HeadShotHitSoundPath);
                            }
                        }else
                        {
                            attacker.ExecuteClientCommand("play " + Config.HeadShotHitSoundPath);
                        }
                    }
                }
            }else
            {
                if (!string.IsNullOrEmpty(Config.BodyHitSoundPath))
                {
                    
                    if (personData.BoolValue4)
                    {
                        
                    }else
                    {
                        if(ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false)
                        {
                            if(attackerteam != victimteam)
                            {
                                attacker.ExecuteClientCommand("play " + Config.BodyHitSoundPath);
                            }
                        }else
                        {
                            attacker.ExecuteClientCommand("play " + Config.BodyHitSoundPath);
                        }
                    }
                }
            }
        }
        return HookResult.Continue;
    }
    private class PersonData
    {
        public int Id { get; set; }
        public bool BoolValue1 { get; set; }
        public bool BoolValue2 { get; set; }
        public bool BoolValue3 { get; set; }
        public bool BoolValue4 { get; set; }
        public DateTime Date { get; set; }
    }

    private void SaveToJsonFile(int id, bool boolValue1, bool boolValue2, bool boolValue3, bool boolValue4, DateTime date)
    {
        string Fpath = Path.Combine(ModuleDirectory, "../../plugins/Kill_Sound/Cookies/");
        string Fpathc = Path.Combine(ModuleDirectory, "../../plugins/Kill_Sound/Cookies/Kill_Sound_Cookies.json");
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

            PersonData existingPerson = allPersonsData.Find(p => p.Id == id)!;

            if (existingPerson != null)
            {
                if (existingPerson.BoolValue1 != boolValue1)
                    existingPerson.BoolValue1 = boolValue1;
                if (existingPerson.BoolValue2 != boolValue2)
                    existingPerson.BoolValue2 = boolValue2;
                if (existingPerson.BoolValue3 != boolValue3)
                    existingPerson.BoolValue3 = boolValue3;
                if (existingPerson.BoolValue4 != boolValue4)
                    existingPerson.BoolValue4 = boolValue4;

                existingPerson.Date = date;
            }
            else
            {
                PersonData newPerson = new PersonData { Id = id, BoolValue1 = boolValue1, BoolValue2 = boolValue2, BoolValue3 = boolValue3, BoolValue4 = boolValue4, Date = date };
                allPersonsData.Add(newPerson);
            }
            allPersonsData.RemoveAll(p => (DateTime.Now - p.Date).TotalDays > Config.RemovePlayerCookieOlderThanXDays);

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

    private PersonData RetrievePersonDataById(int targetId)
    {
        string Fpath = Path.Combine(ModuleDirectory, "../../plugins/Kill_Sound/Cookies/");
        string Fpathc = Path.Combine(ModuleDirectory, "../../plugins/Kill_Sound/Cookies/Kill_Sound_Cookies.json");
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                PersonData targetPerson = allPersonsData.Find(p => p.Id == targetId)!;

                if (targetPerson != null && DateTime.Now - targetPerson.Date <= TimeSpan.FromDays(Config.RemovePlayerCookieOlderThanXDays))
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
                    }
                }
            }
        }
        catch
        {
        }
        return new PersonData();
    }
    public override void Unload(bool hotReload)
    {
        menuon.Clear();
    }
    private void OnMapEnd()
    {
        menuon.Clear();
    }
}