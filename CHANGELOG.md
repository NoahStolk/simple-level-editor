# Changelog

## SimpleLevelEditor

### 0.13.0

- Now supports more image formats in addition to Tga:
  - Bmp
  - Gif
  - Jpeg
  - Pbm
  - Png
  - Tiff
  - WebP
- Improved grid and line rendering.
- Small UI improvements.

### 0.12.4

- Improved messages window.
- Rewrote raycasting.

### 0.12.3

- Fixed render filter bugs.

### 0.12.2

- Internal changes only.

### 0.12.1

- Fixed JSON serialization not being compatible with optimized release builds due to assembly trimming.

### 0.12.0

- Added support for models with multiple meshes and materials (although only the material's diffuse texture is used in the renderer for now).
- Assigning arbitrary textures to meshes (now models) has been removed in favor of rendering models.
- The formats have been entirely rewritten. Everything is now stored in JSON format.
- Entities can now be rendered as points, boxes, spheres, sprites, and models.
- Various improvements have been made to the renderer and editor.

## SimpleLevelEditor.Formats

This library uses [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

### 0.2.2

#### Fixed

- Fixed not including internal dependency.

### 0.2.1

#### Changed

- The `SimpleLevelEditorJsonSerializer` class is now marked with `RequiresUnreferencedCodeAttribute`.

### 0.2.0

#### Changed

- The formats have been changed to JSON and have undergone many breaking changes.
- Use the following methods to read and write level and entity config files:
  - `SimpleLevelEditorJsonSerializer.DeserializeEntityConfig`
  - `SimpleLevelEditorJsonSerializer.DeserializeLevel`
  - `SimpleLevelEditorJsonSerializer.SerializeEntityConfig`
  - `SimpleLevelEditorJsonSerializer.SerializeLevel`
