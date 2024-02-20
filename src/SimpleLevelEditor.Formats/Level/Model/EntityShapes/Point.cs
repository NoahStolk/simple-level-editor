namespace SimpleLevelEditor.Formats.Level.Model.EntityShapes;

public record Point
{
	public Point DeepCopy()
	{
		return new();
	}
}
