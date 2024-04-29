namespace SimpleLevelEditor.Formats.Types

[<Struct>]
type Rgb =
    struct
        val mutable R: byte
        val mutable G: byte
        val mutable B: byte

        new(r, g, b) = { R = r; G = g; B = b }
    end
