namespace SimpleLevelEditor.Model.Level.EntityShapes;

public record Point : IEntityShape
{
	public Point DeepCopy()
	{
		return new();
	}
}
