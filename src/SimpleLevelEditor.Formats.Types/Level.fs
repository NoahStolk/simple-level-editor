module SimpleLevelEditor.Formats.Types.Level

open System
open System.Globalization
open System.Numerics

// Shapes
[<Literal>]
let PointId = "point"

[<Literal>]
let SphereId = "sphere"

[<Literal>]
let AabbId = "aabb"

// Properties
[<Literal>]
let BoolId = "bool"

[<Literal>]
let IntId = "s32"

[<Literal>]
let FloatId = "float"

[<Literal>]
let Vector2Id = "float2"

[<Literal>]
let Vector3Id = "float3"

[<Literal>]
let Vector4Id = "float4"

[<Literal>]
let StringId = "str"

[<Literal>]
let RgbId = "rgb"

[<Literal>]
let RgbaId = "rgba"

type Shape =
    | Point  of Vector3
    | Sphere of Vector3 * float32
    | Aabb   of Vector3 * Vector3

    member this.DeepCopy() =
        match this with
        | Point p         -> Point p
        | Sphere (c, r)   -> Sphere (c, r)
        | Aabb (min, max) -> Aabb (min, max)

type ShapeDescriptor =
    | Point
    | Sphere of Radius: float32
    | Aabb   of Min: Vector3 * Max: Vector3 // This should be: float * float * float

    member this.DeepCopy() =
        match this with
        | Point           -> Point
        | Sphere r        -> Sphere r
        | Aabb (min, max) -> Aabb (min, max)

    member this.GetShapeId() =
        match this with
        | Point    -> PointId
        | Sphere _ -> SphereId
        | Aabb _   -> AabbId

    member this.WriteValue() =
        match this with
        | Point           -> ""
        | Sphere r        -> r.ToString(CultureInfo.InvariantCulture)
        | Aabb (min, max) -> $"{min.X.ToString(CultureInfo.InvariantCulture)} {min.Y.ToString(CultureInfo.InvariantCulture)} {min.Z.ToString(CultureInfo.InvariantCulture)} {max.X.ToString(CultureInfo.InvariantCulture)} {max.Y.ToString(CultureInfo.InvariantCulture)} {max.Z.ToString(CultureInfo.InvariantCulture)}"

    static member FromShapeId(shapeId: string, value: string) =
        match shapeId with
        | PointId  -> Point
        | SphereId -> Sphere(Single.Parse(value, CultureInfo.InvariantCulture))
        | AabbId   ->
            let split = value.Split(' ')
            Aabb(
                Vector3(Single.Parse(split[0], CultureInfo.InvariantCulture), Single.Parse(split[1], CultureInfo.InvariantCulture), Single.Parse(split[2], CultureInfo.InvariantCulture)),
                Vector3(Single.Parse(split[3], CultureInfo.InvariantCulture), Single.Parse(split[4], CultureInfo.InvariantCulture), Single.Parse(split[5], CultureInfo.InvariantCulture)))
        | _        -> failwithf $"Unknown shape id: %s{shapeId}"

type EntityPropertyValue =
    | Bool    of Value: bool
    | Int     of Value: int
    | Float   of Value: float32
    | Vector2 of Value: Vector2
    | Vector3 of Value: Vector3
    | Vector4 of Value: Vector4
    | String  of Value: string
    | Rgb     of Value: Color.Rgb
    | Rgba    of Value: Color.Rgba

    member this.GetTypeId() =
        match this with
        | Bool    _ -> BoolId
        | Int     _ -> IntId
        | Float   _ -> FloatId
        | Vector2 _ -> Vector2Id
        | Vector3 _ -> Vector3Id
        | Vector4 _ -> Vector4Id
        | String  _ -> StringId
        | Rgb     _ -> RgbId
        | Rgba    _ -> RgbaId

    member this.WriteValue() =
        match this with
        | Bool    v -> v.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()
        | Int     v -> v.ToString(CultureInfo.InvariantCulture)
        | Float   v -> v.ToString(CultureInfo.InvariantCulture)
        | Vector2 v -> $"{v.X.ToString(CultureInfo.InvariantCulture)} {v.Y.ToString(CultureInfo.InvariantCulture)}";
        | Vector3 v -> $"{v.X.ToString(CultureInfo.InvariantCulture)} {v.Y.ToString(CultureInfo.InvariantCulture)} {v.Z.ToString(CultureInfo.InvariantCulture)}"
        | Vector4 v -> $"{v.X.ToString(CultureInfo.InvariantCulture)} {v.Y.ToString(CultureInfo.InvariantCulture)} {v.Z.ToString(CultureInfo.InvariantCulture)} {v.W.ToString(CultureInfo.InvariantCulture)}"
        | String  v -> v
        | Rgb     v -> $"{v.R.ToString(CultureInfo.InvariantCulture)} {v.G.ToString(CultureInfo.InvariantCulture)} {v.B.ToString(CultureInfo.InvariantCulture)}"
        | Rgba    v -> $"{v.R.ToString(CultureInfo.InvariantCulture)} {v.G.ToString(CultureInfo.InvariantCulture)} {v.B.ToString(CultureInfo.InvariantCulture)} {v.A.ToString(CultureInfo.InvariantCulture)}"

    static member FromTypeId(typeId: string, value: string) =
        match typeId with
        | BoolId    -> EntityPropertyValue.Bool(bool.Parse(value))
        | IntId     -> EntityPropertyValue.Int(Int32.Parse(value, CultureInfo.InvariantCulture))
        | FloatId   -> EntityPropertyValue.Float(Single.Parse(value, CultureInfo.InvariantCulture))
        | Vector2Id ->
            let split = value.Split(' ')
            EntityPropertyValue.Vector2(new Vector2(Single.Parse(split[0], CultureInfo.InvariantCulture), Single.Parse(split[1], CultureInfo.InvariantCulture)))
        | Vector3Id ->
            let split = value.Split(' ')
            EntityPropertyValue.Vector3(new Vector3(Single.Parse(split[0], CultureInfo.InvariantCulture), Single.Parse(split[1], CultureInfo.InvariantCulture), Single.Parse(split[2], CultureInfo.InvariantCulture)))
        | Vector4Id ->
            let split = value.Split(' ')
            EntityPropertyValue.Vector4(new Vector4(Single.Parse(split[0], CultureInfo.InvariantCulture), Single.Parse(split[1], CultureInfo.InvariantCulture), Single.Parse(split[2], CultureInfo.InvariantCulture), Single.Parse(split[3], CultureInfo.InvariantCulture)))
        | StringId  -> EntityPropertyValue.String(value)
        | RgbId     ->
            let split = value.Split(' ')
            let rgb: Color.Rgb = { R = Byte.Parse(split[0], CultureInfo.InvariantCulture); G = Byte.Parse(split[1], CultureInfo.InvariantCulture); B = Byte.Parse(split[2], CultureInfo.InvariantCulture) };
            EntityPropertyValue.Rgb(rgb)
        | RgbaId    ->
            let split = value.Split(' ')
            let rgba: Color.Rgba = { R = Byte.Parse(split[0], CultureInfo.InvariantCulture); G = Byte.Parse(split[1], CultureInfo.InvariantCulture); B = Byte.Parse(split[2], CultureInfo.InvariantCulture); A = Byte.Parse(split[3], CultureInfo.InvariantCulture) }
            EntityPropertyValue.Rgba(rgba)
        | _         -> failwithf $"Unknown type id: %s{typeId}"
