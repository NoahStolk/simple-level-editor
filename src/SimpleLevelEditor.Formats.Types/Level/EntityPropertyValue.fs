namespace SimpleLevelEditor.Formats.Types.Level

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

    member this.DeepCopy() =
        match this with
        | Bool    v -> Bool v
        | Int     v -> Int v
        | Float   v -> Float v
        | Vector2 v -> Vector2 v
        | Vector3 v -> Vector3 v
        | Vector4 v -> Vector4 v
        | String  v -> String v
        | Rgb     v -> Rgb v
        | Rgba    v -> Rgba v

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
