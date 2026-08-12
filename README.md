# 🤖 Mega Man (1987) - Remake

A love letter to the NES classic, rebuilt from scratch in **Unity + C#**.

Run, jump, climb, shoot, and take down **Cut Man**.

This was my final project for **Introduction to Unity**, the first course of the Game Design & Development minor at Bezalel. 

![Gameplay](docs/gameplay.gif)

---

## Download Game Here
https://drive.google.com/file/d/1w-PofjojQYsQDQxfo_Y6fIxplysprtdB/view


## 🎮 Controls

| Key | Action |
|-----|--------|
| ⬅️ ➡️ | Move left / right |
| ⬆️ ⬇️ | Climb ladders |
| `Z` | Jump |
| `X` | Shoot |


## 🕹️ Cheat Code

Stuck on the stage and just want the boss?

| Key | What it does |
|-----|--------------|
| `Space` | Teleport straight to the Cut Man boss room |

## 🔥 What's in it

- **The full Cut Man stage** - ladders, spikes, bottomless pits, and enemies.
- **Cut Man himself** - A challenging boss fight with Cut Man that hops around the boss room and throws his **Rolling Cutter** at you.
- **Blaster enemies** that pop out, fire, and duck back into cover.
- **Power-up drops** - grab extra points, or a slow-motion pickup - an exclusive feature that I added into the game.
- Full **sound effects and music**



## Gameplay Video
https://github.com/user-attachments/assets/68f6969f-0597-422a-aaaa-f4db3117ada4


## 🛠️ Built With

**Unity** · **C#** · New Input System · Cinemachine · DOTween

The player movement runs on a **state machine** (grounded / in-air / climbing), bullets and audio sources come out of **object pools** instead of being spawned and destroyed, and systems talk to each other through a central **event hub** rather than poking each other directly.

<details>
<summary>📁 Project structure</summary>

```
Assets/Scripts/
├── Mega man/        # player controller, shooting, animation + movement states
├── Bosses/Cut Man/  # Cut Man AI and animation
├── Enemies/Blaster/ # enemy controller + spawner
├── Projectiles/     # Mega Man buster, blaster bullets, Rolling Cutter
├── Pools/           # generic pool + bullet & audio pools
├── Managers/        # game, health, sound, drops
├── Camera/          # room transitions, boss room approach
├── Drops/           # extra points, slow-motion power-up
├── Events/          # central event hub
├── Interfaces/      # IMovementState, IMovementContext, IPoolable
└── UI/, StartScreen/, EndScreen/, Ladder/, Environment/
```

</details>

## 📝 Disclaimer

This is a **non-commercial fan project**, built purely to learn Unity as a student exercise.

*Mega Man*, Cut Man, and all related characters, sprites, and music are the property of **Capcom**. All original assets belong to them - this project is purely educational.

## 🙋 Credits

Made solo by **Rami Hubeishi** - [@FlubbedTulip](https://github.com/FlubbedTulip).

Sprites and tilesets were taken from [The Spriters Resource](https://www.spriters-resource.com/), a public archive of ripped game assets.

More of my games over on [itch.io](https://flubbedtulip.itch.io/) 🎮

---
