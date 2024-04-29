namespace SimpleLevelEditor.Formats.Types.Level

open System
open System.Numerics
open SimpleLevelEditor.Formats.Types

type EntityPropertyValue =
    | Bool    of Value: bool
    | Int     of Value: int
    | Float   of Value: float32
    | Vector2 of Value: Vector2
    | Vector3 of Value: Vector3
    | Vector4 of Value: Vector4
    | String  of Value: string
    | Rgb     of Value: Rgb
    | Rgba    of Value: Rgba

    member this.GetTypeId() =
        match this with
        | Bool    _ -> TypeIds.BoolId
        | Int     _ -> TypeIds.IntId
        | Float   _ -> TypeIds.FloatId
        | Vector2 _ -> TypeIds.Vector2Id
        | Vector3 _ -> TypeIds.Vector3Id
        | Vector4 _ -> TypeIds.Vector4Id
        | String  _ -> TypeIds.StringId
        | Rgb     _ -> TypeIds.RgbId
        | Rgba    _ -> TypeIds.RgbaId

    member this.WriteValue() =
        match this with
        | Bool    v -> v.ToDataString
        | Int     v -> v.ToDataString
        | Float   v -> v.ToDataString
        | Vector2 v -> v.ToDataString
        | Vector3 v -> v.ToDataString
        | Vector4 v -> v.ToDataString
        | String  v -> v
        | Rgb     v -> v.ToDataString
        | Rgba    v -> v.ToDataString

    static member FromTypeId(id: string, data: string) =
        match id with
        | TypeIds.BoolId    -> EntityPropertyValue.Bool(bool.FromDataString(data))
        | TypeIds.IntId     -> EntityPropertyValue.Int(Int32.FromDataString(data))
        | TypeIds.FloatId   -> EntityPropertyValue.Float(Single.FromDataString(data))
        | TypeIds.Vector2Id -> EntityPropertyValue.Vector2(Vector2.FromDataString(data))
        | TypeIds.Vector3Id -> EntityPropertyValue.Vector3(Vector3.FromDataString(data))
        | TypeIds.Vector4Id -> EntityPropertyValue.Vector4(Vector4.FromDataString(data))
        | TypeIds.StringId  -> EntityPropertyValue.String(data)
        | TypeIds.RgbId     -> EntityPropertyValue.Rgb(Rgb.FromDataString(data))
        | TypeIds.RgbaId    -> EntityPropertyValue.Rgba(Rgba.FromDataString(data))
        | _         -> failwithf $"Unknown type id: %s{id}"
