module SimpleLevelEditor.Formats.Types.EntityConfig

open System
open System.Numerics
open SimpleLevelEditor.Formats.Types
open SimpleLevelEditor.Formats.Types.Level

type EntityShape =
    | Point
    | Sphere
    | Aabb

type EntityPropertyTypeDescriptor =
    | BoolProperty    of DefaultValue: bool
    | IntProperty     of DefaultValue: int     * Step: Nullable<int>     * MinValue: Nullable<int>     * MaxValue: Nullable<int>
    | FloatProperty   of DefaultValue: float32 * Step: Nullable<float32> * MinValue: Nullable<float32> * MaxValue: Nullable<float32>
    | Vector2Property of DefaultValue: Vector2 * Step: Nullable<float32> * MinValue: Nullable<float32> * MaxValue: Nullable<float32>
    | Vector3Property of DefaultValue: Vector3 * Step: Nullable<float32> * MinValue: Nullable<float32> * MaxValue: Nullable<float32>
    | Vector4Property of DefaultValue: Vector4 * Step: Nullable<float32> * MinValue: Nullable<float32> * MaxValue: Nullable<float32>
    | StringProperty  of DefaultValue: string
    | RgbProperty     of DefaultValue: Color.Rgb
    | RgbaProperty    of DefaultValue: Color.Rgba

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
        | IntProperty     (_, step, _, _) -> if step.HasValue then float32 step.Value else 0f
        | FloatProperty   (_, step, _, _) -> if step.HasValue then step.Value else 0f
        | Vector2Property (_, step, _, _) -> if step.HasValue then step.Value else 0f
        | Vector3Property (_, step, _, _) -> if step.HasValue then step.Value else 0f
        | Vector4Property (_, step, _, _) -> if step.HasValue then step.Value else 0f
        | _ -> 0f

    member this.MinValue : float32 =
        match this with
        | IntProperty     (_, _, min, _) -> if min.HasValue then float32 min.Value else 0f
        | FloatProperty   (_, _, min, _) -> if min.HasValue then min.Value else 0f
        | Vector2Property (_, _, min, _) -> if min.HasValue then min.Value else 0f
        | Vector3Property (_, _, min, _) -> if min.HasValue then min.Value else 0f
        | Vector4Property (_, _, min, _) -> if min.HasValue then min.Value else 0f
        | _ -> 0f

    member this.MaxValue : float32 =
        match this with
        | IntProperty     (_, _, _, max) -> if max.HasValue then float32 max.Value else 0f
        | FloatProperty   (_, _, _, max) -> if max.HasValue then max.Value else 0f
        | Vector2Property (_, _, _, max) -> if max.HasValue then max.Value else 0f
        | Vector3Property (_, _, _, max) -> if max.HasValue then max.Value else 0f
        | Vector4Property (_, _, _, max) -> if max.HasValue then max.Value else 0f
        | _ -> 0f

    member this.GetTypeId() =
        match this with
        | BoolProperty    _ -> BoolId
        | IntProperty     _ -> IntId
        | FloatProperty   _ -> FloatId
        | Vector2Property _ -> Vector2Id
        | Vector3Property _ -> Vector3Id
        | Vector4Property _ -> Vector4Id
        | StringProperty  _ -> StringId
        | RgbProperty     _ -> RgbId
        | RgbaProperty    _ -> RgbaId

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
