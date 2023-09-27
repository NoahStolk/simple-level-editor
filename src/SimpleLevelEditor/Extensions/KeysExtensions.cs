using Silk.NET.GLFW;

namespace SimpleLevelEditor.Extensions;

public static class KeysExtensions
{
	public static bool IsNumber(this Keys key)
	{
		int code = (int)key;
		return code is >= (int)Keys.Number0 and <= (int)Keys.Number9 or >= (int)Keys.Keypad0 and <= (int)Keys.Keypad9;
	}

	public static bool IsLetter(this Keys key)
	{
		int code = (int)key;
		return code is >= (int)Keys.A and <= (int)Keys.Z;
	}

	public static char? GetChar(this Keys key, bool isShiftKeyHeld)
	{
		if (key.IsLetter())
		{
			char c = (char)key;
			return isShiftKeyHeld ? c : char.ToLower(c);
		}

		return key switch
		{
			Keys.Keypad0 => '0',
			Keys.Keypad1 => '1',
			Keys.Keypad2 => '2',
			Keys.Keypad3 => '3',
			Keys.Keypad4 => '4',
			Keys.Keypad5 => '5',
			Keys.Keypad6 => '6',
			Keys.Keypad7 => '7',
			Keys.Keypad8 => '8',
			Keys.Keypad9 => '9',

			Keys.Number0 => isShiftKeyHeld ? ')' : '0',
			Keys.Number1 => isShiftKeyHeld ? '!' : '1',
			Keys.Number2 => isShiftKeyHeld ? '@' : '2',
			Keys.Number3 => isShiftKeyHeld ? '#' : '3',
			Keys.Number4 => isShiftKeyHeld ? '$' : '4',
			Keys.Number5 => isShiftKeyHeld ? '%' : '5',
			Keys.Number6 => isShiftKeyHeld ? '^' : '6',
			Keys.Number7 => isShiftKeyHeld ? '&' : '7',
			Keys.Number8 => isShiftKeyHeld ? '*' : '8',
			Keys.Number9 => isShiftKeyHeld ? '(' : '9',

			Keys.Enter => '\n',
			Keys.Space => ' ',

			Keys.Comma => isShiftKeyHeld ? '<' : ',',
			Keys.Period => isShiftKeyHeld ? '>' : '.',
			Keys.Slash => isShiftKeyHeld ? '?' : '/',

			Keys.Semicolon => isShiftKeyHeld ? ':' : ';',
			Keys.Apostrophe => isShiftKeyHeld ? '"' : '\'',

			Keys.LeftBracket => isShiftKeyHeld ? '{' : '[',
			Keys.RightBracket => isShiftKeyHeld ? '}' : ']',
			Keys.BackSlash => isShiftKeyHeld ? '|' : '\\',

			Keys.Minus => isShiftKeyHeld ? '_' : '-',
			Keys.Equal => isShiftKeyHeld ? '+' : '=',

			Keys.GraveAccent => isShiftKeyHeld ? '~' : '`',

			_ => null,
		};
	}
}
