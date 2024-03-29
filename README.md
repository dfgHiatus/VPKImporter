# VPKImporter

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for extracts and imports [VPK](https://developer.valvesoftware.com/wiki/VPK_File_Format) files into [Resonite](https://resonite.com/). 

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
1. Place and extract [VPKImporter.rar](https://github.com/dfgHiatus/VPKImporter/releases/latest) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
1. Download the version of [ValveResourceFormat](https://github.com/SteamDatabase/ValveResourceFormat/releases/latest) that matches your OS and place it in `rml_mods/vpk_extractor`. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods\vpk_extractor` for a default install. For example, on Windows this will be `Decompiler.exe`, so place it under `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods\vpk_extractor\Decompiler.exe`.
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.

## FAQs
1. <b>What does this mod do exactly?</b> This mod solely extracts the content of VPKs and imports them into Resonite
1. <b>Will this mod run on the Linux version of the game?</b> No*
1. <b>Do I need the non-steam build or steam build for this mod to work?</b> Both builds are compatible 
1. <b>How many packages can I import at once?</b> In theory you should be able to import many at once, but in my experience one-at-a-time is your best friend here given the <i>massive</i> file size VPKs can be
1. <b>Help! The files I imported are file-looking things!</b> Using [ResoniteModSettings](https://github.com/badhaloninja/ResoniteModSettings), you'll see the top most option is to "Import files directly into Resonite". You can set this to false, but be wary as you may import thousands of things all at once!
1. <b>What kind of files does this mod import?</b>
Presently, it supports:
- Text
- Images
- Documents 
- 3D Models (including point clouds)
- Audio
- Fonts
- Videos
- And raw binary variants of the above for file sharing

## Known Issues and Limitations
- Presently, THIS WILL FREEZE RESONITE DURING IMPORT. Recommended to import one VPK at a time, pairs nicely with [ResoniteModSettings](https://github.com/badhaloninja/ResoniteModSettings)
- Source 1 VPKs are not supported at this time.
- The importer doesn't place files under a single root, which can be a little messy
- Should be able to mix and match packages with the import of other files, currently untested

*This mod has the capacity to run on MacOS and Linux, although in its present state the functionality for it is yet to be added.
