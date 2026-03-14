# TechTech's Sound Mod
A mod that allows you to edit some Among Us sounds by just dragging .wav files in the Among Us folder.




## Installation

- Go to [Tags](https://github.com/techtechcy/TechTechsSoundMod/releases) and click on your Among Us version. Then, download `TechTechsSoundMod.dll`

- Open your Among Us game folder (probably at `C:\Games\Steam\steamapps\common\Among Us\`, if not, open Steam, go to your library, go to Among Us, click on the gear icon -> Properties, go to installed files, and press `Browse`)

- If you don't already have BepInEx, install it for your Among Us version

- Download the [Dependencies](https://github.com/techtechcy/TechTechsSoundMod#Dependencies) and put them in `Among Us\BepInEx\plugins`

## Dependencies

- [Reactor](https://github.com/techtechcy/TechTechsSoundMod/releases)
- [Mira API](https://github.com/NuclearPowered/Reactor/releases)


## Guide

- After installing the mod, start your Among Us once (if it's your first time opening Among Us after modding it, expect it to take a long time).

- After it finishes loading, quit the game and navigate to your Among Us folder

- Locate the `TechTechsSounds` folder and open it

- Put all your .wav files here and rename them accordingly (open the README.txt in the folder for more info)
    #### WARNING: The files need to be processed for Among Us to accept them. More info in [Audio Files](https://github.com/techtechcy/TechTechsSoundMod#Audio) 

- Relaunch the game, verify that the mod is enabled (Go to `Settings` -> `SM Tab` and verify that `Mod Enabled` is true)


## Audio
Among us doesn't just accept every .wav file. In order to process your audio file, it is recommended that you use a program like [Audacity](https://github.com/techtechcy/TechTechsSoundMod/releases).

### Steps using Audacity:
- Drag & Drop your .wav file into the editor.
- Click `File` -> `Export Audio` -> `Export to computer`
- Select an output path
- Set up the settings as seen below:
    - Channels: `Stereo`
    - Sample Rate: `44100 Hz`
    - Encoding: `Signed 16-bit PCM`
<img width="491" height="158" alt="image" src="https://github.com/user-attachments/assets/8e43f5cb-01cd-4d5f-a066-0cfdcb67d809" />

- Press `Export`
