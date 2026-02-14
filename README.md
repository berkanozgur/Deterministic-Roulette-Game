# Deterministic Roulette

Unity game prototype for controlled roulette gameplay with deterministic outcome selection.
[(Gameplay Sample Video)](https://youtu.be/UEiviCNBfZ4)

## Initial Setup

- **Unity Version**: 6000.2.12f1
- **Roulette Type**: American (38 pockets including 0 and 00)

## Features

- Wheel spinning animation with smooth deceleration
- Deterministic outcome system (select next result)
- Ball simulation with opposite-direction spinning
- Full betting system with inside and outside bets
- Automatic bet location generation
- Chip management and bank persistence
- Bet history tracking with win/loss statistics
- Audio and visual feedback for outcomes
- Result marker animation

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

<img width="1128" height="1126" alt="image" src="https://github.com/user-attachments/assets/e9ad5efe-b8f8-4d16-90db-96119328c5e6" />

## Feedback & Effects

**Visual Effects:**
- Win particle effect
- Tiered win sounds based on profit/even/loss wins
- Animated result marker showing winning number
- Chip stacking visualization on bet locations

**Audio:**
- Ball rolling sound with dynamic pitch
- Pocket landing sound when ball settles
- Chip placement sound
- Tiered celebration sounds for different win amounts

## Architecture

**Core Scripts:**
- `RouletteWheel.cs` - Wheel spinning mechanics
- `RouletteBall.cs` - Ball animation with physics simulation
- `RouletteController.cs` - Game flow and UI management
- `BettingManager.cs` - Bet placement, tracking, and payout processing
- `BetLocationManager.cs` - Input handling for bet placement
- `BetHistory.cs` - Persistent bet tracking and statistics
- `WinLoseEffect.cs` - Visual and audio feedback for results

**Editor Tools:**
- `BetTableGenerator.cs` - Automated bet location generation
- `BetTableGeneratorEditor.cs` - Custom inspector for generation tool

## Assets & Attribution

### 3D Models
- **Roulette Table Model**: ["Simple Roulette Table"](https://sketchfab.com/3d-models/simple-roulette-table-a128e643ea6149e686fe6b73ceb48ef9) from Sketchfab
- **Dolly Model**: ["Pawn"](https://sketchfab.com/3d-models/pawn-432f996dec844209bb2c8497404c8eef) from Sketchfab

### Audio
- **Chip Handling Sound**: ["Short handling of poker chips 3"](https://www.zapsplat.com/music/short-handling-of-poker-chips-3/) from Zapsplat
- **Win Sound (Tier 2)**: ["Game sound orchestra complete or finish"](https://www.zapsplat.com/music/game-sound-orchestra-complete-or-finish/) from Zapsplat
- **Win Sound (Tier 3)**: ["Game sound orchestra win or complete"](https://www.zapsplat.com/music/game-sound-orchestra-win-or-complete/) from Zapsplat
- **Ball Rolling Sound**: ["Marble rolling"](https://freesound.org/people/YleArkisto/sounds/280200/) by YleArkisto from Freesound.org

### UI & VFX
- **UI Sprites**: ["Sleek Essential UI Pack"](https://assetstore.unity.com/packages/2d/gui/icons/sleek-essential-ui-pack-170650) from Unity Asset Store
- **Confetti Visuals**: ["Hyper Casual FX"](https://assetstore.unity.com/packages/vfx/particles/hyper-casual-fx-200333) from Unity Asset Store

All assets are used in accordance with their respective licenses.

## Author

Berkan Özgür - 2026
