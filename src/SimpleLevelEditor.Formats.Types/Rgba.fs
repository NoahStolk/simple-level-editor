namespace SimpleLevelEditor.Formats.Types

open System.Numerics

[<Struct>]
type Rgba =
    struct
        val mutable R: byte
        val mutable G: byte
        val mutable B: byte
        val mutable A: byte

        new(r, g, b, a) = { R = r; G = g; B = b; A = a }

        member this.ToVector4() = Vector4(float32 this.R / 255.0f, float32 this.G / 255.0f, float32 this.B / 255.0f, float32 this.A / 255.0f)

        static member Default = Rgba(0uy, 0uy, 0uy, 0uy)
    end
