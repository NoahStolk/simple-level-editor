namespace SimpleLevelEditor.Model.EntityShapes;

public record Point : IEntityShape
{
	public Point DeepCopy()
	{
		return new();
	}
}
