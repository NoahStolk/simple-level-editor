# Simple Level Editor (ALPHA)

Simple cross-platform 3D level editor written in C# (.NET 7) using OpenGL and ImGui.

## Features

Build levels using .obj meshes and .tga textures. More formats will be supported later.

Levels are saved as XML for readability and ease of use. More output formats will be supported later.

Currently there is only a Windows build, but it should be easy to create builds for Mac and Linux.

## Controls

| Input        | Context             | Action           |
|--------------|---------------------|------------------|
| Scroll wheel | 3D view             | Zoom in/out      |
| RMB          | 3D view             | Look around      |
| MMB          | 3D view             | Move camera      |
| LMB          | 3D view / Add mode  | Add object       |
| LMB          | 3D view / Edit mode | Select object    |
| Hold R       | Edit mode           | Open rotate menu |
| Hold G       | Edit mode           | Open scale menu  |

## Shortcuts

| Key          | Action          |
|--------------|-----------------|
| CTRL N       | New level       |
| CTRL O       | Open level      |
| CTRL S       | Save level      |
| CTRL SHIFT S | Save level as   |
| F1           | Go to add mode  |
| F2           | Go to edit mode |

## Dependencies

### Bindings
- [Silk.NET](https://github.com/dotnet/Silk.NET) ([OpenGL](https://www.opengl.org), [GLFW](https://github.com/glfw/glfw))
- [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET) ([Dear ImGui](https://github.com/ocornut/imgui))
- [NativeFileDialogSharp](https://github.com/milleniumbug/NativeFileDialogSharp) ([nativefiledialog](https://github.com/mlabbe/nativefiledialog))

### Other dependencies
- [OneOf](https://github.com/mcintyre321/OneOf)
