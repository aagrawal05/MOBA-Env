# 🎮 MOBA-Env 🤖

A MOBA-style multiplayer reinforcement learning environment built with [Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents). Teams of agents (Support, Damage, Tank) compete in arena battles, trained via self-play or against fixed opponents.

## 📋 Overview

- **Engine:** Unity 2023.2
- **RL framework:** Unity ML-Agents
- **Modes:** 3v3 team battles, 1v1
- **Win condition:** Eliminate the opposing team within 300 seconds

## ✅ Requirements

- **Unity:** 2023.2.x (2023.2.3f1 tested)
- **ML-Agents:** Unity ML-Agents package (see [Setup](#setup))
- **Platform:** Standalone (Linux headless for training, macOS/Windows for visualization)

## 📁 Project Structure

```
MOBA-Unity/
├── Assets/
│   ├── Editor/           # Build scripts (headless)
│   ├── HeadlessPrefabs/  # Lightweight prefabs for headless builds
│   ├── ML-Agents/        # ML-Agents configs, timers, examples
│   ├── Prefabs/          # Agents, arena, projectiles, UI
│   ├── Resources/        # Materials, shaders
│   ├── Scenes/           # Game scenes (3v3, 1v1, optimized)
│   └── Scripts/          # Core game logic
│       ├── MOBAAgent.cs       # Base agent class
│       ├── MOBAEnvController.cs  # Environment controller
│       ├── DamageAgent.cs      # Ranged DPS
│       ├── SupportAgent.cs     # Healer
│       ├── TankAgent.cs        # Melee tank
│       ├── Projectile.cs       # Projectile logic
│       └── ...
├── Packages/
│   └── manifest.json     # Dependencies (ML-Agents, etc.)
├── ProjectSettings/
└── design.md             # Game design spec
```

## 🚀 Setup

### 1️⃣ Clone and Open

```bash
git clone <repo-url>
cd MOBA-Unity
```

Open the project in Unity Hub with Unity 2023.2.x.

### 2️⃣ ML-Agents

This project depends on **Unity ML-Agents**. If `Packages/manifest.json` references local `file:` paths, update them for your setup:

**Option A – Local ML-Agents clone**

If you have [ml-agents](https://github.com/Unity-Technologies/ml-agents) cloned locally, set the path in `Packages/manifest.json`:

```json
"com.unity.ml-agents": "file:/path/to/your/ml-agents/com.unity.ml-agents",
"com.unity.ml-agents.extensions": "file:/path/to/your/ml-agents/com.unity.ml-agents.extensions"
```

**Option B – Package Manager**

You can add ML-Agents via Git URL or the Unity Package Manager. See the [ML-Agents installation guide](https://github.com/Unity-Technologies/ml-agents/blob/release_21/docs/Installation.md).

### 3️⃣ Open a Scene

Open one of:

- `Assets/Scenes/MOBA_1v1.unity` – 1v1 mode
- `Assets/Scenes/MOBA.unity` – 3v3
- `Assets/Scenes/MOBAOptimized.unity` – 3v3 (optimized for training)

Press ▶️ Play to run in the editor.

## 🎮 Manual Control (Editor Testing)

You can test the environment in the editor without trained models. With no Python connection, agents use the built-in **Heuristic** (keyboard control).

### ⌨️ Keyboard Controls

| Key | Action |
|-----|--------|
| **W** | Move forward |
| **S** | Move backward |
| **A** | Rotate left |
| **D** | Rotate right |
| **Q** | Strafe left |
| **E** | Strafe right |
| **X** | Attack |
| **Space** | Ability |

> **Note:** In editor, all agents respond to the same keys and move together. Focus the Game view for input.

## 🖥️ Building Headless (Training)

For RL training, build a Linux headless executable:

1. **Build > Build Headless** (Editor menu)
2. Or run from the command line using Unity’s batch mode.

Output goes to `Builds/Headless/`.

Example Python launch (with a compatible RL wrapper):

```bash
python train.py --env-path Builds/Headless/MOBA-Unity.x86_64
```

## ⚔️ Agent Roles

| Role    | Style      | Attack           | Ability                     |
|---------|------------|------------------|-----------------------------|
| 💚 Support | Healer   | Heal projectile  | Global team heal (60s CD)   |
| 💥 Damage  | Ranged DPS | Arrows         | Piercing arrow (20s CD)     |
| 🛡️ Tank    | Melee    | AOE swing        | Empowerment buff (30s CD)   |

See `design.md` for full stats and mechanics.

## 📊 Environment Details

- **Observation space:** 👀 Agent-centric observations (walls, allies, enemies, projectiles)
- **Action space:** 🎯 Discrete (movement, attack, ability)
- **Rewards:** 🏆 Win/loss, draw penalty, optional shaping
- **Reset:** 🔄 Configurable max steps (default 25,000)

## 🎬 Scenes

| Scene                | Description                    |
|----------------------|--------------------------------|
| MOBA.unity           | 3v3 with visuals               |
| MOBAOptimized.unity  | 3v3, optimized for training    |
| MOBA_1v1.unity       | 1v1                            |
| MOBA_1v1Optimized.unity | 1v1, optimized              |
| MOBA_1v1B.unity      | 1v1 variant                    |

## 📚 References

- [Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents)
- [ML-Agents Documentation](https://github.com/Unity-Technologies/ml-agents/blob/release_21/docs/Readme.md)
