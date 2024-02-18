using System.Xml.Serialization;

namespace SimpleLevelEditor.Model.EntityConfig;

public record EntityPropertyDescriptor
{
	[XmlAttribute]
	public required string Name;

	[XmlAttribute]
	public required EntityPropertyType Type;

	[XmlAttribute]
	public required string? Description;

	[XmlAttribute]
	public required string? DefaultValue;

	/// <summary>
	/// Only applicable to <see cref="EntityPropertyType.Int"/>, <see cref="EntityPropertyType.Float"/>, <see cref="EntityPropertyType.Vector2"/>, <see cref="EntityPropertyType.Vector3"/> and <see cref="EntityPropertyType.Vector4"/>.
	/// </summary>
	[XmlAttribute]
	public required string? Step;

	/// <summary>
	/// Only applicable to <see cref="EntityPropertyType.Int"/>, <see cref="EntityPropertyType.Float"/>, <see cref="EntityPropertyType.Vector2"/>, <see cref="EntityPropertyType.Vector3"/> and <see cref="EntityPropertyType.Vector4"/>.
	/// </summary>
	[XmlAttribute]
	public required string? MinValue;

	/// <summary>
	/// Only applicable to <see cref="EntityPropertyType.Int"/>, <see cref="EntityPropertyType.Float"/>, <see cref="EntityPropertyType.Vector2"/>, <see cref="EntityPropertyType.Vector3"/> and <see cref="EntityPropertyType.Vector4"/>.
	/// </summary>
	[XmlAttribute]
	public required string? MaxValue;
}
