# Simple Level Editor

## Data Types

Data types are individual parts that can be assigned to an entity descriptor. They are the building blocks of an entity descriptor.

An entity descriptor can have zero or more data types, but a data type can only be assigned once to an entity descriptor.

Data types are made up of a name and one or more fields, like a struct.

### Default Data Types

Default data types are used for rendering entities in the level editor.

- `diffuse_color`
  - `r: u8`
  - `g: u8`
  - `b: u8`
  - `a: u8`
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
- `model`
  - `model_path: str`
- `billboard`
  - `texture_path: str`
- `wireframe`
  - `thickness: f32`
  - `shape: str`

None of the default data types are required.

- Position is the world position in the level.
- Rotation is the Euler rotation in degrees.
- Scale is the scaling factor.
- Diffuse color is the color of the entity.
- Visualizer is the type of visualization for the entity. This uses all the above data types to render the entity in the level editor.
  - Default cases:
    - If no position is provided, the entity will be placed at the origin.
    - If no rotation is provided, the entity will have no rotation.
    - If no scale is provided, the entity will have a scale of 1.
    - If no diffuse color is provided, the entity will have a white color.
  - Visualizer types:
    - Model: A 3D model.
    - Billboard: A 2D sprite. (Note: This will probably need to ignore the rotation data type.)
    - Wireframe: A wireframe shape.

Entities that do not need to be rendered and only need to show up in the entity list can simply omit all the default data types. They can use custom data types. This is useful for controller entities.

### Custom Data Types

Custom data types can be created by the user.

#### Examples of Custom Data Types

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
  - `shape: str`
- `spawn_entity`
  - `name: str`
- `spawn_interval`
  - `value: f32`
- `audio_path`
  - `path: str`

## Entity Descriptor

Describes an entity by combining multiple components. A component contains one data type, and can either be "fixed" or "varying".

Fixed components are components that are always the same for all entities of the same type. Varying components are components that can be different for each entity of the same type.

- `entity_descriptor`
  - `name: str`
  - `fixed_components: list[fixed_component]`
  - `varying_components: list[varying_component]`

### Fixed Components

Fixed components are components that are always the same for all entities of the same type.

- `fixed_component`
  - `data_type: data_type`
  - `value: str`

### Varying Components

Varying components are components that can be different for each entity of the same type.

- `varying_component`
  - `data_type: data_type`
  - `default_value: str`
  - `slider_config: slider_config / null`
    - `step: f32`
    - `min_value: f32`
    - `max_value: f32`

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
    - `model`
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
    - `billboard`
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
    - `wireframe`
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
    - `wireframe`
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
  - `data_types: list[data_type]`
  - `entity_descriptors: list[entity_descriptor]`
