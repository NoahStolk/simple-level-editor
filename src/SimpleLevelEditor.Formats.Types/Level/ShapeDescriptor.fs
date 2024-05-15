namespace SimpleLevelEditor.Formats.Types.Level

open System.Numerics
open SimpleLevelEditor.Formats.Types

type ShapeDescriptor =
    | Point
    | Sphere of Radius: float32
    | Aabb   of Size: Vector3

    member this.DeepCopy() =
        match this with
        | Point    -> Point
        | Sphere r -> Sphere r
        | Aabb s   -> Aabb s

    member this.GetShapeId() =
        match this with
        | Point    -> ShapeIds.PointId
        | Sphere _ -> ShapeIds.SphereId
        | Aabb _   -> ShapeIds.AabbId

    member this.WriteValue() =
        match this with
        | Point    -> ""
        | Sphere r -> r.ToDataString
        | Aabb s   -> s.ToDataString
