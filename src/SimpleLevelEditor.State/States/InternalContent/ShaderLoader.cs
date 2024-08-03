using Silk.NET.OpenGL;

namespace SimpleLevelEditor.State.States.InternalContent;

public static class ShaderLoader
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
