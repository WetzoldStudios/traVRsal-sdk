[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/WetzoldStudios/traVRsal-sdk/master/LICENSE.md)
![Discord](https://img.shields.io/discord/653315487437946880)
![Twitter Follow](https://img.shields.io/twitter/follow/OutOrDead?style=flat-square)

<p align="center">
  <img src="Editor/Images/travrsal-300.png">
</p>

# Introduction

traVRsal brings natural walking to VR. It is an engine on top of Unity for creating immersive, room-scale experiences utilizing impossible spaces and non-euclidean geometry. Using the SDK, anybody can now easily create such experiences.

![Cover](https://raw.githubusercontent.com/wiki/WetzoldStudios/traVRsal-sdk/Images/EngineDescription.jpg)

# Getting Started

* [Experience It](https://sidequestvr.com/app/449/1) - Download the app
* [Create Levels](https://github.com/WetzoldStudios/traVRsal-sdk/wiki) - Follow the wiki
* [Levels Repository](https://github.com/WetzoldStudios/traVRsal-levels) - See how existing levels were built and reuse parts

# Roadmap

This is just the very beginning, to get the initial creation and publishing flow established. A lot of features will now be built on top of this. More detailed plans and vision documents will be shared soon.

# Contact

Join the [Discord server](https://discord.gg/67fNz4F) for all questions and feedback.

# Known Bugs & Limitations

This is an early alpha version to get feedback and steer the direction of future development. Many things are still rough around the edges. It will help tremendously if you report every error you encounter but also every feature you are missing to the Discord SDK channel.

* levels cannot yet be tested in VR, the app will release later in July
* only very few nodes are available right now and your feedback will help to fill the gaps
* shaders need to support stencils to be visible through transitions
* transparent objects are not visible through transitions
* switching to game mode does not allow to move with cursor keys for some reason (it works in the editor)
* gizmos are sometimes not easily visible (F5 to reload level works as a mitigation)
* incorrect JSON files will cause the editor to become stuck
* auto-refresh sometimes does not load the newest packaged assets, restarting the editor solves this
* the minimum tile count in a grid right now is 4 due to a limitation with maze generation
* deleting the Levels folder will spam the Unity console with errors if the Publish UI of the traVRsal SDK is still open
* custom interactions are not possible yet (only premade Button for now)
* sky boxes are not yet supported 
* audio is not assigned to correct mixer channels yet 