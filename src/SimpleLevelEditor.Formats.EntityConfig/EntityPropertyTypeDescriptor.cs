using Dunet;
using SimpleLevelEditor.Formats.Core;
using System.Diagnostics;
using System.Numerics;

namespace SimpleLevelEditor.Formats.EntityConfig;

[Union]
public partial record EntityPropertyTypeDescriptor
{
	public sealed partial record BoolProperty(bool Default);
	public sealed partial record IntProperty(int Default, int? Step, int? Min, int? Max);
	public sealed partial record FloatProperty(float Default, float? Step, float? Min, float? Max);
	public sealed partial record Vector2Property(Vector2 Default, float? Step, float? Min, float? Max);
	public sealed partial record Vector3Property(Vector3 Default, float? Step, float? Min, float? Max);
	public sealed partial record Vector4Property(Vector4 Default, float? Step, float? Min, float? Max);
	public sealed partial record StringProperty(string Default);
	public sealed partial record RgbaProperty(Rgba Default);
	public sealed partial record RgbProperty(Rgb Default);

	// public EntityPropertyValue GetDefaultValue()

	public float GetStep()
	{
		return this switch
		{
			IntProperty p => p.Step ?? 0,
			FloatProperty p => p.Step ?? 0,
			Vector2Property p => p.Step ?? 0,
			Vector3Property p => p.Step ?? 0,
			Vector4Property p => p.Step ?? 0,
			_ => 0,
		};
	}

	public float GetMin()
	{
		return this switch
		{
			IntProperty p => p.Min ?? 0,
			FloatProperty p => p.Min ?? 0,
			Vector2Property p => p.Min ?? 0,
			Vector3Property p => p.Min ?? 0,
			Vector4Property p => p.Min ?? 0,
			_ => 0,
		};
	}

	public float GetMax()
	{
		return this switch
		{
			IntProperty p => p.Max ?? 0,
			FloatProperty p => p.Max ?? 0,
			Vector2Property p => p.Max ?? 0,
			Vector3Property p => p.Max ?? 0,
			Vector4Property p => p.Max ?? 0,
			_ => 0,
		};
	}

	public string GetTypeId()
	{
		return this switch
		{
			BoolProperty => "Bool",
			IntProperty => "Int",
			FloatProperty => "Float",
			Vector2Property => "Vector2",
			Vector3Property => "Vector3",
			Vector4Property => "Vector4",
			StringProperty => "String",
			RgbaProperty => "Rgba",
			RgbProperty => "Rgb",
			_ => throw new UnreachableException(),
		};
	}

	public Vector4 GetDisplayColor()
	{
		return this switch
		{
			BoolProperty => new Vector4(0.00f, 0.25f, 1.00f, 1.00f),
			IntProperty => new Vector4(0.00f, 0.50f, 1.00f, 1.00f),
			FloatProperty => new Vector4(0.00f, 0.70f, 0.00f, 1.00f),
			Vector2Property => new Vector4(0.00f, 0.80f, 0.00f, 1.00f),
			Vector3Property => new Vector4(0.00f, 0.90f, 0.00f, 1.00f),
			Vector4Property => new Vector4(0.00f, 1.00f, 0.00f, 1.00f),
			StringProperty => new Vector4(1.00f, 0.50f, 0.00f, 1.00f),
			RgbaProperty => new Vector4(1.00f, 0.75f, 0.00f, 1.00f),
			RgbProperty => new Vector4(1.00f, 1.00f, 0.00f, 1.00f),
			_ => throw new UnreachableException(),
		};
	}

	public EntityPropertyTypeDescriptor DeepCopy()
	{
		return this switch
		{
			BoolProperty p => new BoolProperty(p.Default),
			IntProperty p => new IntProperty(p.Default, p.Step, p.Min, p.Max),
			FloatProperty p => new FloatProperty(p.Default, p.Step, p.Min, p.Max),
			Vector2Property p => new Vector2Property(p.Default, p.Step, p.Min, p.Max),
			Vector3Property p => new Vector3Property(p.Default, p.Step, p.Min, p.Max),
			Vector4Property p => new Vector4Property(p.Default, p.Step, p.Min, p.Max),
			StringProperty p => new StringProperty(p.Default),
			RgbaProperty p => new RgbaProperty(p.Default),
			RgbProperty p => new RgbProperty(p.Default),
			_ => throw new UnreachableException(),
		};
	}
}
