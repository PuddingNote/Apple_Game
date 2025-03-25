# Google Play In-App Updates Plugin for Unity

## Overview

Google Play Plugins for Unity provide C# APIs for accessing Play in-game features from within the Unity Engine. These plugins also provide Unity Editor features for building a game before publishing it on Google Play.  

Play In-App Updates lets you prompt users to update to the latest version of your game, when a new version is available, without the user needing to visit the Play Store.

Refer to the [documentation](https://developer.android.com/guide/playcore/in-app-updates/unity) and [Runtime API reference](https://developer.android.com/reference/unity/namespace/Google/Play/AppUpdate) for more information.

## Pre-requisites

### Required Unity Version

To use the Google Play In-App Updates plugin, you must use a supported Unity version:

- All versions of Unity 2019.x, 2020.x, and newer are supported.
- If you use Unity 2018.x, version 2018.4 or newer are supported.
- If you use Unity 2017.x, only Unity 2017.4.40 is supported. All other versions aren't supported.

### Required Android Version
To use Google Play In-App Updates plugin, you must target a minimum Android SDK version of:

- Android Lollipop (API level 21)

### Required Play Plugins

The following Google Play plugins will be installed automatically when you install the Google Play In-App Updates plugin using OpenUPM or when importing the package from GitHub:

- [External Dependency Manager plugin for Unity (EDM4U)](https://github.com/googlesamples/unity-jar-resolver). This plugin resolves AAR dependencies such as the Play Core library.
- [Play Core plugin for Unity](https://github.com/google/play-core-unity). The Play Core plugin provides the Play Core Library required by some other Play plugins, such as Play Asset Delivery, Play Integrity, Play In-app Reviews, and Play In-app Updates.
- [Play Common plugin for Unity](https://github.com/google/play-common-unity). The Play Common plugin provides common files required by some other Play plugins, such as Play Asset Delivery, Play Integrity, Play In-app Reviews, and Play In-app Updates.

## Install the Plugin

To install the Google Play In-App Updates plugin using Open UPM, follow the instructions to [install via package manager](https://openupm.com/packages/com.google.play.appupdate/#modal-manualinstallation) or [install via command-line](https://openupm.com/packages/com.google.play.appupdate/#modal-commandlinetool).

Alternatively, you can download the latest `.unitypackage` from the Google Play In-App Updates plugin [GitHub releases page](https://github.com/google/play-in-app-updates-unity/releases) and import it [using these instructions](https://developers.google.com/unity/instructions#install-unitypackage) on the Google APIs for Unity site.

## Sample app
You can find a sample app for Google Play In-App Updates plugin in the [Samples folder](https://github.com/google/play-in-app-updates-unity/tree/main/Samples) on Github. This sample app shows you a working example of the feature in a Unity game.

## Support

To request features or report issues with the plugin, please use the [GitHub issue tracker](https://github.com/google/play-in-app-updates-unity/issues).

You can find more resources and support options in the official documentation for [Google Play In-App Updates](https://developer.android.com/guide/playcore/in-app-updates).

## Data Collection

When you upload a game using this plugin to Google Play, Google collects the following data to help improve our products and services:

- Package name
- Version number
- Google Play Plugin version number

To opt-out of this data collection, remove the `packagename.metadata.jar` file found under each plugin's `Runtime/Plugins` folder.

**Note:** This data collection is independent of Google’s collection of library dependencies declared in Gradle when you upload your game to Google Play.

## Related Plugins

Browse other [Google Play plugins for Unity](https://developers.google.com/unity/packages#google_play).
