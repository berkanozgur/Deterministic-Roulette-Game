# Deterministic Roulette

Unity game prototype for controlled roulette gameplay with deterministic outcome selection.

## Initial Setup

- **Unity Version**: 6000.2.12f1
- **Roulette Type**: American (38 pockets including 0 and 00)

## Current Progress

- Wheel spinning animation with smooth deceleration
- Deterministic outcome system (select next result)
- American roulette wheel layout (0, 00, 1-36)
- Accumulated rotation tracking across spins

## Roulette Wheel Mechanics

The wheel uses a deterministic system where the outcome is pre-calculated, then the wheel and ball animate to that result.

**Key Components:**
- **Wheel Rotation**: Spins counter-clockwise to position the target number at 12 o'clock
- **Ball Animation**: Spins clockwise (opposite direction) around the rim, then settles at 12 o'clock
- **Container Rotation**: Parent container adds random rotation (eg. 720-1080°) so results appear at different visual positions each spin
- **Smooth Settling**: Extra easing phase after main spin prevents mechanical stops

![wheelmechanics](https://github.com/user-attachments/assets/d2f3d585-3cff-4b1a-9fbc-ef8cff592ec3)


## Assets & Attribution

**Roulette Table Model**: ["Simple Roulette Table"](https://sketchfab.com/3d-models/simple-roulette-table-a128e643ea6149e686fe6b73ceb48ef9) by **Designed By Jonathan** is licensed under [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)

## Author

Berkan Özgür - 2026
