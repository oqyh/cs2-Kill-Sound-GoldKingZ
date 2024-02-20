# [CS2] Kill-Sound (1.0.1)

### Sound On , Kill , HeadShot , Body

![menu](https://github.com/oqyh/cs2-Kill-Sound/assets/48490385/d9e90a91-0381-449c-95a4-fa686f9bf278)

![111](https://github.com/oqyh/cs2-Kill-Sound/assets/48490385/ac812651-3a64-4010-a708-30890ddd8cc4)


## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## .:[ Configuration ]:.
```json
{
  // you can test any sound path ingame console type "play <soundpath>"
  // Sound Path will in  https://github.com/oqyh/cs2-Kill-Sound/blob/main/sounds/sounds.txt
  //TO DISABLE MAKE IT "" empty
  "HeadShotKillSoundPath": "sounds/training/bell_normal.vsnd_c",
  //TO DISABLE MAKE IT "" empty
  "BodyKillSoundPath": "sounds/training/timer_bell.vsnd_c",
  //TO DISABLE MAKE IT "" empty
  "HeadShotHitSoundPath": "sounds/training/bell_impact.vsnd_c",
  //TO DISABLE MAKE IT "" empty
  "BodyHitSoundPath": "sounds/training/timer_bell.vsnd_c",
  
//-----------------------------------------------------------------------------------------

  //Commands To Open Sound Menu Add Many Commands As You Like And If You Like To Disable It Make It Empty Like This ""
  "SoundDisableCommandsMenu": "!soundmenu,!soundsmenu,!soundsetting,!soundsettings",
  //Commands To Disable All Sounds Add Many Commands  As You Like And If You Like To Disable It Make It Empty Like This ""
  "SoundDisableCommands": "!stopsound,!stopsounds",
  //Delete Inactive Players Older Than X Days (Save Cookies in ../addons/counterstrikesharp/plugins/Kill_Sound/Cookies/)
  "RemovePlayerCookieOlderThanXDays": 7,
  
//-----------------------------------------------------------------------------------------
  "ConfigVersion": 1
}
```

![colors](https://github.com/oqyh/cs2-Kill-Sound/assets/48490385/6c0717b0-0a7e-45c3-ab7e-cd164ee74aae)


## .:[ Language ]:.
```json
{
	//==========================
	//        Colors Chat
	//==========================
	//{Yellow} {Gold} {Silver} {Blue} {DarkBlue} {BlueGrey} {Magenta} {LightRed}
	//{LightBlue} {Olive} {Lime} {Red} {Purple} {Grey}
	//{Default} {White} {Darkred} {Green} {LightYellow}
	//==========================
	
	"Chat.AllSoundsOff": "{green}Gold KingZ {grey}| All Sounds {darkred}Disabled.",

	"Chat.HeadShotKillOff": "{green}Gold KingZ {grey}| Head-Shot Kill Sound {darkred}Disabled.",
	"Chat.HeadShotKillOn": "{green}Gold KingZ {grey}| Head-Shot Kill Sound {lime}Enabled.",

	"Chat.HeadShotHitOff": "{green}Gold KingZ {grey}| Head-Shot Hit Sound {darkred}Disabled.",
	"Chat.HeadShotHitOn": "{green}Gold KingZ {grey}| Head-Shot Hit Sound {lime}Enabled.",

	"Chat.BodyKillOff": "{green}Gold KingZ {grey}| Body-Shot Kill Sound {darkred}Disabled.",
	"Chat.BodyKillOn": "{green}Gold KingZ {grey}| Body-Shot Kill Sound {lime}Enabled.",

	"Chat.BodyHitOff": "{green}Gold KingZ {grey}| Body-Shot Hit Sound {darkred}Disabled.",
	"Chat.BodyHitOn": "{green}Gold KingZ {grey}| Body-Shot Hit Sound {lime}Enabled.",

	//==========================
	//        Colors Menu
	//==========================
	//Red = {1}TEXT{0}
	//Cyan = {2}TEXT{0}
	//Blue = {3}TEXT{0}
	//DarkBlue = {4}TEXT{0}
	//LightBlue = {5}TEXT{0}
	//Purple = {6}TEXT{0}
	//Yellow = {7}TEXT{0}
	//Lime = {8}TEXT{0}
	//Magenta = {9}TEXT{0}
	//Pink = {10}TEXT{0}
	//Grey = {11}TEXT{0}
	//Green = {12}TEXT{0}
	//Orange = {13}TEXT{0}
	//==========================

	"Menu.HeadShotKillOff": "{7}[ !1 ]{0} {12}Head-Shot Kill Sound{0} : {1}Off{0}",
	"Menu.HeadShotKillOn": "{7}[ !1 ]{0} {12}Head-Shot Kill Sound{0} : {12}On{0}",

	"Menu.HeadShotHitOff": "{7}[ !2 ]{0} {12}Head-Shot Hit Sound{0} : {1}Off{0}",
	"Menu.HeadShotHitOn": "{7}[ !2 ]{0} {12}Head-Shot Hit Sound{0} : {12}On{0}",

	"Menu.BodyKillOff": "{7}[ !3 ]{0} {12}Body-Shot Kill Sound{0} : {1}Off{0}",
	"Menu.BodyKillOn": "{7}[ !3 ]{0} {12}Body-Shot Kill Sound{0} : {12}On{0}",

	"Menu.BodyHitOff": "{7}[ !4 ]{0} {12}Body-Shot Hit Sound{0} : {1}Off{0}",
	"Menu.BodyHitOn": "{7}[ !4 ]{0} {12}Body-Shot Hit Sound{0} : {12}On{0}",

	"Menu.Exit": "{7}[ !5 ]{0} {1}Close Menu{0}"
}
```


## .:[ Change Log ]:.
```
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
