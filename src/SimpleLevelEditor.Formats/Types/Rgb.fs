namespace SimpleLevelEditor.Formats.Types

open System.Numerics

[<Struct>]
type Rgb =
    struct
        val mutable R: byte
        val mutable G: byte
        val mutable B: byte

        new(r, g, b) = { R = r; G = g; B = b }

        member this.ToVector3() =
            Vector3(float32 this.R / 255.0f, float32 this.G / 255.0f, float32 this.B / 255.0f)

        member this.ToVector4() =
            Vector4(float32 this.R / 255.0f, float32 this.G / 255.0f, float32 this.B / 255.0f, 1.0f)

        member this.ToDisplayString() =
            $"%s{this.R.ToDisplayString} %s{this.G.ToDisplayString} %s{this.B.ToDisplayString}"

        static member Default = Rgb(0uy, 0uy, 0uy)
    end
