namespace SimpleLevelEditor.Formats;

public interface IBinarySerializable<out TSelf>
	where TSelf : IBinarySerializable<TSelf>
{
	static abstract TSelf Read(BinaryReader br);

	void Write(BinaryWriter bw);
}
