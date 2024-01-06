# Contributing

## General Rules
- If you want to contribute code to the project, please, read these rules:
  1. You may NOT steal proprietary code, you may only add MIT or BSD licensed code.
  2. The code must NOT contain all kind of malware, tokens, passwords, api keys, unlawful and obfuscated code.
  3. Please, insert the code in proper folder (see Project tree)
  - If these conditions are met, make a Pull Request. Owner will check your code, and decide, include this (apply) or not (deny).
- If you want to contribute assets to the project, please, open [this](https://github.com/Yni-Viar/scp-assets) link for more information.

## Project tree:
- Assets - used for most assets except special ones, mentioned below.
- Decals - for decals and particles.
- FPSController is the home of Player prefab and player classes.
   - PlayerClassPrefab - a prefab, which is applied to a player when forceclassing.
   - PlayerClassPrefabGameover - a prefab, which is applied to a player, when HP has reached zero.
   - PlayerClassResources contains resources of classes (needed for forceclassing).
   - PlayerClassScript - Scripts for PlayerClassPrefab.
- GDSh - plugin for in-game console.
- Inventory system
   - Items is the base directory for items.
      - InHandPrefabs - used for first and third person item view (third person will be moved to Pickable prefabs later)
      - PickablePrefabs - used for pickables.
      - Projectiles - used for special items.
- LoadingScreen
- MapGen contains the map generation algorityhm, it's 2D visualization and all facility objects (doors, rooms, props, etc).
- Scenes contain **only** main game scenes.
- Scripts contain rest of the code.
- Shaders.
- Sounds.
- UI contains Godot style assets.