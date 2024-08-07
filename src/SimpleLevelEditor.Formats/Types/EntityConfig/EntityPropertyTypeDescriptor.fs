namespace SimpleLevelEditor.Formats.Types.EntityConfig

open System.Numerics
open SimpleLevelEditor.Formats.Types
open SimpleLevelEditor.Formats.Types.Level

type EntityPropertyTypeDescriptor =
    | BoolProperty    of Default: bool
    | IntProperty     of Default: int     * Step: Option<int>     * Min: Option<int>     * Max: Option<int>
    | FloatProperty   of Default: float32 * Step: Option<float32> * Min: Option<float32> * Max: Option<float32>
    | Vector2Property of Default: Vector2 * Step: Option<float32> * Min: Option<float32> * Max: Option<float32>
    | Vector3Property of Default: Vector3 * Step: Option<float32> * Min: Option<float32> * Max: Option<float32>
    | Vector4Property of Default: Vector4 * Step: Option<float32> * Min: Option<float32> * Max: Option<float32>
    | StringProperty  of Default: string
    | RgbProperty     of Default: Rgb
    | RgbaProperty    of Default: Rgba

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

    // TODO: Return Option<float32> instead of float32
    member this.Step : float32 =
        match this with
        | IntProperty     (_, step, _, _) -> if step.IsSome then float32 step.Value else 0f
        | FloatProperty   (_, step, _, _) -> if step.IsSome then step.Value else 0f
        | Vector2Property (_, step, _, _) -> if step.IsSome then step.Value else 0f
        | Vector3Property (_, step, _, _) -> if step.IsSome then step.Value else 0f
        | Vector4Property (_, step, _, _) -> if step.IsSome then step.Value else 0f
        | _ -> 0f

    // TODO: Return Option<float32> instead of float32
    member this.MinValue : float32 =
        match this with
        | IntProperty     (_, _, min, _) -> if min.IsSome then float32 min.Value else 0f
        | FloatProperty   (_, _, min, _) -> if min.IsSome then min.Value else 0f
        | Vector2Property (_, _, min, _) -> if min.IsSome then min.Value else 0f
        | Vector3Property (_, _, min, _) -> if min.IsSome then min.Value else 0f
        | Vector4Property (_, _, min, _) -> if min.IsSome then min.Value else 0f
        | _ -> 0f

    // TODO: Return Option<float32> instead of float32
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
        | BoolProperty    _ -> nameof Bool
        | IntProperty     _ -> nameof Int
        | FloatProperty   _ -> nameof Float
        | Vector2Property _ -> nameof Vector2
        | Vector3Property _ -> nameof Vector3
        | Vector4Property _ -> nameof Vector4
        | StringProperty  _ -> nameof String
        | RgbProperty     _ -> nameof Rgb
        | RgbaProperty    _ -> nameof Rgba

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

    member this.DeepCopy() =
        match this with
        | BoolProperty    value                   -> BoolProperty    value
        | IntProperty     (value, step, min, max) -> IntProperty     (value, step, min, max)
        | FloatProperty   (value, step, min, max) -> FloatProperty   (value, step, min, max)
        | Vector2Property (value, step, min, max) -> Vector2Property (value, step, min, max)
        | Vector3Property (value, step, min, max) -> Vector3Property (value, step, min, max)
        | Vector4Property (value, step, min, max) -> Vector4Property (value, step, min, max)
        | StringProperty  value                   -> StringProperty  value
        | RgbProperty     value                   -> RgbProperty     value
        | RgbaProperty    value                   -> RgbaProperty    value
