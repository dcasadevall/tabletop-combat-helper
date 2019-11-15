# Tabletop Combat Helper

A networked helper application for players of table top roleplaying games to visualize combat elements such as player positions, health, etc..

## Current State:

The following features are implemented:
* Loading a map from a given list. The list can be modified.
* Moving a set of initial characters in the map. These characters can be modified.
* Navigate through sections of a map.
* Spawn units with the "U" key.
* Draw on the map
* Replay any actions performed this session
* Save the session replay
* Multiplayer support. Anyone can launch the client to join or start a session.

## Setup

### Method A: Without local content

You can run the game out of the box in editor by going to Window -> Asset Management -> Addressable Groups.
Then selecting Play Mode Script -> Use Existing Build.
You should be able to play the game within the editor, or build to device.

### Method B: Downloading local content

If you want to modify / add to maps and units, you can download the content from http://dcasadevall.com/apps/tabletop-combat-helper-content.
Checkout this repository somewhere on your local drive, then create a symbolic link to tabletop-combat-helper-content/Content
in Assets/Content.
This allows you to play in the editor without having to download assets from the remote server, as well as modify the list of units / maps and add
to them for quick iteration.

Note that if you build to device, the local content will not be used. For that, refer to Method C.

### Method C: Building your own content

Feel free to clone the contents of http://dcasadevall.com/apps/tabletop-combat-helper-content and modifying them.

If you want to build your own set of assets, you will have to create a new build profile, pointing to your own server 
where you want to host your assets.
You can go to Window -> Asset Management -> Profiles and click on Create -> Profile.
Modify "RemoteBuildPath" to point to the server which you will upload your assets to.

You can go to Window -> Asset Management -> Addressable Groups and publish to your newly created profile.
Go to the project root (outside of Assets) ServerData/ and copy the files to your server.
You should now be able to build to whatever platform you choose.

Refer to https://blogs.unity3d.com/2019/07/15/addressable-asset-system/ for more info on addressables.

## Building to WebGL

Note that the game does work on WebGL. The only caveat is that for that to happen, you must either configure CORS to properly download
assets from your content remote path or host the assets on the same domain where you host your WebGL application.
For a working example check out:
http://dcasadevall.com/apps/tabletop-combat-helper/
