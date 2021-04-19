[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/WetzoldStudios/traVRsal-sdk/master/LICENSE.md)
![Discord](https://img.shields.io/discord/653315487437946880)
![Twitter Follow](https://img.shields.io/twitter/follow/traVRsal_Robert?style=flat-square)

<p align="center">
  <img src="Editor/Images/travrsal-300.png">
</p>

# Introduction

traVRsal brings natural walking to VR. It is an engine on top of Unity for creating immersive, room-scale experiences utilizing impossible spaces and non-euclidean geometry. Using the SDK, anybody can now easily create such experiences.

![Cover](https://raw.githubusercontent.com/wiki/WetzoldStudios/traVRsal-sdk/Images/EngineDescription.jpg)

# Getting Started

* [Experience It](https://sidequestvr.com/app/449/1) - Download the app
* [Create Worlds](https://github.com/WetzoldStudios/traVRsal-sdk/wiki) - Follow the wiki
* [Worlds Repository](https://github.com/WetzoldStudios/traVRsal-worlds) - See how existing worlds were built and reuse parts

# Contact

Join the [Discord server](https://discord.gg/67fNz4F) for all questions and feedback.

# Known Bugs & Limitations

This is an early alpha version to get feedback and steer the direction of future development. Many things are still rough around the edges. It will help tremendously if you report every error you encounter but also every feature you are missing to the Discord SDK channel.

* TextMeshPro is not yet visible through portals and will suddenly pop into view
* transparent objects are not visible through transitions
* incorrect JSON files will cause the studio to become stuck
* audio is not assigned to correct mixer channels yet 
* objects needs to be marked "Enable Read/Write" at import, otherwise NavMesh will fail or might even result in crashes

# Licenses

This SDK is licensed under the MIT license except for all contents in "Third-Party" folders which follows its original licenses. This includes:

* Async Coroutine
* AWS SDK
* bhaptics SDK
* DOTween
* Unity NavMeshComponents
* JSONnet
* XNode