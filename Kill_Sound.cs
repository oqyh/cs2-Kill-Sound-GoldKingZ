using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace Kill_Sound;

public class KillSoundConfig : BasePluginConfig
{
    [JsonPropertyName("HeadShotSoundPath")] public string HeadShotSoundPath { get; set; } = "sounds/training/bell_normal.vsnd_c";
}

public class KillSound : BasePlugin, IPluginConfig<KillSoundConfig>
{
    public override string ModuleName => "Kill Sound";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Kill Sound , HeadShot";
    public KillSoundConfig Config { get; set; } = new KillSoundConfig();
    public void OnConfigParsed(KillSoundConfig config)
    {
        Config = config;
    }
    
    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerDeath>(EventPlayerDeath);
    }
    private HookResult EventPlayerDeath(EventPlayerDeath @event, GameEventInfo _)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;
        var headshot = @event.Headshot;

        if (attacker.IsValid && victim.IsValid && (attacker != victim) && !attacker.IsBot)
        {
            if (headshot)
            {
                if (!string.IsNullOrEmpty(Config.HeadShotSoundPath))
                {
                    attacker.ExecuteClientCommand("play " + Config.HeadShotSoundPath);
                }
            }
        }
        return HookResult.Continue;
    }
}