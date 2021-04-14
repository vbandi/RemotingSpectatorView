# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [4.2.3] - 2020-05-28

Update Remoting dlls to version 2.1.3

## [4.2.2] - 2020-04-23

Update Remoting dlls to version 2.1.2

## [4.2.1] - 2020-02-25

Update Remoting dlls to version 2.0.20

## [4.2.0] - 2020-01-29

Update Microsoft.Holographic.AppRemoting.dll to version 2.0.18
Update license copyright

## [4.1.0] - 2019-12-12

Update UWP x64 Remoting library to pass Microsoft WACK test.
Fix issue in remoting library that caused disconnect to enter a bad state.

## [4.0.0] - 2019-11-14

Deprecate Package

## [3.0.6] - 2019-10-09

Update Remoting plugins to version 2.0.10.
Add support for existing remoting plugins to work with UWP x64 builds when remoting to HL2 devices.

## [3.0.5] - 2019-08-26

Update remoting dlls for Hololens 2
-- Fixes crash when remoting with shared depth buffer enabled

## [3.0.4] - 2019-08-09

Add updated remoting dll for Hololens 2 (new name same functionality)

## [3.0.3] - 2019-07-24

Fix .meta file issues for x64 remoting plugins
Move remoting plugins into x86_64 folder
Update Remoting plugins with latest version from Microsoft

## [3.0.2] - 2019-06-04

Remoting functionality is now compatible with HoloLens 2 devices using recent enough versions of Unity. Minimum compatible versions:
- 2019.3.0a6
- 2019.2.0b5
- 2019.1.6
- 2018.4.2

## [3.0.1] - 2019-05-28

Update dependencies for arm64 version of AudioPluginMsHRTF.dll so that it is compatible with Hololens 2.

## [3.0.0] - 2019-05-06

Include reference in the asmdef file to include ugui so that HoloLens input will correctly compile and work.

## [2.0.3] - 2019-04-18

Update AudioPluginMSHRTF.dll

## [2.0.2] - 2019-03-28

Update changelog with proper versions

## [2.0.1] - 2019-03-27

Remove Documentation.meta file that is no longer needed
Update package version to 2.0.1

## [2.0.0] - 2019-03-26

Rename Documentation folder to Documentation~
Add com.unity.ugui package dependency
Update package minimum supported version to 2019.2

## [1.0.8] - 2019-01-09

Update the PerceptionSimulationManager.dll for HoloLens simulation in the editor

## [1.0.7] - 2018-12-04

Update assembly definition to only include WSA references
Update License files

## [1.0.6] - 2018-10-23

Update the 64-bit PerceptionRemotingPlugin.dll 

## [1.0.5] - 2018-07-17

Update the PerceptionSimulationManager.dll and the simulation rooms for HoloLens simulation in the editor

## [1.0.4] - 2018-06-01

Add Editor to the Runtime assembly defintion so that it compiles in the Editor regardless of the selected platform.

## [1.0.3] - 2018-05-17

Remove HolographicStreamerDesktop.dll and update the .meta to point to HolographicStreamer.dll

## [1.0.2] - 2018-05-17

Update perception simulation library and add the holographic streamer library for UWP only.
Update Unity compatible versions to 2018.3 only

## [1.0.1] - 2018-05-03

Update runtime .asmdef to remove Editor as it's not necessary

## [1.0.0] - 2018-04-20

First official version of the package.
Update name to com.unity.xr.windowsmr.metro

## [0.0.4] - 2018-04-04

Update Runtime assembly definition file to only support the specific platforms that WindowsMR scripts should work on.

## [0.0.3] - 2018-03-29

Update Assembly definitions for Editor and Runtime

## [0.0.2] - 2018-03-28

Update package name to com.unity.windowsmr.metro

## [0.0.1] - 2018-01-10

### This is the first release of *Unity Windows Mixed Reality Package*.

*Short description of this release*
This is the first test iteration of the WindowsMR XR Package using the Package Manager process. This package is a replica of the builds.zip hosted at:
https://ono.unity3d.com/unity-extra/vr-hololens-external#trunk

We are bringing over all libraries from the builds.zip to be included and distributed through the package manager.
