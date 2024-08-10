namespace GameEntityConfig.Emit;

public class FieldDescriptor
{
	public FieldDescriptor(string fieldName, Type fieldType)
	{
		FieldName = fieldName;
		FieldType = fieldType;
	}

	public string FieldName { get; }
	public Type FieldType { get; }
}
