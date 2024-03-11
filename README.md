# [CS2] Kill-Sound (1.0.4)

### Sound On , Kill , HeadShot , Body , Menu

![Untitled](https://github.com/oqyh/cs2-Kill-Sound/assets/48490385/c2ffb156-c689-40f1-8d6d-fe9299ed4072)

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

![menucolors](https://github.com/oqyh/cs2-Kill-Sound/assets/48490385/d19bfc83-eb81-437b-89d9-9d645e6506dd)


## .:[ Language ]:.
```json
{
	"Chat.AllSoundsOff": "{green}Gold KingZ {grey}| All Sounds {darkred}Disabled.",

	"Chat.FreezeOff": "{green}Gold KingZ {grey}| Freeze When Menu Open {darkred}Disabled.",
	"Chat.FreezeOn": "{green}Gold KingZ {grey}| Freeze When Menu Open {lime}Enabled.",

	"Chat.HeadShotKillOff": "{green}Gold KingZ {grey}| Head-Shot Kill Sound {darkred}Disabled.",
	"Chat.HeadShotKillOn": "{green}Gold KingZ {grey}| Head-Shot Kill Sound {lime}Enabled.",

	"Chat.HeadShotHitOff": "{green}Gold KingZ {grey}| Head-Shot Hit Sound {darkred}Disabled.",
	"Chat.HeadShotHitOn": "{green}Gold KingZ {grey}| Head-Shot Hit Sound {lime}Enabled.",

	"Chat.BodyKillOff": "{green}Gold KingZ {grey}| Body-Shot Kill Sound {darkred}Disabled.",
	"Chat.BodyKillOn": "{green}Gold KingZ {grey}| Body-Shot Kill Sound {lime}Enabled.",

	"Chat.BodyHitOff": "{green}Gold KingZ {grey}| Body-Shot Hit Sound {darkred}Disabled.",
	"Chat.BodyHitOn": "{green}Gold KingZ {grey}| Body-Shot Hit Sound {lime}Enabled.",



	"Menu.Intro": "<font class='fontSize-l' color='yellow'> Opening Menu... </font>  <br> <br> <img src='https://cdn.discordapp.com/attachments/1175717468724015144/1215033982790410280/BtQI.gif?ex=65fb4793&is=65e8d293&hm=a68453fe98e8cdcca309f632183fae225964d012fe223a7705d780f3e1a89a98&' class=''> <br> <br>",

	"Menu.Freeze": "Freeze Menu Open",
	"Menu.HeadShotKill": "HeadShot Kill Sound",
	"Menu.HeadShotHit": "HeadShot Hit Sound",
	"Menu.BodyKill": "Body Kill Sound",
	"Menu.BodyHit": "Body Hit Sound",
	"Menu.Bottom": "<br>           <font color='cyan'>[ WASD - To Native ]</font> <br><font color='purple'>[ <img src='https://cdn.discordapp.com/attachments/1175717468724015144/1214996807055183912/output-onlinegiftools_14.gif?ex=65fb24f4&is=65e8aff4&hm=2e94cf44a713c2fe3add69f79ed5e11a2371c7d26166de3cbb7f6b55d5fd60c3&' class=''> - To Exit ]<br>",

	"Menu.Outro": "<font class='fontSize-m' color='red'> Saving And Exiting Menu... </font>  <br> <br> <img src='https://cdn.discordapp.com/attachments/1175717468724015144/1215034880912523375/767dec44b7f42e7d0b338f3caba3a5db-ezgif.com-resize.gif?ex=65fb4869&is=65e8d369&hm=f299dba2118f5a2500b697d6ee83e690b8d8516028e8723d242e7eaf43ea6606&' class=''> <br> <br>"
}
```


## .:[ Change Log ]:.
```
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
