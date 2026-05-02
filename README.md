# Love Bites
A library mod that extends the Ravenscript API. 

### Installation

This mod requires BepInEx to run. To ensure stability, please install the latest 5.x.x version  of BepInEx. Using BepInEx 6 will not work.

You may find BepInEx (and how to install it) [here](https://github.com/BepInEx/BepInEx).

Once BepInEx is installed, run the game once. Afterwards, place the LoveBites.dll into the plugins folder. You'll know it's working if mods that use specific features from this plugin work properly. You can also check the BepInEx logs to confirm by searching for Love Bites within the file.

### Features
* Ability to override individual actor colors
* Exposed Ravenscript
  * Retrieving teams from Multi-Teams Battle

### Bugs
* Blood decals currently do not change to overridden actor colors
  * The only way to fix this that I am aware of would compromise game performance, which I am not willing to do  
