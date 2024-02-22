namespace SimpleLevelEditor.Formats.Level.BinaryModel;

internal static class BinaryModelConstants
{
	public const int MeshesSectionId = 0;
	public const int TexturesSectionId = 1;
	public const int WorldObjectsSectionId = 2;
	public const int EntitiesSectionId = 3;

	public const int EntityShapePoint = 0;
	public const int EntityShapeSphere = 1;
	public const int EntityShapeAabb = 2;

	public const int EntityPropertyTypeBool = 0;
	public const int EntityPropertyTypeInt = 1;
	public const int EntityPropertyTypeFloat = 2;
	public const int EntityPropertyTypeVector2 = 3;
	public const int EntityPropertyTypeVector3 = 4;
	public const int EntityPropertyTypeVector4 = 5;
	public const int EntityPropertyTypeString = 6;
	public const int EntityPropertyTypeRgb = 7;
	public const int EntityPropertyTypeRgba = 8;

	public static ReadOnlySpan<byte> Header => "SLEL"u8;
}
