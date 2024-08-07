# Simple Level Editor

## Data Type

### Default Data Types

- `i8`
- `i16`
- `i32`
- `i64`
- `u8`
- `u16`
- `u32`
- `u64`
- `f16`
- `f32`
- `f64`
- `str`
- `vec2`
  - `x: f32`
  - `y: f32`
- `vec3`
  - `x: f32`
  - `y: f32`
  - `z: f32`
- `vec4`
  - `x: f32`
  - `y: f32`
  - `z: f32`
  - `w: f32`
- `rgb`
  - `r: u8`
  - `g: u8`
  - `b: u8`
- `rgba`
  - `r: u8`
  - `g: u8`
  - `b: u8`
  - `a: u8`
- `model`
  - `model_path: str`
- `billboard`
  - `texture_path: str`
- `wireframe`
  - `thickness: f32`
  - `shape: union(cube / sphere / cylinder)`

### Custom Data Types

Custom data types can be created by the user.

#### Examples of Custom Data Types

- `cube`
  - `min: vec3`
  - `max: vec3`
- `sphere`
  - `radius: f32`

## Component

TODO: Is there really a difference between a component and a data type?

Components are individual parts that can be assigned to an entity descriptor. They are the building blocks of an entity descriptor.

An entity descriptor can have zero or more components, but a component type can only be assigned once to an entity descriptor.

Components are made up of a name and multiple data types, exactly like a struct. Component fields may have a union type.

### Default Components

Default components are used for rendering entities in the level editor.

- `position`
  - `x: f32`
  - `y: f32`
  - `z: f32`
- `rotation`
  - `x: f32`
  - `y: f32`
  - `z: f32`
- `scale`
  - `x: f32`
  - `y: f32`
  - `z: f32`
- `diffuse_color`
  - `r: u8`
  - `g: u8`
  - `b: u8`
  - `a: u8`
- `visualizer`
  - `type: union(model / billboard / wireframe)`

None of the default components are required.

- Position is the world position in the level.
- Rotation is the Euler rotation in degrees.
- Scale is the scaling factor.
- Diffuse color is the color of the entity.
- Visualizer is the type of visualization for the entity. This uses all the above components to render the entity in the level editor.
  - Default cases:
    - If no position is provided, the entity will be placed at the origin.
    - If no rotation is provided, the entity will have no rotation.
    - If no scale is provided, the entity will have a scale of 1.
    - If no diffuse color is provided, the entity will have a white color.
  - Visualizer types:
    - Model: A 3D model.
    - Billboard: A 2D sprite. (Note: This will probably need to ignore the rotation component.)
    - Wireframe: A wireframe shape.

Entities that do not need to be rendered and only need to show up in the entity list can simply omit all the default components. They can use custom components. This is useful for controller entities.

### Custom Components

Custom components can be created by the user. Custom data types can be used as the data type of a custom component.

#### Examples of Custom Components

- `health`
  - `amount: u16`
- `damage`
  - `amount: u8`
- `direction`
  - `x: f32`
  - `y: f32`
  - `z: f32`
- `speed`
  - `value: f32`
- `mass`
  - `value: f32`
- `hitbox`
  - `shape: cube / sphere`
- `spawn_entity`
  - `name: str`
- `spawn_interval`
  - `value: f32`
- `audio_path`
  - `path: str`

## Entity Descriptor

Describes an entity by combining multiple components.

Components can either be "fixed", or "varying". Fixed components are components that are always the same for all entities of the same type. Varying components are components that can be different for each entity of the same type.

- `entity_descriptor`
  - `name: str`
  - `fixed_components: list[fixed_component]`
  - `varying_components: list[varying_component]`

### Fixed Components

Fixed components are components that are always the same for all entities of the same type.

- `fixed_component`
  - `component_type: str`
  - `component: (component data)`

### Varying Components

Varying components are components that can be different for each entity of the same type.

- `varying_component`
  - `component_type: str`
  - `default_value: (component data)`
  - `step_value: (component data or none)`
  - `min_value: (component data or none)`
  - `max_value: (component data or none)`

The actual data for varying components will be stored in the entity itself (in the level), not in the entity descriptor.

### Examples of Entity Descriptors

- `player`
  - `fixed_components`
    - `scale`
      - `x: 1`
      - `y: 1`
      - `z: 1`
    - `diffuse_color`
      - `r: 0`
      - `g: 255`
      - `b: 90`
    - `visualizer`
      - `type: model`
        - `model_path: "player.obj"`
    - `health`
      - `amount: 100`
  - `varying_components`
    - `position`
- `light_source`
  - `fixed_components`
    - `scale`
      - `x: 0.5`
      - `y: 0.5`
      - `z: 0.5`
    - `diffuse_color`
      - `r: 255`
      - `g: 255`
      - `b: 255`
    - `visualizer`
      - `type: billboard`
        - `texture_path: "light.png"`
    - `radius`
      - `amount: 10`
  - `varying_components`
    - `position`
    - `direction`
- `spawner`
  - `fixed_components`
    - `diffuse_color`
      - `r: 255`
      - `g: 0`
      - `b: 0`
    - `scale`
      - `x: 0.5`
      - `y: 0.5`
      - `z: 0.5`
    - `visualizer`
      - `type: wireframe`
        - `thickness: 0.1`
        - `shape: sphere`
  - `varying_components`
    - `position`
    - `rotation`
    - `spawn_entity`
    - `spawn_interval`
- `area`
  - `fixed_components`
    - `diffuse_color`
      - `r: 0`
      - `g: 0`
      - `b: 255`
    - `visualizer`
      - `type: wireframe`
        - `thickness: 0.1`
        - `shape: cube`
  - `varying_components`
    - `position`
    - `scale`
- `world_object`
  - `fixed_components`
  - `varying_components`
    - `position`
    - `rotation`
    - `scale`
    - `diffuse_color`
    - `visualizer`
- `level_music`
  - `fixed_components`
  - `varying_components`
    - `audio_path`

## Game Config

Describes a game by combining multiple entity descriptors. This can be used to build levels with.

- `game_config`
  - `name: str`
  - `model_paths: list[str]`
  - `texture_paths: list[str]`
  - `entity_descriptors: list[entity_descriptor]`
