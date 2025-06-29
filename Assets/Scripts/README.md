# LavaRiser System Integration Guide

The LavaRiser system provides a rising lava mechanic for ToppleRun with the following features:

## Features

- **Rising Lava**: Lava GameObject rises from the bottom of the play area
- **Speed Acceleration**: Rising speed increases based on player's Y position (higher = faster)
- **Burn State**: 3-second countdown when player touches lava
- **Platform Safety**: Burn cancelled if player reaches a platform before countdown ends
- **Game Over**: Triggers game over if burn countdown completes
- **Visual Feedback**: Optional player flashing/color change during burn state
- **Configurable**: Inspector fields for all key parameters

## Quick Setup

### 1. Create Lava GameObject
1. Create a new GameObject in your scene (name it "Lava")
2. Add a Collider2D component (BoxCollider2D works well)
3. Set the collider as a trigger (`Is Trigger = true`)
4. Position it at the bottom of your play area
5. Add a SpriteRenderer or other visual representation for the lava

### 2. Add LavaRiser Script
1. Add the `LavaRiser.cs` script to your lava GameObject
2. Configure the settings in the Inspector:
   - **Initial Speed**: Base rising speed (e.g., 1.0)
   - **Acceleration Factor**: How much speed increases per unit of player height (e.g., 0.1)
   - **Max Speed**: Maximum lava speed (e.g., 10.0)
   - **Burn Duration**: Time before game over when touching lava (default: 3.0 seconds)
   - **Platform Layer Mask**: Layers that count as "safe platforms"

### 3. Assign Player Reference
- Drag your player GameObject to the **Player Transform** field
- The script will automatically find Player Collider2D and SpriteRenderer
- Alternatively, tag your player with "Player" tag and the script will find it automatically

### 4. Configure Platform Detection
- Set the **Platform Layer Mask** to include layers that should be considered "safe ground"
- Make sure your platform GameObjects are on the correct layer
- The system uses raycasting to detect when the player is standing on a platform

## Advanced Integration

### Game Manager Integration
Use the provided `GameManager.cs` as an example:

```csharp
void Start()
{
    // Subscribe to game over events
    LavaRiser.OnGameOver += HandleGameOver;
}

private void HandleGameOver()
{
    // Handle game over (restart, show UI, etc.)
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
```

### Runtime Player Assignment
```csharp
LavaRiser lavaRiser = FindObjectOfType<LavaRiser>();
lavaRiser.SetPlayer(playerTransform);
```

### Querying System State
```csharp
bool isBurning = lavaRiser.IsPlayerBurning();
float currentSpeed = lavaRiser.GetCurrentSpeed();
```

## Configuration Parameters

### Lava Rising Settings
- **Initial Speed**: Base speed of lava rising (units per second)
- **Acceleration Factor**: Multiplier for player height to increase speed
- **Max Speed**: Maximum speed cap to prevent excessive acceleration

### Burn State Settings
- **Burn Duration**: Seconds before game over when in lava (default: 3.0)
- **Platform Layer Mask**: Which layers count as safe platforms

### Visual Feedback
- **Enable Burn Visuals**: Toggle player visual feedback during burn
- **Burn Color**: Color to flash player during burn state
- **Flash Speed**: How fast the player flashes during burn

## Layer Setup Recommendations

1. **Player Layer**: Put your player on a dedicated layer
2. **Platform Layer**: Put platforms/ground on a specific layer
3. **Lava Layer**: Put lava on its own layer to avoid unwanted collisions

Example layer setup:
- Layer 8: "Player"
- Layer 9: "Platforms" 
- Layer 10: "Lava"

## Player Controller Requirements

Your player controller should have:
- **Transform**: For position tracking
- **Collider2D**: For collision detection with lava
- **SpriteRenderer**: For visual burn effects (optional)

The system is designed to work with any player controller - it only requires these basic components.

## Troubleshooting

### Player Not Detected
- Ensure player has "Player" tag OR assign manually in Inspector
- Check that player has a Collider2D component

### Burn Not Cancelling on Platform
- Verify Platform Layer Mask includes your platform layers
- Check that platforms have Collider2D components
- Ensure player's Collider2D bounds are set correctly

### Visual Effects Not Working
- Check that player has a SpriteRenderer component
- Ensure "Enable Burn Visuals" is checked
- Verify burn color is different from player's original color

### Performance Considerations
- The system uses a single raycast per frame when player is burning
- Visual effects use coroutines for efficient flashing
- No Update overhead when player is not burning

## Quick Test Setup

For immediate testing, use the provided `LavaRiserTestSetup.cs` script:

1. Create an empty GameObject in your scene
2. Add the `LavaRiserTestSetup` script to it
3. Enter Play mode - the script will automatically create:
   - A rising lava block with LavaRiser component
   - Several test platforms
   - A simple player with basic movement (arrow keys/WASD + spacebar to jump)
   - A following camera
   - A game manager for handling game over

This gives you a complete test environment to immediately see the LavaRiser system in action!

## Manual Scene Setup

For custom integration:

1. Create a lava GameObject at the bottom of your scene
2. Add BoxCollider2D (set as trigger) and SpriteRenderer to lava
3. Add LavaRiser script and configure settings
4. Create platform GameObjects with Collider2D components
5. Set platforms to "Platforms" layer and update LavaRiser's Platform Layer Mask
6. Add GameManager script to handle game over events
7. Test by letting player touch lava and then jump to platform

The system is now ready to use!