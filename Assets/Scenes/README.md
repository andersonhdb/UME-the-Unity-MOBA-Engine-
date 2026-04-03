# UME Scenes

This directory contains the Unity scene files for the UME project.

## Scenes

| Scene | Purpose |
|-------|---------|
| `LaunchScreen.unity` | Initial loading screen. Initialises core singletons (GameManager, SceneLoader, LobbyManager). |
| `MainNavigation.unity` | Main menu with links to Play (Lobby), Training, Map Creator, and Settings. Uses `MainMenuUI`. |
| `Lobby.unity` | Pre-game lobby where players join/create sessions via UGS Lobby + Relay. Uses `LobbyManager`. |
| `GameConfig.unity` | Game configuration screen (map selection, team assignments, custom rules). |
| `MainGame.unity` | Primary gameplay scene. Contains the NavMesh, lane spawners, item shop, camera rig, and HUD. |
| `Training.unity` | Practice/tutorial mode. Single-player sandbox with dummies and skill testing. |
| `MapCreator.unity` | Visual map editor using `MapCreatorController`. Saves `MapData` ScriptableObjects. |

## Setup Instructions

1. Open Unity 6.4 (or compatible) and open this project.
2. Unity will import packages from `Packages/manifest.json` automatically.
3. Create scenes via `File > New Scene` and save them with the file names listed above.
4. Add each scene to `File > Build Settings` in the order listed.
5. Assign the `GameManager` prefab (from `Assets/Prefabs/`) as the first object in `LaunchScreen`.
