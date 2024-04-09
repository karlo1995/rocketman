Assumed Downloaded: Unity, Little RocketMan Game
Recommended Software Download: Screen Recorder Pro

*Notes*:
1. Please wait a couple of seconds if platforms and backgrounds don't immediately update or you can restart the editor window
2. Everything you need should be in the "Level Editor" and "Sprites" folder; no need to touch anything in the Hierarchy
3. Please calibrate the platform using the Editor Window rather than through the inspector as this will cause exporting issues
4. We are only asking to calibrate the platforms. We will calibrate other items or obstacles in the future
5. If you made a mistake in the entry, you can manually change it by opening the Data.csv file located in the Level Editor folder

*Screenshot Instructions*:
1. Open Screen Recorder Pro
2. Set Output Folder to "../Little Rocket Man/Assets/Level Editor/.." (Whichever World the Screenshot is For)
3. Run Little RocketMan
4. Full Screen Little RocketMan
5. Remove Taskbar
6. Take Screenshot Using [Window] Recording Option
7. Rename Screenshot Appropriately (See Below)

*Folder Structure and File Naming*:
The "Level Editor" folder holds all of the screenshots and are organized into their respective Worlds. The "Sprites" folder holds the different platform sprites. 
Name each screenshot with two numbers separated by a minus sign:
1. First number states the player level in the screenshot
2. Second number states the number platform of that level (since one level can have more than one platform)
Example: World 1, Level 1, First Platform (Not starting point): "1-1" because we are level 1 at the beginning and we are entering data for the first platform of level 1

*Create Data Entries*:
MAXIMIZE YOUR GAME VIEW THE BEST YOU CAN TO MORE EASILY CALLIBRATE
Find the Level Editor at: Tools >> SayDesign >> Level Editor
1. Configure
--- Input the Platform Sprite and the Background Sprite you want to use
--- The Background Sprite changes for every entry but the Platform Sprite might be the same from level to level or platform to platform
2. Entry
--- Input the World, Level, and Platform number (Level and Platform number should be the same as the Background Sprite series)
--- Select how the Platform is moving (None, 1D, 2D)
--- Use the controller to calibrate/find the position of the platform (Instructions on how to eyeball the platform position below)
--- Reset the platform position back to (0, 0) if needed
--- Make sure your data entry is correct and then proceed to export to the Data.csv file
*Each entry should be different (no duplicates)
*We need an entry for the first platform RocketMan is standing on because that is not always (0, 0); enter "0-1" for the level-platform label for origin entries
*If callibrating for World 3 or World 6 (bosses), keep the level 1 and just increase the platform number for each new entry

*How to Eyeball the Platform's Position*:
The platform we are moving around is slightly bigger than the actual platforms from the original game. 
1. Make sure the platform is in the middle
2. Make sure that the TOP SURFACE of the platform is aligned with the background's platform
3. For moving platforms, calibrate the platform's position in the axis that remains constant
--- For a platform moving up and down, calibrate for the x-coordinate and enter a random y-coordinate
--- For a platform moving left and right, calibrate for the y-coordinate and enter a random x-coordinate
--- For a platform moving diagnoally, calibrate for the platform's lowest position (in the end we'll most likely estimate this one)