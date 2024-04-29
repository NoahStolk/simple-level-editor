namespace SimpleLevelEditor.Formats.Types.Level

open System
open System.Numerics
open SimpleLevelEditor.Formats.Types

type ShapeDescriptor =
    | Point
    | Sphere of Radius: float32
    | Aabb   of Min: Vector3 * Max: Vector3 // This should be: X: float * Y: float * Z: float

    member this.DeepCopy() =
        match this with
        | Point           -> Point
        | Sphere r        -> Sphere r
        | Aabb (min, max) -> Aabb (min, max)

    member this.GetShapeId() =
        match this with
        | Point    -> ShapeIds.PointId
        | Sphere _ -> ShapeIds.SphereId
        | Aabb _   -> ShapeIds.AabbId

    member this.WriteValue() =
        match this with
        | Point           -> ""
        | Sphere r        -> r.ToDataString
        | Aabb (min, max) -> $"{min.ToDataString} {max.ToDataString}"

    static member FromShapeId(shapeId: string, shapeData: string) =
        match shapeId with
        | ShapeIds.PointId  -> Point
        | ShapeIds.SphereId -> ShapeDescriptor.ParseSphereData(shapeData)
        | ShapeIds.AabbId   -> ShapeDescriptor.ParseAabbData(shapeData)
        | _                 -> failwithf $"Unknown shape id: %s{shapeId}"

    static member private ParseSphereData(sphereData: String) =
        Sphere(Single.FromDataString(sphereData))

    static member private ParseAabbData(aabbData: String) =
        let parts = aabbData.Split(' ')
        if parts.Length <> 6 then failwithf $"Invalid point data: %s{aabbData}"
        Aabb(
            Vector3(Single.FromDataString(parts[0]), Single.FromDataString(parts[1]), Single.FromDataString(parts[2])),
            Vector3(Single.FromDataString(parts[3]), Single.FromDataString(parts[4]), Single.FromDataString(parts[5])))
