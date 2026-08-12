# 🤖 Mega Man (1987) — Remake

A love letter to the NES classic, rebuilt from scratch in **Unity + C#**.

Run, jump, climb, shoot, and take down **Cut Man** in his own room — just like 1987, minus the blowing-into-the-cartridge part.

This was my final project for **Introduction to Unity**, the first course of the Game Design & Development minor at Bezalel. 🎓

![Gameplay](docs/gameplay.gif)

<!--
🎥 WANT THE FULL DEMO VIDEO HERE?
Drag `megaman-demo-compressed.mp4` (in your Downloads) into the GitHub README editor
in your browser. GitHub uploads it and pastes a link — put that link right below.
-->

---

## 🎮 Controls

| Key | Action |
|-----|--------|
| ⬅️ ➡️ | Move left / right |
| ⬆️ ⬇️ | Climb ladders |
| `Z` | Jump |
| `X` | Shoot |

> Yes, `Z` and `X`. If you grew up with an NES emulator, your hands already know this.

## 🕹️ Cheat Code

Stuck on the stage and just want the boss? I got you.

| Key | What it does |
|-----|--------------|
| `Space` | Teleport straight to the Cut Man boss room |

## 🔥 What's in it

- **The full Cut Man stage** — ladders, spikes, bottomless pits, and enemies that will absolutely clip you mid-jump
- **Cut Man himself** — hops around the boss room and throws his **Rolling Cutter** at you
- **Blaster enemies** that pop out, fire, and duck back into cover
- **Power-up drops** — grab extra points, or a slow-motion pickup when things get spicy
- **Ladder climbing** with proper NES-style mount/dismount
- **Screen-by-screen room transitions**, like the original — the camera locks and slides you into the next room
- **Health bar, score, start screen, and an end screen** so it actually feels like a game and not a tech demo
- Full **sound effects and music**

## 🛠️ Built With

**Unity** · **C#** · New Input System · Cinemachine · DOTween

Under the hood it's a bit tidier than it needs to be — the player movement runs on a **state machine** (grounded / in-air / climbing), bullets and audio sources come out of **object pools** instead of being spawned and destroyed, and systems talk to each other through a central **event hub** rather than poking each other directly.

<details>
<summary>📁 Project structure (for the curious)</summary>

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

*Mega Man*, Cut Man, and all related characters, sprites, and music are the property of **Capcom**. All original assets belong to them — I'm just a fan who wanted to understand how the game worked by rebuilding it.

## 🙋 Credits

Made solo by **Rami Hubeishi** — [@FlubbedTulip](https://github.com/FlubbedTulip)

More of my games over on [itch.io](https://flubbedtulip.itch.io/) 🎮

---

Thanks for stopping by — now go beat Cut Man. 🔪
