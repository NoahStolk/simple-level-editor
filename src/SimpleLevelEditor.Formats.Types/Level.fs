module SimpleLevelEditor.Formats.Types.Level

open System.Numerics

type Shape =
    | Point of Vector3
    | Sphere of Vector3 * float32
    | Aabb of Vector3 * Vector3

    member this.DeepCopy() =
        match this with
        | Point p -> Point p
        | Sphere (c, r) -> Sphere (c, r)
        | Aabb (min, max) -> Aabb (min, max)

type ShapeDescriptor =
    | Point
    | Sphere of Radius: float32
    | Aabb of Min: Vector3 * Max: Vector3 // This should be: float * float * float

    member this.DeepCopy() =
        match this with
        | Point -> Point
        | Sphere r -> Sphere r
        | Aabb (min, max) -> Aabb (min, max)

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
