using SimpleLevelEditorV2.Formats.EntityConfig.Model;
using System.Numerics;
using System.Text.Json.Serialization;

namespace SimpleLevelEditorV2.Formats.Level.Model;

public sealed record LevelEntity
{
	[JsonConstructor]
	public LevelEntity(string entityDescriptorName, Dictionary<string, string> data)
	{
		EntityDescriptorName = entityDescriptorName;
		Data = data;
	}

	public string EntityDescriptorName { get; }

	public Dictionary<string, string> Data { get; }

	public Vector4? DiffuseColor
	{
		get
		{
			if (!Data.TryGetValue(DataType.DiffuseColor.Name, out string? diffuseColor) || !DataTypeParsing.TryParseVector4(diffuseColor, out Vector4 vector4))
				return null;

			const float max = byte.MaxValue;
			return new Vector4(
				vector4.X / max,
				vector4.Y / max,
				vector4.Z / max,
				vector4.W / max);
		}
	}

	public Vector3? Position => Data.TryGetValue(DataType.Position.Name, out string? position) && DataTypeParsing.TryParseVector3(position, out Vector3 vector3) ? vector3 : null;

	public Vector3? Rotation => Data.TryGetValue(DataType.Rotation.Name, out string? rotation) && DataTypeParsing.TryParseVector3(rotation, out Vector3 vector3) ? vector3 : null;

	public Vector3? Scale => Data.TryGetValue(DataType.Scale.Name, out string? scale) && DataTypeParsing.TryParseVector3(scale, out Vector3 vector3) ? vector3 : null;

	public string? ModelPath => Data.GetValueOrDefault(DataType.Model.Name);

	public string? TexturePath => Data.GetValueOrDefault(DataType.Billboard.Name);

	public float? WireframeThickness
	{
		get
		{
			string? wireframeData = Data.GetValueOrDefault(DataType.Wireframe.Name);
			if (wireframeData == null)
				return null;

			string[] split = wireframeData.Split(FormattingConstants.Separator);
			return split.Length == 2 && PrimitiveParsing.TryParseF32(split[0], out float size) ? size : null;
		}
	}

	public string? WireframeShape
	{
		get
		{
			string? wireframeData = Data.GetValueOrDefault(DataType.Wireframe.Name);
			if (wireframeData == null)
				return null;

			string[] split = wireframeData.Split(FormattingConstants.Separator);
			return split.Length == 2 ? split[1] : null;
		}
	}

	public bool IsModel => Data.ContainsKey(DataType.Model.Name);

	public bool IsBillboard => Data.ContainsKey(DataType.Billboard.Name);

	public bool IsWireframe => Data.ContainsKey(DataType.Wireframe.Name);
}
