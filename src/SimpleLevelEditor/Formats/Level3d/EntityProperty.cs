using OneOf;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Level3d.Enums;

namespace SimpleLevelEditor.Formats.Level3d;

public class EntityProperty : IBinarySerializable<EntityProperty>
{
	public EntityProperty(string key, OneOf<bool, byte, ushort, int, float, Vector2, Vector3, Vector4, Quaternion, string> value)
	{
		Key = key;
		Value = value;
	}

	public string Key { get; set; }

	public OneOf<bool, byte, ushort, int, float, Vector2, Vector3, Vector4, Quaternion, string> Value { get; set; }

	public static EntityProperty Read(BinaryReader br)
	{
		string key = br.ReadString();
		PropertyValueType propertyType = (PropertyValueType)br.ReadByte();
		return propertyType switch
		{
			PropertyValueType.Boolean => new(key, br.ReadBoolean()),
			PropertyValueType.UInt8 => new(key, br.ReadByte()),
			PropertyValueType.UInt16 => new(key, br.ReadUInt16()),
			PropertyValueType.Int32 => new(key, br.ReadInt32()),
			PropertyValueType.Float32 => new(key, br.ReadSingle()),
			PropertyValueType.Vector2Float32 => new(key, br.ReadVector2()),
			PropertyValueType.Vector3Float32 => new(key, br.ReadVector3()),
			PropertyValueType.Vector4Float32 => new(key, br.ReadVector4()),
			PropertyValueType.QuaternionFloat32 => new(key, br.ReadQuaternion()),
			PropertyValueType.String => new(key, br.ReadString()),
			_ => throw new InvalidDataException("Invalid property type."),
		};
	}

	public void Write(BinaryWriter bw)
	{
		bw.Write(Key);

		switch (Value.Value)
		{
			case bool b:
				bw.Write((byte)PropertyValueType.Boolean);
				bw.Write(b);
				break;
			case byte u8:
				bw.Write((byte)PropertyValueType.UInt8);
				bw.Write(u8);
				break;
			case ushort u16:
				bw.Write((byte)PropertyValueType.UInt16);
				bw.Write(u16);
				break;
			case int i32:
				bw.Write((byte)PropertyValueType.Int32);
				bw.Write(i32);
				break;
			case float f32:
				bw.Write((byte)PropertyValueType.Float32);
				bw.Write(f32);
				break;
			case Vector2 v2:
				bw.Write((byte)PropertyValueType.Vector2Float32);
				bw.Write(v2);
				break;
			case Vector3 v3:
				bw.Write((byte)PropertyValueType.Vector3Float32);
				bw.Write(v3);
				break;
			case Vector4 v4:
				bw.Write((byte)PropertyValueType.Vector4Float32);
				bw.Write(v4);
				break;
			case Quaternion q:
				bw.Write((byte)PropertyValueType.QuaternionFloat32);
				bw.Write(q);
				break;
			case string s:
				bw.Write((byte)PropertyValueType.String);
				bw.Write(s);
				break;
		}
	}
}
