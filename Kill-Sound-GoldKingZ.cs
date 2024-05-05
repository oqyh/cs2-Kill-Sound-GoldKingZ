using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API;
using System.Text;
using CounterStrikeSharp.API.Modules.Memory;
using Kill_Sound_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using MySqlConnector;

namespace Kill_Sound_GoldKingZ;

[MinimumApiVersion(164)]

public class KillSoundGoldKingZ : BasePlugin
{
    public override string ModuleName => "Kill Sound ( Kill , HeadShot , Quake )";
    public override string ModuleVersion => "1.1.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    internal static IStringLocalizer? Stringlocalizer;
    private CounterStrikeSharp.API.Modules.Timers.Timer? HUDTimer;
    public override void Load(bool hotReload)
    {
        Configs.Load(ModuleDirectory);
        Stringlocalizer = Localizer;
        Configs.Shared.CookiesModule = ModuleDirectory;
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeathQuake);
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterEventHandler<EventPlayerChat>(OnEventPlayerChat, HookMode.Post);
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterEventHandler<EventPlayerConnectFull>(OnEventPlayerConnectFull);
    }

    
    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;

        if(!string.IsNullOrEmpty(Configs.GetConfigData().KS_OnlyAllowTheseGroupsToToggle) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().KS_OnlyAllowTheseGroupsToToggle))
        {
            if (!Globals.allow_groups.ContainsKey(playerid))
            {
                Globals.allow_groups.Add(playerid, true);
            }
        }

        if(Configs.GetConfigData().KS_UseMySql)
        {
            async Task PerformDatabaseOperationAsync()
            {
                try
                {
                    var connectionSettings = JsonConvert.DeserializeObject<MySqlDataManager.MySqlConnectionSettings>(await File.ReadAllTextAsync(Path.Combine(Path.Combine(ModuleDirectory, "config"), "MySql_Settings.json")));
                    var connectionString = new MySqlConnectionStringBuilder
                    {
                        Server = connectionSettings!.MySqlHost,
                        Port = (uint)connectionSettings.MySqlPort,
                        Database = connectionSettings.MySqlDatabase,
                        UserID = connectionSettings.MySqlUsername,
                        Password = connectionSettings.MySqlPassword
                    }.ConnectionString;

                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        var personData = await MySqlDataManager.RetrievePersonDataByIdAsync(playerid, connection);
                        if (personData.PlayerSteamID != 0)
                        {
                            DateTime personDate = DateTime.Now;

                            Helper.SaveToJsonFile(playerid, Configs.GetConfigData().KS_DefaultValue_FreezeOnOpenMenu ? !personData.freezemenu : personData.freezemenu, Configs.GetConfigData().KS_DefaultValue_HeadShotKillSound ? !personData.headshotkill : personData.headshotkill, Configs.GetConfigData().KS_DefaultValue_HeadShotHitSound ? !personData.headshothit : personData.headshothit, Configs.GetConfigData().KS_DefaultValue_BodyKillSound ? !personData.bodyshotkill : personData.bodyshotkill, Configs.GetConfigData().KS_DefaultValue_BodyHitSound ? !personData.bodyshothit : personData.bodyshothit, personData.quakesounds, personData.quakehmessages, personData.quakecmessages, personDate);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"======================== ERROR =============================");
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine($"======================== ERROR =============================");
                }
            }

            Task.Run(PerformDatabaseOperationAsync);
        }

        return HookResult.Continue;
    }

    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        var victim = @event.Userid;
        var attacker = @event.Attacker;

        if (victim == null || !victim.IsValid)return HookResult.Continue;
        var victimteam = victim.TeamNum;
        if (attacker == null || !attacker.IsValid || attacker.IsBot)return HookResult.Continue;
        var headshot = @event.Headshot;
        var attackerteam = attacker.TeamNum;
        var attackerid = attacker.SteamID;
        Helper.PersonData personData = Helper.RetrievePersonDataById(attackerid);
        DateTime personDate = DateTime.Now;

        if (attacker != victim)
        {
            if(headshot)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_AddMenu_HeadShotKillSoundPath))
                {
                    if(ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false)
                    {
                        if(attackerteam != victimteam)
                        {
                            if(Configs.GetConfigData().KS_DefaultValue_HeadShotKillSound)
                            {
                                if (personData.headshotkill)
                                {
                                    //skip
                                }else
                                {
                                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_HeadShotKillSoundPath);
                                }
                                
                            }else
                            {
                                if (personData.headshotkill)
                                {
                                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_HeadShotKillSoundPath);
                                }else
                                {
                                    //skip
                                }
                            }
                            
                        }
                    }else
                    {
                        if(Configs.GetConfigData().KS_DefaultValue_HeadShotKillSound)
                        {
                            if (personData.headshotkill)
                            {
                                //skip
                            }else
                            {
                                attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_HeadShotKillSoundPath);
                            }
                            
                        }else
                        {
                            if (personData.headshotkill)
                            {
                                attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_HeadShotKillSoundPath);
                            }else
                            {
                                //skip
                            }
                        }
                    }
                }
            }else
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_AddMenu_BodyKillSoundPath))
                {
                    if(ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false)
                    {
                        if(attackerteam != victimteam)
                        {
                            if(Configs.GetConfigData().KS_DefaultValue_BodyKillSound)
                            {
                                if (personData.bodyshotkill)
                                {
                                    //skip
                                }else
                                {
                                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_BodyKillSoundPath);
                                }
                                
                            }else
                            {
                                if (personData.bodyshotkill)
                                {
                                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_BodyKillSoundPath);
                                }else
                                {
                                    //skip
                                }
                            }
                            
                        }
                    }else
                    {
                        if(Configs.GetConfigData().KS_DefaultValue_BodyKillSound)
                        {
                            if (personData.bodyshotkill)
                            {
                                //skip
                            }else
                            {
                                attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_BodyKillSoundPath);
                            }
                            
                        }else
                        {
                            if (personData.bodyshotkill)
                            {
                                attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_BodyKillSoundPath);
                            }else
                            {
                                //skip
                            }
                        }
                    }
                }
            }
        }
        return HookResult.Continue;
    }
    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        var victim = @event.Userid;
        var attacker = @event.Attacker;
        var hitgroup = @event.Hitgroup;

        if (victim == null || !victim.IsValid)return HookResult.Continue;
        var victimteam = victim.TeamNum;
        if (attacker == null || !attacker.IsValid || attacker.IsBot)return HookResult.Continue;
        var attackerteam = attacker.TeamNum;
        var attackerid = attacker.SteamID;
        Helper.PersonData personData = Helper.RetrievePersonDataById(attackerid);
        DateTime personDate = DateTime.Now;

        if (attacker != victim)
        {
            if(hitgroup == 1)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_AddMenu_HeadShotHitSoundPath))
                {
                    if(ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false)
                    {
                        if(attackerteam != victimteam)
                        {
                            if(Configs.GetConfigData().KS_DefaultValue_HeadShotHitSound)
                            {
                                if (personData.headshothit)
                                {
                                    //skip
                                }else
                                {
                                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_HeadShotHitSoundPath);
                                }
                                
                            }else
                            {
                                if (personData.headshothit)
                                {
                                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_HeadShotHitSoundPath);
                                }else
                                {
                                    //skip
                                }
                            }

                            
                        }
                    }else
                    {
                        if(Configs.GetConfigData().KS_DefaultValue_HeadShotHitSound)
                        {
                            if (personData.headshothit)
                            {
                                //skip
                            }else
                            {
                                attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_HeadShotHitSoundPath);
                            }
                            
                        }else
                        {
                            if (personData.headshothit)
                            {
                                attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_HeadShotHitSoundPath);
                            }else
                            {
                                //skip
                            }
                        }
                    }
                    
                }
            }else
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_AddMenu_BodyHitSoundPath))
                {
                    if(ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false)
                    {
                        if(attackerteam != victimteam)
                        {
                            if(Configs.GetConfigData().KS_DefaultValue_BodyHitSound)
                            {
                                if (personData.bodyshothit)
                                {
                                    //skip
                                }else
                                {
                                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_BodyHitSoundPath);
                                }
                                
                            }else
                            {
                                if (personData.bodyshothit)
                                {
                                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_BodyHitSoundPath);
                                }else
                                {
                                    //skip
                                }
                            }
                        }
                    }else
                    {
                        if(Configs.GetConfigData().KS_DefaultValue_BodyHitSound)
                        {
                            if (personData.bodyshothit)
                            {
                                //skip
                            }else
                            {
                                attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_BodyHitSoundPath);
                            }
                            
                        }else
                        {
                            if (personData.bodyshothit)
                            {
                                attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_AddMenu_BodyHitSoundPath);
                            }else
                            {
                                //skip
                            }
                        }
                    }
                }
            }
        }

        return HookResult.Continue;
    }

    private HookResult OnPlayerDeathQuake(EventPlayerDeath @event, GameEventInfo info)
    {
        if(!Configs.GetConfigData().KS_EnableQuakeSounds || @event == null)return HookResult.Continue;
        if(Configs.GetConfigData().KS_DisableQuakeSoundsOnWarmUp && Helper.IsWarmup())return HookResult.Continue;
        var victim = @event.Userid;
        var attacker = @event.Attacker;
        bool KilledHimSelf = @event.Weapon.Contains("worldent");
        if (KilledHimSelf)
        {
            try
            {
                string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
                var json = Helper.LoadJsonFromFile(_json);

                string SsoundPath = "";
                float SIntervalHUD = 10;
                bool Sanouncement = false;
                bool SShowChat = false;
                bool SShowCenter = false;
                if (json.ContainsKey("SelfKill"))
                {
                    SsoundPath = json["SelfKill"]["Path"]?.ToString()!;
                    if (!json["SelfKill"].ContainsKey("ShowCenter_InSecs")) SIntervalHUD = 0; else float.TryParse(json["SelfKill"]["ShowCenter_InSecs"]?.ToString(), out SIntervalHUD);
                    if (!json["SelfKill"].ContainsKey("Announcement")) Sanouncement = false; else bool.TryParse(json["SelfKill"]["Announcement"]?.ToString(), out Sanouncement);
                    if (!json["SelfKill"].ContainsKey("ShowChat")) SShowChat = false; else bool.TryParse(json["SelfKill"]["ShowChat"]?.ToString(), out SShowChat);
                    if (!json["SelfKill"].ContainsKey("ShowCenter")) SShowCenter = false; else bool.TryParse(json["SelfKill"]["ShowCenter"]?.ToString(), out SShowCenter);
                }
                
                if (!string.IsNullOrEmpty(SsoundPath))
                {
                    if(Sanouncement)
                    {
                        var allplayers = Helper.GetAllController();
                        allplayers.ForEach(players => 
                        {
                            if(players != null && players.IsValid && !players.IsBot)
                            {
                                var playerid = players.SteamID;
                                Helper.PersonData personData = Helper.RetrievePersonDataById(playerid);
                                if(personData.quakesounds)
                                {

                                }else
                                {
                                    players.ExecuteClientCommand("play " + SsoundPath);
                                }
                                if(personData.quakecmessages)
                                {

                                }else
                                {
                                    if (SShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.selfkill"]))
                                    {
                                        Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.selfkill"], victim.PlayerName);
                                    }
                                }
                                if(personData.quakehmessages)
                                {

                                }else
                                {
                                    if (SShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.selfkill"]))
                                    {
                                        Server.NextFrame(() =>
                                        {
                                            if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                            {
                                                Globals.ShowHud_Kill.Remove(players.SteamID);
                                            }
                                            if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                            {
                                                Globals.ShowHud_Kill_Name = victim.PlayerName;
                                                Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.selfkill"]);
                                            }
                                            HUDTimer?.Kill();
                                            HUDTimer = null;
                                            HUDTimer = AddTimer(SIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                        });
                                    }
                                }
                                
                            }
                        });

                    }else
                    {
                        var playerid = attacker.SteamID;
                        Helper.PersonData personDataA = Helper.RetrievePersonDataById(playerid);
                        if(personDataA.quakesounds)
                        {

                        }else
                        {
                            attacker.ExecuteClientCommand("play " + SsoundPath);
                        }

                        if(personDataA.quakecmessages)
                        {

                        }else
                        {
                            if (SShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.selfkill"]))
                            {
                                Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.selfkill"]);
                            }
                        }
                        
                        if(personDataA.quakehmessages)
                        {

                        }else
                        {
                            if (SShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.selfkill"]))
                            {
                                Server.NextFrame(() =>
                                {
                                    if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                    {
                                        Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                    }
                                    if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                    {
                                        Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.selfkill"]);
                                    }
                                    HUDTimer?.Kill();
                                    HUDTimer = null;
                                    HUDTimer = AddTimer(SIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                });
                            }
                        }
                        
                    }
                }
            }catch{}
        }
        
        if (victim == null || !victim.IsValid)return HookResult.Continue;

        var playeridvictim = victim.SteamID;
        if (Globals.Kill_Streak.ContainsKey(playeridvictim))
        {
            Globals.Kill_Streak[playeridvictim] = 0;
        }
        if (Globals.Kill_StreakHS.ContainsKey(playeridvictim))
        {
            Globals.Kill_StreakHS[playeridvictim] = 0;
        }
        if (Globals.Kill_Knife.ContainsKey(playeridvictim))
        {
            Globals.Kill_Knife[playeridvictim] = 0;
        }
        if (Globals.Kill_Nade.ContainsKey(playeridvictim))
        {
            Globals.Kill_Nade[playeridvictim] = 0;
        }
        if (Globals.Kill_Molly.ContainsKey(playeridvictim))
        {
            Globals.Kill_Molly[playeridvictim] = 0;
        }

        
        if (attacker == null || !attacker.IsValid || attacker.IsBot)return HookResult.Continue;
        var attackerteam = attacker.TeamNum;
        var victimteam = victim.TeamNum;
        var headshot = @event.Headshot;
        
        bool knifekill = @event.Weapon.Contains("knife");
        bool NadeKill = @event.Weapon.Contains("hegrenade");
        bool MollyKill = @event.Weapon.Contains("inferno");
        bool TaserKill = @event.Weapon.Contains("taser");
        
        var playeridattacker = attacker.SteamID;
        Helper.PersonData personData = Helper.RetrievePersonDataById(playeridattacker);
        if (attacker == victim)
        {
            try
            {
                string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
                var json = Helper.LoadJsonFromFile(_json);

                string SsoundPath = "";
                float SIntervalHUD = 10;
                bool Sanouncement = false;
                bool SShowChat = false;
                bool SShowCenter = false;
                if (json.ContainsKey("SelfKill"))
                {
                    SsoundPath = json["SelfKill"]["Path"]?.ToString()!;
                    if (!json["SelfKill"].ContainsKey("ShowCenter_InSecs")) SIntervalHUD = 0; else float.TryParse(json["SelfKill"]["ShowCenter_InSecs"]?.ToString(), out SIntervalHUD);
                    if (!json["SelfKill"].ContainsKey("Announcement")) Sanouncement = false; else bool.TryParse(json["SelfKill"]["Announcement"]?.ToString(), out Sanouncement);
                    if (!json["SelfKill"].ContainsKey("ShowChat")) SShowChat = false; else bool.TryParse(json["SelfKill"]["ShowChat"]?.ToString(), out SShowChat);
                    if (!json["SelfKill"].ContainsKey("ShowCenter")) SShowCenter = false; else bool.TryParse(json["SelfKill"]["ShowCenter"]?.ToString(), out SShowCenter);
                }
                
                if (!string.IsNullOrEmpty(SsoundPath))
                {
                    if(Sanouncement)
                    {
                        var allplayers = Helper.GetAllController();
                        allplayers.ForEach(players => 
                        {
                            if(players != null && players.IsValid && !players.IsBot)
                            {
                                Helper.PersonData personDataa = Helper.RetrievePersonDataById(players.SteamID);
                                if(personDataa.quakesounds)
                                {

                                }else
                                {
                                    players.ExecuteClientCommand("play " + SsoundPath);
                                }
                                if(personDataa.quakecmessages)
                                {

                                }else
                                {
                                    if (SShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.selfkill"]))
                                    {
                                        Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.selfkill"], victim.PlayerName);
                                    }
                                }
                                
                                if(personDataa.quakehmessages)
                                {

                                }else
                                {
                                    if (SShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.selfkill"]))
                                    {
                                        Server.NextFrame(() =>
                                        {
                                            if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                            {
                                                Globals.ShowHud_Kill.Remove(players.SteamID);
                                            }
                                            if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                            {
                                                Globals.ShowHud_Kill_Name = victim.PlayerName;
                                                Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.selfkill"]);
                                            }
                                            HUDTimer?.Kill();
                                            HUDTimer = null;
                                            HUDTimer = AddTimer(SIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                        });
                                    }
                                }
                                
                                
                            }
                        });

                    }else
                    {
                        if(personData.quakesounds)
                        {

                        }else
                        {
                            attacker.ExecuteClientCommand("play " + SsoundPath);
                        }

                        if(personData.quakecmessages)
                        {

                        }else
                        {
                            if (SShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.selfkill"]))
                            {
                                Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.selfkill"]);
                            }
                        }
                        
                        if(personData.quakehmessages)
                        {

                        }else
                        {
                            if (SShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.selfkill"]))
                            {
                                Server.NextFrame(() =>
                                {
                                    if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                    {
                                        Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                    }
                                    if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                    {
                                        Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.selfkill"]);
                                    }
                                    HUDTimer?.Kill();
                                    HUDTimer = null;
                                    HUDTimer = AddTimer(SIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                });
                            }
                        }
                        
                    }
                    return HookResult.Continue;
                }
            }catch{}
        }else if (attacker != victim)
        {
            if (!Globals.Kill_Streak.ContainsKey(playeridattacker))
            {
                Globals.Kill_Streak.Add(playeridattacker, 0);
            }
            if (!Globals.Kill_StreakHS.ContainsKey(playeridattacker))
            {
                Globals.Kill_StreakHS.Add(playeridattacker, 0);
            }
            if (!Globals.Kill_Knife.ContainsKey(playeridattacker))
            {
                Globals.Kill_Knife.Add(playeridattacker, 0);
            }
            if (!Globals.Kill_Nade.ContainsKey(playeridattacker))
            {
                Globals.Kill_Nade.Add(playeridattacker, 0);
            }
            if (!Globals.Kill_Molly.ContainsKey(playeridattacker))
            {
                Globals.Kill_Molly.Add(playeridattacker, 0);
            }
            if (!Globals.Kill_Taser.ContainsKey(playeridattacker))
            {
                Globals.Kill_Taser.Add(playeridattacker, 0);
            }
            
            if (Globals.Kill_Streak.ContainsKey(playeridattacker) || Globals.Kill_StreakHS.ContainsKey(playeridattacker) || Globals.Kill_Knife.ContainsKey(playeridattacker) || Globals.Kill_Nade.ContainsKey(playeridattacker) || Globals.Kill_Molly.ContainsKey(playeridattacker) || Globals.Kill_Taser.ContainsKey(playeridattacker))
            {
                if (attackerteam != victimteam) Globals.Kill_Streak[playeridattacker]++;
                if (headshot) Globals.Kill_StreakHS[playeridattacker]++;
                if (knifekill) Globals.Kill_Knife[playeridattacker]++;
                if (NadeKill) Globals.Kill_Nade[playeridattacker]++;
                if (MollyKill) Globals.Kill_Molly[playeridattacker]++;
                if (TaserKill) Globals.Kill_Taser[playeridattacker]++;

                int numberofkills = Globals.Kill_Streak[playeridattacker];
                int numberofkillsHS = Globals.Kill_StreakHS[playeridattacker];
                int numberofknifekill = Globals.Kill_Knife[playeridattacker];
                int numberofnadekill = Globals.Kill_Nade[playeridattacker];
                int numberofmollykill = Globals.Kill_Molly[playeridattacker];
                int numberoftaserkill = Globals.Kill_Taser[playeridattacker];

                try
                {
                    string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
                    var json = Helper.LoadJsonFromFile(_json);

                    string TsoundPath = "";
                    float TIntervalHUD = 10;
                    bool Tanouncement = false;
                    bool TShowChat = false;
                    bool TShowCenter = false;
                    if (json.ContainsKey("TeamKill"))
                    {
                        TsoundPath = json["TeamKill"]["Path"]?.ToString()!;
                        if (!json["TeamKill"].ContainsKey("ShowCenter_InSecs")) TIntervalHUD = 0; else float.TryParse(json["TeamKill"]["ShowCenter_InSecs"]?.ToString(), out TIntervalHUD);
                        if (!json["TeamKill"].ContainsKey("Announcement")) Tanouncement = false; else bool.TryParse(json["TeamKill"]["Announcement"]?.ToString(), out Tanouncement);
                        if (!json["TeamKill"].ContainsKey("ShowChat")) TShowChat = false; else bool.TryParse(json["TeamKill"]["ShowChat"]?.ToString(), out TShowChat);
                        if (!json["TeamKill"].ContainsKey("ShowCenter")) TShowCenter = false; else bool.TryParse(json["TeamKill"]["ShowCenter"]?.ToString(), out TShowCenter);
                    }
					
                    if (attackerteam == victimteam && !string.IsNullOrEmpty(TsoundPath))
                    {
						if(Tanouncement)
						{
							var allplayers = Helper.GetAllController();
							allplayers.ForEach(players => 
							{
								if(players != null && players.IsValid && !players.IsBot)
								{
                                    Helper.PersonData personDataa = Helper.RetrievePersonDataById(players.SteamID);
                                    if(personDataa.quakesounds)
                                    {

                                    }else
                                    {
                                        players.ExecuteClientCommand("play " + TsoundPath);
                                    }

                                    if(personDataa.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (TShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.teamkill"]))
                                        {
                                            Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.teamkill"], attacker.PlayerName, victim.PlayerName);
                                        }
                                    }
									

                                    if(personDataa.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (TShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.teamkill"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(players.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                    Globals.ShowHud_Kill_Name2 = victim.PlayerName;
                                                    Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.teamkill"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(TIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }

									
									
								}
							});

						}else
						{
                            if(personData.quakesounds)
                            {

                            }else
                            {
                                attacker.ExecuteClientCommand("play " + TsoundPath);
                            }
							
                            if(personData.quakecmessages)
                            {

                            }else
                            {
                                if (TShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.teamkill"]))
                                {
                                    Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.teamkill"], victim.PlayerName);
                                }
                            }
							

                            if(personData.quakehmessages)
                            {

                            }else
                            {
                                if (TShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.teamkill"]))
                                {
                                    Server.NextFrame(() =>
                                    {
                                        if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                        {
                                            Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                        }
                                        if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                        {
                                            Globals.ShowHud_Kill_Name = victim.PlayerName;
                                            Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.teamkill"]);
                                        }
                                        HUDTimer?.Kill();
                                        HUDTimer = null;
                                        HUDTimer = AddTimer(TIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                    });
                                }
                            }
						}
						return HookResult.Continue;
                    }

                    
                    
                    string FsoundPath = "";
                    float FIntervalHUD = 10;
                    bool Fanouncement = false;
                    bool FShowChat = false;
                    bool FShowCenter = false;
                    if (json.ContainsKey("FirstBlood"))
                    {
                        FsoundPath = json["FirstBlood"]["Path"]?.ToString()!;
                        if (!json["FirstBlood"].ContainsKey("ShowCenter_InSecs")) FIntervalHUD = 0; else float.TryParse(json["FirstBlood"]["ShowCenter_InSecs"]?.ToString(), out FIntervalHUD);
                        if (!json["FirstBlood"].ContainsKey("Announcement")) Fanouncement = false; else bool.TryParse(json["FirstBlood"]["Announcement"]?.ToString(), out Fanouncement);
                        if (!json["FirstBlood"].ContainsKey("ShowChat")) FShowChat = false; else bool.TryParse(json["FirstBlood"]["ShowChat"]?.ToString(), out FShowChat);
                        if (!json["FirstBlood"].ContainsKey("ShowCenter")) FShowCenter = false; else bool.TryParse(json["FirstBlood"]["ShowCenter"]?.ToString(), out FShowCenter);
                    }
					
                    if (Globals.First_Blood && !string.IsNullOrEmpty(FsoundPath))
                    {
                        if(Fanouncement)
                        {
                            var allplayers = Helper.GetAllController();
                            allplayers.ForEach(players => 
                            {
                                if(players != null && players.IsValid && !players.IsBot)
                                {
                                    Helper.PersonData personDataa = Helper.RetrievePersonDataById(players.SteamID);
                                    if(personDataa.quakesounds)
                                    {

                                    }else
                                    {
                                        players.ExecuteClientCommand("play " + FsoundPath);
                                    }
                                    if(personDataa.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (FShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.firstblood"]))
                                        {
                                            Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.firstblood"], attacker.PlayerName, victim.PlayerName);
                                        }
                                    }
                                    
                                    if(personDataa.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (FShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.firstblood"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(players.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                    Globals.ShowHud_Kill_Name2 = victim.PlayerName;
                                                    Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.firstblood"]);
                                                }

                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(FIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                    
                                }
                            });

                        }else
                        {
                            if(personData.quakesounds)
                            {

                            }else
                            {
                                attacker.ExecuteClientCommand("play " + FsoundPath);
                            }
                            
                            if(personData.quakecmessages)
                            {

                            }else
                            {
                                if (FShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.firstblood"]))
                                {
                                    Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.firstblood"], victim.PlayerName);
                                }
                            }
                            

                            if(personData.quakehmessages)
                            {

                            }else
                            {
                                if (FShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.firstblood"]))
                                {
                                    Server.NextFrame(() =>
                                    {
                                        if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                        {
                                            Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                        }
                                        if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                        {
                                            Globals.ShowHud_Kill_Name = victim.PlayerName;
                                            Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.firstblood"]);
                                        }
                                        HUDTimer?.Kill();
                                        HUDTimer = null;
                                        HUDTimer = AddTimer(FIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                    });
                                }
                            }
                        }
                        Globals.First_Blood = false;
                        return HookResult.Continue; 
                        
                    }
                    
                    string NsoundPath = "";
                    double NInterval = 5;
                    float NIntervalHUD = 10;
                    bool Nanouncement = false;
                    bool NSteak = false;
                    bool NShowChat = false;
                    bool NShowCenter = false;
                    if (json.ContainsKey("KnifeKill"))
                    {
                        NSteak = false;
                        NsoundPath = json["KnifeKill"]["Path"]?.ToString()!;
                        if (!json["KnifeKill"].ContainsKey("Interval_InSecs")) NInterval = 5; else double.TryParse(json["KnifeKill"]["Interval_InSecs"]?.ToString(), out NInterval);
                        if (!json["KnifeKill"].ContainsKey("ShowCenter_InSecs")) NIntervalHUD = 0; else float.TryParse(json["KnifeKill"]["ShowCenter_InSecs"]?.ToString(), out NIntervalHUD);
                        if (!json["KnifeKill"].ContainsKey("Announcement")) Nanouncement = false; else bool.TryParse(json["KnifeKill"]["Announcement"]?.ToString(), out Nanouncement);
                        if (!json["KnifeKill"].ContainsKey("ShowChat")) NShowChat = false; else bool.TryParse(json["KnifeKill"]["ShowChat"]?.ToString(), out NShowChat);
                        if (!json["KnifeKill"].ContainsKey("ShowCenter")) NShowCenter = false; else bool.TryParse(json["KnifeKill"]["ShowCenter"]?.ToString(), out NShowCenter);
                    }else if (json.ContainsKey("KnifeKill_" + numberofknifekill))
                    {
                        NSteak = true;
                        NsoundPath = json["KnifeKill_" + numberofknifekill]["Path"]?.ToString()!;
                        if (!json["KnifeKill_" + numberofknifekill].ContainsKey("Interval_InSecs")) NInterval = 5; else double.TryParse(json["KnifeKill_" + numberofknifekill]["Interval_InSecs"]?.ToString(), out NInterval);
                        if (!json["KnifeKill_" + numberofknifekill].ContainsKey("ShowCenter_InSecs")) NIntervalHUD = 0; else float.TryParse(json["KnifeKill_" + numberofknifekill]["ShowCenter_InSecs"]?.ToString(), out NIntervalHUD);
                        if (!json["KnifeKill_" + numberofknifekill].ContainsKey("Announcement")) Nanouncement = false; else bool.TryParse(json["KnifeKill_" + numberofknifekill]["Announcement"]?.ToString(), out Nanouncement);
                        if (!json["KnifeKill_" + numberofknifekill].ContainsKey("ShowChat")) NShowChat = false; else bool.TryParse(json["KnifeKill_" + numberofknifekill]["ShowChat"]?.ToString(), out NShowChat);
                        if (!json["KnifeKill_" + numberofknifekill].ContainsKey("ShowCenter")) NShowCenter = false; else bool.TryParse(json["KnifeKill_" + numberofknifekill]["ShowCenter"]?.ToString(), out NShowCenter);
                    }
                    if (knifekill && !string.IsNullOrEmpty(NsoundPath) && NInterval > 0)
                    {
                        if (!Globals.lastPlayTimesKnife.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimesKnife[playeridattacker]).TotalSeconds >= NInterval)
                        {
                            if(Nanouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid && !players.IsBot)
                                    {
                                        Helper.PersonData personDataa = Helper.RetrievePersonDataById(players.SteamID);
                                        if(personDataa.quakesounds)
                                        {

                                        }else
                                        {
                                            players.ExecuteClientCommand("play " + NsoundPath);
                                        }
                                        
                                        if(NSteak)
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (NShowChat && !string.IsNullOrEmpty(Localizer[$"chat.announce.quake.knife.streak.{numberofknifekill}"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer[$"chat.announce.quake.knife.streak.{numberofknifekill}"], attacker.PlayerName, numberofknifekill);
                                                }
                                            }
                                            
                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (NShowCenter && !string.IsNullOrEmpty(Localizer[$"center.announce.quake.knife.streak.{numberofknifekill}"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name2 = "";
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_int = numberofknifekill;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer[$"center.announce.quake.knife.streak.{numberofknifekill}"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(NIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }else
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (NShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.knife"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.knife"], attacker.PlayerName, victim.PlayerName);
                                                }
                                            }
                                            
                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (NShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.knife"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_Name2 = victim.PlayerName;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.knife"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(NIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }
                                    }
                                });

                            }else
                            {
                                if(personData.quakesounds)
                                {

                                }else
                                {
                                    attacker.ExecuteClientCommand("play " + NsoundPath);
                                }
                                
                                if(NSteak)
                                {
                                    if(personData.quakecmessages)
                                    {
                                        
                                    }else
                                    {
                                        if (NShowChat && !string.IsNullOrEmpty(Localizer[$"chat.quake.knife.streak.{numberofknifekill}"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer[$"chat.quake.knife.streak.{numberofknifekill}"], numberofknifekill);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (NShowCenter && !string.IsNullOrEmpty(Localizer[$"center.quake.knife.streak.{numberofknifekill}"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name2 = "";
                                                    Globals.ShowHud_Kill_int = numberofknifekill;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.knife.streak.{numberofknifekill}"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(NIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }else
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (NShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.knife"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.knife"], victim.PlayerName);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (NShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.knife"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name = "";
                                                    Globals.ShowHud_Kill_Name = victim.PlayerName;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.knife"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(NIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }
                            }
                            Globals.lastPlayTimesKnife[playeridattacker] = DateTime.Now;
                            return HookResult.Continue; 
                        }
                    }

                    string GsoundPath = "";
                    double GInterval = 5;
                    float GIntervalHUD = 10;
                    bool Ganouncement = false;
                    bool GSteak = false;
                    bool GShowChat = false;
                    bool GShowCenter = false;
                    if (json.ContainsKey("GrenadeKill"))
                    {
                        GSteak = false;
                        GsoundPath = json["GrenadeKill"]["Path"]?.ToString()!;
                        if (!json["GrenadeKill"].ContainsKey("Interval_InSecs")) GInterval = 5; else double.TryParse(json["GrenadeKill"]["Interval_InSecs"]?.ToString(), out GInterval);
                        if (!json["GrenadeKill"].ContainsKey("ShowCenter_InSecs")) GIntervalHUD = 0; else float.TryParse(json["GrenadeKill"]["ShowCenter_InSecs"]?.ToString(), out GIntervalHUD);
                        if (!json["GrenadeKill"].ContainsKey("Announcement")) Ganouncement = false; else bool.TryParse(json["GrenadeKill"]["Announcement"]?.ToString(), out Ganouncement);
                        if (!json["GrenadeKill"].ContainsKey("ShowChat")) GShowChat = false; else bool.TryParse(json["GrenadeKill"]["ShowChat"]?.ToString(), out GShowChat);
                        if (!json["GrenadeKill"].ContainsKey("ShowCenter")) GShowCenter = false; else bool.TryParse(json["GrenadeKill"]["ShowCenter"]?.ToString(), out GShowCenter);
                    }else if (json.ContainsKey("GrenadeKill_" + numberofnadekill))
                    {
                        GSteak = true;
                        GsoundPath = json["GrenadeKill_" + numberofnadekill]["Path"]?.ToString()!;
                        if (!json["GrenadeKill_" + numberofnadekill].ContainsKey("Interval_InSecs")) GInterval = 5; else double.TryParse(json["GrenadeKill_" + numberofnadekill]["Interval_InSecs"]?.ToString(), out GInterval);
                        if (!json["GrenadeKill_" + numberofnadekill].ContainsKey("ShowCenter_InSecs")) GIntervalHUD = 0; else float.TryParse(json["GrenadeKill_" + numberofnadekill]["ShowCenter_InSecs"]?.ToString(), out GIntervalHUD);
                        if (!json["GrenadeKill_" + numberofnadekill].ContainsKey("Announcement")) Ganouncement = false; else bool.TryParse(json["GrenadeKill_" + numberofnadekill]["Announcement"]?.ToString(), out Ganouncement);
                        if (!json["GrenadeKill_" + numberofnadekill].ContainsKey("ShowChat")) GShowChat = false; else bool.TryParse(json["GrenadeKill_" + numberofnadekill]["ShowChat"]?.ToString(), out GShowChat);
                        if (!json["GrenadeKill_" + numberofnadekill].ContainsKey("ShowCenter")) GShowCenter = false; else bool.TryParse(json["GrenadeKill_" + numberofnadekill]["ShowCenter"]?.ToString(), out GShowCenter);
                    }
                    if (NadeKill && !string.IsNullOrEmpty(GsoundPath) && GInterval > 0)
                    {
                        if (!Globals.lastPlayTimesNade.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimesNade[playeridattacker]).TotalSeconds >= GInterval)
                        {
                            if(Ganouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid && !players.IsBot)
                                    {
                                        Helper.PersonData personDataa = Helper.RetrievePersonDataById(players.SteamID);
                                        if(personDataa.quakesounds)
                                        {

                                        }else
                                        {
                                            players.ExecuteClientCommand("play " + GsoundPath);
                                        }
                                        
                                        if(GSteak)
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (GShowChat && !string.IsNullOrEmpty(Localizer[$"chat.announce.quake.grenade.streak.{numberofnadekill}"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer[$"chat.announce.quake.grenade.streak.{numberofnadekill}"], attacker.PlayerName, numberofnadekill);
                                                }
                                            }
                                            

                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (GShowCenter && !string.IsNullOrEmpty(Localizer[$"center.announce.quake.grenade.streak.{numberofnadekill}"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name2 = "";
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_int = numberofnadekill;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer[$"center.announce.quake.grenade.streak.{numberofnadekill}"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(GIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }else
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (GShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.grenade"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.grenade"], attacker.PlayerName, victim.PlayerName);
                                                }
                                            }
                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (GShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.grenade"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_Name2 = victim.PlayerName;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.grenade"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(GIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                        }
                                    }
                                });

                            }else
                            {
                                if(personData.quakesounds)
                                {

                                }else
                                {
                                    attacker.ExecuteClientCommand("play " + GsoundPath);
                                }
                                
                                if(GSteak)
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (GShowChat && !string.IsNullOrEmpty(Localizer[$"chat.quake.grenade.streak.{numberofnadekill}"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer[$"chat.quake.grenade.streak.{numberofnadekill}"], numberofnadekill);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (GShowCenter && !string.IsNullOrEmpty(Localizer[$"center.quake.grenade.streak.{numberofnadekill}"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name2 = "";
                                                    Globals.ShowHud_Kill_Name = "";
                                                    Globals.ShowHud_Kill_int = numberofnadekill;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.grenade.streak.{numberofnadekill}"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(GIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }else
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (GShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.grenade"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.grenade"], victim.PlayerName);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (GShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.grenade"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name = victim.PlayerName;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.grenade"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(GIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }
                            }
                            Globals.lastPlayTimesNade[playeridattacker] = DateTime.Now;
                            return HookResult.Continue; 
                        }
                    }

                    string MsoundPath = "";
                    double MInterval = 5;
                    float MIntervalHUD = 10;
                    bool Manouncement = false;
                    bool MSteak = false;
                    bool MShowChat = false;
                    bool MShowCenter = false;
                    if (json.ContainsKey("MollyKill"))
                    {
                        MSteak = false;
                        MsoundPath = json["MollyKill"]["Path"]?.ToString()!;
                        if (!json["MollyKill"].ContainsKey("Interval_InSecs")) MInterval = 5; else double.TryParse(json["MollyKill"]["Interval_InSecs"]?.ToString(), out MInterval);
                        if (!json["MollyKill"].ContainsKey("ShowCenter_InSecs")) MIntervalHUD = 0; else float.TryParse(json["MollyKill"]["ShowCenter_InSecs"]?.ToString(), out MIntervalHUD);
                        if (!json["MollyKill"].ContainsKey("Announcement")) Manouncement = false; else bool.TryParse(json["MollyKill"]["Announcement"]?.ToString(), out Manouncement);
                        if (!json["MollyKill"].ContainsKey("ShowChat")) MShowChat = false; else bool.TryParse(json["MollyKill"]["ShowChat"]?.ToString(), out MShowChat);
                        if (!json["MollyKill"].ContainsKey("ShowCenter")) MShowCenter = false; else bool.TryParse(json["MollyKill"]["ShowCenter"]?.ToString(), out MShowCenter);
                    }else if (json.ContainsKey("MollyKill_" + numberofmollykill))
                    {
                        MSteak = true;
                        MsoundPath = json["MollyKill_" + numberofmollykill]["Path"]?.ToString()!;
                        if (!json["MollyKill_" + numberofmollykill].ContainsKey("Interval_InSecs")) MInterval = 5; else double.TryParse(json["MollyKill_" + numberofmollykill]["Interval_InSecs"]?.ToString(), out MInterval);
                        if (!json["MollyKill_" + numberofmollykill].ContainsKey("ShowCenter_InSecs")) MIntervalHUD = 0; else float.TryParse(json["MollyKill_" + numberofmollykill]["ShowCenter_InSecs"]?.ToString(), out MIntervalHUD);
                        if (!json["MollyKill_" + numberofmollykill].ContainsKey("Announcement")) Manouncement = false; else bool.TryParse(json["MollyKill_" + numberofmollykill]["Announcement"]?.ToString(), out Manouncement);
                        if (!json["MollyKill_" + numberofmollykill].ContainsKey("ShowChat")) MShowChat = false; else bool.TryParse(json["MollyKill_" + numberofmollykill]["ShowChat"]?.ToString(), out MShowChat);
                        if (!json["MollyKill_" + numberofmollykill].ContainsKey("ShowCenter")) MShowCenter = false; else bool.TryParse(json["MollyKill_" + numberofmollykill]["ShowCenter"]?.ToString(), out MShowCenter);
                    }
                    if (MollyKill && !string.IsNullOrEmpty(MsoundPath) && MInterval > 0)
                    {
                        if (!Globals.lastPlayTimesMolly.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimesMolly[playeridattacker]).TotalSeconds >= MInterval)
                        {
                            if(Manouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid && !players.IsBot)
                                    {
                                        Helper.PersonData personDataa = Helper.RetrievePersonDataById(players.SteamID);
                                        if(personDataa.quakesounds)
                                        {

                                        }else
                                        {
                                            players.ExecuteClientCommand("play " + MsoundPath);
                                        }
                                        
                                        if(MSteak)
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (MShowChat && !string.IsNullOrEmpty(Localizer[$"chat.announce.quake.molly.streak.{numberofmollykill}"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer[$"chat.announce.quake.molly.streak.{numberofmollykill}"], attacker.PlayerName, numberofmollykill);
                                                }
                                            }
                                            
                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (MShowCenter && !string.IsNullOrEmpty(Localizer[$"center.announce.quake.molly.streak.{numberofmollykill}"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name2 = "";
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_int = numberofmollykill;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer[$"center.announce.quake.molly.streak.{numberofmollykill}"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(MIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }else
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (MShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.molly"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.molly"], attacker.PlayerName, victim.PlayerName);
                                                }
                                            }
                                            
                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (MShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.molly"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_Name2 = victim.PlayerName;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.molly"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(MIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }
                                    }
                                });

                            }else
                            {
                                if(MSteak)
                                {
                                    if(personData.quakesounds)
                                    {

                                    }else
                                    {
                                        attacker.ExecuteClientCommand("play " + MsoundPath);
                                    }

                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (MShowChat && !string.IsNullOrEmpty(Localizer[$"chat.quake.molly.streak.{numberofmollykill}"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer[$"chat.quake.molly.streak.{numberofmollykill}"], numberofmollykill);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (MShowCenter && !string.IsNullOrEmpty(Localizer[$"center.quake.molly.streak.{numberofmollykill}"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name2 = "";
                                                    Globals.ShowHud_Kill_Name = "";
                                                    Globals.ShowHud_Kill_int = numberofmollykill;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.molly.streak.{numberofmollykill}"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(MIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }else
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (MShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.molly"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.molly"], victim.PlayerName);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (MShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.molly"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name = victim.PlayerName;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.molly"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(MIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }
                            }
                            Globals.lastPlayTimesMolly[playeridattacker] = DateTime.Now;
                            return HookResult.Continue; 
                        }
                    }
                    
                    string ZsoundPath = "";
                    double ZInterval = 5;
                    float ZIntervalHUD = 10;
                    bool Zanouncement = false;
                    bool ZSteak = false;
                    bool ZShowChat = false;
                    bool ZShowCenter = false;
                    if (json.ContainsKey("TaserKill"))
                    {
                        ZSteak = false;
                        ZsoundPath = json["TaserKill"]["Path"]?.ToString()!;
                        if (!json["TaserKill"].ContainsKey("Interval_InSecs")) ZInterval = 5; else double.TryParse(json["TaserKill"]["Interval_InSecs"]?.ToString(), out ZInterval);
                        if (!json["TaserKill"].ContainsKey("ShowCenter_InSecs")) ZIntervalHUD = 0; else float.TryParse(json["TaserKill"]["ShowCenter_InSecs"]?.ToString(), out ZIntervalHUD);
                        if (!json["TaserKill"].ContainsKey("Announcement")) Zanouncement = false; else bool.TryParse(json["TaserKill"]["Announcement"]?.ToString(), out Zanouncement);
                        if (!json["TaserKill"].ContainsKey("ShowChat")) ZShowChat = false; else bool.TryParse(json["TaserKill"]["ShowChat"]?.ToString(), out ZShowChat);
                        if (!json["TaserKill"].ContainsKey("ShowCenter")) ZShowCenter = false; else bool.TryParse(json["TaserKill"]["ShowCenter"]?.ToString(), out ZShowCenter);
                    }else if (json.ContainsKey("TaserKill_" + numberoftaserkill))
                    {
                        ZSteak = true;
                        ZsoundPath = json["TaserKill_" + numberoftaserkill]["Path"]?.ToString()!;
                        if (!json["TaserKill_" + numberoftaserkill].ContainsKey("Interval_InSecs")) ZInterval = 5; else double.TryParse(json["TaserKill_" + numberoftaserkill]["Interval_InSecs"]?.ToString(), out ZInterval);
                        if (!json["TaserKill_" + numberoftaserkill].ContainsKey("ShowCenter_InSecs")) ZIntervalHUD = 0; else float.TryParse(json["TaserKill_" + numberoftaserkill]["ShowCenter_InSecs"]?.ToString(), out ZIntervalHUD);
                        if (!json["TaserKill_" + numberoftaserkill].ContainsKey("Announcement")) Zanouncement = false; else bool.TryParse(json["TaserKill_" + numberoftaserkill]["Announcement"]?.ToString(), out Zanouncement);
                        if (!json["TaserKill_" + numberoftaserkill].ContainsKey("ShowChat")) ZShowChat = false; else bool.TryParse(json["TaserKill_" + numberoftaserkill]["ShowChat"]?.ToString(), out ZShowChat);
                        if (!json["TaserKill_" + numberoftaserkill].ContainsKey("ShowCenter")) ZShowCenter = false; else bool.TryParse(json["TaserKill_" + numberoftaserkill]["ShowCenter"]?.ToString(), out ZShowCenter);
                    }
                    if (TaserKill && !string.IsNullOrEmpty(ZsoundPath) && ZInterval > 0)
                    {
                        if (!Globals.lastPlayTimesTaser.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimesTaser[playeridattacker]).TotalSeconds >= ZInterval)
                        {
                            if(Zanouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid && !players.IsBot)
                                    {
                                        Helper.PersonData personDataa = Helper.RetrievePersonDataById(players.SteamID);
                                        if(personDataa.quakesounds)
                                        {

                                        }else
                                        {
                                            players.ExecuteClientCommand("play " + ZsoundPath);
                                        }
                                        
                                        if(ZSteak)
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (ZShowChat && !string.IsNullOrEmpty(Localizer[$"chat.announce.quake.taser.streak.{numberoftaserkill}"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer[$"chat.announce.quake.taser.streak.{numberoftaserkill}"], attacker.PlayerName, numberoftaserkill);
                                                }
                                            }
                                            
                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (ZShowCenter && !string.IsNullOrEmpty(Localizer[$"center.announce.quake.taser.streak.{numberoftaserkill}"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name2 = "";
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_int = numberoftaserkill;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer[$"center.announce.quake.taser.streak.{numberoftaserkill}"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(ZIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }

                                            
                                        }else
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (ZShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.taser"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.taser"], attacker.PlayerName, victim.PlayerName);
                                                }
                                            }
                                            
                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (ZShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.taser"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_Name2 = victim.PlayerName;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.taser"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(ZIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }
                                    }
                                });

                            }else
                            {
                                if(personData.quakesounds)
                                {

                                }else
                                {
                                    attacker.ExecuteClientCommand("play " + ZsoundPath);
                                }
                                
                                if(ZSteak)
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (ZShowChat && !string.IsNullOrEmpty(Localizer[$"chat.quake.taser.streak.{numberoftaserkill}"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer[$"chat.quake.taser.streak.{numberoftaserkill}"], numberoftaserkill);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (ZShowCenter && !string.IsNullOrEmpty(Localizer[$"center.quake.taser.streak.{numberoftaserkill}"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name2 = "";
                                                    Globals.ShowHud_Kill_Name = "";
                                                    Globals.ShowHud_Kill_int = numberoftaserkill;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.taser.streak.{numberoftaserkill}"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(ZIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }else
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (ZShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.taser"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.taser"], victim.PlayerName);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (ZShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.taser"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name = victim.PlayerName;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.taser"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(ZIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }
                            }
                            Globals.lastPlayTimesTaser[playeridattacker] = DateTime.Now;
                            return HookResult.Continue; 
                        }
                    }


                    string HsoundPath = "";
                    double HInterval = 5;
                    float HIntervalHUD = 10;
                    bool Hanouncement = false;
                    bool HSteak = false;
                    bool HShowChat = false;
                    bool HShowCenter = false;
                    if (json.ContainsKey("HeadShot"))
                    {
                        HSteak = false;
                        HsoundPath = json["HeadShot"]["Path"]?.ToString()!;
                        if (!json["HeadShot"].ContainsKey("Interval_InSecs")) HInterval = 5; else double.TryParse(json["HeadShot"]["Interval_InSecs"]?.ToString(), out HInterval);
                        if (!json["HeadShot"].ContainsKey("ShowCenter_InSecs")) HIntervalHUD = 0; else float.TryParse(json["HeadShot"]["ShowCenter_InSecs"]?.ToString(), out HIntervalHUD);
                        if (!json["HeadShot"].ContainsKey("Announcement")) Hanouncement = false; else bool.TryParse(json["HeadShot"]["Announcement"]?.ToString(), out Hanouncement);
                        if (!json["HeadShot"].ContainsKey("ShowChat")) HShowChat = false; else bool.TryParse(json["HeadShot"]["ShowChat"]?.ToString(), out HShowChat);
                        if (!json["HeadShot"].ContainsKey("ShowCenter")) HShowCenter = false; else bool.TryParse(json["HeadShot"]["ShowCenter"]?.ToString(), out HShowCenter);
                    }else if (json.ContainsKey("HeadShot_" + numberofkillsHS))
                    {
                        HSteak = true;
                        HsoundPath = json["HeadShot_" + numberofkillsHS]["Path"]?.ToString()!;
                        if (!json["HeadShot_" + numberofkillsHS].ContainsKey("Interval_InSecs")) HInterval = 5; else double.TryParse(json["HeadShot_" + numberofkillsHS]["Interval_InSecs"]?.ToString(), out HInterval);
                        if (!json["HeadShot_" + numberofkillsHS].ContainsKey("ShowCenter_InSecs")) HIntervalHUD = 0; else float.TryParse(json["HeadShot_" + numberofkillsHS]["ShowCenter_InSecs"]?.ToString(), out HIntervalHUD);
                        if (!json["HeadShot_" + numberofkillsHS].ContainsKey("Announcement")) Hanouncement = false; else bool.TryParse(json["HeadShot_" + numberofkillsHS]["Announcement"]?.ToString(), out Hanouncement);
                        if (!json["HeadShot_" + numberofkillsHS].ContainsKey("ShowChat")) HShowChat = false; else bool.TryParse(json["HeadShot_" + numberofkillsHS]["ShowChat"]?.ToString(), out HShowChat);
                        if (!json["HeadShot_" + numberofkillsHS].ContainsKey("ShowCenter")) HShowCenter = false; else bool.TryParse(json["HeadShot_" + numberofkillsHS]["ShowCenter"]?.ToString(), out HShowCenter);
                    }
                    if (headshot && !string.IsNullOrEmpty(HsoundPath) && HInterval > 0)
                    {
                        if (!Globals.lastPlayTimesHS.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimesHS[playeridattacker]).TotalSeconds >= HInterval)
                        {
                            if(Hanouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid && !players.IsBot)
                                    {
                                        Helper.PersonData personDataa = Helper.RetrievePersonDataById(players.SteamID);
                                        if(personDataa.quakesounds)
                                        {
                                            //skip
                                        }else
                                        {
                                            players.ExecuteClientCommand("play " + HsoundPath);
                                        }
                                        
                                        if(HSteak)
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (HShowChat && !string.IsNullOrEmpty(Localizer[$"chat.announce.quake.headshot.streak.{numberofkillsHS}"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer[$"chat.announce.quake.headshot.streak.{numberofkillsHS}"], attacker.PlayerName, numberofkillsHS);
                                                }
                                            }
                                            

                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (HShowCenter && !string.IsNullOrEmpty(Localizer[$"center.announce.quake.headshot.streak.{numberofkillsHS}"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name2 = "";
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_int = numberofkillsHS;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer[$"center.announce.quake.headshot.streak.{numberofkillsHS}"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(HIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }else
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (HShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.headshot"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.headshot"], attacker.PlayerName, victim.PlayerName);
                                                }
                                            }
                                            
                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (HShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.headshot"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_Name2 = victim.PlayerName;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.headshot"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(HIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }
                                    }
                                });

                            }else
                            {
                                if(personData.quakesounds)
                                {

                                }else
                                {
                                    attacker.ExecuteClientCommand("play " + HsoundPath);
                                }
                                
                                if(HSteak)
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {   
                                        if (HShowChat && !string.IsNullOrEmpty(Localizer[$"chat.quake.headshot.streak.{numberofkillsHS}"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer[$"chat.quake.headshot.streak.{numberofkillsHS}"], numberofkillsHS);
                                        }
                                    }

                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (HShowCenter && !string.IsNullOrEmpty(Localizer[$"center.quake.headshot.streak.{numberofkillsHS}"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name2 = "";
                                                    Globals.ShowHud_Kill_int = numberofkillsHS;
                                                    Globals.ShowHud_Kill_Name = "";
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.headshot.streak.{numberofkillsHS}"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(HIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }else
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (HShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.headshot"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.headshot"], victim.PlayerName);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (HShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.headshot"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name = victim.PlayerName;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.headshot"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(HIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }
                            }
                            Globals.lastPlayTimesHS[playeridattacker] = DateTime.Now;
                            return HookResult.Continue; 
                        }
                    }
                    
                    string soundPath = "";
                    double Interval = 5;
                    float IntervalHUD = 10;
                    bool Kanouncement = false;
                    bool KSteak = false;
                    bool KShowChat = false;
                    bool KShowCenter = false;
                    if (json.ContainsKey("Kill"))
                    {
                        KSteak = false;
                        soundPath = json["Kill"]["Path"]?.ToString()!;
                        if (!json["Kill"].ContainsKey("Interval_InSecs")) Interval = 5; else double.TryParse(json["Kill"]["Interval_InSecs"]?.ToString(), out Interval);
                        if (!json["Kill"].ContainsKey("ShowCenter_InSecs")) IntervalHUD = 0; else float.TryParse(json["Kill"]["ShowCenter_InSecs"]?.ToString(), out IntervalHUD);
                        if (!json["Kill"].ContainsKey("Announcement")) Kanouncement = false; else bool.TryParse(json["Kill"]["Announcement"]?.ToString(), out Kanouncement);
                        if (!json["Kill"].ContainsKey("ShowChat")) KShowChat = false; else bool.TryParse(json["Kill"]["ShowChat"]?.ToString(), out KShowChat);
                        if (!json["Kill"].ContainsKey("ShowCenter")) KShowCenter = false; else bool.TryParse(json["Kill"]["ShowCenter"]?.ToString(), out KShowCenter);
                    }else if (json.ContainsKey("Kill_" + numberofkills))
                    {
                        KSteak = true;
                        soundPath = json["Kill_" + numberofkills]["Path"]?.ToString()!;
                        if (!json["Kill_" + numberofkills].ContainsKey("Interval_InSecs")) Interval = 5; else double.TryParse(json["Kill_" + numberofkills]["Interval_InSecs"]?.ToString(), out Interval);
                        if (!json["Kill_" + numberofkills].ContainsKey("ShowCenter_InSecs")) IntervalHUD = 0; else float.TryParse(json["Kill_" + numberofkills]["ShowCenter_InSecs"]?.ToString(), out IntervalHUD);
                        if (!json["Kill_" + numberofkills].ContainsKey("Announcement")) Kanouncement = false; else bool.TryParse(json["Kill_" + numberofkills]["Announcement"]?.ToString(), out Kanouncement);
                        if (!json["Kill_" + numberofkills].ContainsKey("ShowChat")) KShowChat = false; else bool.TryParse(json["Kill_" + numberofkills]["ShowChat"]?.ToString(), out KShowChat);
                        if (!json["Kill_" + numberofkills].ContainsKey("ShowCenter")) KShowCenter = false; else bool.TryParse(json["Kill_" + numberofkills]["ShowCenter"]?.ToString(), out KShowCenter);
                    }
                    if (!string.IsNullOrEmpty(soundPath) && Interval > 0)
                    {
                        if (!Globals.lastPlayTimes.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimes[playeridattacker]).TotalSeconds >= Interval)
                        {
                            if(Kanouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid && !players.IsBot)
                                    {
                                        Helper.PersonData personDataa = Helper.RetrievePersonDataById(players.SteamID);
                                        if(personDataa.quakesounds)
                                        {

                                        }else
                                        {
                                            players.ExecuteClientCommand("play " + soundPath);
                                        }
                                        
                                        if(KSteak)
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (KShowChat && !string.IsNullOrEmpty(Localizer[$"chat.announce.quake.kill.streak.{numberofkills}"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer[$"chat.announce.quake.kill.streak.{numberofkills}"], attacker.PlayerName, numberofkills);
                                                }
                                            }
                                            

                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (KShowCenter && !string.IsNullOrEmpty(Localizer[$"center.announce.quake.kill.streak.{numberofkills}"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name2 = "";
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_int = numberofkills;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer[$"center.announce.quake.kill.streak.{numberofkills}"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(IntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }else
                                        {
                                            if(personDataa.quakecmessages)
                                            {

                                            }else
                                            {
                                                if (KShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.kill"]))
                                                {
                                                    Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.kill"], attacker.PlayerName, victim.PlayerName);
                                                }
                                            }
                                            
                                            if(personDataa.quakehmessages)
                                            {

                                            }else
                                            {
                                                if (KShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.kill"]))
                                                {
                                                    Server.NextFrame(() =>
                                                    {
                                                        if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill.Remove(players.SteamID);
                                                        }
                                                        if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                                        {
                                                            Globals.ShowHud_Kill_Name = attacker.PlayerName;
                                                            Globals.ShowHud_Kill_Name2 = victim.PlayerName;
                                                            Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.kill"]);
                                                        }
                                                        HUDTimer?.Kill();
                                                        HUDTimer = null;
                                                        HUDTimer = AddTimer(IntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                                    });
                                                }
                                            }
                                            
                                        }
                                    }
                                });

                            }else
                            {
                                if(personData.quakesounds)
                                {

                                }else
                                {
                                    attacker.ExecuteClientCommand("play " + soundPath);
                                }
                                
                                if(KSteak)
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (KShowChat && !string.IsNullOrEmpty(Localizer[$"chat.quake.kill.streak.{numberofkills}"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer[$"chat.quake.kill.streak.{numberofkills}"], numberofkills);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (KShowCenter && !string.IsNullOrEmpty(Localizer[$"center.quake.kill.streak.{numberofkills}"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name2 = "";
                                                    Globals.ShowHud_Kill_int = numberofkills;
                                                    Globals.ShowHud_Kill_Name = "";
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.kill.streak.{numberofkills}"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(IntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }else
                                {
                                    if(personData.quakecmessages)
                                    {

                                    }else
                                    {
                                        if (KShowChat && !string.IsNullOrEmpty(Localizer["chat.quake.kill"]))
                                        {
                                            Helper.AdvancedPrintToChat(attacker, Localizer["chat.quake.kill"], victim.PlayerName);
                                        }
                                    }
                                    
                                    if(personData.quakehmessages)
                                    {

                                    }else
                                    {
                                        if (KShowCenter && !string.IsNullOrEmpty(Localizer["center.quake.kill"]))
                                        {
                                            Server.NextFrame(() =>
                                            {
                                                if(Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill.Remove(attacker.SteamID);
                                                }
                                                if(!Globals.ShowHud_Kill.ContainsKey(attacker.SteamID))
                                                {
                                                    Globals.ShowHud_Kill_Name = victim.PlayerName;
                                                    Globals.ShowHud_Kill.Add(attacker.SteamID, Localizer[$"center.quake.kill"]);
                                                }
                                                HUDTimer?.Kill();
                                                HUDTimer = null;
                                                HUDTimer = AddTimer(IntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                            });
                                        }
                                    }
                                    
                                }
                            }
                            Globals.lastPlayTimes[playeridattacker] = DateTime.Now;
                        }
                    }
                    
                }catch{}
            }
        }
        return HookResult.Continue;
    }
    
    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(!Configs.GetConfigData().KS_EnableQuakeSounds || @event == null || Helper.IsWarmup())return HookResult.Continue;
        Globals.First_Blood = true;
        Globals.Round_Start = false;

        if (Globals.Timers.IsRunning)
        {
            Globals.Timers.Stop();
        }

        if(Configs.GetConfigData().KS_ResetKillStreakOnEveryRound)
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
        }
        Globals.ShowHud_Kill.Clear();

        try
        {
            string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
            var json = Helper.LoadJsonFromFile(_json);

            string PsoundPath = "";
            float PIntervalHUD = 10;
            bool PShowChat = false;
            bool PShowCenter = false;
            if (json.ContainsKey("RoundPrepare"))
            {
                PsoundPath = json["RoundPrepare"]["Path"]?.ToString()!;
                if (!json["RoundPrepare"].ContainsKey("ShowCenter_InSecs")) PIntervalHUD = 0; else float.TryParse(json["RoundPrepare"]["ShowCenter_InSecs"]?.ToString(), out PIntervalHUD);
                if (!json["RoundPrepare"].ContainsKey("ShowChat")) PShowChat = false; else bool.TryParse(json["RoundPrepare"]["ShowChat"]?.ToString(), out PShowChat);
                if (!json["RoundPrepare"].ContainsKey("ShowCenter")) PShowCenter = false; else bool.TryParse(json["RoundPrepare"]["ShowCenter"]?.ToString(), out PShowCenter);
            }
            
            if (!string.IsNullOrEmpty(PsoundPath))
            {
                var allplayers = Helper.GetAllController();
                allplayers.ForEach(players => 
                {
                    if(players != null && players.IsValid && !players.IsBot)
                    {
                        var playerid = players.SteamID;
                        Helper.PersonData personData = Helper.RetrievePersonDataById(playerid);
                        if(personData.quakesounds)
                        {

                        }else
                        {
                            players.ExecuteClientCommand("play " + PsoundPath);
                        }
                        if(personData.quakecmessages)
                        {

                        }else
                        {
                            if (PShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.roundprepare"]))
                            {
                                Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.roundprepare"]);
                            }
                        }
                        
                        if(personData.quakehmessages)
                        {

                        }else
                        {
                            if (PShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.roundprepare"]))
                            {
                                Server.NextFrame(() =>
                                {
                                    if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                    {
                                        Globals.ShowHud_Kill.Remove(players.SteamID);
                                    }
                                    if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                    {
                                        Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.roundprepare"]);
                                    }
                                    HUDTimer?.Kill();
                                    HUDTimer = null;
                                    HUDTimer = AddTimer(PIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                });
                            }
                        }
                        
                        
                    }
                });
            }
        }catch{}
        
        Globals.Round_Start = true;
        int mp_freezetime = ConVar.Find("mp_freezetime")!.GetPrimitiveValue<int>();
        Globals.Takefreezetime = mp_freezetime;
        Globals.Timers.Start();

        return HookResult.Continue;
    }
    private void HUDTimer_Callback()
    {
        var allplayers = Helper.GetAllController();
        allplayers.ForEach(players => 
        {
            if(players != null && players.IsValid && !players.IsBot)
            {
                if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                {
                    Globals.ShowHud_Kill.Remove(players.SteamID);
                }
            }
        });
    }

    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        var player = @event.Userid;
        
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;
        Helper.PersonData personData = Helper.RetrievePersonDataById(playerid);
        DateTime personDate = DateTime.Now;

        Globals.Kill_Streak.Remove(playerid);
        Globals.Kill_StreakHS.Remove(playerid);
        Globals.Kill_Knife.Remove(playerid);
        Globals.Kill_Nade.Remove(playerid);
        Globals.Kill_Molly.Remove(playerid);
        Globals.Kill_Taser.Remove(playerid);
        Globals.lastPlayTimes.Remove(playerid);
        Globals.lastPlayTimesHS.Remove(playerid);
        Globals.lastPlayTimesKnife.Remove(playerid);
        Globals.lastPlayTimesNade.Remove(playerid);
        Globals.lastPlayTimesMolly.Remove(playerid);
        Globals.lastPlayTimesTaser.Remove(playerid);
        Globals.allow_groups.Remove(playerid);
        Globals.menuon.Remove(playerid);
        Globals.currentIndexDict.Remove(playerid);
        Globals.buttonPressed.Remove(playerid);
        Globals.ShowHud_Kill.Remove(playerid);

        if(Configs.GetConfigData().KS_UseMySql)
        {
            Task.Run(async () =>
            {
                try
                {
                    var connectionSettings = JsonConvert.DeserializeObject<MySqlDataManager.MySqlConnectionSettings>(await File.ReadAllTextAsync(Path.Combine(Path.Combine(ModuleDirectory, "config"), "MySql_Settings.json")));
                    var connectionString = new MySqlConnectionStringBuilder
                    {
                        Server = connectionSettings!.MySqlHost,
                        Port = (uint)connectionSettings.MySqlPort,
                        Database = connectionSettings.MySqlDatabase,
                        UserID = connectionSettings.MySqlUsername,
                        Password = connectionSettings.MySqlPassword
                    }.ConnectionString;
                    
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        await MySqlDataManager.CreatePersonDataTableIfNotExistsAsync(connection);

                        DateTime personDate = DateTime.Now;
                        var personData = Helper.RetrievePersonDataById(playerid);
                        if (personData.PlayerSteamID != 0)
                        {
                            await MySqlDataManager.SaveToMySqlAsync(playerid, Configs.GetConfigData().KS_DefaultValue_FreezeOnOpenMenu ? !personData.freezemenu : personData.freezemenu, Configs.GetConfigData().KS_DefaultValue_HeadShotKillSound ? !personData.headshotkill : personData.headshotkill, Configs.GetConfigData().KS_DefaultValue_HeadShotHitSound ? !personData.headshothit : personData.headshothit, Configs.GetConfigData().KS_DefaultValue_BodyKillSound ? !personData.bodyshotkill : personData.bodyshotkill, Configs.GetConfigData().KS_DefaultValue_BodyHitSound ? !personData.bodyshothit : personData.bodyshothit, personData.quakesounds, personData.quakehmessages, personData.quakecmessages, personDate, connection, connectionSettings);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"======================== ERROR =============================");
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine($"======================== ERROR =============================");
                }
            });
        }

        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        Helper.ClearVariables();
        if (Globals.Timers.IsRunning)
        {
            Globals.Timers.Stop();
        }
        HUDTimer?.Kill();
        HUDTimer = null;
    }
    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables();
        if (Globals.Timers.IsRunning)
        {
            Globals.Timers.Stop();
        }
        HUDTimer?.Kill();
        HUDTimer = null;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        var eventplayer = @event.Userid;
        var eventmessage = @event.Text;
        var player = Utilities.GetPlayerFromUserid(eventplayer);
        
        if (player == null || !player.IsValid)return HookResult.Continue;
        var playerid = player.SteamID;

        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;
        string trimmedMessageStart = eventmessage.TrimStart();
        string message = trimmedMessageStart.TrimEnd();
        string[] KSInGameMenu = Configs.GetConfigData().KS_InGameMenu.Split(',');
        
        if (KSInGameMenu.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_OnlyAllowTheseGroupsToToggle) && !Globals.allow_groups.ContainsKey(playerid))
            {
                if (!string.IsNullOrEmpty(Localizer["player.not.allowed"]))
                {
                    Helper.AdvancedPrintToChat(player, Localizer["player.not.allowed"]);
                }
                return HookResult.Continue;
            }
            Helper.PersonData personData = Helper.RetrievePersonDataById(playerid);
            if(Configs.GetConfigData().KS_DefaultValue_FreezeOnOpenMenu)
            {
                if (personData.freezemenu)
                {
                    //skip
                }else
                {
                    if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                        player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_NONE;
                        Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 0);
                        Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                    }
                }
                
            }else
            {
                if (personData.freezemenu)
                {
                    if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                        player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_NONE;
                        Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 0);
                        Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                    }
                }else
                {
                    //skip
                }
            }

            if (!Globals.menuon.ContainsKey(playerid))
            {
                Globals.menuon.Add(playerid, true);
            }
            if (!Globals.currentIndexDict.ContainsKey(playerid))
            {
                Globals.currentIndexDict.Add(playerid, 0);
            }
            if (!Globals.buttonPressed.ContainsKey(playerid))
            {
                Globals.buttonPressed.Add(playerid, false);
            }
            
        }
        return HookResult.Continue;
    }
    public void OnTick()
    {
        if(Globals.Round_Start)
        {
            if (Globals.Takefreezetime > 0)
            {
                if (Globals.Timers.ElapsedMilliseconds >= 1000)
                {
                    Globals.Takefreezetime--;
                    Globals.Timers.Restart();
                }
            }

            if (Globals.Takefreezetime < 1)
            {
                try
                {
                    string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
                    var json = Helper.LoadJsonFromFile(_json);

                    string RsoundPath = "";
                    float RIntervalHUD = 10;
                    bool RShowChat = false;
                    bool RShowCenter = false;
                    if (json.ContainsKey("RoundStart"))
                    {
                        RsoundPath = json["RoundStart"]["Path"]?.ToString()!;
                        if (!json["RoundStart"].ContainsKey("ShowCenter_InSecs")) RIntervalHUD = 0; else float.TryParse(json["RoundStart"]["ShowCenter_InSecs"]?.ToString(), out RIntervalHUD);
                        if (!json["RoundStart"].ContainsKey("ShowChat")) RShowChat = false; else bool.TryParse(json["RoundStart"]["ShowChat"]?.ToString(), out RShowChat);
                        if (!json["RoundStart"].ContainsKey("ShowCenter")) RShowCenter = false; else bool.TryParse(json["RoundStart"]["ShowCenter"]?.ToString(), out RShowCenter);
                    }
					
                    if (!string.IsNullOrEmpty(RsoundPath))
                    {
						var allplayers = Helper.GetAllController();
						allplayers.ForEach(players => 
						{
							if(players != null && players.IsValid && !players.IsBot)
							{
                                var playerid = players.SteamID;
                                Helper.PersonData personData = Helper.RetrievePersonDataById(playerid);
                                if(personData.quakesounds)
                                {

                                }else
                                {
                                    players.ExecuteClientCommand("play " + RsoundPath);
                                }

								if(personData.quakecmessages)
                                {

                                }else
                                {
                                    if (RShowChat && !string.IsNullOrEmpty(Localizer["chat.announce.quake.roundstart"]))
                                    {
                                        Helper.AdvancedPrintToChat(players, Localizer["chat.announce.quake.roundstart"]);
                                    }
                                }
								
                                if(personData.quakehmessages)
                                {

                                }else
                                {
                                    if (RShowCenter && !string.IsNullOrEmpty(Localizer["center.announce.quake.roundstart"]))
                                    {
                                        Server.NextFrame(() =>
                                        {
                                            if(Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                            {
                                                Globals.ShowHud_Kill.Remove(players.SteamID);
                                            }
                                            if(!Globals.ShowHud_Kill.ContainsKey(players.SteamID))
                                            {
                                                Globals.ShowHud_Kill.Add(players.SteamID, Localizer["center.announce.quake.roundstart"]);
                                            }
                                            HUDTimer?.Kill();
                                            HUDTimer = null;
                                            HUDTimer = AddTimer(RIntervalHUD, HUDTimer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                                        });
                                    }
                                }
								
							}
						});
                    }
                }catch{}
                Globals.Round_Start = false;
                Globals.Timers.Stop();
            }
        }

        foreach (var player in Helper.GetAllController())
        {
            if (player == null || !player.IsValid || !player.PawnIsAlive || player.IsBot || player.IsHLTV) continue;

            var playerid = player.SteamID;
            if (Globals.ShowHud_Kill.ContainsKey(playerid))
            {
                StringBuilder builder = new StringBuilder();
                string test;
                
                if (!string.IsNullOrEmpty(Globals.ShowHud_Kill_Name2))
                {
                    test = Localizer[Globals.ShowHud_Kill[playerid], Globals.ShowHud_Kill_Name, Globals.ShowHud_Kill_Name2];
                }
                else
                {
                    test = Localizer[Globals.ShowHud_Kill[playerid], Globals.ShowHud_Kill_Name, Globals.ShowHud_Kill_int];
                }

                builder.AppendLine(test);
                builder.AppendLine("</div>");
                var centerhtml = builder.ToString();
                player.PrintToCenterHtml(centerhtml);
            }

            if (Globals.menuon.ContainsKey(playerid))
            {
                Helper.PersonData personData = Helper.RetrievePersonDataById(playerid);
                DateTime personDate = DateTime.Now;
                
                List<string> MenuSetupList = new List<string>();
                List<bool> boolValuesList = new List<bool>();

                if (Configs.GetConfigData().KS_AddMenu_FreezeOnOpenMenu)
                {
                    MenuSetupList.Add(Localizer["menu.item.freeze"]);
                    if(Configs.GetConfigData().KS_DefaultValue_FreezeOnOpenMenu)
                    {
                        boolValuesList.Add(personData.freezemenu);
                    }else
                    {
                        boolValuesList.Add(!personData.freezemenu);
                    }
                }

                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_AddMenu_BodyHitSoundPath))
                {
                    MenuSetupList.Add(Localizer["menu.item.bodyhit"]);
                    if(Configs.GetConfigData().KS_DefaultValue_BodyHitSound)
                    {
                        boolValuesList.Add(personData.bodyshothit);
                    }else
                    {
                        boolValuesList.Add(!personData.bodyshothit);
                    }
                }

                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_AddMenu_BodyKillSoundPath))
                {
                    MenuSetupList.Add(Localizer["menu.item.bodykill"]);
                    if(Configs.GetConfigData().KS_DefaultValue_BodyKillSound)
                    {
                        boolValuesList.Add(personData.bodyshotkill);
                    }else
                    {
                        boolValuesList.Add(!personData.bodyshotkill);
                    }
                }

                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_AddMenu_HeadShotHitSoundPath))
                {
                    MenuSetupList.Add(Localizer["menu.item.headshothit"]);
                    
                    if(Configs.GetConfigData().KS_DefaultValue_HeadShotHitSound)
                    {
                        boolValuesList.Add(personData.headshothit);
                    }else
                    {
                        boolValuesList.Add(!personData.headshothit);
                    }
                }

                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_AddMenu_HeadShotKillSoundPath))
                {
                    MenuSetupList.Add(Localizer["menu.item.headshotkill"]);
                    if(Configs.GetConfigData().KS_DefaultValue_HeadShotKillSound)
                    {
                        boolValuesList.Add(personData.headshotkill);
                    }else
                    {
                        boolValuesList.Add(!personData.headshotkill);
                    }
                    
                }

                if (Configs.GetConfigData().KS_AddMenu_QuakeSoundsToggle)
                {
                    MenuSetupList.Add(Localizer["menu.item.quake.sounds"]);
                    boolValuesList.Add(personData.quakesounds);
                }
                if (Configs.GetConfigData().KS_AddMenu_QuakeCenterMessageToggle)
                {
                    MenuSetupList.Add(Localizer["menu.item.quake.center.messages"]);
                    boolValuesList.Add(personData.quakehmessages);
                }
                if (Configs.GetConfigData().KS_AddMenu_QuakeChatMessageToggle)
                {
                    MenuSetupList.Add(Localizer["menu.item.quake.chat.messages"]);
                    boolValuesList.Add(personData.quakecmessages);
                }
                
                string[] MenuSetup = MenuSetupList.ToArray();
                bool[] boolValues = boolValuesList.ToArray();

                Dictionary<string, bool> menuOptionsWithValues = new Dictionary<string, bool>();

                for (int i = 0; i < MenuSetup.Length; i++)
                {
                    menuOptionsWithValues.Add(MenuSetup[i], boolValues[i]);
                }

                
                int currentIndex = Globals.currentIndexDict.ContainsKey(playerid) ? Globals.currentIndexDict[playerid] : 0;
                currentIndex = Math.Max(0, Math.Min(MenuSetup.Length - 1, currentIndex));
                string BottomMenu = string.IsNullOrEmpty(Localizer["menu.bottom"]) ? "" : Localizer["menu.bottom"];
                string Imageleft = string.IsNullOrEmpty(Localizer["menu.left.image"]) ? "" : Localizer["menu.left.image"];
                string ImageRight = string.IsNullOrEmpty(Localizer["menu.right.image"]) ? "" : Localizer["menu.right.image"];
                int visibleOptions = 5; 
                int startIndex = Math.Max(0, currentIndex - (visibleOptions - 1));
                if (player.Buttons == 0)
                {
                    Globals.buttonPressed[playerid] = false;
                }else if (player.Buttons == PlayerButtons.Back && !Globals.buttonPressed[playerid])
                {
                    currentIndex = Math.Min(MenuSetup.Length - 1, currentIndex + 1);
                    Globals.currentIndexDict[playerid] = currentIndex;
                    player.ExecuteClientCommand("play sounds/ui/csgo_ui_contract_type4.vsnd_c");
                    Globals.buttonPressed[playerid] = true;
                }
                else if (player.Buttons == PlayerButtons.Forward && !Globals.buttonPressed[playerid])
                {
                    currentIndex = Math.Max(0, currentIndex - 1);
                    Globals.currentIndexDict[playerid] = currentIndex;
                    player.ExecuteClientCommand("play sounds/ui/csgo_ui_contract_type4.vsnd_c");
                    Globals.buttonPressed[playerid] = true;
                }else if ((player.Buttons == PlayerButtons.Moveleft || player.Buttons == PlayerButtons.Moveright) && !Globals.buttonPressed[playerid] && ((PlayerFlags)player.Pawn.Value!.Flags & PlayerFlags.FL_ONGROUND) == PlayerFlags.FL_ONGROUND)
                {
                    int currentLineIndex = Globals.currentIndexDict[playerid];
                    string currentLineName = MenuSetup[currentLineIndex];
                    if (currentLineName == Localizer["menu.item.freeze"])
                    {
                        personData.freezemenu = !personData.freezemenu;
                        if(Configs.GetConfigData().KS_DefaultValue_FreezeOnOpenMenu)
                        {
                            if(personData.freezemenu)
                            {
                                if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                                player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
                                Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 2);
                                Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                                }
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.freeze.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.freeze.off"]);
                                }
                            }else
                            {
                                if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                                player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_NONE;
                                Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 0);
                                Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                                }
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.freeze.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.freeze.on"]);
                                }
                            }
                        }else
                        {
                            if(personData.freezemenu)
                            {
                                if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                                player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_NONE;
                                Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 0);
                                Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                                }
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.freeze.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.freeze.on"]);
                                }
                            }else
                            {
                                if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                                player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
                                Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 2);
                                Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                                }
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.freeze.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.freeze.off"]);
                                }
                            }
                        }
                        
                        Helper.SaveToJsonFile(playerid, personData.freezemenu, personData.headshotkill, personData.headshothit, personData.bodyshotkill, personData.bodyshothit,personData.quakesounds,personData.quakehmessages,personData.quakecmessages, personDate);
                    }
                    else if (currentLineName == Localizer["menu.item.bodyhit"])
                    {
                        personData.bodyshothit = !personData.bodyshothit;
                        if(Configs.GetConfigData().KS_DefaultValue_BodyHitSound)
                        {
                            if(personData.bodyshothit)
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.bodyhit.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.bodyhit.off"]);
                                }
                            }else
                            {
                                
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.bodyhit.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.bodyhit.on"]);
                                }
                            }
                        }else
                        {
                            if(personData.bodyshothit)
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.bodyhit.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.bodyhit.on"]);
                                }
                            }else
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.bodyhit.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.bodyhit.off"]);
                                }
                            }
                        }
                        
                        Helper.SaveToJsonFile(playerid, personData.freezemenu, personData.headshotkill, personData.headshothit, personData.bodyshotkill, personData.bodyshothit,personData.quakesounds,personData.quakehmessages,personData.quakecmessages, personDate);
                    }
                    else if (currentLineName == Localizer["menu.item.bodykill"])
                    {
                        personData.bodyshotkill = !personData.bodyshotkill;
                        if(Configs.GetConfigData().KS_DefaultValue_BodyKillSound)
                        {
                            if(personData.bodyshotkill)
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.bodykill.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.bodykill.off"]);
                                }
                            }else
                            {
                                
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.bodykill.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.bodykill.on"]);
                                }
                            }
                        }else
                        {
                            if(personData.bodyshotkill)
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.bodykill.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.bodykill.on"]);
                                }
                            }else
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.bodykill.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.bodykill.off"]);
                                }
                            }
                        }
                        Helper.SaveToJsonFile(playerid, personData.freezemenu, personData.headshotkill, personData.headshothit, personData.bodyshotkill, personData.bodyshothit,personData.quakesounds,personData.quakehmessages,personData.quakecmessages, personDate);
                    }
                    else if (currentLineName == Localizer["menu.item.headshothit"])
                    {
                        personData.headshothit = !personData.headshothit;
                        if(Configs.GetConfigData().KS_DefaultValue_HeadShotHitSound)
                        {
                            if(personData.headshothit)
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.headshothit.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.headshothit.off"]);
                                }
                            }else
                            {
                                
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.headshothit.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.headshothit.on"]);
                                }
                            }
                        }else
                        {
                            if(personData.headshothit)
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.headshothit.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.headshothit.on"]);
                                }
                            }else
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.headshothit.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.headshothit.off"]);
                                }
                            }
                        }
                        Helper.SaveToJsonFile(playerid, personData.freezemenu, personData.headshotkill, personData.headshothit, personData.bodyshotkill, personData.bodyshothit,personData.quakesounds,personData.quakehmessages,personData.quakecmessages, personDate);
                    }
                    else if (currentLineName == Localizer["menu.item.headshotkill"])
                    {
                        personData.headshotkill = !personData.headshotkill;
                        if(Configs.GetConfigData().KS_DefaultValue_HeadShotKillSound)
                        {
                            if(personData.headshotkill)
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.headshotkill.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.headshotkill.off"]);
                                }
                            }else
                            {
                                
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.headshotkill.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.headshotkill.on"]);
                                }
                            }
                        }else
                        {
                            if(personData.headshotkill)
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.headshotkill.on"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.headshotkill.on"]);
                                }
                            }else
                            {
                                if (!string.IsNullOrEmpty(Localizer["player.toggle.headshotkill.off"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["player.toggle.headshotkill.off"]);
                                }
                            }
                        }
                        Helper.SaveToJsonFile(playerid, personData.freezemenu, personData.headshotkill, personData.headshothit, personData.bodyshotkill, personData.bodyshothit,personData.quakesounds,personData.quakehmessages,personData.quakecmessages, personDate);
                    }
                    else if (currentLineName == Localizer["menu.item.quake.sounds"])
                    {
                        personData.quakesounds = !personData.quakesounds;
                        
                        if(personData.quakesounds)
                        {
                            if (!string.IsNullOrEmpty(Localizer["player.toggle.quake.sounds.off"]))
                            {
                                Helper.AdvancedPrintToChat(player, Localizer["player.toggle.quake.sounds.off"]);
                            }
                        }else
                        {
                            
                            if (!string.IsNullOrEmpty(Localizer["player.toggle.quake.sounds.on"]))
                            {
                                Helper.AdvancedPrintToChat(player, Localizer["player.toggle.quake.sounds.on"]);
                            }
                        }
                        Helper.SaveToJsonFile(playerid, personData.freezemenu, personData.headshotkill, personData.headshothit, personData.bodyshotkill, personData.bodyshothit,personData.quakesounds,personData.quakehmessages,personData.quakecmessages, personDate);
                        
                    }
                    else if (currentLineName == Localizer["menu.item.quake.center.messages"])
                    {
                        personData.quakehmessages = !personData.quakehmessages;
                        if(personData.quakehmessages)
                        {
                            if (!string.IsNullOrEmpty(Localizer["player.toggle.quake.center.message.off"]))
                            {
                                Helper.AdvancedPrintToChat(player, Localizer["player.toggle.quake.center.message.off"]);
                            }
                        }else
                        {
                            
                            if (!string.IsNullOrEmpty(Localizer["player.toggle.quake.center.message.on"]))
                            {
                                Helper.AdvancedPrintToChat(player, Localizer["player.toggle.quake.center.message.on"]);
                            }
                        }
                        Helper.SaveToJsonFile(playerid, personData.freezemenu, personData.headshotkill, personData.headshothit, personData.bodyshotkill, personData.bodyshothit,personData.quakesounds,personData.quakehmessages,personData.quakecmessages, personDate);
                    }
                    else if (currentLineName == Localizer["menu.item.quake.chat.messages"])
                    {
                        personData.quakecmessages = !personData.quakecmessages;
                        if(personData.quakecmessages)
                        {
                            if (!string.IsNullOrEmpty(Localizer["player.toggle.quake.chat.message.off"]))
                            {
                                Helper.AdvancedPrintToChat(player, Localizer["player.toggle.quake.chat.message.off"]);
                            }
                        }else
                        {
                            
                            if (!string.IsNullOrEmpty(Localizer["player.toggle.quake.chat.message.on"]))
                            {
                                Helper.AdvancedPrintToChat(player, Localizer["player.toggle.quake.chat.message.on"]);
                            }
                        }
                        Helper.SaveToJsonFile(playerid, personData.freezemenu, personData.headshotkill, personData.headshothit, personData.bodyshotkill, personData.bodyshothit,personData.quakesounds,personData.quakehmessages,personData.quakecmessages, personDate);
                    }
                    
                    player.ExecuteClientCommand("play sounds/ui/item_sticker_select.vsnd_c");
                    Globals.buttonPressed[playerid] = true;
                }else if ((long)player.Buttons == 8589934592 && !Globals.buttonPressed[playerid])
                {
                    if (Globals.currentIndexDict.ContainsKey(playerid))
                    {
                        Globals.currentIndexDict.Remove(playerid);
                    }
                    if(Globals.buttonPressed.ContainsKey(playerid))
                    {
                        Globals.buttonPressed.Remove(playerid);
                    }
                    if(Globals.menuon.ContainsKey(playerid))
                    {
                        Globals.menuon.Remove(playerid);
                    }
                    if(player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid){
                    player.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
                    Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 2);
                    Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                    }
                    player.ExecuteClientCommand("play sounds/ui/menu_focus.vsnd_c");
                    Globals.buttonPressed[playerid] = true;
                }


                if (Globals.currentIndexDict.ContainsKey(playerid))
                {
                    StringBuilder builder = new StringBuilder();

                    for (int i = startIndex; i < startIndex + visibleOptions && i < MenuSetup.Length; i++)
                    {
                        string currentMenuOption = MenuSetup[i];
                        bool status = menuOptionsWithValues.ContainsKey(currentMenuOption) ? menuOptionsWithValues[currentMenuOption] : false;

                        if (i == currentIndex)
                        {
                            string statusText = status ?  "<font color='red'>Off</font>" : "<font color='lime'>On</font>";
                            string lineHtml = $"<font color='orange'> {Imageleft} {currentMenuOption} : {statusText} {ImageRight} </font><br>";
                            builder.AppendLine(lineHtml);
                        }
                        else
                        {
                            string statusText = status ?  "<font color='red'>Off</font>" : "<font color='lime'>On</font>";
                            string lineHtml = $"<font color='white' class='fontSize-sm'>  {currentMenuOption} : {statusText}  </font><br>";
                            builder.AppendLine(lineHtml);
                        }
                    }
                    if (startIndex + visibleOptions < MenuSetup.Length)
                    {
                        string moreItemsIndicator = Localizer["menu.more.down"];
                        builder.AppendLine(moreItemsIndicator);
                    }
                    builder.AppendLine("<br>" + BottomMenu);
                    builder.AppendLine("</div>");
                    var centerhtml = builder.ToString();
                    player?.PrintToCenterHtml(centerhtml);
                }
            }
        }
    }
    /* private HookResult OnMatchStart(EventRoundAnnounceMatchStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        
        return HookResult.Continue;
    } */
}