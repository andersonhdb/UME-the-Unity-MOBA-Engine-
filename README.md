# UME — Unity MOBA Engine

An open-source framework that gives you the boilerplate to build your own MOBA game with Unity 6.

---

## Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| **Unity** | 6.4 | Game engine |
| **Unity NetCode for GameObjects** | 2.2.0 | Authoritative networking |
| **Unity Gaming Services – Lobby** | 1.2.2 | Pre-game matchmaking |
| **Unity Gaming Services – Relay** | 1.1.1 | NAT-punch-through transport |
| **Cinemachine** | 3.1.2 | Top-down camera |
| **Addressables** | 2.3.1 | Asset loading & Asset Bundles |
| **AI Navigation (NavMesh)** | 2.0.5 | Pathfinding |
| **Input System** | 1.11.2 | Point-and-click + keyboard input |
| **TextMeshPro** | 3.0.9 | All UI text |

---

## Project Structure

```
Assets/
├── Scenes/                    # All game scenes (see Scenes/README.md)
├── Scripts/
│   ├── Core/                  # GameManager, GamePhase, GameConfig, SceneLoader
│   ├── Characters/
│   │   ├── Base/              # UnitBase, UnitStats
│   │   ├── Hero/              # HeroController, HeroData
│   │   └── Minion/            # MinionController, MinionData
│   ├── Camera/                # TopDownCameraController (Cinemachine)
│   ├── Movement/              # PointClickMovement (NavMesh + Input System)
│   ├── Network/               # LobbyManager, NetworkGameState, NetworkPlayerData
│   ├── Systems/               # ResourceSystem, ItemSystem, ItemData, SkillData,
│   │                          # SkillBuilder, MinionSpawner, KDATracker
│   ├── MapCreator/            # MapCreatorController, MapData
│   └── UI/
│       ├── HUD/               # BottomBarUI, HealthBarUI, KDATrackerUI, SkillSlotUI
│       ├── Menus/             # MainMenuUI, HeroSelectionUI, HeroBanningUI, HeroCardUI
│       └── Shop/              # ItemShopUI, ItemCardUI
├── ScriptableObjects/
│   ├── Heroes/                # HeroData assets
│   ├── Items/                 # ItemData assets
│   ├── Minions/               # MinionData assets
│   ├── Skills/                # SkillData assets
│   └── Maps/                  # MapData assets
├── Prefabs/
│   ├── Heroes/
│   ├── Minions/
│   ├── UI/
│   ├── Map/
│   └── Network/
├── Art/                       # Materials, Textures, Models, Animations
├── Audio/                     # Music, SFX
└── AddressableAssetsData/     # Addressables group configuration
Packages/
└── manifest.json              # All package dependencies
ProjectSettings/
├── ProjectSettings.asset      # Company/product name, build targets
├── TagManager.asset           # Tags and layer definitions
├── EditorSettings.asset       # Root namespace = UME
└── EditorBuildSettings.asset  # Scene order in build
```

---

## Scenes

| Scene | File | Key Script(s) |
|---|---|---|
| Launch Screen | `LaunchScreen.unity` | `GameManager`, `SceneLoader` |
| Main Navigation | `MainNavigation.unity` | `MainMenuUI` |
| Lobby | `Lobby.unity` | `LobbyManager` |
| Game Config | `GameConfig.unity` | `GameConfig` (SO) |
| Main Game | `MainGame.unity` | `TopDownCameraController`, `BottomBarUI`, `MinionSpawner`, `ItemShopUI` |
| Training | `Training.unity` | `HeroController`, `SkillBuilder` |
| Map Creator | `MapCreator.unity` | `MapCreatorController` |

---

## Key Systems

### Characters
- **`UnitBase`** – Base class with health, armor, team, death/heal events.
- **`HeroController`** – Extends `UnitBase` with levelling, XP, gold, items, skills, KDA.
- **`MinionController`** – NavMesh-driven AI; follows lane path, attacks nearest enemy, scales per wave.

### Movement
- **`PointClickMovement`** – Right-click casts a ray onto the NavMesh and sets the agent destination. Works with Unity Input System or legacy input.

### Camera
- **`TopDownCameraController`** – Wraps a Cinemachine virtual camera with edge-scroll pan, keyboard pan, scroll-wheel zoom, and hero lock/unlock.

### Networking (Unity NetCode)
- **`LobbyManager`** – Creates/joins UGS Lobby + Relay sessions, handles heartbeat, and starts `NetworkManager` host/client.
- **`NetworkGameState`** – Server-authoritative `NetworkBehaviour` with `NetworkVariable`s for score, match time, and game phase.
- **`NetworkPlayerData`** – Per-client `NetworkBehaviour` for player name, team, hero selection, ready-state, and KDA.

### Systems
- **`ResourceSystem`** – Passive gold ticks per hero + resource node (jungle camp) collection.
- **`ItemSystem`** – Purchase validation (component checks, gold deduction, slot availability) and selling.
- **`SkillBuilder`** – Equips up to four skills (Q/W/E/R), tracks cooldowns, manages resource cost per level.
- **`MinionSpawner`** – Sends melee + ranged (+ siege every N waves) waves along a lane using Addressables to load prefabs.
- **`KDATracker`** – Aggregates kill/death/assist records for all players; fires events for scoreboards.

### ScriptableObjects
| SO | Menu Path | Key Fields |
|---|---|---|
| `HeroData` | UME/Hero/HeroData | stats, roles, skill list, Addressable keys |
| `MinionData` | UME/Minion/MinionData | stats, gold/XP rewards, wave scaling, attack config |
| `ItemData` | UME/Item/ItemData | cost, stat bonuses, components (build tree), category |
| `SkillData` | UME/Skill/SkillData | damage, cooldown, resource cost per level, VFX keys |
| `GameConfig` | UME/Core/GameConfig | players/team, bans, timers, starting gold |
| `MapData` | UME/Map/MapData | grid size, spawn positions, jungle camps, objectives |

### UI
- **`BottomBarUI`** – Portrait, health/resource bars, skill slots (Q/W/E/R with cooldown overlay), gold, KDA, XP bar.
- **`HealthBarUI`** – World-space slider that auto-follows a `UnitBase` and faces the camera.
- **`KDATrackerUI`** – Live K/D/A display with computed KDA ratio.
- **`HeroSelectionUI`** – Scrollable grid with search + role filter, preview panel, pick timer, and random pick.
- **`HeroBanningUI`** – Alternating-team ban flow with timer and ban display strips.
- **`ItemShopUI`** – Category tabs, search, item detail panel, purchase/sell buttons.
- **`MapCreatorController`** – Grid tile placement (left-click place, right-click erase), spawn marker tools, and save/export.

---

## Getting Started

### Prerequisites
- Unity 6.4 (or compatible Unity 6 LTS)
- Unity Hub

### Setup
1. Clone this repository.
2. Open the project in Unity Hub.
3. Unity will automatically resolve packages from `Packages/manifest.json`.
4. Create the scene files listed in `Assets/Scenes/README.md` and add them to **File → Build Settings** in order.
5. Create your first `HeroData` via **Assets → Create → UME → Hero → HeroData**.
6. Assign `GameConfig` to the `GameManager` component in the `LaunchScreen` scene.
7. Enter Play Mode from `LaunchScreen` to begin testing.

### Creating a Hero
1. `Assets → Create → UME → Hero → HeroData` → fill in stats, role, skills.
2. Create a hero prefab with `HeroController`, `PointClickMovement`, `SkillBuilder`, `NavMeshAgent`, and a `Collider`.
3. Set the `heroPrefabKey` in `HeroData` to the Addressable address of the prefab.
4. Add the hero to `HeroSelectionUI.availableHeroes`.

### Creating a Skill
1. `Assets → Create → UME → Skill → SkillData` → set damage, cooldown, VFX keys.
2. Assign the `SkillData` to a hero's `HeroData.activeSkills` list.

### Networking Flow
1. `LaunchScreen` initialises `LobbyManager.InitializeAsync()`.
2. Host calls `LobbyManager.CreateLobbyAsync()` → allocates Relay → starts host.
3. Clients call `LobbyManager.JoinLobbyAsync(code)` → joins Relay → starts client.
4. `NetworkGameState` (spawned by server) drives game phase transitions across all clients.

---

## Extending the Framework

- **New mechanic**: Add a new `MonoBehaviour` in the appropriate `Scripts/` subfolder and reference it via the relevant `ScriptableObject` or manager.
- **New hero ability**: Subclass or compose with `SkillData` + custom `ExecuteSkill` logic in a child of `SkillBuilder`.
- **New map tile type**: Add a prefab to `Assets/Prefabs/Map/` and register it in `MapCreatorController.tilePrefabs`.
- **Custom minion wave**: Create a `MinionData` SO, assign it in `MinionSpawner`, and set the Addressable prefab key.

---

## License

MIT — see [LICENSE](LICENSE).
