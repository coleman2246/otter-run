# Some Notes
- Not sure how much detail is expected, so I may have gone a little overboard on the instructions.
- I am by no means an expert in signal processing and learned quite a lot about this as I went. Some of the things I have done in this project with regards to signal processing may not be "optimal", but it seems to work reasonably well.
- Some sections may be a little bit hard for most 12-15 year olds to follow, but the majority of it should be easily followable.
- There are various systems that have been implemented in this demo, but for the sake of brevity only the major ones are going to be covered in depth.

# Background
## Concept
The goal of this project is to create procedurally generated rhythm platforming game that is similar in concept to that [Geometry Dash](https://en.wikipedia.org/wiki/Geometry_Dash), but could work with songs of your choosing. In practice it really only works well with instrumental music, but with a more sophisticated song analysis system could work with other songs.

## Prerequisites
- Basic understanding of some math (basic algebra, trigonometry). Understanding of calculus would be useful, but you can treat some things as a black box.
- Some background in basic kinematics would be useful 
- Some basic background in signal/audio processing would be useful
- Basic understanding of algorithms


## Dependencies
- The sample game has been developed using Unity 2021.3.16f on Arch Linux, but should work on other versions/platforms.
- Unity doesn't provide some features that are going to be needed to create this game. In particular it does not offer a way to perform a Fast Fourier Transform (FFT) on arbitrary data and it does not allow the user to have access to the file browser outside of the editor. In an effort to not reinvent the wheel on those things 2 open source libraries were used.
[StandAlone File Browser](https://github.com/gkngkc/UnityStandaloneFileBrowser) for the file browser and [BurstFFT](https://github.com/keijiro/BurstFFT) for the FFT


## Format of Instructions

# Prefabs
Before we get started you are going to need to create 2 sets of prefabs. For the time being these prefabs are just going to be filled with sprite renderers and an apropriate sprite. The first set of prefabs (Building Block Prefabs) that you are going to need to create are as follows:
- Floor
- Floor Support
- Death
- Bonus Point
- Level End

You are also going to need to create a player prefab.

You can see examples of these prefabs in `Assets/Prefabs`. Please ensure that each prefab is 1 meter wide and for all of them except the player ensure they are 1 meter tall.

# Level Generation
This is likely the most complex system in this game, but things will tried to be kept as simple as possible.
## Concept
Lets start by assuming we have a song we want to setup the world for. Start by making our player move at a high constant speed in the x direction. Lets now break the world up into a series of giant grids that connect one after another, lets call one of these grids a level unit. We can represent each level unit as 2D array filled with building blocks. Fill the middle of the level unit with a Floor with random increases or decreases in height. Note that the random seed is set based on the songs hash. Use a flood fill algorithm to fill everything bellow the floor with floor support building blocks. Place bonus objects based on a random value in the air above the floor. Place a death building block based on audio analysis, which should correspond to an increase in power at that point in the song. In the first and last level unit dont put obstacles. In the last put a wall of level end building blocks. After the grid is created loop through it and instantiate each prefab accordingly.
## Audio Analysis
For this section lets assume that that the level unit is going to last for 2 seconds, the audio sampling rate in 22khz, and that our [FFT window](https://support.ircam.fr/docs/AudioSculpt/3.0/co/Window%20Size.html) is 1024. 
### What are we trying to do ?
We are trying to do an audio analysis on the audio that will be playing a during level unit. We are trying to find the point in the level unit where each [audio band](https://www.teachmeaudio.com/mixing/techniques/audio-spectrum) peaks in power level. These peaks in power should correspoond to where you can "feel" the music. A report of where each band peaks should be generated, as well as the average power increase for that band.
## Finding Points of Max Power Increase For Each Band

This section is very difficult to express simply in words, you may have an easier time reading the code of the sample. The code can be found in `Assets/Scripts/AudioAnalysis.cs`

Start by breaking that 2 second portion of the song into 20 windows (each window should last 1/10 of a second). Perform an FFT with [blackmann windowing](https://docs.scipy.org/doc/scipy/reference/generated/scipy.signal.windows.blackman.html) on every window to get the power of each frequency at each window. 


Loop over ever frequency and than loop over every window. Make sure to keep track of the current band that you are in. Find the change of in power level between the current frequency at the current window and the first window. When you change frequencies such that you are no longer in a band, record the following info about the band:
- The average power increase of that band over the level unit
- The percentage through the level unit the highest increase in power occurs

## Floor
Each floor prefab is going to need 3 2D colliders, one 2D rigidbody, a floor collision handler script, and and a death object script. The death object script should trigger the death of a player on collission and the floor collision should indicate that the player has hit the floor. The 3 colliders should perform the following:
- A collider with the left side of the floor set to a trigger and should be used to trigger the death object script.
- A collider that surrounds the floor but is slightly smaller than the death collider on the left side so the user will not be able to pass through the floor from any angle.
- Finally a collider on the top of the floor set to trigger the floor collission handler script used to indicate if the player is touching the floor.
## Death Objects
Death objects are just a sprite render with a collider set as a trigger. A death object script should be attached to this prefab that will call the death function created in the player script. 


## Bonus
This section is very similar to that of the death object, except it will call the bonus point increase part of the player script. The bonus sprite should also disappear on impact so player knows they have collidded. To add a challenge this object should also be oscillating between two heights (make use of some trigonometric functions).

# Player
This is the section that has all the most moving parts. This section is going to be responsible for:
- Managing movement
- Controls
- Player state
- Keeping track of stats 
- Animation state control
- Song control

You can find most of the sample code described in this section in `Assets/Scripts/Player.cs`


## Movement 
There are a few things that the movement section is responsible for. They are as follows:
- Maintaining a constant speed in the X direction
- Allowing for a double jump that lets the user jump a specified distance in a specified amount of time
- Forcing the stop of all movement when player is dead or is paused
- Reposition the player when the level restart

## Jump
When the player presses the space bar down, the player should jump

## Player State
Various states are going to have to be kept track. You should keep track off:
- Is the player alive
- Is the game paused
- Has the player reached the end of the level
- How many jumps does the player have remaining in the double jump
- Is the player in the air 
- Are you currently expecting the player to return to the ground

All of these states need to be able to be reset if the player restarts the level

## Keeping Track of Statistics
There are various stats that are going to have to be kept track of so the UI section can display them. They are as follows:
- ### Bonus Multiplier
    - This value should increment whenever the player interacts with a bonus object
- ### Score
    - This value should increment every frame. Value should increment by change in x since last frame multiplied by multiplier.
- ## Percent done the level
    - This value should be the percent distance in the x the player is done the level.

## Song Control
When the player dies or pauses, pause the music playback. The ability for the song to be restarted should be implemented.

## Animation State Management
There are 3 animation states of the player is going too need to have, they are as follows:
- Running
- Jumping
- Landing

There are animation parameters that also need to be set
### Animation Parameters 
- Jump 
    - This is a trigger for when the player presses the jump button
- Hit Floor
    - This is a trigger for when the player hits the ground after a jump


### Animation State Transitions
Transitions should be as follows:
- The default state should be the run state
- When Jump is triggered you should transition to Jump state
- When Hit Floor is triggered and you are in the Jump state transition to the land state
- After the land animation is finished transition back to the run state



# Camera
The camera should follow the players current location, with the player being centered on the screen.

# Settings
There are various settings that are going to have to be kept track of. All of these settings should be saved so they persist between restarts. The settings that should be tracked are:
- Music Path
    - This is the directory that the mp3s are going to be kept in 
- Volume Level
    - A value between 0-100% which will be used to set volume of song
- Selected Character
    - The character that the player wishes to play as 

# High Scores
The high scores for each song should be saved so they are saved between sessions. The high scores should be updated whenever a new high score has been set. The scores should be associated with the hash of the song, so they are invariant of name/location of the file. 

# UI & Scene Transition
## Main Menu
This should be the first scene, in is basically pure navigation. It should have the follow buttons:
- Play Button
    - This button should transition to the level picker menu scene
- Settings Button
    - This button should transition to the settings menu scene
- Quit Button
    - This button should quit the game
## Settings Menu
This menu should consist of the following:
- Path selection button
    - This is used to set path in the settings section.The file browser should open when clicked
- Character selection foldout
    - This is used to set the selected character in settings section.
    - This fold out should show all available characters sprite preview
    - The currently selected one should be highlighted
- Volume slider
    - This slider is used to set the volume parameter in the settings section
    - Values should range from 0-100;

Pressing the escape button should navigate back to the main menu

## Level Picker Menu
This menu should consist of all mp3 files in the folder specified in the settings. These songs should be displayed in a giant list that is scrollable. Each song element should be consist of the following:
- A button with the name of the song. When the button is pressed it should switch scenes to the level scene. Upon switching scenes the following should be done in the game level
    - The song should be loaded from disk into to unity and passed to the level scene
- The high score of the song displayed just under this button 

If no songs are present a button that prompts the user to navigate to the settings should be displayed instead.

Pressing the escape button should navigate back to the main menu

## Pause Menu
The pause menu should consist of 3 items centered on the screen:
- A restart button that will reset the player and song to the start of the level
- The quit button should navigate back to the level picker menu scene
- The volume slider should have the same functionality as in the settings

## Level Stats UI
The level stats UI should get all of its information from the player section discussed earlier. It should contain the following UI elements:
- Text displaying the players current score
- Text displaying the players current multiplier
- Text displaying the high score for he song. Should be current score if current score is higher than high score.
- A progress bar that displays the percent complete the level is as well as is filled in to that percent
