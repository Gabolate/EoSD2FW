# EoSD2FW
Converts Touhou 6's (EoSD) Scripts into Touhou 20 (FW)

# Info
**Why did i make this?**

Tbh i think the main motivation was that EoSD New Classic was releasing this year,
tho its not the first time i tried to port TH6 into somewhere else (I tried to do it on Scratch like 2 years ago and wasn't the best outcome lmao)

So im posting this here if anyone wants to see how my crappy code works or just as reference ~~(or contributions if you have any to make this mod release earlier)~~

# How to build

[Install .NET 8 SDK and Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Open a terminal window (or cmd) and type this while being on the project's folder:

- On Windows:

``dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true
``

- On Linux:

``dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true
``

*Note: Scripts produced by Windows and Linux might be a little bit different tho their contents should be roughly the same*.

# How to use

- Copy all of EoSD's "ecldata" files into the binary's directory

- Create a folder called ``modded`` along the converter's binary

- Open EoSD2FW

- Type the stage number (1 - 7. 7 is for the extra stage) then enter

- You should get a .txt file on the "modded" folder, that's the converted script.

- To pack it to use it on FW you need to use [thtk](https://thcrap.thpatch.net/thtk-nightly/thtk-win32-x64.zip) with [zero318's ECLMap](https://github.com/zero318/TouhouMaps/blob/main/th20_thtk.eclm)

- Use it like any other mod (oh gosh, i can't make a full guide on this rn sorry  :'v)
