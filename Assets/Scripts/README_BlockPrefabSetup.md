# Block Prefab Setup Instructions

## Creating the Block Prefab for ToppleRun

To create a Block prefab that works with the BlockSpawner system:

### 1. Create Basic Block GameObject
1. Create a new GameObject in the scene (GameObject > Create Empty)
2. Name it "Block"
3. Reset transform to (0, 0, 0)

### 2. Add Visual Component
1. Add a child GameObject and name it "Visual"
2. Add a SpriteRenderer component to the Visual GameObject
3. Assign a square or rectangular sprite (create a simple colored square in your art program or use Unity's built-in sprites)
4. Set the sprite color as desired

### 3. Add Physics Components
1. Add Rigidbody2D component to the main Block GameObject:
   - Mass: 1
   - Linear Drag: 0.5
   - Angular Drag: 0.5
   - Gravity Scale: 1
   - Collision Detection: Continuous

2. Add BoxCollider2D component to the main Block GameObject:
   - Size: (1, 1) - this will be scaled by the BlockSpawner
   - Material: Assign a Physics2D Material with:
     - Friction: 0.4
     - Bounciness: 0.1

### 4. Add Layer Setup
1. Create a new Layer called "Blocks" in the Layer settings
2. Assign the Block GameObject to the "Blocks" layer
3. Make sure the Player is on a different layer (e.g., "Player")

### 5. Create Physics Material (Optional but Recommended)
1. Create a Physics Material 2D asset (Assets > Create > Physics Material 2D)
2. Name it "BlockMaterial"
3. Set Friction: 0.4, Bounciness: 0.1
4. Assign this material to the BoxCollider2D

### 6. Save as Prefab
1. Drag the configured Block GameObject from the hierarchy to the Project window
2. This creates a prefab that can be assigned to the BlockSpawner
3. Delete the original Block GameObject from the scene

### 7. Configure BlockSpawner
1. Add BlockSpawner script to an empty GameObject in the scene
2. Assign the Block prefab to the "Block Prefab" field
3. Configure spawn settings as needed
4. Assign the "BlockMaterial" if created

### Layer Configuration for Ground Detection
Make sure to set up collision layers properly:
- Player layer should collide with Ground and Blocks layers
- Blocks layer should collide with Ground, Player, and other Blocks
- Ground layer should collide with Player and Blocks

Use the Physics2D settings (Edit > Project Settings > Physics2D) to configure layer collision matrix.