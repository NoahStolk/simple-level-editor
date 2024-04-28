namespace SimpleLevelEditor.Formats.Types.Level

open System
open System.Globalization
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
        | Sphere r        -> r.ToString(CultureInfo.InvariantCulture)
        | Aabb (min, max) -> $"{min.X.ToString(CultureInfo.InvariantCulture)} {min.Y.ToString(CultureInfo.InvariantCulture)} {min.Z.ToString(CultureInfo.InvariantCulture)} {max.X.ToString(CultureInfo.InvariantCulture)} {max.Y.ToString(CultureInfo.InvariantCulture)} {max.Z.ToString(CultureInfo.InvariantCulture)}"

    static member FromShapeId(shapeId: string, value: string) =
        match shapeId with
        | ShapeIds.PointId  -> Point
        | ShapeIds.SphereId -> Sphere(Single.Parse(value, CultureInfo.InvariantCulture))
        | ShapeIds.AabbId   ->
            let split = value.Split(' ')
            Aabb(
                Vector3(Single.Parse(split[0], CultureInfo.InvariantCulture), Single.Parse(split[1], CultureInfo.InvariantCulture), Single.Parse(split[2], CultureInfo.InvariantCulture)),
                Vector3(Single.Parse(split[3], CultureInfo.InvariantCulture), Single.Parse(split[4], CultureInfo.InvariantCulture), Single.Parse(split[5], CultureInfo.InvariantCulture)))
        | _        -> failwithf $"Unknown shape id: %s{shapeId}"
