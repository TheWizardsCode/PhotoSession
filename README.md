# Photo Session

## Introduction

Photo Session allows allows you to pause the game, move around with the camera and capture screenshots. It works in the Editor's Play mode and in the Build.

Malbers released his awesome Realistic Wolves and I wanted to capture close screenshot of those beautiful furry models. 
I tried to use NVidia's Ansel for the capturing, but it turned out that Ansel is unsupported for Unity and the version on the Unity Asset Store doesn't work for HDRP.

So I quickly coded this tool.

Example:

![wolfy-example](https://user-images.githubusercontent.com/10963432/111339454-0ef3fa80-8678-11eb-844a-58e38db3081f.jpg)

## Usage

Currently the focus is on the Malbers assets, but it should work for other purposes as well. In order to use this, add the PhotoSession script to FreeLookCameraRig.

![add-script](https://user-images.githubusercontent.com/10963432/111336115-3b5a4780-8675-11eb-8e0c-73d5064d3b62.png)

This will provide these settings in the inspector:

![settings](https://user-images.githubusercontent.com/10963432/111337110-1914f980-8676-11eb-9066-f731aac5434b.png)

Description of the properties:

* Toggle Key: is used to toggle between Gameplay and Photo Session mode
* Photo Camera: the camera that's being used for navigating around in the scene in Photo Session mode. This is usually the Main Camera.
* Other Camera Settings: custom movement sensitivity settings
* Reuse Previous Camera: If enabled, then toggling Game and Photo Session mode will restore the camera transform of the previous session. Otherwise the transform of the current gameplay camera will be used for position and rotation of the Photo Camera 
* Disabled Components: In order to have the main camera move around as Photo Camera the input of that camera needs to be disabled in Photo Sessionmode. In my case the FreeLookCamera script of the FreeLookCameraRig needs to be disabled.

Example values:

![settings-example](https://user-images.githubusercontent.com/10963432/111337134-1f0ada80-8676-11eb-919b-84cdd08ed0c7.png)

If you hit play in the Unity Editor or if you start a Build, hit the F12 key (depending on your key setting). The game will be paused. You can capture a screenshot by pressing the left mouse button.

The captured screenshots will be stored in parallel to the Assets folder in the Editor or in parallel to the data folder in the Build in a dedicated Screenshots folder.

The size of the screenshots depends on the game window.

## Controls

The controls are currently minimalistic:

* press the Toggle Key (in the example case F12) in order to pause the game
* navigate with the camera using 
  + WSAD keys: move vertical and horizontal
  + QE keys: move up/down
  + Mouse: rotate the camera
* press left mouse button in order to capture and save a screenshot
* press the Toggle Key to continue the game

## Future Ideas

This asset was created for personal purposes, but I thought you guys might find use for it, so I made it public. There are a lot of other future possibilities:

* indicate a camera display using Canvas when in Photo Mode
* simulate a flash when a screenshot is captured and saved
* help indicators
* various settings like fov, view distance
* various post processing additions like Depth of Field
* swappable post processing settings
* use supersize
* capture stereo
* capture 360 degree (e. g. using Unity's Frame Recorder)
* slow down time instead of just pausing
* ...

Lots of possibilities. Feel free to fork, enhance and share.

## Limitations

The functionality currently depends on the Time class and what it provides for pausing a game.

## Credits

* ashleydavis: Unity FreeCam
  - https://gist.github.com/ashleydavis/f025c03a9221bc840a2b

* Chris Bellini: Screenshot Helper
  - http://untitledgam.es




