using Dunet;
using SimpleLevelEditor.Formats.Core;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SimpleLevelEditor.Formats.EntityConfig;

[Union]
[JsonDerivedType(typeof(SimpleSphere), typeDiscriminator: nameof(SimpleSphere))]
[JsonDerivedType(typeof(BillboardSprite), typeDiscriminator: nameof(BillboardSprite))]
[JsonDerivedType(typeof(Model), typeDiscriminator: nameof(Model))]
public partial record PointEntityVisualization
{
	public sealed partial record SimpleSphere(Rgb Color, float Radius);
	public sealed partial record BillboardSprite(string TexturePath, float Size);
	public sealed partial record Model(string ModelPath, float Size);

	public string GetTypeId()
	{
		return this switch
		{
			SimpleSphere => nameof(SimpleSphere),
			BillboardSprite => nameof(BillboardSprite),
			Model => nameof(Model),
			_ => throw new UnreachableException(),
		};
	}

	public PointEntityVisualization DeepCopy()
	{
		return this switch
		{
			SimpleSphere simpleSphere => new SimpleSphere(simpleSphere.Color, simpleSphere.Radius),
			BillboardSprite billboardSprite => new BillboardSprite(billboardSprite.TexturePath, billboardSprite.Size),
			Model model => new Model(model.ModelPath, model.Size),
			_ => throw new UnreachableException(),
		};
	}
}
