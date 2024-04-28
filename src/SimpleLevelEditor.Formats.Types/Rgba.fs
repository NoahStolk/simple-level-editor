namespace SimpleLevelEditor.Formats.Types

open System
open System.Globalization

[<Struct>]
type Rgba =
    struct
        val mutable R: byte
        val mutable G: byte
        val mutable B: byte
        val mutable A: byte

        new(r, g, b, a) = { R = r; G = g; B = b; A = a }
    end
