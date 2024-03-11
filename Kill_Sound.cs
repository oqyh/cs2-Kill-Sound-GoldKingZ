using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API;
using System.Text;
using Newtonsoft.Json;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Memory;

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
    public override string ModuleName => "Kill Sound (Sound On , Kill , HeadShot , Body , Menu)";
    public override string ModuleVersion => "1.0.4";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh/cs2-Kill-Sound";
    public KillSoundConfig Config { get; set; } = new KillSoundConfig();
    private Dictionary<ulong, bool> intromenuon = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> menuon = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> menuclose = new Dictionary<ulong, bool>();
    private Dictionary<ulong, int> currentIndexDict = new Dictionary<ulong, int>();
    private Dictionary<ulong, bool> buttonPressed = new Dictionary<ulong, bool>();

    public void OnConfigParsed(KillSoundConfig config)
    {
        Config = config;
    }
    
    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
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
            
            var playerid = player.SteamID;
            ///////////////////////////////////////////
            //Intro Menu (No needed just incase if you need it)
            if (intromenuon.ContainsKey(playerid))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(Localizer["Menu.Intro"]);//Intro menu
                var centerhtml = builder.ToString();
                player?.PrintToCenterHtml(centerhtml);
                Server.NextFrame(() =>
                {
                    AddTimer(3.0f, () =>
                    {
                        if (intromenuon.ContainsKey(playerid)) // after 3 secs check if he has intro menu remove
                        {
                            intromenuon.Remove(playerid);
                        }
                        if (!menuon.ContainsKey(playerid)) // if he doesnt have main menu give him
                        {
                            menuon.Add(playerid, true);
                        }
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                });
            }
            //Done Intro Menu 
            ///////////////////////////////////////////

            if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) continue;
            ///////////////////////////////////////////
            //Main Menu
            if (menuon.ContainsKey(playerid))
            {
                PersonData personData = RetrievePersonDataById((int)playerid); //load Cookies to personData
                DateTime personDate = DateTime.Now; ////load personData To track Inactive players RemovePlayerCookieOlderThanXDays 

                // BoolValue5 is Freeze On Menu if its on then we freeze him on menu on
                if(!personData.BoolValue5)
                {
                    if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                        player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_NONE;
                        Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 0);
                        Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                    }
                }
                ///////////////////////////////////////////
                //setup menu string and bool first
                //
                List<string> linesList = new List<string>();
                List<bool> boolValuesList = new List<bool>();

                linesList.Add(Localizer["Menu.Freeze"]);//name inside menu + by default i like to have it inside the menu
                boolValuesList.Add(personData.BoolValue5); //also bool for Freeze On Menu

                if (!string.IsNullOrEmpty(Config.HeadShotKillSoundPath)) //check server if he want Head Shot Kill Sound
                {
                    //if he want Head Shot Kill Sound we add string + bool to the list to use it after 
                    linesList.Add(Localizer["Menu.HeadShotKill"]); //name inside menu
                    boolValuesList.Add(personData.BoolValue1);
                }
                if (!string.IsNullOrEmpty(Config.HeadShotHitSoundPath))  //check server if he want Head Shot Hit Sound 
                {
                    //if he want Head Shot Hit Sound we add string + bool to the list to use it after 
                    linesList.Add(Localizer["Menu.HeadShotHit"]);//name inside menu
                    boolValuesList.Add(personData.BoolValue2);
                }
                if (!string.IsNullOrEmpty(Config.BodyKillSoundPath)) //check server if he want Body Kill Sound 
                {
                    //if he want Body Kill Sound we add string + bool to the list to use it after 
                    linesList.Add(Localizer["Menu.BodyKill"]);//name inside menu
                    boolValuesList.Add(personData.BoolValue3);
                }
                if (!string.IsNullOrEmpty(Config.BodyHitSoundPath)) //check server if he want Body Hit Sound 
                {
                    //if he want Body Hit Sound we add string + bool to the list to use it after 
                    linesList.Add(Localizer["Menu.BodyHit"]);//name inside menu
                    boolValuesList.Add(personData.BoolValue4);
                }

                string[] lines = linesList.ToArray();
                bool[] boolValues = boolValuesList.ToArray();
                //
                //Done Setup Menu
                ///////////////////////////////////////////

                ///////////////////////////////////////////
                //Player Pressing 
                if (player.Buttons == 0) //Check If Player Not pressing 
                {
                    buttonPressed[playerid] = false; //make buttonPressed to false that means ready for next index (anti Spam)
                }
                else if (player.Buttons == PlayerButtons.Back && !buttonPressed[playerid])//player going back (S) + buttonPressed false (anti Spam)
                {
                    currentIndexDict[playerid] = (currentIndexDict[playerid] == lines.Length - 1) ? 0 : currentIndexDict[playerid] + 1; //add index +1
                    buttonPressed[playerid] = true; //done pressing give him buttonPressed true (anti Spam)
                    player.ExecuteClientCommand("play sounds/ui/csgo_ui_page_scroll.vsnd_c"); // play sound
                }
                else if (player.Buttons == PlayerButtons.Forward && !buttonPressed[playerid])//player going Forward (W) + buttonPressed false (anti Spam)
                {
                    currentIndexDict[playerid] = (currentIndexDict[playerid] == 0) ? lines.Length - 1 : currentIndexDict[playerid] - 1; //subtract index -1
                    buttonPressed[playerid] = true; //done pressing give him buttonPressed true (anti Spam)
                    player.ExecuteClientCommand("play sounds/ui/csgo_ui_page_scroll.vsnd_c"); // play sound
                }else if ((player.Buttons == PlayerButtons.Moveleft || player.Buttons == PlayerButtons.Moveright) && !buttonPressed[playerid])//player going Left (A) OR Right (D) + buttonPressed false (anti Spam)
                {
                    int currentLineIndex = currentIndexDict[playerid]; //Get CurrentIndex
                    
                    string currentLineName = lines[currentLineIndex]; //Convert CurrentIndex From int to string (better than int)
                    if (currentLineName == Localizer["Menu.Freeze"]) //if CurrentIndex equals Freeze On Menu and press Right or Left 
                    {
                        personData.BoolValue1 = personData.BoolValue1;
                        personData.BoolValue2 = personData.BoolValue2;
                        personData.BoolValue3 = personData.BoolValue3;
                        personData.BoolValue4 = personData.BoolValue4;
                        personData.BoolValue5 = !personData.BoolValue5; //this bool for Freeze On Menu oppsite (on / off)
                        if(!personData.BoolValue5)//if true means off walk (i made oppsite to make less cookies)
                        {
                            if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                            player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_NONE;
                            Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 0);
                            Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                            }

                            if (!string.IsNullOrEmpty(Localizer["Chat.FreezeOn"])) //check first if server  want chat empty means skip Chat message 
                            {
                                player.PrintToChat(Localizer["Chat.FreezeOn"]);
                            }
                        }else //if false means on Freeze
                        {
                            if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                            player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
                            Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 2);
                            Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                            }

                            if (!string.IsNullOrEmpty(Localizer["Chat.FreezeOff"]))
                            {
                                player.PrintToChat(Localizer["Chat.FreezeOff"]);
                            }
                        }
                        SaveToJsonFile((int)playerid, personData.BoolValue1, personData.BoolValue2, personData.BoolValue3, personData.BoolValue4, personData.BoolValue5, personDate);//save values in cookies
                    }else if (currentLineName == Localizer["Menu.HeadShotKill"])//The reset same just oppsite + save values in cookies
                    {
                        personData.BoolValue1 = !personData.BoolValue1;//this bool for Head Shot Kill Sound Menu oppsite (on / off)
                        personData.BoolValue2 = personData.BoolValue2;
                        personData.BoolValue3 = personData.BoolValue3;
                        personData.BoolValue4 = personData.BoolValue4;
                        personData.BoolValue5 = personData.BoolValue5;

                        if(!personData.BoolValue1)
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
                        
                        SaveToJsonFile((int)playerid, personData.BoolValue1, personData.BoolValue2, personData.BoolValue3, personData.BoolValue4, personData.BoolValue5, personDate);
                    }else if (currentLineName == Localizer["Menu.HeadShotHit"])
                    {
                        personData.BoolValue1 = personData.BoolValue1;
                        personData.BoolValue2 = !personData.BoolValue2;//this bool for Head Shot Hit Sound Menu oppsite (on / off)
                        personData.BoolValue3 = personData.BoolValue3;
                        personData.BoolValue4 = personData.BoolValue4;
                        personData.BoolValue5 = personData.BoolValue5;
                        if(!personData.BoolValue2)
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
                        SaveToJsonFile((int)playerid, personData.BoolValue1, personData.BoolValue2, personData.BoolValue3, personData.BoolValue4, personData.BoolValue5, personDate);
                    }else if (currentLineName == Localizer["Menu.BodyKill"])
                    {
                        personData.BoolValue1 = personData.BoolValue1;
                        personData.BoolValue2 = personData.BoolValue2;
                        personData.BoolValue3 = !personData.BoolValue3;//this bool for Body Kill Sound Menu oppsite (on / off)
                        personData.BoolValue4 = personData.BoolValue4;
                        personData.BoolValue5 = personData.BoolValue5;
                        if(!personData.BoolValue3)
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
                        SaveToJsonFile((int)playerid, personData.BoolValue1, personData.BoolValue2, personData.BoolValue3, personData.BoolValue4, personData.BoolValue5, personDate);
                    }else if (currentLineName == Localizer["Menu.BodyHit"])
                    {
                        personData.BoolValue1 = personData.BoolValue1;
                        personData.BoolValue2 = personData.BoolValue2;
                        personData.BoolValue3 = personData.BoolValue3;
                        personData.BoolValue4 = !personData.BoolValue4;//this bool for Body Hit Sound Menu oppsite (on / off)
                        personData.BoolValue5 = personData.BoolValue5;
                        if(!personData.BoolValue4)
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
                        SaveToJsonFile((int)playerid, personData.BoolValue1, personData.BoolValue2, personData.BoolValue3, personData.BoolValue4, personData.BoolValue5, personDate);
                    }
                    buttonPressed[playerid] = true;//done pressing give him buttonPressed true (anti Spam)
                    player.ExecuteClientCommand("play sounds/ui/item_sticker_select.vsnd_c"); // play sound
                }
                else if ((long)player.Buttons == 8589934592) //If press Tab
                {
                    if(!personData.BoolValue5) //if he choose menu open freeze we remove freeze because we exting
                    {
                        if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                        player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
                        Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 2);
                        Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                        }
                    }
                    if (!menuclose.ContainsKey(playerid)) //give outro 
                    {
                        menuclose.Add(playerid, true);
                    }

                    if (menuon.ContainsKey(playerid)) // close main menu open
                    {
                        menuon.Remove(playerid);
                    }

                }
                //Done Player Pressing
                ///////////////////////////////////////////

                ///////////////////////////////////////////
                //Show Menu With Values
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < lines.Length; i++)
                {
                    
                    if (i == currentIndexDict[playerid]) // The One Selected
                    {
                        bool status = (i >= 0 && i < boolValues.Length) ? boolValues[i] : false; //check on or is it off
                        string Imageleft = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1192095755209547856/output-onlinegiftools_9.gif?ex=65fae330&is=65e86e30&hm=413af124a7bdc4f2f65e00217f391f443f75ebd0f02bbffd14b5fdcc15b33936&' class=''>"; //picture of left side
                        string ImageRight = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1192096520279965877/output-onlinegiftools_10.gif?ex=65fae3e6&is=65e86ee6&hm=fc9d6e36506eff89e9cba87c817e61a1c5964516f86b9a3bf163c8547077f434&' class=''>"; //picture of Right side
                        string lineHtml = $"<font color='orange'>{Imageleft} {lines[i]} : {(status ? "<font color='red'>Off</font>" : "<font color='lime'>On</font>")} {ImageRight}</font><br>"; //Overall Selected look like
                        builder.AppendLine(lineHtml);
                    }
                    else // Rest of Lines Make it  white Color
                    {
                        builder.AppendLine($"<font color='white'>{lines[i]}</font><br>");
                    }
                }
                
                builder.AppendLine(Localizer["Menu.Bottom"]);//bottom menu
                builder.AppendLine("</div>");
                var centerhtml = builder.ToString();
                player?.PrintToCenterHtml(centerhtml);
                //Done Show Menu With Values
                ///////////////////////////////////////////

            }
            //Done Main Menu
            ///////////////////////////////////////////

            ///////////////////////////////////////////
            //Outro Menu (No needed just incase if you need it)
            if (menuclose.ContainsKey(playerid))// Outro
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Localizer["Menu.Outro"]);
                var centerhtml = builder.ToString();
                player?.PrintToCenterHtml(centerhtml);
                Server.NextFrame(() =>
                {
                    AddTimer(3.0f, () =>
                    {
                        if (menuclose.ContainsKey(playerid)) // check if player has outro remove 
                        {
                            menuclose.Remove(playerid);
                        }
                        if (buttonPressed.ContainsKey(playerid)) // check if player has buttonPressed remove
                        {
                            buttonPressed.Remove(playerid);
                        }
                        if (menuon.ContainsKey(playerid)) // check if player menu open remove
                        {
                            menuon.Remove(playerid);
                        }
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                });
            }
            //Done Outro Menu 
            ///////////////////////////////////////////
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
        
        if (!string.IsNullOrEmpty(Config.SoundDisableCommandsMenu) && disableCommandsmenu.Any(cmd => cmd.Equals(trimmedMessage, StringComparison.OrdinalIgnoreCase))) //Menu Start Here
        {

            if (!intromenuon.ContainsKey(playerid))//check if not have intro (we going intro > Main Menu then stop if exit > outro > Exit)
            {
                intromenuon.Add(playerid, true);
            }
            if (!currentIndexDict.ContainsKey(playerid)) //check if he has no index start then we start with 0 index in menu 
            {
                currentIndexDict.Add(playerid, 0);
            }
            if (!buttonPressed.ContainsKey(playerid)) //track anti spam
            {
                buttonPressed.Add(playerid, false);
            }
        }

        if (!string.IsNullOrEmpty(Config.SoundDisableCommands) && disableCommands.Any(cmd => cmd.Equals(trimmedMessage, StringComparison.OrdinalIgnoreCase))) //shortcut stop all sounds with commands Config.SoundDisableCommands
        {
            DateTime personDate = DateTime.Now;
            bool boolValue1 = personData.BoolValue1;
            bool boolValue2 = personData.BoolValue2;
            bool boolValue3 = personData.BoolValue3;
            bool boolValue4 = personData.BoolValue4;
            bool boolValue5 = personData.BoolValue5;//we skip freeze menu boolean

            boolValue1 = true;//head shot kill 
            boolValue2 = true;//head shot Hit 
            boolValue3 = true;//Body kill 
            boolValue4 = true;//Body Hit 

            if (!string.IsNullOrEmpty(Localizer["Chat.AllSoundsOff"]))
            {
                player.PrintToChat(Localizer["Chat.AllSoundsOff"]);
            }

            SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, boolValue5, personDate);
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
            if (!menuon.ContainsKey(playerid))
            {
                menuon.Add(playerid, true);
            }
            if (!currentIndexDict.ContainsKey(playerid))
            {
                currentIndexDict.Add(playerid, 0);
            }
            if (!buttonPressed.ContainsKey(playerid))
            {
                buttonPressed.Add(playerid, false);
            }
        }

        if (!string.IsNullOrEmpty(Config.SoundDisableCommands) && disableCommands.Any(cmd => cmd.Equals(trimmedMessage, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            bool boolValue1 = personData.BoolValue1;
            bool boolValue2 = personData.BoolValue2;
            bool boolValue3 = personData.BoolValue3;
            bool boolValue4 = personData.BoolValue4;
            bool boolValue5 = personData.BoolValue5;

            boolValue1 = true;
            boolValue2 = true;
            boolValue3 = true;
            boolValue4 = true;

            if (!string.IsNullOrEmpty(Localizer["Chat.AllSoundsOff"]))
            {
                player.PrintToChat(Localizer["Chat.AllSoundsOff"]);
            }

            SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, boolValue5, personDate);
        }


        if (menuon.ContainsKey(playerid))
        {
            if(!string.IsNullOrEmpty(Config.HeadShotKillSoundPath) && trimmedMessage.Contains("!1"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = !personData.BoolValue1;
                bool boolValue2 = personData.BoolValue2;
                bool boolValue3 = personData.BoolValue3;
                bool boolValue4 = personData.BoolValue4;
                bool boolValue5 = personData.BoolValue5;
                
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, boolValue5, personDate);
            }
            if(!string.IsNullOrEmpty(Config.HeadShotHitSoundPath) && trimmedMessage.Contains("!2"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = personData.BoolValue1;
                bool boolValue2 = !personData.BoolValue2;
                bool boolValue3 = personData.BoolValue3;
                bool boolValue4 = personData.BoolValue4;
                bool boolValue5 = personData.BoolValue5;
                
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, boolValue5, personDate);
            }
            if(!string.IsNullOrEmpty(Config.BodyKillSoundPath) && trimmedMessage.Contains("!3"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = personData.BoolValue1;
                bool boolValue2 = personData.BoolValue2;
                bool boolValue3 = !personData.BoolValue3;
                bool boolValue4 = personData.BoolValue4;
                bool boolValue5 = personData.BoolValue5;
                
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, boolValue5, personDate);
            }
            if(!string.IsNullOrEmpty(Config.BodyHitSoundPath) && trimmedMessage.Contains("!4"))
            {
                DateTime personDate = DateTime.Now;
                bool boolValue1 = personData.BoolValue1;
                bool boolValue2 = personData.BoolValue2;
                bool boolValue3 = personData.BoolValue3;
                bool boolValue4 = !personData.BoolValue4;
                bool boolValue5 = personData.BoolValue5;
                
                SaveToJsonFile((int)playerid, boolValue1, boolValue2, boolValue3, boolValue4, boolValue5, personDate);
            }
            
            if(trimmedMessage.Contains("!5"))
            {
                menuon.Remove(playerid);
            }
        }
        
        return HookResult.Continue;
    }

    //Death Event (Kill HeadShot Or Body Shot)
    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        var attacker = @event.Attacker;
        var victim = @event.Userid;
        if (attacker == null || victim == null || !attacker.IsValid || !victim.IsValid || attacker.IsBot || attacker.IsHLTV)return HookResult.Continue;
        var attackerteam = attacker.TeamNum;
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
    //Hurt Event (Hit HeadShot Or Body Shot)
    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        var attacker = @event.Attacker;
        var victim = @event.Userid;
        if (attacker == null || victim == null || !attacker.IsValid || !victim.IsValid || attacker.IsBot || attacker.IsHLTV)return HookResult.Continue;
        var attackerteam = attacker.TeamNum;
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
    
    ///////////////////////////////////////////////////////////
    //Cookies
    private class PersonData
    {
        public int Id { get; set; }
        public bool BoolValue1 { get; set; }
        public bool BoolValue2 { get; set; }
        public bool BoolValue3 { get; set; }
        public bool BoolValue4 { get; set; }
        public bool BoolValue5 { get; set; }
        public DateTime Date { get; set; }
    }

    private void SaveToJsonFile(int id, bool boolValue1, bool boolValue2, bool boolValue3, bool boolValue4, bool BoolValue5, DateTime date)
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
                if (existingPerson.BoolValue5 != BoolValue5)
                    existingPerson.BoolValue5 = BoolValue5;

                existingPerson.Date = date;
            }
            else
            {
                PersonData newPerson = new PersonData { Id = id, BoolValue1 = boolValue1, BoolValue2 = boolValue2, BoolValue3 = boolValue3, BoolValue4 = boolValue4, BoolValue5 = BoolValue5, Date = date };
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
    ///Done Cookies
    ///////////////////////////////////////////////////////////
    
    ///////////////////////////////////////////////////////////
    //Clean Up Dictionary List if Disconnect Or Plugin Unload Or MapEnd
    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        var player = @event.Userid;
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return HookResult.Continue;
        var playerid = player.SteamID;

        if (intromenuon.ContainsKey(playerid))
        {
            intromenuon.Remove(playerid);
        }

        if (menuon.ContainsKey(playerid))
        {
            menuon.Remove(playerid);
        }

        if (menuclose.ContainsKey(playerid))
        {
            menuclose.Remove(playerid);
        }

        if (currentIndexDict.ContainsKey(playerid))
        {
            currentIndexDict.Remove(playerid);
        }

        if (buttonPressed.ContainsKey(playerid))
        {
            buttonPressed.Remove(playerid);
        }
        return HookResult.Continue;
    }
    public override void Unload(bool hotReload)
    {
        intromenuon.Clear();
        menuon.Clear();
        menuclose.Clear();
        currentIndexDict.Clear();
        buttonPressed.Clear();
    }
    private void OnMapEnd()
    {
        intromenuon.Clear();
        menuon.Clear();
        menuclose.Clear();
        currentIndexDict.Clear();
        buttonPressed.Clear();
    }
    //Done Clean Up Dictionary List 
    ///////////////////////////////////////////////////////////
    
}