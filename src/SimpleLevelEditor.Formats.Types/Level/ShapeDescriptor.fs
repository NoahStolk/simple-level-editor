namespace SimpleLevelEditor.Formats.Types.Level

open System.Numerics

type ShapeDescriptor =
    | Point
    | Sphere of Radius: float32
    | Aabb of Min: Vector3 * Max: Vector3 // This should be: float * float * float

    member this.DeepCopy() =
        match this with
        | Point -> Point
        | Sphere r -> Sphere r
        | Aabb (min, max) -> Aabb (min, max)
