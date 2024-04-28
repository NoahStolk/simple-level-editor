namespace SimpleLevelEditor.Formats.Types.Level

open System
open System.Globalization
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
        | PropertyIds.BoolId    -> EntityPropertyValue.Bool(bool.Parse(value))
        | PropertyIds.IntId     -> EntityPropertyValue.Int(Int32.Parse(value, CultureInfo.InvariantCulture))
        | PropertyIds.FloatId   -> EntityPropertyValue.Float(Single.Parse(value, CultureInfo.InvariantCulture))
        | PropertyIds.Vector2Id ->
            let split = value.Split(' ')
            EntityPropertyValue.Vector2(new Vector2(Single.Parse(split[0], CultureInfo.InvariantCulture), Single.Parse(split[1], CultureInfo.InvariantCulture)))
        | PropertyIds.Vector3Id ->
            let split = value.Split(' ')
            EntityPropertyValue.Vector3(new Vector3(Single.Parse(split[0], CultureInfo.InvariantCulture), Single.Parse(split[1], CultureInfo.InvariantCulture), Single.Parse(split[2], CultureInfo.InvariantCulture)))
        | PropertyIds.Vector4Id ->
            let split = value.Split(' ')
            EntityPropertyValue.Vector4(new Vector4(Single.Parse(split[0], CultureInfo.InvariantCulture), Single.Parse(split[1], CultureInfo.InvariantCulture), Single.Parse(split[2], CultureInfo.InvariantCulture), Single.Parse(split[3], CultureInfo.InvariantCulture)))
        | PropertyIds.StringId  -> EntityPropertyValue.String(value)
        | PropertyIds.RgbId     ->
            let split = value.Split(' ')
            let rgb = SimpleLevelEditor.Formats.Types.Rgb(Byte.Parse(split[0], CultureInfo.InvariantCulture), Byte.Parse(split[1], CultureInfo.InvariantCulture), Byte.Parse(split[2], CultureInfo.InvariantCulture));
            EntityPropertyValue.Rgb(rgb)
        | PropertyIds.RgbaId    ->
            let split = value.Split(' ')
            let rgba = SimpleLevelEditor.Formats.Types.Rgba(Byte.Parse(split[0], CultureInfo.InvariantCulture), Byte.Parse(split[1], CultureInfo.InvariantCulture), Byte.Parse(split[2], CultureInfo.InvariantCulture), Byte.Parse(split[3], CultureInfo.InvariantCulture));
            EntityPropertyValue.Rgba(rgba)
        | _         -> failwithf $"Unknown type id: %s{typeId}"
