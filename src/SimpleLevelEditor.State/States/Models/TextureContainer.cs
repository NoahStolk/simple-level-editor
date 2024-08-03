using Detach.Parsers.Texture;
using Silk.NET.OpenGL;
using SimpleLevelEditor.State.States.Messages;
using SimpleLevelEditor.State.Utils;

namespace SimpleLevelEditor.State.States.Models;

// TODO: We would need a robust way to resolve file paths from:
// - The entity config file.
// - The level file.
// - The .obj file.
// - The .mtl file.
public static class TextureContainer
{
	private static readonly Dictionary<TextureData, uint> _textures = new();

	public static uint GetTexture(GL gl, TextureData textureData)
	{
		if (_textures.TryGetValue(textureData, out uint textureId))
			return textureId;

		MessagesState.AddInfo("Loading unallocated texture...");

		textureId = GlObjectUtils.CreateTexture(gl, textureData.Width, textureData.Height, textureData.ColorData);
		_textures.Add(textureData, textureId);
		return textureId;
	}
}
