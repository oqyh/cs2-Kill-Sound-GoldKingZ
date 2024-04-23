# [CS2] Kill-Sound-GoldKingZ (1.0.5)

### Kill Sound ( Kill , HeadShot , Quake )

![configs](https://github.com/oqyh/cs2-Kill-Sound-GoldKingZ/assets/48490385/3c08cec9-6e29-4c00-8d74-bc9ae3cf4f85)


## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## .:[ Configuration ]:.

> [!CAUTION]
> Config Located In ..\addons\counterstrikesharp\plugins\Kill-Sound-GoldKingZ\config\config.json                                           
>

> [!NOTE]
> To Use Modify Version And Lower Volume                                                                                                                              
> Download https://github.com/Source2ZE/MultiAddonManager  With Gold KingZ WorkShop                                                                                                                             
> https://steamcommunity.com/sharedfiles/filedetails/?id=3230015783                                                                                                                             
> mm_extra_addons 3230015783                                                                                                                             
> OtherWise Use Normal Sounds https://github.com/oqyh/cs2-Kill-Sound-GoldKingZ/blob/main/sounds/sounds.txt                                                                                                                              

```json
{
  //To Enable Kill_Settings.json 
  "KS_EnableQuakeSounds": false,

  //If You Want To Disable Any Make it Empty Like This ""
  //Sound HeadShot Kill
  "KS_HeadShotKillSoundPath": "sounds/GoldKingZ/Training/bell_normal.vsnd_c",

  //Sound BodyShot Kill
  "KS_BodyKillSoundPath": "sounds/GoldKingZ/Training/timer_bell.vsnd_c",

  //Sound HeadShot Hit
  "KS_HeadShotHitSoundPath": "sounds/GoldKingZ/Training/bell_impact.vsnd_c",

  //Sound BodyShot Hit
  "KS_BodyHitSoundPath": "sounds/GoldKingZ/Training/timer_bell.vsnd_c",
}
```


## .:[ Configuration Quake ]:.

> [!CAUTION]
> Config Located In ..\addons\counterstrikesharp\plugins\Kill-Sound-GoldKingZ\config\Kill_Settings.json                                           
>

> [!NOTE]
> To Use These You Need To Enable KS_EnableQuakeSounds First In config.json                                                                                                                               
> Then Download https://github.com/Source2ZE/MultiAddonManager  With Gold KingZ WorkShop                                                                                                                              
> https://steamcommunity.com/sharedfiles/filedetails/?id=3230015783                                                                                                                             
> mm_extra_addons 3230015783                                                                                                                             

```
	Available Sounds [ "HeadShot" , "Kill" , "KnifeKill" , "GrenadeKill" , "MollyKill" , "SelfKill" , "TeamKill" , "FirstBlood" , "RoundPrepare" , "RoundStart" ]
	
	HeadShot Can Be Used 
	HeadShot = Will Start Sound Loop On Every HeadShot Kill
	HeadShot_X = Will Start Sound Depend Kill X HeadShot Kill

	Kill Can Be Used 
	Kill = Will Start Sound Loop On Every Normal Kill
	Kill_X = Will Start Sound Depend Kill X Normal Kill

	KnifeKill Can Be Used 
	KnifeKill = Will Start Sound Loop On Every Knife Kill
	KnifeKill_X = Will Start Sound Depend Kill X Knife Kill

	GrenadeKill Can Be Used 
	GrenadeKill = Will Start Sound Loop On Every Grenade Kill
	GrenadeKill_X = Will Start Sound Depend Kill X Grenade Kill

	MollyKill Can Be Used 
	MollyKill = Will Start Sound Loop On Every Molotov Kill
	MollyKill_X = Will Start Sound Depend Kill X Molotov Kill

```


![colors](https://github.com/oqyh/cs2-Kill-Sound/assets/48490385/6c0717b0-0a7e-45c3-ab7e-cd164ee74aae)



## .:[ Language ]:.
```json
{
    //==========================
    //        Colors
    //==========================
    //{Yellow} {Gold} {Silver} {Blue} {DarkBlue} {BlueGrey} {Magenta} {LightRed}
    //{LightBlue} {Olive} {Lime} {Red} {Purple} {Grey}
    //{Default} {White} {Darkred} {Green} {LightYellow}
    //==========================
    //        Other
    //==========================
    //{nextline} = Print On Next Line
    //==========================
	
	"announce.round.prepare": "{green}Gold KingZ {grey}| {grey} Prepare To Fight...",

	"announce.round.start": "{green}Gold KingZ {grey}| {lime}Round Start !",

	"quake.kill.streak": "{green}Gold KingZ {grey}| You On Kill Streak {yellow}{0}",
	"quake.kill.streak.26": "{green}Gold KingZ {grey}| Nice Work!!! You On Kill Streak {yellow}{0}",
	"quake.kill": "{green}Gold KingZ {grey}| You Killed {blue}{0}",
	"announce.quake.kill.streak": "{green}Gold KingZ {grey}| {purple}{0} {grey}On Kill Streak {yellow}{1}",
	"announce.quake.kill.streak.26": "{green}Gold KingZ {grey}| {grey}HOOOLLYY!!!! {purple}{0} {grey}Is On Kill Streak {yellow}{1}",
	"announce.quake.kill": "{green}Gold KingZ {grey}| {purple}{0} {grey} Killed {blue}{1}",

	"quake.headshot.streak": "{green}Gold KingZ {grey}| You On HeadShot Streak {yellow}{0}",
	"quake.headshot.streak.5": "{green}Gold KingZ {grey}| You Are HeadHunter!! {yellow}{0}",
	"quake.headshot": "{green}Gold KingZ {grey}| You HeadShotted {blue}{0}",
	"announce.quake.headshot.streak": "{green}Gold KingZ {grey}| {purple}{0} {grey}On HeadShot Streak {yellow}{1}",
	"announce.quake.headshot.streak.5": "{green}Gold KingZ {grey}| {purple}{0} {grey}Is HeadHunter!!!! {grey}[{yellow}{1} {grey}HeadShot Streak]",
	"announce.quake.headshot": "{green}Gold KingZ {grey}| {purple}{0} {grey} HeadShotted {blue}{1}",


	"quake.grenade.streak": "{green}Gold KingZ {grey}| You On Grenade Streak {yellow}{0}",
	"quake.grenade": "{green}Gold KingZ {grey}| You Grenade {blue}{0}",
	"announce.quake.grenade.streak": "{green}Gold KingZ {grey}| {purple}{0} {grey}On Grenade Streak {yellow}{1}",
	"announce.quake.grenade.streak.5": "{green}Gold KingZ {grey}| {grey}Nade KING !!!! {purple}{0} {grey}Is On Kill Streak {yellow}{1}",
	"announce.quake.grenade": "{green}Gold KingZ {grey}| {purple}{0} {grey} Grenade {blue}{1}",


	"quake.molly.streak": "{green}Gold KingZ {grey}| You On Molotov Streak {yellow}{0}",
	"quake.molly": "{green}Gold KingZ {grey}| You Molotov {blue}{0}",
	"announce.quake.molly.streak": "{green}Gold KingZ {grey}| {purple}{0} {grey}On Molotov Streak {yellow}{1}",
	"announce.quake.molly.streak.5": "{green}Gold KingZ {grey}| {grey}Molotov KING !!!! {purple}{0} {grey}Is On Kill Streak {yellow}{1}",
	"announce.quake.molly": "{green}Gold KingZ {grey}| {purple}{0} {grey} Molotov {blue}{1}",
	
	"quake.knife.streak": "{green}Gold KingZ {grey}| You On Knife Streak {yellow}{0}",
	"quake.knife": "{green}Gold KingZ {grey}| You Knife {blue}{0}",
	"announce.quake.knife.streak": "{green}Gold KingZ {grey}| {purple}{0} {grey}On Knife Streak {yellow}{1}",
	"announce.quake.knife.streak.5": "{green}Gold KingZ {grey}| {grey}Knife Slayer!!! {purple}{0} {grey}Is On Kill Streak {yellow}{1}",
	"announce.quake.knife": "{green}Gold KingZ {grey}| {purple}{0} {grey} Knifed {blue}{1}",


	"quake.firstblood": "{green}Gold KingZ {grey}| FirstBlood You Killed {blue}{0}",
	"announce.quake.firstblood": "{green}Gold KingZ {grey}| {purple}{0} {grey} FirstBlood Killed {blue}{1}",

	"quake.teamkill": "{green}Gold KingZ {grey}| {darkred}You Team Killed {blue}{0}",
	"announce.quake.teamkill": "{green}Gold KingZ {grey}| {purple}{0} {darkred} Team Killed {blue}{1}",

	"quake.selfkill": "{green}Gold KingZ {grey}| {darkred}You Killed Your Self!",
	"announce.quake.selfkill": "{green}Gold KingZ {grey}| {purple}{0} {grey}Killed Himself !"
}
```


## .:[ Change Log ]:.
```
(1.0.5)
-Rework Kill Sound Plugin
-Disable Menu (Temp)
-Disable Toggle (Temp)
-Added Quake [ "HeadShot" , "Kill" , "KnifeKill" , "GrenadeKill" , "MollyKill" , "SelfKill" , "TeamKill" , "FirstBlood" , "RoundPrepare" , "RoundStart" ]

(1.0.4)
-Rework Menu (SoundDisableCommandsMenu)

(1.0.3)
-Fix Null

(1.0.2)
-Fix Sounds On Teammate

(1.0.1)
-Added HeadShotKillSoundPath
-Added BodyKillSoundPath
-Added HeadShotHitSoundPath
-Added BodyHitSoundPath
-Added SoundDisableCommandsMenu
-Added SoundDisableCommands
-Added RemovePlayerCookieOlderThanXDays

(1.0.0)
-Initial Release
```

## .:[ Donation ]:.

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/oQYh)
