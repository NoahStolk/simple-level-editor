module SimpleLevelEditor.Formats.Types.Color

[<Struct>]
type Rgb =
    struct
        val R: byte
        val G: byte
        val B: byte

        new (r, g, b) = { R = r; G = g; B = b }
    end

[<Struct>]
type Rgba =
    struct
        val R: byte
        val G: byte
        val B: byte
        val A: byte

        new (r, g, b, a) = { R = r; G = g; B = b; A = a }
    end
