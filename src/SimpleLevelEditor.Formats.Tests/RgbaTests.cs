using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Types;
using System.Numerics;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class RgbaTests
{
	private const float _epsilon = 0.00001f;

	[TestMethod]
	public void CreateDefault()
	{
		Rgba defaultRgb = Rgba.Default;
		Assert.AreEqual(0, defaultRgb.R);
		Assert.AreEqual(0, defaultRgb.G);
		Assert.AreEqual(0, defaultRgb.B);
		Assert.AreEqual(0, defaultRgb.A);
	}

	[TestMethod]
	public void ToVector4()
	{
		Rgba orange = new(255, 165, 0, 127);
		Vector4 orangeVector = orange.ToVector4();
		Assert.AreEqual(1, orangeVector.X, _epsilon);
		Assert.AreEqual(0.6470588f, orangeVector.Y, _epsilon);
		Assert.AreEqual(0, orangeVector.Z, _epsilon);
		Assert.AreEqual(0.498039216f, orangeVector.W, _epsilon);
	}

	[TestMethod]
	public void ToDisplayString()
	{
		Rgba orange = new(255, 165, 0, 127);
		Assert.AreEqual("255 165 0 127", orange.ToDisplayString());
	}
}
