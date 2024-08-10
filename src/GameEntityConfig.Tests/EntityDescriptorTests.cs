using GameEntityConfig.Core.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

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
				.WithFixedComponent(new Scale(Vector3.One))
				.WithFixedComponent(new Scale(Vector3.One))
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
				.WithVaryingComponent(new Scale(Vector3.One))
				.WithVaryingComponent(new Scale(Vector3.One))
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
				.WithFixedComponent(new Scale(Vector3.One))
				.WithVaryingComponent(new Scale(Vector3.One))
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
				.WithFixedComponent(new Scale(Vector3.One))
				.WithFixedComponent(new Scale(Vector3.Zero))
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
				.WithVaryingComponent(new Scale(Vector3.One))
				.WithVaryingComponent(new Scale(Vector3.Zero))
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
				.WithFixedComponent(new Scale(Vector3.One))
				.WithVaryingComponent(new Scale(Vector3.Zero))
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
				.WithVaryingComponent(new Scale(Vector3.One))
				.WithVaryingComponent(new Scale(Vector3.One), 0.1f, 0.0f, 1.0f)
				.Build();
		});
	}
}
