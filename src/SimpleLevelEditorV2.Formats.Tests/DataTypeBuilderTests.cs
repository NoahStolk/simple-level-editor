using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditorV2.Formats.EntityConfig;
using SimpleLevelEditorV2.Formats.EntityConfig.Model;

namespace SimpleLevelEditorV2.Formats.Tests;

[TestClass]
public class DataTypeBuilderTests
{
	[TestMethod]
	public void FailOnDuplicateFieldName()
	{
		Assert.ThrowsException<ArgumentException>(() => DataTypeBuilder.Build(
			"Test",
			[
				new DataTypeField("Field", Primitive.I32),
				new DataTypeField("Field", Primitive.I32),
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
			Assert.ThrowsException<ArgumentException>(() => DataTypeBuilder.Build(typeName, [new DataTypeField("Test1", Primitive.I32)]));
		else
			Assert.IsNotNull(DataTypeBuilder.Build(typeName, [new DataTypeField("Test1", Primitive.I32)]));
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
			Assert.ThrowsException<ArgumentException>(() => DataTypeBuilder.Build("Test", [new DataTypeField(fieldName, Primitive.I32)]));
		else
			Assert.IsNotNull(DataTypeBuilder.Build("Test", [new DataTypeField(fieldName, Primitive.I32)]));
	}

	[TestMethod]
	public void BuildType()
	{
		DataType typeInfo = DataTypeBuilder.Build(
			"Test",
			[
				new DataTypeField("Test1", Primitive.I32),
				new DataTypeField("Test2", Primitive.F32),
				new DataTypeField("Test3", Primitive.Str),
			]);

		Assert.IsNotNull(typeInfo);
		Assert.AreEqual("Test", typeInfo.Name);
		Assert.AreEqual(3, typeInfo.Fields.Count);
	}
}
