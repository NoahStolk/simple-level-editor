using GameEntityConfig.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace GameEntityConfig.Tests;

[TestClass]
public class ComponentTypeBuilderTests
{
	[TestMethod]
	public void FailOnDuplicateFieldName()
	{
		Assert.ThrowsException<ArgumentException>(() => ComponentTypeBuilder.CompileResultTypeInfo(
			"Test",
			[
				new FieldDescriptor("Field", typeof(int)),
				new FieldDescriptor("Field", typeof(int)),
			]));
	}

	[DataTestMethod]
	[DataRow(true, "Test1")]
	[DataRow(false, "")]
	[DataRow(false, " ")]
	[DataRow(true, "A")]
	[DataRow(false, "A ")]
	[DataRow(false, " A")]
	[DataRow(false, "1Test")]
	[DataRow(false, "Test!")]
	[DataRow(false, "Test 1")]
	[DataRow(false, "Test 1")]
	[DataRow(false, "Test-1")]
	[DataRow(true, "Test_1")]
	[DataRow(false, "Test.")]
	[DataRow(false, "Test@")]
	[DataRow(false, "Test#")]
	[DataRow(false, "Test$")]
	[DataRow(false, "Test%")]
	[DataRow(false, "Test^")]
	[DataRow(false, "Test&")]
	[DataRow(false, "Test*")]
	[DataRow(false, "Test(")]
	public void FailOnInvalidTypeName(bool expected, string typeName)
	{
		if (!expected)
			Assert.ThrowsException<ArgumentException>(() => ComponentTypeBuilder.CompileResultTypeInfo(typeName, [new FieldDescriptor("Test1", typeof(int))]));
		else
			Assert.IsNotNull(ComponentTypeBuilder.CompileResultTypeInfo(typeName, [new FieldDescriptor("Test1", typeof(int))]));
	}

	[DataTestMethod]
	[DataRow(true, "Test1")]
	[DataRow(true, "_Field")]
	[DataRow(true, "_field")]
	[DataRow(true, "Field")]
	[DataRow(true, "field")]
	[DataRow(false, "")]
	[DataRow(false, " ")]
	[DataRow(true, "A")]
	[DataRow(false, "A ")]
	[DataRow(false, " A")]
	[DataRow(false, "1Test")]
	[DataRow(false, "Test!")]
	[DataRow(false, "Test 1")]
	[DataRow(false, "Test 1")]
	[DataRow(false, "Test-1")]
	[DataRow(true, "Test_1")]
	[DataRow(false, "Test.")]
	[DataRow(false, "Test@")]
	[DataRow(false, "Test#")]
	[DataRow(false, "Test$")]
	[DataRow(false, "Test%")]
	[DataRow(false, "Test^")]
	[DataRow(false, "Test&")]
	[DataRow(false, "Test*")]
	[DataRow(false, "Test(")]
	public void FailOnInvalidFieldName(bool expected, string fieldName)
	{
		if (!expected)
			Assert.ThrowsException<ArgumentException>(() => ComponentTypeBuilder.CompileResultTypeInfo("Test", [new FieldDescriptor(fieldName, typeof(int))]));
		else
			Assert.IsNotNull(ComponentTypeBuilder.CompileResultTypeInfo("Test", [new FieldDescriptor(fieldName, typeof(int))]));
	}

	[TestMethod]
	public void BuildType()
	{
		TypeInfo typeInfo = ComponentTypeBuilder.CompileResultTypeInfo(
			"Test",
			[
				new FieldDescriptor("Test1", typeof(int)),
				new FieldDescriptor("Test2", typeof(float)),
				new FieldDescriptor("Test3", typeof(string)),
			]);

		Assert.IsNotNull(typeInfo);
		Assert.AreEqual("Test", typeInfo.Name);
		Assert.AreEqual(3, typeInfo.DeclaredFields.Count());
	}
}
