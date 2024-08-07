using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Types;
using System.Numerics;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class RgbTests
{
	private const float _epsilon = 0.00001f;

	[TestMethod]
	public void CreateDefault()
	{
		Rgb defaultRgb = Rgb.Default;
		Assert.AreEqual(0, defaultRgb.R);
		Assert.AreEqual(0, defaultRgb.G);
		Assert.AreEqual(0, defaultRgb.B);
	}

	[TestMethod]
	public void ToVector3()
	{
		Rgb orange = new(255, 165, 0);
		Vector3 orangeVector = orange.ToVector3();
		Assert.AreEqual(1, orangeVector.X, _epsilon);
		Assert.AreEqual(0.6470588f, orangeVector.Y, _epsilon);
		Assert.AreEqual(0, orangeVector.Z, _epsilon);
	}

	[TestMethod]
	public void ToVector4()
	{
		Rgb orange = new(255, 165, 0);
		Vector4 orangeVector = orange.ToVector4();
		Assert.AreEqual(1, orangeVector.X, _epsilon);
		Assert.AreEqual(0.6470588f, orangeVector.Y, _epsilon);
		Assert.AreEqual(0, orangeVector.Z, _epsilon);
		Assert.AreEqual(1, orangeVector.W, _epsilon);
	}

	[TestMethod]
	public void ToDisplayString()
	{
		Rgb orange = new(255, 165, 0);
		Assert.AreEqual("255 165 0", orange.ToDisplayString());
	}
}
