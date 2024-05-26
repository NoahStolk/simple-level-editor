[<AutoOpen>]
module SimpleLevelEditor.Formats.Types.DataFormatExtensions

open System
open System.Globalization
open System.Numerics

type Boolean with
    member this.ToDisplayString =
        this.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()

type Byte with
    member this.ToDisplayString =
        this.ToString(CultureInfo.InvariantCulture)

type Int32 with
    member this.ToDisplayString =
        this.ToString(CultureInfo.InvariantCulture)

type Single with
    member this.ToDisplayString =
        this.ToString(CultureInfo.InvariantCulture)

type Vector2 with
    member this.ToDisplayString =
        $"%s{this.X.ToDisplayString} %s{this.Y.ToDisplayString}"

type Vector3 with
    member this.ToDisplayString =
        $"%s{this.X.ToDisplayString} %s{this.Y.ToDisplayString} %s{this.Z.ToDisplayString}"

type Vector4 with
    member this.ToDisplayString =
        $"%s{this.X.ToDisplayString} %s{this.Y.ToDisplayString} %s{this.Z.ToDisplayString} %s{this.W.ToDisplayString}"
