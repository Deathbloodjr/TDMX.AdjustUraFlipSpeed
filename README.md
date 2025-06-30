# TDMX.ModTemplate
| Mono | IL2CPP |
|:--:|:--:|
|✅|✅|

 A template for creating mods for TDMX.

# Requirements
 Visual Studio 2022\
 A 1.3.0+ version of the game to compile IL2CPP versions, Mono is buildable without any version of the game.

# How to use
 On github, click the "Use this template" button -> Create a new repository\
 Give a Repository name\
 Select Private or Public repository\
 Clone the repo locally

 Change a few instances of "ModTemplate" to whatever you will name your mod <ModName>\
 That would be:
 - The main directory of this
 - ModTemplate.sln
    - Also open the .sln file in a text editor to change "ModTemplate", "ModTemplate\ModTemplate.csproj", replacing ModTemplate with <ModName>
 - ModTemplate folder
 - Within that folder, ModTemplate.csproj

 Now open the solution itself\
 In the .csproj file (double click the <ModName> project in solution explorer)\
 Everything you'd need to change will be in the first PropertyGroup at the top.
 - Change AuthorName to your name
   - This will be reflected in the resulting dll name
   - For example, my dlls end up looking like "com.DB.TDMX.SingleHitBigNotes"
   - Your name will be in place of "DB"
 - Change ModName to your ModName
 - Change ModVersion to a fitting version in the format of x.x.x (or whatever your preference is)
 - Change Description to whatever your mod does (Or remember to do this later)

In Plugin.cs
 - Change the namespace to what you selected as RootNamespace earlier
    - In Visual Studio, you can highlight the namespace, hit Ctrl + R, Ctrl + R, and enter your new namespace to change all instances of it. 
    - You can change the namespaces in the 2 example patches as well, to RootNamespace.Patches
 - Change public const string ModName = your ModName

 At this point, build the project, and it should have some errors
 - Return to File Explorer and open the .csproj.user file
 - Put your TDMX game install directory in GameDir

You project should now be in a buildable state
 
 Outside of code, edit the README.md file
 - Change TDMX.ModTemplate to your mod's name
 - Change the description.
 - Delete this "#How to use" section
 - Make any other changes you feel like making

# Build
 Attempt to build the project, or copy the .csproj.user file from the Resources file to the same directory as the .csproj file.\
 Edit the .csproj.user file and place your Rhythm Festival file location in the "GameDir" variable.

Add BepInEx as a nuget package source (https://nuget.bepinex.dev/v3/index.json)


## Mono
 Install the latest version of [BepInEx 5.x.x](https://github.com/BepInEx/BepInEx/releases) into your TDMX v1.2.2 directory and launch the game.\
 Make sure you install the BepInEx_win_x64 version.


## IL2CPP
 Install [BepInEx 6.0.0-pre.2](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.2) into your TDMX v1.3.0+ directory and launch the game.\
 This will generate all the dummy dlls in the interop folder that will be used as references.\
 Make sure you install the Unity.IL2CPP-win-x64 version.\
 Newer versions of BepInEx could have breaking API changes until the first stable v6 release, so those are not recommended at this time.
 


