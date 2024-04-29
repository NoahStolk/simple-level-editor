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
        | Bool    _ -> PropertyIds.BoolId
        | Int     _ -> PropertyIds.IntId
        | Float   _ -> PropertyIds.FloatId
        | Vector2 _ -> PropertyIds.Vector2Id
        | Vector3 _ -> PropertyIds.Vector3Id
        | Vector4 _ -> PropertyIds.Vector4Id
        | String  _ -> PropertyIds.StringId
        | Rgb     _ -> PropertyIds.RgbId
        | Rgba    _ -> PropertyIds.RgbaId

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

    static member FromTypeId(typeId: string, value: string) =
        match typeId with
        | PropertyIds.BoolId    -> EntityPropertyValue.Bool(bool.FromDataString(value))
        | PropertyIds.IntId     -> EntityPropertyValue.Int(Int32.FromDataString(value))
        | PropertyIds.FloatId   -> EntityPropertyValue.Float(Single.FromDataString(value))
        | PropertyIds.Vector2Id -> EntityPropertyValue.Vector2(Vector2.FromDataString(value))
        | PropertyIds.Vector3Id -> EntityPropertyValue.Vector3(Vector3.FromDataString(value))
        | PropertyIds.Vector4Id -> EntityPropertyValue.Vector4(Vector4.FromDataString(value))
        | PropertyIds.StringId  -> EntityPropertyValue.String(value)
        | PropertyIds.RgbId     -> EntityPropertyValue.Rgb(Rgb.FromDataString(value))
        | PropertyIds.RgbaId    -> EntityPropertyValue.Rgba(Rgba.FromDataString(value))
        | _         -> failwithf $"Unknown type id: %s{typeId}"
