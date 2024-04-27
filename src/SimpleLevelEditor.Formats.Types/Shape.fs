namespace SimpleLevelEditor.Formats.Types

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

type Shape =
    | Point of Vector3
    | Sphere of Vector3 * float32
    | Aabb of Vector3 * Vector3

    member this.DeepCopy() =
        match this with
        | Point p -> Point p
        | Sphere (c, r) -> Sphere (c, r)
        | Aabb (min, max) -> Aabb (min, max)
