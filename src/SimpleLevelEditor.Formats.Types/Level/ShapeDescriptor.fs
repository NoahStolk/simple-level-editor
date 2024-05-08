namespace SimpleLevelEditor.Formats.Types.Level

open System
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

    static member FromShapeId(id: string, data: string) : Option<ShapeDescriptor> =
        if id = null then failwith "Shape id is null."
        if data = null then failwith "Shape data is null."

        match id with
        | ShapeIds.PointId  -> Some Point
        | ShapeIds.SphereId -> ShapeDescriptor.ParseSphereData(data)
        | ShapeIds.AabbId   -> ShapeDescriptor.ParseAabbData(data)
        | _                 -> None

    static member private ParseSphereData(data: string) : Option<ShapeDescriptor> =
        let radius = Single.FromDataString(Some data)
        match radius with
        | Some r -> Some(Sphere r)
        | None   -> None

    static member private ParseAabbData(data: string) : Option<ShapeDescriptor> =
        let size = Vector3.FromDataString(Some data)
        match size with
        | Some s -> Some(Aabb s)
        | None   -> None
