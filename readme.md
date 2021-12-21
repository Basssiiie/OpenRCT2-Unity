# OpenRCT2 implementation in Unity3D

This fork of the original OpenRCT2 repository is an open-source attempt at running OpenRCT2 in a 3D game engine.

## Progress videos

[#3: Three Monkeys Park onride (30th of May)](https://www.youtube.com/watch?v=9JKYXVSoDlE)

[#2: Diamond Heights (3th of May)](https://www.youtube.com/watch?v=5NeOn0p9ORY)

[#1: Iceberg Islands (22nd of April)](https://www.youtube.com/watch?v=Urvp0uQcGX4)

## How to get it running

As of now there are no release builds yet, because the project is still very much in alpha stage.

### Prerequisites

To get it running inside of Unity, you need the following prerequisites:

- Everything mentioned in OpenRCT2's original [building prerequisites](#31-building-prerequisites).
- [Unity Editor version LTS 2020.3.25f1](https://unity3d.com/unity/whats-new/2020.3.25).

**Note:** only the Windows setup has been tested.

### Running the game

1. Follow the steps in [compiling and running](#32-compiling-and-running) chapter for OpenRCT2. 
    - Make sure to set the target platform to x64 if you have a 64-bit OS! (As explained in the paragraph after step 4 in the linked chapter.)    
    - It is not necessary to run OpenRCT2.
2. Open openrct2.sln in Visual Studio 2019.
3. Find the `openrct2-dll` project in the Solution Explorer and build it. (If it doesn't build, you messed something up in the previous steps.)
4. Start the Unity Editor and navigate to wherever you cloned the repository.
5. Open the project in this directory: [`/src/openrct2-unity/`](https://github.com/Basssiiie/OpenRCT2-Unity/tree/develop/src/openrct2-unity)
6. Once Unity has launched with the project, go to the 'Scenes' folder in the Project window and open ParkScene.
7. Select the OpenRCT2 object in the Hierarchy and copy/paste the full path(s) of your RCT game(s) into the correct fields in the Inspector.
    - **OpenRCT2 data path:** data folder for OpenRCT2 data, like new sprites objects. This should be set to use the repository's data path automatically, but can be changed if you know what you are doing.
    - **RCT2 path:** points to where RCT2 base game is installed, e.g. `C:/Games/Rollercoaster Tycoon 2`. Required for showing sprites.
    - **RCT1 path:** points to where RCT1 game is installed, e.g. `C:/Games/Rollercoaster Tycoon 1`. Required for showing RCT1 sprites, if the park has any.
    - **Parks path:** the folder where to load parks from. Defaults to a folder in the repository, but can be changed to another folder.
8. Select a park from the dropdown box.
9. Press the Play button at the top of the editor.

### Ingame camera controls

When the Unity editor is in playmode, the controls for the camera are as follows:
- **Hold right mouse button + move mouse:** rotate the camera.
- **Hold middle mouse button + move mouse:** pan the camera.
- **Arrow keys or WASD keys:** pan the camera faster.
- **Scroll mouse wheel:** zoom in and out.
- **Numpad plus:** make the game run twice as fast.
- **Numpad minus:** make the game run twice as slow.

## How?

OpenRCT2 is an open source C++ project, which allows easy forking and extending the project. Internally this project is seperated into a few different subprojects, all found in the [`/src/`](https://github.com/Basssiiie/OpenRCT2-Unity/tree/develop/src) folder. The interesting one of the bunch is the static library project called `libopenrct2`, because it contains all the code to run logic of the game.

This fork adds two more projects to the mix: `openrct2-dll` and `openrct2-unity`:

- `openrct2-dll` imports `libopenrct2` and adds  bindings for useful functions within OpenRCT2. These functions together with the static library are then exported as a C++ Dynamicly Linked Library (DLL) for use elsewhere.
- `openrct2-unity` is the Unity C# project. The DLL is imported into this project and its C++ functions are then [marshalled through Platform Invoke](https://docs.microsoft.com/en-us/dotnet/framework/interop/marshaling-data-with-platform-invoke), which is a technology which allows me to call unmanaged C++ libraries from managed C# .NET Framework code. 

From there on, the C# code can call the right functions to start the game, load any park and retrieve information about said park, like where it's scenery, rides and peeps are. All this information is then displayed in Unity in real-time, while `libopenrct2` runs doing all the logic of the game in the background.

## Note about contributions:

Since this is my work in progress pet project, the code is subject to random changes. I'm open to contributions but it would be benefitial for both of us to let me know via message (over Discord or Youtube). :D

---

---

---

# OpenRCT2
An open-source re-implementation of RollerCoaster Tycoon 2. A construction and management simulation video game that simulates amusement park management.

---

![OpenRCT2.org Group Park 5](https://i.imgur.com/e7CK5Sc.png)

---

### Download
| Latest release | Latest development build |
|----------------|--------------------------|
| [![OpenRCT2.org](https://img.shields.io/badge/master-v0.3.3-green.svg)](https://openrct2.org/downloads/master/latest) | [![OpenRCT2.org](https://img.shields.io/badge/develop-v0.3.3+-blue.svg)](https://openrct2.org/downloads/develop/latest) |

---

### Chat
Chat takes place on Discord. You will need to create a Discord account if you don't yet have one.

If you want to help *make* the game, join the developer channel.

If you need help, want to talk to the developers, or just want to stay up to date then join the non-developer channel for your language.

If you want to help translate the game to your language, please stop by the Localisation channel.

| Language | Non Developer | Developer | Localisation |
|----------|---------------|-----------|--------------|
| English | [![Discord](https://img.shields.io/badge/discord-general-blue.svg)](https://discord.gg/ZXZd8D8) | [![Discord](https://img.shields.io/badge/discord-development-yellowgreen.svg)](https://discord.gg/fsEwSWs) | [![Discord](https://img.shields.io/badge/discord-localisation-green.svg)](https://discord.gg/sxnrvX9) |
| Nederlands | [![Discord](https://img.shields.io/badge/discord-general-blue.svg)](https://discord.gg/cQYSXzW) | | |

---

# Contents
- 1 - [Introduction](#1-introduction)
- 2 - [Downloading the game (pre-built)](#2-downloading-the-game-pre-built)
- 3 - [Building the game](#3-building-the-game)
  - 3.1 - [Building prerequisites](#31-building-prerequisites)
  - 3.2 - [Compiling and running](#32-compiling-and-running)
- 4 - [Contributing](#4-contributing)
  - 4.1 - [Bug fixes](#41-bug-fixes)
  - 4.2 - [New features](#42-new-features)
  - 4.3 - [Translation](#43-translation)
  - 4.4 - [Graphics](#44-graphics)
  - 4.5 - [Audio](#45-audio)
  - 4.6 - [Scenarios](#46-scenarios)
- 5 - [Licence](#5-licence)
- 6 - [More information](#6-more-information)
- 7 - [Sponsors](#7-sponsors)

---

# 1. Introduction

**OpenRCT2** is an open-source re-implementation of RollerCoaster Tycoon 2 (RCT2). The gameplay revolves around building and maintaining an amusement park containing attractions, shops and facilities. The player must try to make a profit and maintain a good park reputation whilst keeping the guests happy. OpenRCT2 allows for both scenario and sandbox play. Scenarios require the player to complete a certain objective in a set time limit whilst sandbox allows the player to build a more flexible park with optionally no restrictions or finance.

RollerCoaster Tycoon 2 was originally written by Chris Sawyer in x86 assembly and is the sequel to RollerCoaster Tycoon. The engine was based on Transport Tycoon, an older game which also has an equivalent open-source project, [OpenTTD](https://openttd.org). OpenRCT2 attempts to provide everything from RCT2 as well as many improvements and additional features, some of these include support for modern platforms, an improved interface, improved guest and staff AI, more editing tools, increased limits, and cooperative multiplayer. It also re-introduces mechanics from RollerCoaster Tycoon that were not present in RollerCoaster Tycoon 2. Some of those include; mountain tool in-game, the *"have fun"* objective, launched coasters (not passing-through the station) and several buttons on the toolbar.

---

# 2. Downloading the game (pre-built)

OpenRCT2 requires original files of RollerCoaster Tycoon 2 to play. It can be bought at either [Steam](https://store.steampowered.com/app/285330/) or [GOG.com](https://www.gog.com/game/rollercoaster_tycoon_2). If you have the original RollerCoaster Tycoon and its expansion packs, you can [point OpenRCT2 to these](https://github.com/OpenRCT2/OpenRCT2/wiki/Loading-RCT1-scenarios-and-data) in order to play the original scenarios.

[OpenRCT2.org](https://openrct2.org/downloads) offers precompiled builds and installers of the latest master and the develop branch. There is also a cross platform [Launcher](https://github.com/LRFLEW/OpenRCT2Launcher/releases) available that will automatically update your build of the game so that you always have the latest version.

[Flathub](https://flathub.org/) offers flatpaks for Linux distributions that support this application distribution system:
* [Latest stable release](https://flathub.org/repo/appstream/io.openrct2.OpenRCT2.flatpakref)
* [Latest development build](https://flathub.org/beta-repo/appstream/io.openrct2.OpenRCT2.flatpakref)

Some Linux distributions offer native packages already. These packages are usually third-party, but we're trying to resolve issues they are facing.
* ArchLinux AUR: [openrct2-git](https://aur.archlinux.org/packages/openrct2-git) and [openrct2](https://aur.archlinux.org/packages/openrct2)
* Ubuntu PPA: [`develop` branch](https://launchpad.net/~openrct2/+archive/ubuntu/nightly) (nightly builds)
* openSUSE OBS: [games/openrct2](https://software.opensuse.org/download.html?project=games&package=openrct2)
* Gentoo (main portage tree): [games-simulation/openrct2](https://packages.gentoo.org/packages/games-simulation/openrct2)
* NixOS (`nixos-unstable` channel): [openrct2](https://github.com/NixOS/nixpkgs/blob/master/pkgs/games/openrct2/default.nix)
* Fedora 28 i386/amd64: [openrct2](https://copr.fedorainfracloud.org/coprs/nauticalnexus/openrct2/)

Some \*BSD operating systems offer native packages. These packages are usually third-party, but we're trying to resolve issues they are facing.
* FreeBSD: [games/openrct2](https://www.freshports.org/games/openrct2)
* OpenBSD: [games/openrct2](https://openports.se/games/openrct2)

---

# 3. Building the game

## 3.1 Building prerequisites

OpenRCT2 requires original files of RollerCoaster Tycoon 2 to play. It can be bought at either [Steam](https://store.steampowered.com/app/285330/) or [GOG.com](https://www.gog.com/game/rollercoaster_tycoon_2).

### Windows:
- Visual Studio 2019 (Enterprise / Professional / [Community (Free)](https://www.visualstudio.com/vs/community/))
  - Desktop development with C++

### macOS:
- Xcode 10+

The program can also be built as a command line program using CMake. This type of build requires:

- Xcode Command Line Tools
- [Homebrew](https://brew.sh)
- CMake (available through Homebrew)


### Linux:
- sdl2 (only for UI client)
- freetype (can be disabled)
- fontconfig (can be disabled)
- libzip (>= 1.0)
- libpng (>= 1.2)
- speexdsp (only for UI client)
- curl (only if building with http support)
- nlohmann-json (>= 3.6.0)
- openssl (>= 1.0; only if building with multiplayer support)
- icu (>= 59.0)
- zlib
- gl (commonly provided by Mesa or GPU vendors; only for UI client, can be disabled)
- duktape (unless scripting is disabled)
- cmake
- innoextract (optional runtime dependency; used for GOG installer extraction during setup)

Refer to https://github.com/OpenRCT2/OpenRCT2/wiki/Building-OpenRCT2-on-Linux#required-packages-general for more information about installing the packages.

---

## 3.2 Compiling and running
### Windows:
1. Check out the repository. This can be done using [GitHub Desktop](https://desktop.github.com) or [other tools](https://help.github.com/articles/which-remote-url-should-i-use).
2. Open a new Developer Command Prompt for VS 2019, then navigate to the repository (e.g. `cd C:\GitHub\OpenRCT2`).
3. To build the 64-bit version, use `msbuild openrct2.proj /t:build /p:platform=x64`.

   To build the 32-bit version, use `msbuild openrct2.proj /t:build /p:platform=Win32`.
4. Run the game, `bin\openrct2`

Once you have ran msbuild once, further development can be done within Visual Studio by opening `openrct2.sln`. Make sure to select the correct target platform for which you ran the build in point #3 (`Win32` for the 32-bit version, `x64` for the 64-bit version), otherwise the build will fail in Visual Studio.

Other examples:
```
set platform=x64
msbuild openrct2.proj /t:clean
msbuild openrct2.proj /t:rebuild /p:configuration=release
msbuild openrct2.proj /t:g2
msbuild openrct2.proj /t:PublishPortable
```

### macOS:
#### Xcode:
The recommended way of building OpenRCT2 for macOS is with Xcode. The Xcode build will create a self-contained application bundles which include all the necessary game files and dependencies. Open the project file OpenRCT2.xcodeproj in Xcode and build from there. Building this way will handle the dependencies for you automatically. You can also invoke an Xcode build from the command line using `xcodebuild`.

#### CMake:
A command line version of OpenRCT2 can be built using CMake. CMake will retrieve the dependences from [Dependencies](https://github.com/OpenRCT2/Dependencies/) automatically. You can build the project using CMake using the following commands:
```
mkdir build
cd build
cmake ..
make
ln -s ../data data
```
Then you can run the game by running `./openrct2`.

Detailed instructions can be found on [Building OpenRCT2 on macOS using CMake](https://github.com/OpenRCT2/OpenRCT2/wiki/Building-OpenRCT2-on-macOS-using-CMake).

### Linux:
The standard CMake build procedure is to install the [required libraries](https://github.com/OpenRCT2/OpenRCT2#linux), then:
```
mkdir build
cd build
cmake ../ # set your standard cmake options, e.g. build type here - For example, -DCMAKE_BUILD_TYPE=RelWithDebInfo
make # you can parallelise your build job with e.g. -j8 or consider using ninja
DESTDIR=. make install # the install target creates all the necessary files in places we expect them
```

You can also use Ninja in place of Make, if you prefer, see Wiki for details.

Detailed instructions can be found on [Building OpenRCT2 on Linux](https://github.com/OpenRCT2/OpenRCT2/wiki/Building-OpenRCT2-on-Linux).

---

# 4. Contributing
OpenRCT2 uses the [gitflow workflow](https://www.atlassian.com/git/tutorials/comparing-workflows#gitflow-workflow). If you are implementing a new feature or logic from the original game, please branch off and perform pull requests to ```develop```. If you are fixing a bug for the next release, please branch off and perform pull requests to the correct release branch. ```master``` only contains tagged releases, you should never branch off this.

Please read our [contributing guidelines](https://github.com/OpenRCT2/OpenRCT2/blob/develop/CONTRIBUTING.md) for information.

## 4.1 Bug fixes
A list of bugs can be found on the [issue tracker](https://github.com/OpenRCT2/OpenRCT2/issues). Feel free to work on any bug and submit a pull request to the develop branch with the fix. Mentioning that you intend to fix a bug on the issue will prevent other people from trying as well.

## 4.2 New features
Please talk to the OpenRCT2 team first before starting to develop a new feature. We may already have plans for or reasons against something that you'd like to work on. Therefore contacting us will allow us to help you or prevent you from wasting any time. You can talk to us via Discord, see links at the top of this page.

## 4.3 Translation
You can translate the game into other languages by editing the language files in ```data/language``` directory. Please join discussions in the [#localisation channel on Discord](https://discordapp.com/invite/sxnrvX9) and submit pull requests to [OpenRCT2/Localisation](https://github.com/OpenRCT2/Localisation).

## 4.4 Graphics
You can help create new graphics for the game by visiting the [OpenGraphics project](https://github.com/OpenRCT2/OpenGraphics). 3D modellers needed!

## 4.5 Audio
You can help create the music and sound effects for the game. Check out the OpenMusic repository and drop by our [#open-sound-and-music channel on Discord](https://discord.gg/9y8WbcX) to find out more.

## 4.6 Scenarios
We would also like to distribute additional scenarios with the game, when the time comes. For that, we need talented scenario makers! Check out the [OpenScenarios repository](https://github.com/PFCKrutonium/OpenRCT2-OpenScenarios).

---

# 5. Licence
**OpenRCT2** is licensed under the GNU General Public License version 3.

---

# 6. More information
- [GitHub](https://github.com/OpenRCT2/OpenRCT2)
- [OpenRCT2.org](https://openrct2.org)
- [Forums](https://openrct2.org/forums/)
- [Facebook](https://www.facebook.com/OpenRCT2)
- [RCT subreddit](https://www.reddit.com/r/rct/)
- [OpenRCT2 subreddit](https://www.reddit.com/r/openrct2/)

## Similar Projects

| [OpenLoco](https://github.com/OpenLoco/OpenLoco) | [OpenTTD](https://github.com/OpenTTD/OpenTTD) | [openage](https://github.com/SFTtech/openage) | [OpenRA](https://github.com/OpenRA/OpenRA) |
|:------------------------------------------------:|:----------------------------------------------------------------------------------------------------------:|:-------------------------------------------------------------------------------------------------------:|:-------------------------------------------------------------------------------------------------------------:|
| [![icon_x128](https://user-images.githubusercontent.com/604665/53047651-2c533c00-3493-11e9-911a-1a3540fc1156.png)](https://github.com/OpenLoco/OpenLoco) | [![](https://github.com/OpenTTD/OpenTTD/raw/850d05d24d4768c81d97765204ef2a487dd4972c/media/openttd.128.png)](https://github.com/OpenTTD/OpenTTD) | [![](https://user-images.githubusercontent.com/550290/36507534-4693f354-175a-11e8-93a7-faa0481474fb.png)](https://github.com/SFTtech/openage) | [![](https://raw.githubusercontent.com/OpenRA/OpenRA/bleed/packaging/linux/icons/ra_128x128.png)](https://github.com/OpenRA/OpenRA) |
| Chris Sawyer's Locomotion | Transport Tycoon Deluxe | Age of Empires 2 | Red Alert |

# 7. Sponsors

Companies that kindly allow us to use their stuff:

| [DigitalOcean](https://www.digitalocean.com/) | [JetBrains](https://www.jetbrains.com/) | [Backtrace](https://backtrace.io/) |
|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------|
| [![do_logo_vertical_blue svg](https://user-images.githubusercontent.com/550290/36508276-8b572f0e-175c-11e8-8622-9febbce756b2.png)](https://www.digitalocean.com/) | [![jetbrains](https://user-images.githubusercontent.com/550290/36413299-0e0985ea-161e-11e8-8a01-3ef523b5905b.png)](https://www.jetbrains.com/) | [![backtrace](https://user-images.githubusercontent.com/550290/47113259-d0647680-d258-11e8-97c3-1a2c6bde6d11.png)](https://backtrace.io/) |
| Hosting of various services | CLion and other products | Minidump uploads and inspection |
