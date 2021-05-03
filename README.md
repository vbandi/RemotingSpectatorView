# RemotingSpectatorView
A third person camera view that can run in the Unity Editor / Play Mode.

# Download
Download the release or clone the repository. You will only need the contents of the Assets/RemotingSpectatorView folder in your own project
(this is what is in the release .unitypackage file). The RemotingSpectatorView itself is not device-specific.

The rest of the repository contains the MRTK (needed for the HoloLens demo) and a demo scene.

# Installation
Just import the .unitypackage file (or copy the contents of the Assets/RemotingSpectatorView folder) into your own project and you're good to go.

# Usage

- In the menu, select Remoting Spectator View / Show window
- Start and connect Holographic Remoting
- Start your app in the editor.
- Select your webcam from the dropdown list (DirectShow cams are not supported)
- Press "Play" in the Remo
- Make sure your webcam is horizontal
- Position your head right in front of the webcam, at the distance specified by "Distance from head"
- Press "Reset Spectator Camera Position"
- Hold up your hand so that the Hololens can see it
- Set camera FOV so that the detected and actual hands are of the same size
- Enjoy!

# Future enhancements
- A friendlier calibration experience that can also handle non-horizontal cameras
- Movable camera to better illustrate 3D!
- Human occlusion with the help of Nvidia Broadcast and similar apps

# Contributing
Feel free to enhance the tool in any way and contribute back to the repo. If you're looking for ideas, the ones in the Future Enhancements section will provide a 
sizeable challenge to any developer.

Before investing a large amount of your time however, it'd be good to have a disussion of your plans. You can use any of the github tools for this: issues, PRs,
or just contact me and let's chat!

# Contact
Remoting Spectator View is created by [András Velvárt](https://github.com/vbandi) and [Szász "River" Attila](https://github.com/RiverResponse)
