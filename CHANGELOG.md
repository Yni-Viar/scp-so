# Changelog
The current state of the game - Alpha.
## Alpha versions
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