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

## Betting System

The betting system supports all standard American roulette bet types with automatic bet location generation.

**Bet Types Supported:**
- **Inside Bets**: Straight (35:1), Split (17:1), Street (11:1), Corner (8:1), Six Line (5:1)
- **Outside Bets**: Red/Black, Even/Odd, High/Low (1:1), Dozens (2:1), Columns (2:1), Basket (6:1)(American-specific)

**Architecture:**
- `BetLocation`: Clickable areas on the table with hover effects and collision detection
- `BetNumber`: Represents individual numbers (including "00" as -1)
- `Bet`: Data structure storing bet amount, covered numbers, and payout ratio
- `BettingManager`: Singleton managing chip values, active bets, and payout calculations
- `BetLocationManager`: Handles raycasting and mouse input using the new Input System

**Automatic Generation:**
The `BetTableGenerator` editor tool automatically creates bet locations for inside bets:
1. Add BetTableGenerator component to an empty GameObject
2. Configure table dimensions (cell width/height) and start position
3. Assign materials for visual distinction between bet types
4. Click "Generate Bet Locations" in the inspector
5. All straight, split, corner, street, and six-line bet locations are created automatically

Important notes: 
- Outside bets (red/black, dozens, columns) are placed manually as there are fewer of them.
- I made this tool to save some time as it places 143 inside bet locations automatically. Some may need manual adjustment if the bet table is not symmetrical (like the one in this project).
- Careful, you might end up deleting your BetLocations permanently, as this is an editor tool. Create locations and remove the GameObject with the script from the scene. 

## Assets & Attribution

**Roulette Table Model**: ["Simple Roulette Table"](https://sketchfab.com/3d-models/simple-roulette-table-a128e643ea6149e686fe6b73ceb48ef9) by **Designed By Jonathan** is licensed under [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)

## Author

Berkan Özgür - 2026
