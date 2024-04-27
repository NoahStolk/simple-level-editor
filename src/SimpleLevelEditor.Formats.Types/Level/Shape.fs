namespace SimpleLevelEditor.Formats.Types.Level

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
