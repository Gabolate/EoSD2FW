# ☯️ EoSD2FW 🔁
Converts Touhou 6's (EoSD) Scripts into Touhou 20 (FW)

# 🗒️ Info
**Why did i make this?**

Tbh i think the main motivation was that EoSD New Classic was releasing this year,
tho its not the first time i tried to port TH6 into somewhere else (I tried to do it on Scratch like 2 years ago and wasn't the best outcome lmao)

So im posting this here if anyone wants to see how my crappy code works or just as reference ~~(or contributions if you have any to make this mod release earlier)~~

# 🛠️ How to build

[Install .NET 8 SDK and Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Open a terminal window (or cmd) and type this while being on the project's folder:

- 🪟 On Windows:

``dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true
``

- 🐧 On Linux:

``dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true
``

*Note: Scripts produced by Windows and Linux might be a little bit different tho their contents should be roughly the same*.

# 💾 How to use

- Copy all of EoSD's "ecldataX.txt" decompiled files into the binary's directory

- Create a folder called ``modded`` along the converter's binary

- Open EoSD2FW

- Type the name of the [Sub-Params](https://github.com/Gabolate/EoSD2FW/blob/main/Params.spell) file (The default one available since v0.2 on the releases tab is "Params.spell")

- Type the stage number 1 - 7 (7 is for the extra stage) then [ENTER]

- You should get a .txt file on the "modded" folder, that's the converted script.

- To pack it to use it on FW you need to use [thtk](https://thcrap.thpatch.net/thtk-nightly/thtk-win32-x64.zip) with [zero318's ECLMap](https://github.com/zero318/TouhouMaps/blob/main/th20_thtk.eclm)

- Use it like any other mod (oh gosh, i can't make a full guide on this rn sorry  :'v)

# 🌐 Credits

- [@zero318](https://github.com/zero318) Bullet transform 0x1 fix Binhack, ECLMap + General Help.
- [@Neo-Nickz](https://github.com/Neo-Nickz)      General Help.
- [@DarkCatyYT](https://www.youtube.com/@DarkCatyYT)    General Help.
- [@Priw8](https://github.com/Priw8)         Website with various ECL tutorials and resources.
- [@ExpHP](https://github.com/ExpHP)         Website with ANM scripts documentation.
- [@ManDude](https://github.com/ManDude/)        EoSD ECL documentation.
- [THWiki.cc](https://thwiki.cc/%E8%84%9A%E6%9C%AC%E5%AF%B9%E7%85%A7%E8%A1%A8/ECL/%E7%AC%AC%E5%9B%9B%E4%B8%96%E4%BB%A3)      4th Gen ECL documentation.
- [@GensokyoClub](https://github.com/GensokyoClub) EoSD Decompile for Reference
- [THPatch](https://www.thpatch.net)        Original EoSD's English and Spanish Game Translations and Touhou Toolkit for extracting/repacking game data.
