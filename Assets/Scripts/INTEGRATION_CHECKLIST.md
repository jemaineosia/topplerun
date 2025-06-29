# LavaRiser Integration Checklist

Use this checklist to ensure proper LavaRiser system integration:

## ✅ Basic Setup

- [ ] Created Scripts folder in Assets
- [ ] Added LavaRiser.cs to project
- [ ] Created lava GameObject in scene
- [ ] Added Collider2D to lava GameObject (set as trigger)
- [ ] Added LavaRiser script to lava GameObject
- [ ] Positioned lava at bottom of play area

## ✅ Player Configuration

- [ ] Player GameObject has Transform component
- [ ] Player GameObject has Collider2D component  
- [ ] Player GameObject has SpriteRenderer component (for visual effects)
- [ ] Player GameObject tagged as "Player" OR manually assigned in Inspector
- [ ] Player reference assigned to LavaRiser script

## ✅ Platform Setup

- [ ] Platform GameObjects have Collider2D components
- [ ] Platforms are on appropriate layer (Default layer or custom)
- [ ] LavaRiser Platform Layer Mask includes platform layers
- [ ] Tested platform detection with player standing on platforms

## ✅ Inspector Configuration

### Lava Rising Settings
- [ ] Initial Speed set (recommended: 1.0)
- [ ] Acceleration Factor set (recommended: 0.1)
- [ ] Max Speed set (recommended: 10.0)

### Burn State Settings  
- [ ] Burn Duration set (default: 3.0 seconds)
- [ ] Platform Layer Mask configured correctly

### Visual Feedback (Optional)
- [ ] Enable Burn Visuals checked (if desired)
- [ ] Burn Color set (default: red)
- [ ] Flash Speed set (default: 10.0)

## ✅ Game Manager Integration

- [ ] Created GameManager script OR using existing game manager
- [ ] Subscribed to LavaRiser.OnGameOver event
- [ ] Implemented game over handling (restart, UI, etc.)
- [ ] Tested game over functionality

## ✅ Testing

- [ ] Player can move around the scene
- [ ] Lava rises from bottom at initial speed
- [ ] Lava speed increases as player goes higher
- [ ] Player touching lava triggers burn state
- [ ] Visual feedback works during burn (if enabled)
- [ ] Standing on platform cancels burn state
- [ ] Staying in lava for full duration triggers game over
- [ ] Game over handling works correctly

## ✅ Quick Test (Optional)

Alternative to manual setup:
- [ ] Added LavaRiserTestSetup script to empty GameObject
- [ ] Entered Play mode to auto-generate test environment
- [ ] Tested all functionality with generated player/platforms

## 🎯 Integration Complete!

Once all items are checked, your LavaRiser system is ready to use in ToppleRun!

## Common Issues

**Player not detected:** Ensure player has "Player" tag or is manually assigned
**Burn not cancelling:** Check Platform Layer Mask includes your platform layers  
**No visual effects:** Verify player has SpriteRenderer and Enable Burn Visuals is checked
**Game not ending:** Make sure GameManager subscribes to LavaRiser.OnGameOver event