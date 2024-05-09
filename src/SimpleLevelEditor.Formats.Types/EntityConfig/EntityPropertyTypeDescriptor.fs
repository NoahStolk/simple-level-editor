namespace SimpleLevelEditor.Formats.Types.EntityConfig

open System
open System.Numerics
open SimpleLevelEditor.Formats.Types
open SimpleLevelEditor.Formats.Types.Level

type EntityPropertyTypeDescriptor =
    | BoolProperty    of DefaultValue: bool
    | IntProperty     of DefaultValue: int     * Step: Option<int>     * MinValue: Option<int>     * MaxValue: Option<int>
    | FloatProperty   of DefaultValue: float32 * Step: Option<float32> * MinValue: Option<float32> * MaxValue: Option<float32>
    | Vector2Property of DefaultValue: Vector2 * Step: Option<float32> * MinValue: Option<float32> * MaxValue: Option<float32>
    | Vector3Property of DefaultValue: Vector3 * Step: Option<float32> * MinValue: Option<float32> * MaxValue: Option<float32>
    | Vector4Property of DefaultValue: Vector4 * Step: Option<float32> * MinValue: Option<float32> * MaxValue: Option<float32>
    | StringProperty  of DefaultValue: string
    | RgbProperty     of DefaultValue: Rgb
    | RgbaProperty    of DefaultValue: Rgba

    member this.DefaultValue =
        match this with
        | BoolProperty    value            -> EntityPropertyValue.Bool    value
        | IntProperty     (value, _, _, _) -> EntityPropertyValue.Int     value
        | FloatProperty   (value, _, _, _) -> EntityPropertyValue.Float   value
        | Vector2Property (value, _, _, _) -> EntityPropertyValue.Vector2 value
        | Vector3Property (value, _, _, _) -> EntityPropertyValue.Vector3 value
        | Vector4Property (value, _, _, _) -> EntityPropertyValue.Vector4 value
        | StringProperty  value            -> EntityPropertyValue.String  value
        | RgbProperty     value            -> EntityPropertyValue.Rgb     value
        | RgbaProperty    value            -> EntityPropertyValue.Rgba    value

    member this.Step : float32 =
        match this with
        | IntProperty     (_, step, _, _) -> if step.IsSome then float32 step.Value else 0f
        | FloatProperty   (_, step, _, _) -> if step.IsSome then step.Value else 0f
        | Vector2Property (_, step, _, _) -> if step.IsSome then step.Value else 0f
        | Vector3Property (_, step, _, _) -> if step.IsSome then step.Value else 0f
        | Vector4Property (_, step, _, _) -> if step.IsSome then step.Value else 0f
        | _ -> 0f

    member this.MinValue : float32 =
        match this with
        | IntProperty     (_, _, min, _) -> if min.IsSome then float32 min.Value else 0f
        | FloatProperty   (_, _, min, _) -> if min.IsSome then min.Value else 0f
        | Vector2Property (_, _, min, _) -> if min.IsSome then min.Value else 0f
        | Vector3Property (_, _, min, _) -> if min.IsSome then min.Value else 0f
        | Vector4Property (_, _, min, _) -> if min.IsSome then min.Value else 0f
        | _ -> 0f

    member this.MaxValue : float32 =
        match this with
        | IntProperty     (_, _, _, max) -> if max.IsSome then float32 max.Value else 0f
        | FloatProperty   (_, _, _, max) -> if max.IsSome then max.Value else 0f
        | Vector2Property (_, _, _, max) -> if max.IsSome then max.Value else 0f
        | Vector3Property (_, _, _, max) -> if max.IsSome then max.Value else 0f
        | Vector4Property (_, _, _, max) -> if max.IsSome then max.Value else 0f
        | _ -> 0f

    member this.GetTypeId() =
        match this with
        | BoolProperty    _ -> TypeIds.BoolId
        | IntProperty     _ -> TypeIds.IntId
        | FloatProperty   _ -> TypeIds.FloatId
        | Vector2Property _ -> TypeIds.Vector2Id
        | Vector3Property _ -> TypeIds.Vector3Id
        | Vector4Property _ -> TypeIds.Vector4Id
        | StringProperty  _ -> TypeIds.StringId
        | RgbProperty     _ -> TypeIds.RgbId
        | RgbaProperty    _ -> TypeIds.RgbaId

    member this.GetDisplayColor() =
        match this with
        | BoolProperty    _ -> new Vector4(0.00f, 0.25f, 1.00f, 1.00f)
        | IntProperty     _ -> new Vector4(0.00f, 0.50f, 1.00f, 1.00f)
        | FloatProperty   _ -> new Vector4(0.00f, 0.70f, 0.00f, 1.00f)
        | Vector2Property _ -> new Vector4(0.00f, 0.80f, 0.00f, 1.00f)
        | Vector3Property _ -> new Vector4(0.00f, 0.90f, 0.00f, 1.00f)
        | Vector4Property _ -> new Vector4(0.00f, 1.00f, 0.00f, 1.00f)
        | StringProperty  _ -> new Vector4(1.00f, 0.50f, 0.00f, 1.00f)
        | RgbProperty     _ -> new Vector4(1.00f, 0.75f, 0.00f, 1.00f)
        | RgbaProperty    _ -> new Vector4(1.00f, 1.00f, 0.00f, 1.00f)

    static member FromXmlData(propertyType: string, defaultValue: Option<string>, step: Option<string>, minValue: Option<string>, maxValue: Option<string>) : EntityPropertyTypeDescriptor =
        match propertyType with
        | TypeIds.BoolId    -> BoolProperty    (Boolean.FromDataString defaultValue |> Option.defaultValue false)
        | TypeIds.IntId     -> IntProperty     (Int32.FromDataString   defaultValue |> Option.defaultValue 0,            step |> Option.map int32,   minValue |> Option.map int32,   maxValue |> Option.map int32)
        | TypeIds.FloatId   -> FloatProperty   (Single.FromDataString  defaultValue |> Option.defaultValue 0f,           step |> Option.map float32, minValue |> Option.map float32, maxValue |> Option.map float32)
        | TypeIds.Vector2Id -> Vector2Property (Vector2.FromDataString defaultValue |> Option.defaultValue Vector2.Zero, step |> Option.map float32, minValue |> Option.map float32, maxValue |> Option.map float32)
        | TypeIds.Vector3Id -> Vector3Property (Vector3.FromDataString defaultValue |> Option.defaultValue Vector3.Zero, step |> Option.map float32, minValue |> Option.map float32, maxValue |> Option.map float32)
        | TypeIds.Vector4Id -> Vector4Property (Vector4.FromDataString defaultValue |> Option.defaultValue Vector4.Zero, step |> Option.map float32, minValue |> Option.map float32, maxValue |> Option.map float32)
        | TypeIds.StringId  -> StringProperty  (defaultValue                        |> Option.defaultValue String.Empty)
        | TypeIds.RgbId     -> RgbProperty     (Rgb.FromDataString     defaultValue |> Option.defaultValue Rgb.Default)
        | TypeIds.RgbaId    -> RgbaProperty    (Rgba.FromDataString    defaultValue |> Option.defaultValue Rgba.Default)
        | _ -> failwithf $"Unknown property type: %s{propertyType}"
