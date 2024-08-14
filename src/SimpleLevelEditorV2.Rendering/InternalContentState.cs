using Detach.Parsers.Texture;
using Silk.NET.OpenGL;

namespace SimpleLevelEditorV2.Rendering;

public static class InternalContentState
{
	private static readonly Dictionary<string, uint> _textures = new();
	private static readonly Dictionary<string, ShaderCacheEntry> _shaders = new();

	public static IReadOnlyDictionary<string, uint> Textures => _textures;
	public static IReadOnlyDictionary<string, ShaderCacheEntry> Shaders => _shaders;

	public static void AddTexture(GL gl, string name, TextureData texture)
	{
		_textures.Add(name, TextureLoader.Load(gl, texture));
	}

	public static void AddShader(GL gl, string name, string vertexCode, string fragmentCode)
	{
		_shaders.Add(name, new ShaderCacheEntry(ShaderLoader.Load(gl, vertexCode, fragmentCode)));
	}

	private static class ShaderLoader
	{
		public static uint Load(GL gl, string vertexCode, string fragmentCode)
		{
			uint vs = gl.CreateShader(ShaderType.VertexShader);
			gl.ShaderSource(vs, vertexCode);
			gl.CompileShader(vs);
			CheckShaderStatus(gl, ShaderType.VertexShader, vs);

			uint fs = gl.CreateShader(ShaderType.FragmentShader);
			gl.ShaderSource(fs, fragmentCode);
			gl.CompileShader(fs);
			CheckShaderStatus(gl, ShaderType.FragmentShader, fs);

			uint id = gl.CreateProgram();

			gl.AttachShader(id, vs);
			gl.AttachShader(id, fs);
			gl.LinkProgram(id);

			gl.DetachShader(id, vs);
			gl.DetachShader(id, fs);

			gl.DeleteShader(vs);
			gl.DeleteShader(fs);

			return id;
		}

		private static void CheckShaderStatus(GL gl, ShaderType shaderType, uint shaderId)
		{
			string infoLog = gl.GetShaderInfoLog(shaderId);
			if (!string.IsNullOrWhiteSpace(infoLog))
				throw new InvalidOperationException($"{shaderType} compile error: {infoLog}");
		}
	}

	private static class TextureLoader
	{
		public static unsafe uint Load(GL gl, TextureData texture)
		{
			uint textureId = gl.GenTexture();

			gl.BindTexture(TextureTarget.Texture2D, textureId);

			gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
			gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
			gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
			gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

			fixed (byte* b = texture.ColorData)
				gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, texture.Width, texture.Height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

			gl.GenerateMipmap(TextureTarget.Texture2D);

			return textureId;
		}
	}
}
