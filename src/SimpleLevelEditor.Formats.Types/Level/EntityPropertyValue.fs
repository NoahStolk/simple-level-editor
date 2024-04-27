namespace SimpleLevelEditor.Formats.Types.Level

open SimpleLevelEditor.Formats.Types
open System.Numerics

type EntityPropertyValue =
    | Bool of Value: bool
    | Int of Value: int
    | Float of Value: float32
    | Vector2 of Value: Vector2
    | Vector3 of Value: Vector3
    | Vector4 of Value: Vector4
    | String of Value: string
    | Rgb of Value: Color.Rgb
    | Rgba of Value: Color.Rgba
