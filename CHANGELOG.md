# Changelog

## 0.12.0

- Added support for models with multiple meshes and materials (although only the material's diffuse texture is used in the renderer for now).
- Assigning arbitrary textures to meshes (now models) has been removed in favor of rendering models.
- The formats have been entirely rewritten. Everything is now stored in JSON format.
- Entities can now be rendered as points, boxes, spheres, sprites, and models.
- Various improvements have been made to the renderer and editor.

### SimpleLevelEditor.Formats 0.2.0

This update includes a release for the `SimpleLevelEditor.Formats` library as well.
The `SimpleLevelEditor.Formats` library uses [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

#### Changed

- The formats have been changed to JSON and have undergone many breaking changes.
- Use the following methods to read and write level and entity config files:
  - `SimpleLevelEditorJsonSerializer.DeserializeEntityConfig`
  - `SimpleLevelEditorJsonSerializer.DeserializeLevel`
  - `SimpleLevelEditorJsonSerializer.SerializeEntityConfig`
  - `SimpleLevelEditorJsonSerializer.SerializeLevel`
