# Simple Level Editor (ALPHA)

Simple cross-platform 3D level editor written in C# (.NET 8) using OpenGL and ImGui.

## Features

Build levels using .obj meshes and .tga textures. More formats will be supported later.

Levels are saved as XML for readability and ease of use. More output formats will be supported later.

Currently there is only a Windows build, but it should be easy to create builds for Mac and Linux.

## Dependencies

### Bindings
- [Silk.NET](https://github.com/dotnet/Silk.NET) ([OpenGL](https://www.opengl.org), [GLFW](https://github.com/glfw/glfw))
- [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET) ([Dear ImGui](https://github.com/ocornut/imgui))
- [NativeFileDialogSharp](https://github.com/milleniumbug/NativeFileDialogSharp) ([nativefiledialog](https://github.com/mlabbe/nativefiledialog))

### Other dependencies
- [Detach](https://github.com/NoahStolk/Detach)
- [OneOf](https://github.com/mcintyre321/OneOf)
