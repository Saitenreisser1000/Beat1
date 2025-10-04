# Beat1 Unity 2D Platformer - AI Coding Instructions

## Project Overview
Beat1 is a Unity 2D platformer featuring dynamic music-synchronized theming. The game uses a skin system that simultaneously changes player appearance, background visuals, tile graphics, and background music across different musical genres (Beat, Punk, Tracht, Klassik, Reggae, Metal).

## Architecture & Core Systems

### Skin System (Central Pattern)
The game's core architecture revolves around synchronized theming:
- **TileSkinManager**: Manages tilemap graphics switching using Dictionary<string, List<TileBase>>
- **ParallaxSkinManager**: Handles background sprite switching with genre-specific sprites
- **MusicManager**: Synchronizes audio clips with skin changes, includes scratch transition effects
- **PlayerMovement**: Uses AnimatorOverrideController for character skin animations

**Key Integration**: All managers work together - skin changes trigger simultaneous updates across tiles, backgrounds, music, and animations.

### Scene Structure
- **17 Levels**: Scenes named V1.unity through V17.unity in Assets/ root
- **Modular Design**: Each scene likely represents a different level/stage
- **Consistent Naming**: Follow V{number}.unity pattern for new scenes

### Component Architecture

#### Player System
- **PlayerMovement.cs**: Comprehensive movement with crawling, ducking, jumping mechanics
- **Input Handling**: Uses both legacy Input class and new Input System
- **Animation States**: "run", "ducking", "crawling", "facingright" with normalized speed
- **Physics**: Rigidbody2D with pixel-perfect camera following

#### Enemy System
- **EnemyPatrol.cs**: Patrol AI with chase behavior and obstacle detection
- **Dual Movement Modes**: Point-to-point patrol OR raycast-based obstacle avoidance
- **Dynamic Behavior**: Switches between patrol/chase speeds based on player proximity
- **Collision Response**: Stops movement when touching player

#### Camera & Visual Effects
- **PixelPerfectFollow.cs**: Pixel-grid aligned camera with world bounds clamping
- **ParallaxLayer.cs**: Multi-layer background parallax with configurable factors
- **TileScaler.cs**: Applies scaling matrices to tilemap tiles (1.01f default)

## Unity-Specific Conventions

### Serialization Patterns
```csharp
[Header("Category Name")]
[SerializeField] private float privateField;
public PublicType publicField;
```

### Component References
- Use `GetComponent<>()` in Start() for internal references
- Public references for cross-object dependencies assigned in Inspector
- Null checks before component usage

### Physics & Movement
- Use `Rigidbody2D.linearVelocity` (Unity 6000.1.5f1)
- Time.deltaTime for frame-rate independent movement
- LayerMask for collision filtering ("Ground" layer standard)

## Package Dependencies
Key Unity packages used:
- **2D Animation (10.2.0)**: Character animation system
- **Input System (1.14.0)**: Modern input handling
- **URP (17.1.0)**: Universal Render Pipeline
- **2D Tilemap Extras (4.3.0)**: Extended tilemap functionality

## Development Patterns

### Naming Conventions
- German comments in code (keep this pattern)
- Component names use PascalCase
- Private fields use camelCase with [SerializeField]
- Scene files: V{number}.unity format

### Manager Pattern
Managers are MonoBehaviours with:
- Public methods for external triggering
- Dictionary-based data organization
- Cross-component communication via public references

### Asset Organization
```
Assets/
├── Scripts/           # All C# scripts
├── Sprites/          # Visual assets
├── Audio/            # Music and sound effects  
├── Animations/       # Animation clips
├── Materials/        # Render materials
├── Tiles/            # Tilemap assets
└── V1-V17.unity     # Scene files in root
```

## Key Files for Understanding
- `PlayerMovement.cs`: Core gameplay mechanics and input patterns
- `TileSkinManager.cs`: Multi-system synchronization example
- `EnemyPatrol.cs`: AI behavior patterns with state switching
- `Assets/InputSystem_Actions.inputactions`: Input mapping configuration

## Working with This Codebase
- Always test skin changes across all managers (tiles, parallax, music, player)
- New enemy types should follow EnemyPatrol pattern with configurable behaviors
- Camera modifications require consideration of pixel-perfect constraints
- Scene additions follow V{next_number}.unity naming convention