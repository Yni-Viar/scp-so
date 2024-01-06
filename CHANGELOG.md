# Changelog
The current state of the game - Alpha.
## Alpha versions
### v.0.7.2 (2024.01.06)
- Added force start command (requested by users)
- Added NPC subsystem (SCP-650 included). (There is a chance for a NPC spawning)
- Added roundstart command.
- Improved weapon system.
- Implemented version checker (currently effective for new versions)
- Fix keycarded door still produce "allow" sound, even if keycard is lower than needed. 
- Now loading screen is also working client-side!
### v.0.7.1 (2023.12.31)
- Fixed mouse sensivity.
- Reworked UI code.
- Fixed mouse input when SpecialScreen is enabled.
- Added outline to arrow.
- Reworked SCP-173 mechanic, so it should not move while not blinking...
- Temporarily removed SCP-173 light detector, because of being unstable.
- Fixed "restart-server-bug" (if you host and exit the server 3 times, the game could not load)
### v.0.7.0 (2023.12.26)
- Changed game logo to nicer one.
- Finally added loading screen!
- More dead ends can be spawned.
- SCP-018 model now has smooth model.
- Modernized LCZ.
- Improved forceclass and pre-round screens.
- Added support for player nicknames.
- Fixed bug, where exiting the server broke the game.
- Added a shader to SCP-173, so it is possible for changing SCP-173's face without changing the main concrete texture.
- Class-Ds now have face randomization.
- Fixed bug, where screen settings were not applied, when restarting.
- Switched to Jolt physics, allowing to support ragdolls.
- Current SCP-079 implementation changed to SCP-2522 "hatbot.aic".
- Removed SCP-650 as player class.
- Revamped door system (again), to make it more flexible.
- Added weapon system.
- Added recontainment system for SCP-173 and SCP-3199.
- Changed the shader for SCP-173, also removed useless Sketchy Shader.
- Fixed sky being used even if player was outside of SZ.
- Reworked movement system (animation-side)
- Fixed Scientist's hands.
- Added primitive admin panel.
- Every December, the Main Menu and SCP-173 face will change to Christmas one.
- New rooms in LCZ + new spawn for SCP-018
- Optimized game by reducing the room visibility range (and reducing the fog range).
- Added win conditions - the game is playable, so the next version will be labeled as "Beta" (I hope)
### v.0.6.2 (2023.10.23)
- Added blackout ability to SCP-079.
- Added new pixel font for SCP-079.
- Added energy bar for SCP-079
- Adjusted a size for medkit (in 0.6.1 it was too large)
- Fixed wrong rotation of guard.
- Added more abilities for both SCP-131 instances.
- "sethp\" command renamed to "givehp"
- Fixed bug, where you could not use player commands, while next connected person left the game.
- Fixed bug, where SCP-106's teleportation caused *all* people to teleport to Larry.
- Re-added SCP-018 (now it causes damage - more speed = more damage).
- Implemented item drop on class change.
### v.0.6.1 (2023.10.19)
- Fixed a flaw in forceclass.
- Changed SCP-650 room from SCP-CB design, to new design. (Old room is still available, but disabled from spawning)
- Fixed size of SCP-079 model in their room (previously it was very small).
- Fixed SCP-079 camera going off rotation limits.
- Added medkits.
- Fixed itemlist not downloading.
- Fixed ragdoll not spawning.
- Fixed door not syncing.
- Added interaction sound. (Better, than in 0.0.5 😎 )
- Fixed a bug, where LCZ unique rooms could duplicate.
- Fixed a bug, where current health was equal 1 all the time (+ updated health panel).
- Added dither shader (used on SCP-650)
- In-hand items now have collisions.
- Added advanced music settings + added glow setting.
### v.0.6.0 (2023.10.15)

- Re-added inventory with items. (Keycard system is also working)
- Increased distance between zones for optimization (except Class-D chamber).
- Items can be refined in SCP-914.
- Added death animations. (initially, there could be ragdolls, but they were not perfect)
- Added SCP-131.
- Added SCP-079 (currently is unfinished)
- Disabled SCP-650 due to being unfinished.
- Reworked SCP-106 (different Stalk mechanic, ability to go through doors)
- Improved map generation - it is modifiable now (plugins still not supported)
- Updated Light Containment Rooms again, now there are better rooms!
- Now you can hear more background music!
- Fixed the connection to ongoing rounds (through the feature is still experimental)
- Fixed bug, where surface elevators couldn't work.
- Added custom shader support (currently used on SCP-173)
- Added custom camera support (used on SCP-079)
- Regular doors and gates can be opened without button (keycard equivalents and elevators still need clicking buttons to function)
- MTF ε-11 "Nine-Tailed Fox" can arrive every 5 minutes!
- Some small bugfixes.

### v.0.5.3 (2023.09.10)

- Fixed falling bug, due to gravityVector always incrementing...
- Added missed props and collisions.
- Changed texture in Class-D chamber.
- Added a plaque near elevators.
- Fixed an issue, when player could see behind walls.
- Fixed an issue, when player could break elevator system.

### v.0.5.2 (2023.09.06)

- Fixed bug, where skybox were seen in Pocket Dimension.
- Added proper model to Science personnel.
- Changed default sensivity to higher value.

### v.0.5.1 (2023.09.05)

- Re-saved old SCP-CB LCZ rooms (and reduced size of the package). Thus, I removed old empty hallways, SCP-012 room, and old SCP-079 room. (most of these rooms have new analogues).
- Added item SCP room (currently, from SCP-CB, but eventually it will be modernized)
- Added abilities message (only SCP-106 and SCP-650 have it).
- Added blankets in D-class chamber.
- Added ability to change mouse sensitivity.
- Changed the ground color in Surface Zone

### v.0.5.0 (2023.09.03)

- Added FPS counter.
- Settings refactor.
- Added own UI style.
- More new rooms in Light Containment Zone.
- Player Class System refactor.
- Fixed a bug in Surface Zone, where player couldn't return to the facility, while being off the road.
- Fixed a bug, where checkpoint rooms couldn't be spawned.
- Fixed a bug, where SDFGI stuttered the game.
- Fixed a bug, where elevators were working only once.
- Fixed SCP-173.
- Implemented proper round start and escape, also you can connect even if round starts.
- We are ON-line now, not OFF-line 😉
- Added SCP-106.
- Added D-Class containment zone.
- Added No-UI mode
- Reworked footstep sound system.

### v.0.4.1 (2023.08.25)

- Fixed bug when an entity had problem in spawning just because the spawn room was not spawned.
- Fixed door button spamming.
- Only debug builds has no mouse capture.
- SCP-914 can now refine players (items are still NOT implemented)

### v.0.4.0.1 (2023.08.24)

- Fixed too dark Surface Zone, when SDFGI is disabled.

### v.0.4.0 (2023.08.24)

- Added gates
- Started the Light Containment Zone overhaul.
- This game is in my project now.
- Improved door system.
- More rooms.
- Nerfed SCP-650.
- SCP-3199 has new home!
- Added Surface Zone!
- Added SCP-914 (currently, it does not refine at all)
- Added MTF.
- No disturbing models in first-person (only for classes, which have models)
- Optimized door system.

### v.0.3.1 (2023.08.13)

- SCP-650 now has animations!
- Fixes the bug, where you cannot kill the player as SCP-173 or SCP-3199.
- Fixes the bug, where player model got duplicated.

### v.0.3.0 (2023.08.12)

- New map gen (again, because previous could not spawn important rooms. This mapgen may also not spawn rooms, but this is a waaay rarier)
- Fixed forceclass working only on first player, who used this command.
- SCP-173 now works as it should. (except some bugs, mentioned below)
- Elevators are working again!
- Added Player HUD Prototype.
- Added SCP-650.
- Added SCP-3199.

## Pre-Alpha versions

### v.0.2.0.2 (2023.08.04)

- Hotfix: forceclass command now works.

### v.0.2.0.1 (2023.08.04)

- Hotfix: added changelog, remove -dev sign and tried to fix mapgen overlapping.

### v.0.2.0 (2023.08.04)

- Ported the game to multiplayer, except inventory and item system.
- Various fixes + old loading screen got deprecated (it is still in game files, but is not used).
- Added player classes (currently only placeholder human and SCP-173 present)

### v.0.1.0 (2023.07.17)

- The items can be used, no refactoring is required :D
- Added debug console.
- Elevator has sounds now.
- The game now has a name!
- Added SCP-018.
- More settings.
- Fixed bug (mouse button is released when using an item).

### v.0.0.6 (2023.07.08)

- Map Generator v2 was not suitable for the game - then meet Map Generator v3 - made by me!
- Added door generation - because new map gen supports this!
- Heavy Containment Zone is back!
- SCP-173 now rotates to player, when blinking.
- Added JSON support (currently is only used by items)
- Added INI support (currently is only used by settings)
- Added elevators between zones - now zones are fully connected!
- Fixed the crunch workaround in SCP-173 (this workaround was made due to bad knowledge 🙂 )
- Fixed dropping items out of the map
- Fixed item dupe.
- Door can be locked (keycards are still not implemented)
- Now you can toggle out background music.
- Now settings can be saved (through INI)

### v.0.0.5 (2023.05.27)

- Added new map generator, which fixes the last room bug.
- Regress: Heavy Containment Zone don't spawn (due to the zone has few rooms), there is no way to transfer between zones.
- Added doors.
- Item pickup sound.

### v.0.0.4 (2023.05.17)

- Inventory
- Item can be moved to inventory, and dropped from it.
- Fixed infinity jump bug.

### v.0.0.3 (2023.05.06)

- Pause menu and exit to menu implemented.
- Updated graphics
- Basic settings
- Added a sound in the endroom
- Moved MapGen outside Addons folder, because it was conflicting with Godot plugins.
- Pickable is now a Godot plugin.

### v.0.0.2 (2023.05.01)
- Picking up items
- Heavy Containment Zone(HCZ) + redesigned SCP-173 Containment Chamber and the return of SCP-049 Containment Chamber from Map Generator era, but now also in HCZ.
- Now SCP-173 will STOP if player looks at them.

### v.0.0.1 (2023.04.22)
- Map Generation
- SCP-173 AI
- SCP-650 AI
- Blinking system