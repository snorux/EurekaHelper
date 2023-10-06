<section id="header" align="center">
  <h1>
    <img href="https://https://github.com/snooooowy/EurekaHelper" src="/EurekaHelper/Resources/icon.png" width="150px" />
    <br>
    <a href="https://git.io/typing-svg"><img src="https://readme-typing-svg.herokuapp.com?font=Patrick+Hand&size=50&duration=3000&pause=2000&center=true&vCenter=true&width=435&lines=Eureka+Helper" alt="Eureka Helper" /></a>
  </h1>
  <div align="center">
    <h5>A FFXIV Dalamud plugin</h5>
    <a href="https://github.com/snooooowy/EurekaHelper/tags"><img src="https://img.shields.io/github/v/tag/snooooowy/EurekaHelper?label=version&style=for-the-badge" /></a>
    <a href="https://github.com/snooooowy/EurekaHelper"><img src="https://img.shields.io/endpoint?style=for-the-badge&url=https%3A%2F%2Fvz32sgcoal.execute-api.us-east-1.amazonaws.com%2FEurekaHelper" /></a>
    <a href="https://github.com/goatcorp/Dalamud"><img src="https://img.shields.io/badge/ffxiv-dalamud-red.svg?&style=for-the-badge" /></a>
  </div>
</section>

<hr>

<details open>
  <summary>
    <span>Table of Contents</span>
  </summary>
  <ol>
    <li><a href="#description">Description</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#commands">Commands</a></li>
    <li><a href="#features">Features</a></li>
    <ol>
      <li><a href="#ffxiv-eureka-tracker-gui">FFXIV Eureka Tracker GUI</a></li>
      <li><a href="#elementals-manager">Elementals Manager</a></li>
      <li><a href="#instance-tracker">Instance Tracker</a></li>
      <li><a href="#relic-helper">Relic Helper</a></li>
      <li><a href="#alarms-manager">Alarms Manager</a></li>
      <li><a href="#cutomizable-configurations">Cutomizable Configurations</a></li>
    </ol>
    <li><a href="#known-issues">Known Issues</a></li>
  </ol>
</details>

<hr>

## Description
EurekaHelper is a plugin for [XIVLauncher](https://goatcorp.github.io/). It's a handy plugin that enhances your Eureka gameplay with an In-Game Eureka Tracker and convenient quality-of-life features.

EurekaHelper allows you to effortlessly join or create an [Eureka Tracker](https://ffxiv-eureka.com/) instance within the game. It also provides a user-friendly graphical user interface (GUI) that closely resembles the website's interface.

To access the main window, simply type any of the following commands: `/eurekahelper`, `/ehelper`, or `/eh`. All available commands are listed in the "About" tab.  

Leave a star ⭐ on this repository if you have enjoyed using the plugin!

## Installation
The plugin is available for download via the Dalamud Plugin Installer. Simply type `/xplugins` in-game and select EurekaHelper for installation.
To opt in for testing versions (when available), right click EurekaHelper in the Plugin Installer and select `Receive plugin testing versions`.

## Commands
Following is a list of all available commands for the plugin.
| Command | Description |
|:-------:|-------------|
| `/ehelper` or `/eurekahelper` or `/eh` | Opens the main window |
| `/etrackers` | Attempts to get a list of public trackers for the current instance in the same datacenter |
| `/arisu` | Display next weather and time for Crab, Cassie & Skoll<br>![image](https://github.com/snooooowy/EurekaHelper/assets/34697265/0b8d6af7-cf68-40c2-972c-dd194dd43c2a) |
| `/erelic` | Opens the [Relic Window](#relic-window) which will allow you to track your Eureka relic progression |
| `/ealarms` | Opens the [Alarms Window](#alarms-window). You will be able to set custom alarms for weather/time in here! |

## Features
### FFXIV Eureka Tracker GUI
The main window of the plugin which displays the in-game FFXIV Eureka Tracker. You can manage the tracker from this window.
| Not Connected to Tracker | Connected To Tracker |
|:-------:|----------------------|
| ![image](https://github.com/snooooowy/EurekaHelper/assets/34697265/b3b3ef48-407c-4cd6-be35-6f421e5a5b14) | ![image](https://github.com/snooooowy/EurekaHelper/assets/34697265/1ff96e27-5216-4059-8648-38845dbf0943) |

### Elementals Manager
Manage all seen Elementals from this tab! All known Elementals position in game are listed [here](https://github.com/snooooowy/EurekaHelper/issues/13).  
Feel free to add new Elemental positions in the link above or you can DM me on Discord (@snorux)  
![image](https://github.com/snooooowy/EurekaHelper/assets/34697265/5655c0f7-7df5-44ba-846c-58f87d542429)

### Instance Tracker
Want to know which Eureka instance you are in? Want to instance hop? You can use this feature to determine the current Eureka instance ID.  
⚠️ **Do read the disclaimer before usage.** ⚠️  
![image](https://github.com/snooooowy/EurekaHelper/assets/34697265/0a321213-5bd0-47c2-8727-21ef97c98ca2)

### Relic Helper
Keep track of your completed Eureka Relics using this feature. The window also shows the number of items required for each relic stage!  
![image](https://github.com/snooooowy/EurekaHelper/assets/34697265/36b6f09c-596b-48fb-a978-ae912b94efe8)

### Alarms Manager
Interested in farming money NMs (e.g Skoll, Crab, Cassie) or lockboxes? You can set an alarm for weather/time conditions using this feature.  
You will receive an in-game notification whenever the Alarm triggers.  
![image](https://github.com/snooooowy/EurekaHelper/assets/34697265/ae7e7c57-6ac2-4848-bf59-9991eaa57867)  
![image](https://github.com/snooooowy/EurekaHelper/assets/34697265/64460d46-f3ff-4ba9-b960-26533f3cb494)

### Cutomizable Configurations
A highly customizable configuration for the main [Eureka Tracker](#ffxiv-eureka-tracker-gui) window.  
![image](https://user-images.githubusercontent.com/34697265/235935187-97466b2a-7d35-485d-aee0-23f5da3d0955.png)

## Known Issues
As of current, if you have `Payload Options` set to `Click to shout` and have `Chat2` plugin installed, the game will freeze once you click on the payload.  
An issue has been made and you can keep track of it [here](https://github.com/ascclemens/plugin-issues/issues/60).

PluginLog is referenced via static interface. This should be updated to use 