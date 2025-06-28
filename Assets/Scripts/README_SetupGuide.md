# ToppleRun MVP - Script Setup Guide

## Overview
This guide explains how to set up and use the ToppleRun MVP scripts in Unity. The system consists of four main scripts that work together to create the core gameplay.

## Scripts Overview

### 1. PlayerController.cs
Handles player movement, jumping, and ground detection with support for both keyboard and mobile touch controls.

### 2. BlockSpawner.cs
Manages procedural block generation with different sizes, physics properties, and spawning patterns.

### 3. BlockCleanup.cs
Automatically cleans up blocks that fall too far below the camera or have existed too long to maintain performance.

### 4. GameManager.cs (Optional)
Coordinates all systems and provides camera following, performance management, and game state control.

## Quick Setup Guide

### Step 1: Player Setup
1. Create a GameObject named "Player"
2. Add the `PlayerController` script
3. Add `Rigidbody2D` and `BoxCollider2D` components (or let PlayerController auto-require them)
4. Set up a visual representation (SpriteRenderer with a sprite)
5. Position the player at your desired starting location

**PlayerController Configuration:**
- Move Speed: 8 (adjust for game feel)
- Jump Force: 12 (adjust for desired jump height)
- Ground Check: Will auto-create if not assigned
- Ground Layer Mask: Set to include ground and block layers

### Step 2: Block Prefab Setup
Follow the detailed instructions in `README_BlockPrefabSetup.md` to create a Block prefab with:
- Visual representation (SpriteRenderer)
- Physics components (Rigidbody2D, BoxCollider2D)
- Proper layer assignment
- Physics materials for realistic interactions

### Step 3: Block Spawner Setup
1. Create an empty GameObject named "BlockSpawner"
2. Add the `BlockSpawner` script
3. Assign your Block prefab to the "Block Prefab" field
4. Configure spawn settings:
   - Spawn Interval: 2 seconds (adjust for difficulty)
   - Spawn Height: 10 units above ground
   - Spawn Range X: Adjust based on your game area width

**Block Size Variations** (pre-configured):
- Square: 1x1, Mass 1
- Wide: 2x1, Mass 1.5
- Tall: 1x2, Mass 1.5
- Extra Wide: 3x1, Mass 2

### Step 4: Ground Setup
1. Create ground GameObjects with `Collider2D` components
2. Assign them to a "Ground" layer
3. Ensure the layer collision matrix allows:
   - Player ↔ Ground
   - Blocks ↔ Ground
   - Player ↔ Blocks
   - Blocks ↔ Blocks

### Step 5: Camera Setup
1. Position the main camera to frame the player
2. Optionally use the GameManager for automatic camera following

### Step 6: Game Manager Setup (Recommended)
1. Create an empty GameObject named "GameManager"
2. Add the `GameManager` script
3. Assign PlayerController and BlockSpawner references
4. Configure camera follow settings if desired

## Layer Configuration

### Recommended Layer Setup:
- Layer 8: "Player"
- Layer 9: "Blocks"  
- Layer 10: "Ground"

### Physics2D Layer Collision Matrix:
```
         Player  Blocks  Ground
Player     ✗       ✓       ✓
Blocks     ✓       ✓       ✓
Ground     ✓       ✓       ✗
```

## Input Controls

### Keyboard:
- A/D or Arrow Keys: Move left/right
- Spacebar: Jump

### Mobile Touch:
- Touch left side of screen: Move left
- Touch right side of screen: Move right  
- Touch upper half of screen: Jump

## Performance Tips

1. **Block Cleanup**: BlockCleanup components are automatically added to spawned blocks
2. **Performance Monitoring**: Use GameManager to monitor active block count
3. **Emergency Cleanup**: Use `BlockCleanup.CleanupAllBlocks()` if performance issues occur
4. **Spawn Rate**: Adjust spawn interval based on device performance

## Debugging Features

### Visual Gizmos:
- **PlayerController**: Green/red circle shows ground detection
- **BlockSpawner**: Yellow box shows spawn area
- **BlockCleanup**: Red line shows cleanup threshold (when selected)

### Debug Options:
- Enable "Show Debug Info" in BlockCleanup for console logging
- Use Context Menu options in GameManager for testing
- Monitor block count with `BlockCleanup.GetActiveBlockCount()`

## Common Issues & Solutions

### Player Not Jumping:
- Check Ground Layer Mask in PlayerController
- Ensure ground has proper Collider2D
- Verify layer collision matrix

### Blocks Not Spawning:
- Ensure Block Prefab is assigned in BlockSpawner
- Check that Block Prefab has Rigidbody2D and Collider2D
- Verify spawn height is above ground level

### Performance Issues:
- Reduce spawn interval
- Enable more aggressive cleanup settings
- Check active block count regularly

### Touch Controls Not Working:
- Ensure you're testing on a device with touch support
- Check touch area divisions (screen halves)
- Verify Input system is configured properly

## Extending the System

### Adding New Block Types:
1. Modify the `blockSizes` array in BlockSpawner
2. Add new BlockSize entries with custom dimensions and mass
3. Blocks will automatically use the new variations

### Custom Cleanup Logic:
1. Extend BlockCleanup with additional cleanup conditions
2. Override cleanup methods for special effects
3. Add custom triggers for cleanup events

### Enhanced Player Abilities:
1. Extend PlayerController with new movement abilities
2. Add power-ups or special states
3. Implement combo systems or scoring

### Advanced Spawning:
1. Create pattern-based spawning in BlockSpawner
2. Add difficulty scaling over time
3. Implement themed block sets or materials

## Testing Checklist

- [ ] Player moves left/right smoothly
- [ ] Player jumps and lands properly  
- [ ] Ground detection works reliably
- [ ] Blocks spawn at regular intervals
- [ ] Blocks have physics interactions
- [ ] Touch controls work on mobile
- [ ] Blocks clean up automatically
- [ ] Performance remains stable
- [ ] Visual gizmos appear in Scene view
- [ ] No console errors or warnings

## Support

For issues or questions:
1. Check console for error messages and warnings
2. Verify component assignments in Inspector
3. Review layer and collision matrix settings
4. Test with debug gizmos enabled
5. Monitor performance with profiler if needed