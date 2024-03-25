namespace SimpleLevelEditor.Formats.Level.BinaryModel;

internal sealed class Section
{
	private readonly int _id;
	private readonly byte[] _data;

	public Section(int id, byte[] data)
	{
		_id = id;
		_data = data;
	}

	public void Write(BinaryWriter bw)
	{
		bw.Write7BitEncodedInt(_id);
		bw.Write7BitEncodedInt(_data.Length);
		bw.Write(_data);
	}
}
