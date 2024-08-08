namespace GameEntityConfig.Core;

public abstract record FixedComponent;

public sealed record FixedComponent<T> : FixedComponent
{
	internal FixedComponent(T value)
	{
		Value = value;
	}

	public T Value { get; }
}
