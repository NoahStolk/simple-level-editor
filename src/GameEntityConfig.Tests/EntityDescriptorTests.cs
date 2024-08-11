using GameEntityConfig.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameEntityConfig.Tests;

[TestClass]
public class EntityDescriptorTests
{
	[TestMethod]
	public void FailOnDuplicateFixedComponentTypes()
	{
		EntityDescriptorBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithName("Invalid")
				.WithFixedComponent(DataType.Scale, "1;1;1")
				.WithFixedComponent(DataType.Scale, "1;1;1")
				.Build();
		});
	}

	[TestMethod]
	public void FailOnDuplicateVaryingComponentTypes()
	{
		EntityDescriptorBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithName("Invalid")
				.WithVaryingComponent(DataType.Scale, "1;1;1")
				.WithVaryingComponent(DataType.Scale, "1;1;1")
				.Build();
		});
	}

	[TestMethod]
	public void FailOnDuplicateFixedAndVaryingComponentTypes()
	{
		EntityDescriptorBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithName("Invalid")
				.WithFixedComponent(DataType.Scale, "1;1;1")
				.WithVaryingComponent(DataType.Scale, "1;1;1")
				.Build();
		});
	}

	[TestMethod]
	public void FailOnDuplicateFixedComponentTypesWithDifferentValues()
	{
		EntityDescriptorBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithName("Invalid")
				.WithFixedComponent(DataType.Scale, "1;1;1")
				.WithFixedComponent(DataType.Scale, "0;0;0")
				.Build();
		});
	}

	[TestMethod]
	public void FailOnDuplicateVaryingComponentTypesWithDifferentValues()
	{
		EntityDescriptorBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithName("Invalid")
				.WithVaryingComponent(DataType.Scale, "1;1;1")
				.WithVaryingComponent(DataType.Scale, "0;0;0")
				.Build();
		});
	}

	[TestMethod]
	public void FailOnDuplicateFixedAndVaryingComponentTypesWithDifferentValues()
	{
		EntityDescriptorBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithName("Invalid")
				.WithFixedComponent(DataType.Scale, "1;1;1")
				.WithVaryingComponent(DataType.Scale, "0;0;0")
				.Build();
		});
	}

	[TestMethod]
	public void FailOnDuplicateVaryingComponentTypesWithSliderConfiguration()
	{
		EntityDescriptorBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithName("Invalid")
				.WithVaryingComponent(DataType.Scale, "1;1;1")
				.WithVaryingComponent(DataType.Scale, "1;1;1", 0.1f, 0.0f, 1.0f)
				.Build();
		});
	}
}
