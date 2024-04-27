namespace SimpleLevelEditor.Formats.Types.EntityConfig

open System
open System.Numerics
open SimpleLevelEditor.Formats.Types

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
