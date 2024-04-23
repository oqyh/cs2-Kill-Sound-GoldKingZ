using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API;
using System.Text;
using Newtonsoft.Json;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Memory;
using Kill_Sound_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Entities;
using System.Collections;
using System.Diagnostics;
using System;
using System.Timers;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace Kill_Sound_GoldKingZ;

[MinimumApiVersion(164)]

public class KillSoundGoldKingZ : BasePlugin
{
    public override string ModuleName => "Kill Sound ( Kill , HeadShot , Quake )";
    public override string ModuleVersion => "1.0.5";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    internal static IStringLocalizer? Stringlocalizer;
    public override void Load(bool hotReload)
    {
        Configs.Load(ModuleDirectory);
        Stringlocalizer = Localizer;
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeathQuake);
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        /* RegisterEventHandler<EventPlayerChat>(OnEventPlayerChat, HookMode.Post); */
        /* RegisterEventHandler<EventRoundAnnounceMatchStart>(OnMatchStart); */
    }

    

    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        var victim = @event.Userid;
        var attacker = @event.Attacker;

        if (victim == null || !victim.IsValid)return HookResult.Continue;
        if (attacker == null || !attacker.IsValid || attacker.IsBot)return HookResult.Continue;
        var headshot = @event.Headshot;

        if (attacker != victim)
        {
            if(headshot)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_HeadShotKillSoundPath))
                {
                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_HeadShotKillSoundPath);
                }
            }else
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_BodyKillSoundPath))
                {
                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_BodyKillSoundPath);
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
        if (attacker == null || !attacker.IsValid || attacker.IsBot)return HookResult.Continue;

        if (attacker != victim)
        {
            if(hitgroup == 1)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_HeadShotHitSoundPath))
                {
                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_HeadShotHitSoundPath);
                }
            }else
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().KS_BodyHitSoundPath))
                {
                    attacker.ExecuteClientCommand("play " + Configs.GetConfigData().KS_BodyHitSoundPath);
                }
            }
        }

        return HookResult.Continue;
    }

    private HookResult OnPlayerDeathQuake(EventPlayerDeath @event, GameEventInfo info)
    {
        if(!Configs.GetConfigData().KS_EnableQuakeSounds || @event == null)return HookResult.Continue;
        var victim = @event.Userid;
        var attacker = @event.Attacker;
        bool KilledHimSelf = @event.Weapon.Contains("worldent");
        if (KilledHimSelf)
        {
            try
            {
                string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
                var json = Helper.LoadJsonFromFile(_json);

                string selfkill = "";
                bool Sanouncement = false;
                if (json.ContainsKey("SelfKill"))
                {
                    selfkill = json["SelfKill"]?["Path"]?.ToString()!;
                    if (!json["SelfKill"].ContainsKey("Announcement")) Sanouncement = false; else bool.TryParse(json["SelfKill"]["Announcement"]?.ToString(), out Sanouncement);
                }

                if (!string.IsNullOrEmpty(selfkill))
                {
                    if(Sanouncement)
                    {
                        var allplayers = Helper.GetAllController();
                        allplayers.ForEach(players => 
                        {
                            if(players != null && players.IsValid)
                            {
                                players.ExecuteClientCommand("play " + selfkill);
                                if (!string.IsNullOrEmpty(Localizer["announce.quake.selfkill"]))
                                {
                                    Helper.AdvancedPrintToChat(players, Localizer["announce.quake.selfkill"], victim.PlayerName);
                                }
                            }
                        });
                    }else
                    {
                        victim.ExecuteClientCommand("play " + selfkill);
                        if (!string.IsNullOrEmpty(Localizer["quake.selfkill"]))
                        {
                            Helper.AdvancedPrintToChat(attacker, Localizer["quake.selfkill"]);
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
        
        var playeridattacker = attacker.SteamID;
        if (attacker == victim)
        {
            try
            {
                string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
                var json = Helper.LoadJsonFromFile(_json);

                string selfkill = "";
                bool Sanouncement = false;
                if (json.ContainsKey("SelfKill"))
                {
                    selfkill = json["SelfKill"]?["Path"]?.ToString()!;
                    if (!json["SelfKill"].ContainsKey("Announcement")) Sanouncement = false; else bool.TryParse(json["SelfKill"]["Announcement"]?.ToString(), out Sanouncement);
                }

                if (!string.IsNullOrEmpty(selfkill))
                {
                    if(Sanouncement)
                    {
                        var allplayers = Helper.GetAllController();
                        allplayers.ForEach(players => 
                        {
                            if(players != null && players.IsValid)
                            {
                                players.ExecuteClientCommand("play " + selfkill);
                                if (!string.IsNullOrEmpty(Localizer["announce.quake.selfkill"]))
                                {
                                    Helper.AdvancedPrintToChat(players, Localizer["announce.quake.selfkill"], victim.PlayerName);
                                }
                            }
                        });
                    }else
                    {
                        attacker.ExecuteClientCommand("play " + selfkill);
                        if (!string.IsNullOrEmpty(Localizer["quake.selfkill"]))
                        {
                            Helper.AdvancedPrintToChat(attacker, Localizer["quake.selfkill"]);
                        }
                        
                    }
                    
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
            
            if (Globals.Kill_Streak.ContainsKey(playeridattacker) || Globals.Kill_StreakHS.ContainsKey(playeridattacker) || Globals.Kill_Knife.ContainsKey(playeridattacker) || Globals.Kill_Nade.ContainsKey(playeridattacker) || Globals.Kill_Molly.ContainsKey(playeridattacker))
            {
                if (attackerteam != victimteam) Globals.Kill_Streak[playeridattacker]++;
                if (headshot) Globals.Kill_StreakHS[playeridattacker]++;
                if (knifekill) Globals.Kill_Knife[playeridattacker]++;
                if (NadeKill) Globals.Kill_Nade[playeridattacker]++;
                if (MollyKill) Globals.Kill_Molly[playeridattacker]++;

                int numberofkills = Globals.Kill_Streak[playeridattacker];
                int numberofkillsHS = Globals.Kill_StreakHS[playeridattacker];
                int numberofknifekill = Globals.Kill_Knife[playeridattacker];
                int numberofnadekill = Globals.Kill_Nade[playeridattacker];
                int numberofmollykill = Globals.Kill_Molly[playeridattacker];

                try
                {
                    string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
                    var json = Helper.LoadJsonFromFile(_json);

                    string soundPathhteamkill = "";
                    bool Tanouncement = false;
                    if (json.ContainsKey("TeamKill"))
                    {
                        soundPathhteamkill = json["TeamKill"]?["Path"]?.ToString()!;
                        if (!json["TeamKill"].ContainsKey("Announcement")) Tanouncement = false; else bool.TryParse(json["TeamKill"]["Announcement"]?.ToString(), out Tanouncement);
                    }

                    if (attackerteam == victimteam && !string.IsNullOrEmpty(soundPathhteamkill))
                    {
                        if(Tanouncement)
                        {
                            var allplayers = Helper.GetAllController();
                            allplayers.ForEach(players => 
                            {
                                if(players != null && players.IsValid)
                                {
                                    players.ExecuteClientCommand("play " + soundPathhteamkill);
                                    if (!string.IsNullOrEmpty(Localizer["announce.quake.teamkill"]))
                                    {
                                        Helper.AdvancedPrintToChat(players, Localizer["announce.quake.teamkill"], attacker.PlayerName, victim.PlayerName);
                                    }
                                }
                            });
                        }else
                        {
                            attacker.ExecuteClientCommand("play " + soundPathhteamkill);
                            if (!string.IsNullOrEmpty(Localizer["quake.teamkill"]))
                            {
                                Helper.AdvancedPrintToChat(attacker, Localizer["quake.teamkill"], victim.PlayerName);
                            }
                        }
                        return HookResult.Continue; 
                    }

                    string soundPathhfirstpath = "";
                    bool Fanouncement = false;
                    if (json.ContainsKey("FirstBlood"))
                    {
                        soundPathhfirstpath = json["FirstBlood"]?["Path"]?.ToString()!;
                        if (!json["FirstBlood"].ContainsKey("Announcement")) Fanouncement = false; else bool.TryParse(json["FirstBlood"]["Announcement"]?.ToString(), out Fanouncement);
                    }

                    if (Globals.First_Blood && !string.IsNullOrEmpty(soundPathhfirstpath))
                    {
                        if(Fanouncement)
                        {
                            var allplayers = Helper.GetAllController();
                            allplayers.ForEach(players => 
                            {
                                if(players != null && players.IsValid)
                                {
                                    players.ExecuteClientCommand("play " + soundPathhfirstpath);
                                    if (!string.IsNullOrEmpty(Localizer["announce.quake.firstblood"]))
                                    {
                                        Helper.AdvancedPrintToChat(players, Localizer["announce.quake.firstblood"], attacker.PlayerName, victim.PlayerName);
                                    }
                                }
                            });
                        }else
                        {
                            attacker.ExecuteClientCommand("play " + soundPathhfirstpath);
                            if (!string.IsNullOrEmpty(Localizer["quake.firstblood"]))
                            {
                                Helper.AdvancedPrintToChat(attacker, Localizer["quake.firstblood"], victim.PlayerName);
                            }
                        }
                        Globals.First_Blood = false;
                        return HookResult.Continue; 
                    }
                    
                    string soundPathhsknife = "";
                    double Intervalknife = 5;
                    bool Nanouncement = false;
                    bool NSteak = false;
                    if (json.ContainsKey("KnifeKill"))
                    {
                        NSteak = false;
                        soundPathhsknife = json["KnifeKill"]?["Path"]?.ToString()!;
                        if (!json["KnifeKill"].ContainsKey("Interval_InSecs")) Intervalknife = 5; else double.TryParse(json["KnifeKill"]["Interval_InSecs"]?.ToString(), out Intervalknife);
                        if (!json["KnifeKill"].ContainsKey("Announcement")) Nanouncement = false; else bool.TryParse(json["KnifeKill"]["Announcement"]?.ToString(), out Nanouncement);
                    }else if (json.ContainsKey("KnifeKill_" + numberofknifekill))
                    {
                        NSteak = true;
                        soundPathhsknife = json["KnifeKill_" + numberofknifekill]?["Path"]?.ToString()!;
                        if (!json["KnifeKill_" + numberofknifekill].ContainsKey("Interval_InSecs")) Intervalknife = 5; else double.TryParse(json["KnifeKill_" + numberofknifekill]["Interval_InSecs"]?.ToString(), out Intervalknife);
                        if (!json["KnifeKill_" + numberofknifekill].ContainsKey("Announcement")) Nanouncement = false; else bool.TryParse(json["KnifeKill_" + numberofknifekill]["Announcement"]?.ToString(), out Nanouncement);
                    }
                    if (knifekill && !string.IsNullOrEmpty(soundPathhsknife) && Intervalknife > 0 )
                    {
                        if (!Globals.lastPlayTimesKnife.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimesKnife[playeridattacker]).TotalSeconds >= Intervalknife)
                        {
                            if(Nanouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid)
                                    {
                                        players.ExecuteClientCommand("play " + soundPathhsknife);
                                        if(NSteak)
                                        {
                                            var checkstreak = Localizer[$"announce.quake.knife.streak.{numberofknifekill}"];
                                            if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"announce.quake.knife.streak.{numberofknifekill}")
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer[$"announce.quake.knife.streak.{numberofknifekill}"], attacker.PlayerName, numberofknifekill);
                                            }else if (!string.IsNullOrEmpty(Localizer["announce.quake.knife.streak"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.knife.streak"], attacker.PlayerName, numberofknifekill);
                                            }
                                        }else
                                        {
                                            if (!string.IsNullOrEmpty(Localizer["announce.quake.knife"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.knife"], attacker.PlayerName, victim.PlayerName);
                                            }
                                        }
                                    }
                                });
                            }else
                            {
                                attacker.ExecuteClientCommand("play " + soundPathhsknife);
                                if(NSteak)
                                {
                                    var checkstreak = Localizer[$"quake.knife.streak.{numberofknifekill}"];
                                    if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"quake.knife.streak.{numberofknifekill}")
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer[$"quake.knife.streak.{numberofknifekill}"], numberofknifekill);
                                    }else if (!string.IsNullOrEmpty(Localizer["quake.knife.streak"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.knife.streak"], numberofknifekill);
                                    }
                                }else
                                {
                                    if (!string.IsNullOrEmpty(Localizer["quake.knife"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.knife"], victim.PlayerName);
                                    }
                                }
                            }
                            Globals.lastPlayTimesKnife[playeridattacker] = DateTime.Now;
                            return HookResult.Continue; 
                        }
                    }

                    string soundPathhsnade = "";
                    double Intervalnade = 5;
                    bool Ganouncement = false;
                    bool GSteak = false;
                    if (json.ContainsKey("GrenadeKill"))
                    {
                        GSteak = false;
                        soundPathhsnade = json["GrenadeKill"]?["Path"]?.ToString()!;
                        if (!json["GrenadeKill"].ContainsKey("Interval_InSecs")) Intervalnade = 5; else double.TryParse(json["GrenadeKill"]["Interval_InSecs"]?.ToString(), out Intervalnade);
                        if (!json["GrenadeKill"].ContainsKey("Announcement")) Ganouncement = false; else bool.TryParse(json["GrenadeKill"]["Announcement"]?.ToString(), out Ganouncement);
                    }else if (json.ContainsKey("GrenadeKill_" + numberofnadekill))
                    {
                        GSteak = true;
                        soundPathhsnade = json["GrenadeKill_" + numberofnadekill]?["Path"]?.ToString()!;
                        if (!json["GrenadeKill_" + numberofnadekill].ContainsKey("Interval_InSecs")) Intervalnade = 5; else double.TryParse(json["GrenadeKill_" + numberofnadekill]["Interval_InSecs"]?.ToString(), out Intervalnade);
                        if (!json["GrenadeKill_" + numberofnadekill].ContainsKey("Announcement")) Ganouncement = false; else bool.TryParse(json["GrenadeKill_" + numberofnadekill]["Announcement"]?.ToString(), out Ganouncement);
                    }
                    if (NadeKill && !string.IsNullOrEmpty(soundPathhsnade) && Intervalnade > 0 )
                    {
                        if (!Globals.lastPlayTimesNade.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimesNade[playeridattacker]).TotalSeconds >= Intervalnade)
                        {
                            if(Ganouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid)
                                    {
                                        players.ExecuteClientCommand("play " + soundPathhsnade);
                                        if(GSteak)
                                        {
                                            var checkstreak = Localizer[$"announce.quake.grenade.streak.{numberofnadekill}"];
                                            if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"announce.quake.grenade.streak.{numberofnadekill}")
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer[$"announce.quake.grenade.streak.{numberofnadekill}"], attacker.PlayerName, numberofnadekill);
                                            }else if (!string.IsNullOrEmpty(Localizer["announce.quake.grenade.streak"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.grenade.streak"], attacker.PlayerName, numberofnadekill);
                                            }
                                        }else
                                        {
                                            if (!string.IsNullOrEmpty(Localizer["announce.quake.grenade"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.grenade"], attacker.PlayerName, victim.PlayerName);
                                            }
                                        }
                                    }
                                });
                            }else
                            {
                                attacker.ExecuteClientCommand("play " + soundPathhsnade);
                                if(GSteak)
                                {
                                    var checkstreak = Localizer[$"quake.grenade.streak.{numberofnadekill}"];
                                    if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"quake.grenade.streak.{numberofnadekill}")
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer[$"quake.grenade.streak.{numberofnadekill}"], numberofnadekill);
                                    }else if (!string.IsNullOrEmpty(Localizer["quake.grenade.streak"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.grenade.streak"], numberofnadekill);
                                    }
                                }else
                                {
                                    if (!string.IsNullOrEmpty(Localizer["quake.grenade"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.grenade"], victim.PlayerName);
                                    }
                                }
                            }
                            Globals.lastPlayTimesNade[playeridattacker] = DateTime.Now;
                            return HookResult.Continue; 
                        }
                    }

                    string soundPathhsmolly = "";
                    double Intervalmolly = 5;
                    bool Manouncement = false;
                    bool MSteak = false;
                    if (json.ContainsKey("MollyKill"))
                    {
                        MSteak = false;
                        soundPathhsmolly = json["MollyKill"]?["Path"]?.ToString()!;
                        if (!json["MollyKill"].ContainsKey("Interval_InSecs")) Intervalmolly = 5; else double.TryParse(json["MollyKill"]["Interval_InSecs"]?.ToString(), out Intervalmolly);
                        if (!json["MollyKill"].ContainsKey("Announcement")) Manouncement = false; else bool.TryParse(json["MollyKill"]["Announcement"]?.ToString(), out Manouncement);
                    }else if (json.ContainsKey("GrenadeKill_" + numberofmollykill))
                    {
                        MSteak = true;
                        soundPathhsmolly = json["MollyKill_" + numberofmollykill]?["Path"]?.ToString()!;
                        if (!json["MollyKill_" + numberofmollykill].ContainsKey("Interval_InSecs")) Intervalmolly = 5; else double.TryParse(json["MollyKill_" + numberofmollykill]["Interval_InSecs"]?.ToString(), out Intervalmolly);
                        if (!json["MollyKill_" + numberofmollykill].ContainsKey("Announcement")) Manouncement = false; else bool.TryParse(json["MollyKill_" + numberofmollykill]["Announcement"]?.ToString(), out Manouncement);
                    }
                    if (MollyKill && !string.IsNullOrEmpty(soundPathhsmolly) && Intervalmolly > 0 )
                    {
                        if (!Globals.lastPlayTimesMolly.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimesMolly[playeridattacker]).TotalSeconds >= Intervalmolly)
                        {
                            if(Manouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid)
                                    {
                                        players.ExecuteClientCommand("play " + soundPathhsmolly);
                                        if(MSteak)
                                        {
                                            var checkstreak = Localizer[$"announce.quake.molly.streak.{numberofmollykill}"];
                                            if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"announce.quake.molly.streak.{numberofmollykill}")
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer[$"announce.quake.molly.streak.{numberofmollykill}"], attacker.PlayerName, numberofmollykill);
                                            }else if (!string.IsNullOrEmpty(Localizer["announce.quake.molly.streak"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.molly.streak"], attacker.PlayerName, numberofmollykill);
                                            }
                                        }else
                                        {
                                            if (!string.IsNullOrEmpty(Localizer["announce.quake.molly"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.molly"], attacker.PlayerName, victim.PlayerName);
                                            }
                                        }
                                    }
                                });
                            }else
                            {
                                attacker.ExecuteClientCommand("play " + soundPathhsmolly);
                                if(MSteak)
                                {
                                    var checkstreak = Localizer[$"quake.molly.streak.{numberofmollykill}"];
                                    if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"quake.molly.streak.{numberofmollykill}")
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer[$"quake.molly.streak.{numberofmollykill}"], numberofmollykill);
                                    }else if (!string.IsNullOrEmpty(Localizer["quake.molly.streak"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.molly.streak"], numberofmollykill);
                                    }
                                }else
                                {
                                    if (!string.IsNullOrEmpty(Localizer["quake.molly"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.molly"], victim.PlayerName);
                                    }
                                }
                            }
                            Globals.lastPlayTimesMolly[playeridattacker] = DateTime.Now;
                            return HookResult.Continue; 
                        }
                    }


                    string soundPathhs = "";
                    double Intervalhhs = 5;
                    bool Hanouncement = false;
                    bool HSteak = false;
                    if (json.ContainsKey("HeadShot"))
                    {
                        HSteak = false;
                        soundPathhs = json["HeadShot"]?["Path"]?.ToString()!;
                        if (!json["HeadShot"].ContainsKey("Interval_InSecs")) Intervalhhs = 5; else double.TryParse(json["HeadShot"]["Interval_InSecs"]?.ToString(), out Intervalhhs);
                        if (!json["HeadShot"].ContainsKey("Announcement")) Hanouncement = false; else bool.TryParse(json["HeadShot"]["Announcement"]?.ToString(), out Hanouncement);
                    }else if (json.ContainsKey("HeadShot_" + numberofkillsHS))
                    {
                        HSteak = true;
                        soundPathhs = json["HeadShot_" + numberofkillsHS]?["Path"]?.ToString()!;
                        if (!json["HeadShot_" + numberofkillsHS].ContainsKey("Interval_InSecs")) Intervalhhs = 5; else double.TryParse(json["HeadShot_" + numberofkillsHS]["Interval_InSecs"]?.ToString(), out Intervalhhs);
                        if (!json["HeadShot_" + numberofkillsHS].ContainsKey("Announcement")) Hanouncement = false; else bool.TryParse(json["HeadShot_" + numberofkillsHS]["Announcement"]?.ToString(), out Hanouncement);
                    }
                    if (headshot && !string.IsNullOrEmpty(soundPathhs) && Intervalhhs > 0 )
                    {
                        if (!Globals.lastPlayTimesHS.ContainsKey(playeridattacker) || (DateTime.Now - Globals.lastPlayTimesHS[playeridattacker]).TotalSeconds >= Intervalhhs)
                        {
                            if(Hanouncement)
                            {
                                var allplayers = Helper.GetAllController();
                                allplayers.ForEach(players => 
                                {
                                    if(players != null && players.IsValid)
                                    {
                                        players.ExecuteClientCommand("play " + soundPathhs);
                                        if(HSteak)
                                        {
                                            var checkstreak = Localizer[$"announce.quake.headshot.streak.{numberofkillsHS}"];
                                            if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"announce.quake.headshot.streak.{numberofkillsHS}")
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer[$"announce.quake.headshot.streak.{numberofkillsHS}"], attacker.PlayerName, numberofkillsHS);
                                            }else if (!string.IsNullOrEmpty(Localizer["announce.quake.headshot.streak"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.headshot.streak"], attacker.PlayerName, numberofkillsHS);
                                            }
                                        }else
                                        {
                                            if (!string.IsNullOrEmpty(Localizer["announce.quake.headshot"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.headshot"], attacker.PlayerName, victim.PlayerName);
                                            }
                                        }
                                    }
                                });
                            }else
                            {
                                attacker.ExecuteClientCommand("play " + soundPathhs);
                                if(HSteak)
                                {
                                    var checkstreak = Localizer[$"quake.headshot.streak.{numberofkillsHS}"];
                                    if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"quake.headshot.streak.{numberofkillsHS}")
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer[$"quake.headshot.streak.{numberofkillsHS}"], numberofkillsHS);
                                    }else if (!string.IsNullOrEmpty(Localizer["quake.headshot.streak"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.headshot.streak"], numberofkillsHS);
                                    }
                                }else
                                {
                                    if (!string.IsNullOrEmpty(Localizer["quake.headshot"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.headshot"], victim.PlayerName);
                                    }
                                }
                            }
                            Globals.lastPlayTimesHS[playeridattacker] = DateTime.Now;
                            return HookResult.Continue; 
                        }
                    }
                    
                    string soundPath = "";
                    double Interval = 5;
                    bool Kanouncement = false;
                    bool KSteak = false;
                    if (json.ContainsKey("Kill"))
                    {
                        KSteak = false;
                        soundPath = json["Kill"]["Path"]?.ToString()!;
                        if (!json["Kill"].ContainsKey("Interval_InSecs")) Interval = 5; else double.TryParse(json["Kill"]["Interval_InSecs"]?.ToString(), out Interval);
                        if (!json["Kill"].ContainsKey("Announcement")) Kanouncement = false; else bool.TryParse(json["Kill"]["Announcement"]?.ToString(), out Kanouncement);
                    }else if (json.ContainsKey("Kill_" + numberofkills))
                    {
                        KSteak = true;
                        soundPath = json["Kill_" + numberofkills]["Path"]?.ToString()!;
                        if (!json["Kill_" + numberofkills].ContainsKey("Interval_InSecs")) Interval = 5; else double.TryParse(json["Kill_" + numberofkills]["Interval_InSecs"]?.ToString(), out Interval);
                        if (!json["Kill_" + numberofkills].ContainsKey("Announcement")) Kanouncement = false; else bool.TryParse(json["Kill_" + numberofkills]["Announcement"]?.ToString(), out Kanouncement);
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
                                    if(players != null && players.IsValid)
                                    {
                                        players.ExecuteClientCommand("play " + soundPath);
                                        if(KSteak)
                                        {
                                            var checkstreak = Localizer[$"announce.quake.kill.streak.{numberofkills}"];
                                            if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"announce.quake.kill.streak.{numberofkills}")
                                            {
                                                Helper.AdvancedPrintToChat(players, checkstreak, attacker.PlayerName, numberofkills);
                                            }
                                            else if (!string.IsNullOrEmpty(Localizer["announce.quake.kill.streak"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.kill.streak"], attacker.PlayerName, numberofkills);
                                            }
                                        }else
                                        {
                                            if (!string.IsNullOrEmpty(Localizer["announce.quake.kill"]))
                                            {
                                                Helper.AdvancedPrintToChat(players, Localizer["announce.quake.kill"], attacker.PlayerName, victim.PlayerName);
                                            }
                                        }
                                        
                                        
                                    }
                                });
                            }else
                            {
                                attacker.ExecuteClientCommand("play " + soundPath);
                                if(KSteak)
                                {
                                    var checkstreak = Localizer[$"quake.kill.streak.{numberofkills}"];
                                    if (!string.IsNullOrEmpty(checkstreak) && checkstreak != $"quake.kill.streak.{numberofkills}")
                                    {
                                        Helper.AdvancedPrintToChat(attacker, checkstreak, numberofkills);
                                    }else if (!string.IsNullOrEmpty(Localizer["quake.kill.streak"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.kill.streak"], numberofkills);
                                    }

                                }else
                                {
                                    if (!string.IsNullOrEmpty(Localizer["quake.kill"]))
                                    {
                                        Helper.AdvancedPrintToChat(attacker, Localizer["quake.kill"], victim.PlayerName);
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
        Helper.ClearVariables();
        try
        {
            string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
            var json = Helper.LoadJsonFromFile(_json);

            string Prepare = "";
            if (json.ContainsKey("RoundPrepare"))
            {
                Prepare = json["RoundPrepare"]?["Path"]?.ToString()!;
            }

            if (!string.IsNullOrEmpty(Prepare))
            {
                var allplayers = Helper.GetAllController();
                allplayers.ForEach(players => 
                {
                    if(players != null && players.IsValid)
                    {
                        players.ExecuteClientCommand("play " + Prepare);
                        if (!string.IsNullOrEmpty(Localizer["announce.round.prepare"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["announce.round.prepare"]);
                        }
                    }
                });
            }
        }catch{}

        int mp_freezetime = ConVar.Find("mp_freezetime")!.GetPrimitiveValue<int>();
        Globals.Takefreezetime = mp_freezetime;
        Globals.Timers.Start();
        RegisterListener<Listeners.OnTick>(OnTick);

        return HookResult.Continue;
    }
    
    private HookResult OnMatchStart(EventRoundAnnounceMatchStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        
        return HookResult.Continue;
    }
    
    public void OnTick()
    {
        if (Globals.Takefreezetime < 1)
        {
            try
            {
                string _json = Path.Combine(ModuleDirectory, "../../plugins/Kill-Sound-GoldKingZ/config/Kill_Settings.json");
                var json = Helper.LoadJsonFromFile(_json);

                string Roundstart = "";
                if (json.ContainsKey("RoundStart"))
                {
                    Roundstart = json["RoundStart"]?["Path"]?.ToString()!;
                }

                if (!string.IsNullOrEmpty(Roundstart))
                {
                    var allplayers = Helper.GetAllController();
                    allplayers.ForEach(players => 
                    {
                        if(players != null && players.IsValid)
                        {
                            players.ExecuteClientCommand("play " + Roundstart);
                            if (!string.IsNullOrEmpty(Localizer["announce.round.start"]))
                            {
                                Helper.AdvancedPrintToServer(Localizer["announce.round.start"]);
                            }
                        }
                    });
                }
            }catch{}

            Globals.Timers.Stop();
            RemoveOnTickListener();
        }
        
        if (Globals.Takefreezetime > 0)
        {
            if (Globals.Timers.ElapsedMilliseconds >= 1000)
            {
                Globals.Takefreezetime--;
                Globals.Timers.Restart();
            }
        }
    }
    private void RemoveOnTickListener()
    {
        var onTick = new Listeners.OnTick(OnTick);
        RemoveListener("OnTick", onTick);
    }

    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        var player = @event.Userid;
        
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;

        Globals.Kill_Streak.Remove(playerid);
        Globals.Kill_StreakHS.Remove(playerid);
        Globals.Kill_Knife.Remove(playerid);
        Globals.Kill_Nade.Remove(playerid);
        Globals.Kill_Molly.Remove(playerid);
        Globals.lastPlayTimes.Remove(playerid);
        Globals.lastPlayTimesHS.Remove(playerid);
        Globals.lastPlayTimesKnife.Remove(playerid);
        Globals.lastPlayTimesNade.Remove(playerid);
        Globals.lastPlayTimesMolly.Remove(playerid);

        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        Helper.ClearVariables();
    }
    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables();
    }

    /* public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        var eventplayer = @event.Userid;
        var eventmessage = @event.Text;
        var Caller = Utilities.GetPlayerFromUserid(eventplayer);
        

        if (Caller == null || !Caller.IsValid)return HookResult.Continue;
        var CallerTeam = Caller.TeamNum;
        var CallerSteamID = Caller.SteamID;
        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;
        string trimmedMessageStart = eventmessage.TrimStart();
        string message = trimmedMessageStart.TrimEnd();
        string[] InGameCommands = Configs.GetConfigData().InGame_Commands.Split(',');
        
        if (InGameCommands.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            
        }
        return HookResult.Continue;
    } */
}