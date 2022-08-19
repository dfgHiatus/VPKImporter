# VPKImporter

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for extracts and imports [VPK](https://developer.valvesoftware.com/wiki/VPK_File_Format) files into [Neos VR](https://neos.com/).

## Installation
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
1. Place and extract [VPKImporter.rar](https://github.com/dfgHiatus/VPKImporter/releases/latest) into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
1. Download the version of [ValveResourceFormat](https://github.com/SteamDatabase/ValveResourceFormat/releases/latest) that matches your OS and place it in `nml_mods/vpk_extractor`. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods\vpk_extractor` for a default install. For example, on Windows this will be `Decompiler.exe`, so place it under `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods\vpk_extractor\Decompiler.exe`.
1. Start the game. If you want to verify that the mod is working you can check your Neos logs.

## FAQs
1. <b>What does this mod do exactly?</b> This mod solely extracts the content of VPKs and imports them into Neos
1. <b>Will this mod run on the Linux version of the game?</b> No*
1. <b>Do I need the non-steam build or steam build for this mod to work?</b> Both builds are compatible 
1. <b>How many packages can I import at once?</b> In theory you should be able to import many at once, but in my experience one-at-a-time is your best friend here given the <i>massive</i> file size Unity Packages can be
1. <b>Help! The files I imported are file-looking things!</b> Using [NeosModSettings](https://github.com/badhaloninja/NeosModSettings), you'll see the top most option is to "Import files directly into Neos". You can set this to false, but be wary as you may import thousands of things all at once!
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
- The importer doesn't place files under a single root, which can be a little messy
- This might hang a little bit during import. Recommended to import one unity package at a time, pairs nicely with [NeosModSettings](https://github.com/badhaloninja/NeosModSettings)
- Should be able to mix and match packages with the import of other files, currently untested

*This mod has the capacity to run on MacOS and Linux, although in its present state the functionality for it is yet to be added.
