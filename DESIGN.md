# 🎮 MOBA Design

A MOBA-style reinforcement learning environment built with Unity ML-Agents.

## 📋 Game Overview

- **Game length:** ⏱️ 300 seconds
- **Team composition:** 3v3 (Support, Damage, Tank) or 1v1
- **Win condition:** 🏆 Eliminate opposing team

## 📊 Base Stats (All Agents)

| Stat | Value |
|------|-------|
| ❤️ HP | 100 |
| 👟 Movespeed | 1.0 |
| 🛡️ Armor | 1.0 |
| ⚡ Power | 10 |
| 🗡️ AtkSpeed | 1.0 |
| 💚 Regen | 1.0 |

### 🎯 Projectile (Default)

- Shape: Sphere
- PowerScaling: 1.0
- Size: 0.75
- Speed: 1.0
- Piercing: False
- Despawns on collision (including walls)

---

## ⚔️ Agent Roles

### 💚 Support

- Power: 8
- Armor: 0.8
- Projectile Speed: 0.8
- Movespeed: 1.15
- **Attack:** Heal projectile (allies only)
- **Ability:** Global heal for all allied units
  - Cooldown: 60s

### 💥 Damage

- Power: 15
- **Attack:** Ranged arrows
- **Ability:** Larger piercing arrow
  - Piercing: True
  - PowerScaling: 2.0
  - Size: 1.15
  - Speed: 3.0
  - Cooldown: 20s

### 🛡️ Tank

- HP: 150
- Armor: 1.3
- Regen: 1.2
- Power: 12
- **Attack:** Melee swing (AOE hitbox, 0.5s duration)
- **Ability:** Empowerment buff for 12s
  - +25% movespeed
  - 2× damage
  - +20% regen
  - Cooldown: 30s

---

## 💡 Future Ideas

- ⚡ Sprinting, dashing, jumping
- 🥊 Stun/CC, Parry
- 💀 Grievous Wounds
- 🐾 Minions, Pets
- 🎒 Items, Levels
- 🏰 Towers and tower dives
- 👥 5v5, varied team compositions
- 🐲 PVE: Neutral bosses, buffs
