# Simple Level Editor

Simple cross-platform 3D level editor written in C# and F# using .NET 8, OpenGL and ImGui.

> Try the latest alpha version [here](https://github.com/NoahStolk/simple-level-editor/releases).

The GitHub releases only list Windows builds for now, but a Linux build can easily be made. You only need the .NET SDK (version 8.0) to compile the project.

There is also a NuGet package for reading level and entity config files: [![NuGet Version](https://img.shields.io/nuget/v/NoahStolk.SimpleLevelEditor.Formats.svg)](https://www.nuget.org/packages/NoahStolk.SimpleLevelEditor.Formats/)

![](images/simple-level-editor.png)

## Features

### Editor

- Easily build level geometry using 3D models
- Full undo/redo support
- Move, rotate, and scale world objects
- Supports rendering Wavefront OBJ models with multiple meshes and materials
- Supports rendering the following texture formats:
  - Bmp
  - Gif
  - Jpeg
  - Pbm
  - Png
  - Tiff
  - Tga
  - WebP

### UI

- Docking support
- Customizable layout
- UI is saved between sessions

### Entity configuration

- Define your own entities using a JSON file
- Entities can be of different shapes with configurable sizes
  - Points
  - Boxes
  - Spheres
  - Sprites
  - Models
- Entities can have custom properties

### Integration

- Save and load levels as JSON files
- Deserialize levels using the NuGet package

## Dependencies

- [Dear ImGui](https://github.com/ocornut/imgui) and [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET)
- [Silk.NET](https://github.com/dotnet/Silk.NET)
- [ImageSharp](https://github.com/SixLabors/ImageSharp)
- [Serilog.Sinks.File](https://github.com/serilog/serilog-sinks-file)
- [Native File Dialog](https://github.com/mlabbe/nativefiledialog) and [Native File Dialog Sharp](https://github.com/milleniumbug/NativeFileDialogSharp)
