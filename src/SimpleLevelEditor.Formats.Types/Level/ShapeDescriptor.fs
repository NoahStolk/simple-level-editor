namespace SimpleLevelEditor.Formats.Types.Level

open SimpleLevelEditor.Formats.Types
open System.Numerics

// TODO: Rename type to EntityShape.
type ShapeDescriptor =
    | Point
    | Sphere of Radius: float32
    | Aabb   of Size: Vector3

    member this.DeepCopy() =
        match this with
        | Point    -> Point
        | Sphere r -> Sphere r
        | Aabb s   -> Aabb s

    member this.ToDisplayString() =
        match this with
        | Point    -> ""
        | Sphere r -> r.ToDisplayString
        | Aabb s   -> s.ToDisplayString
