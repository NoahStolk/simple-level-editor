[<AutoOpen>]
module SimpleLevelEditor.Formats.Types.DataFormatExtensions

open System
open System.Globalization
open System.Numerics

type Boolean with
    member this.ToDataString =
        this.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()

    static member FromDataString (dataString: string) =
        Boolean.Parse(dataString)

type Byte with
    member this.ToDataString =
        this.ToString(CultureInfo.InvariantCulture)

    static member FromDataString (dataString: string) =
        Byte.Parse(dataString, CultureInfo.InvariantCulture)

type Int32 with
    member this.ToDataString =
        this.ToString(CultureInfo.InvariantCulture)

    static member FromDataString (dataString: string) =
        Int32.Parse(dataString, CultureInfo.InvariantCulture)

type Single with
    member this.ToDataString =
        this.ToString(CultureInfo.InvariantCulture)

    static member FromDataString (dataString: string) =
        Single.Parse(dataString, CultureInfo.InvariantCulture)

type Vector2 with
    member this.ToDataString =
        $"%s{this.X.ToDataString} %s{this.Y.ToDataString}"

    static member FromDataString (dataString: string) =
        let parts = dataString.Split(' ')
        if parts.Length <> 2 then failwith $"Invalid Vector2 data string: {dataString}"
        Vector2(float32 parts[0], float32 parts[1])

type Vector3 with
    member this.ToDataString =
        $"%s{this.X.ToDataString} %s{this.Y.ToDataString} %s{this.Z.ToDataString}"

    static member FromDataString (dataString: string) =
        let parts = dataString.Split(' ')
        if parts.Length <> 3 then failwith $"Invalid Vector3 data string: {dataString}"
        Vector3(float32 parts[0], float32 parts[1], float32 parts[2])

type Vector4 with
    member this.ToDataString =
        $"%s{this.X.ToDataString} %s{this.Y.ToDataString} %s{this.Z.ToDataString} %s{this.W.ToDataString}"

    static member FromDataString (dataString: string) =
        let parts = dataString.Split(' ')
        if parts.Length <> 4 then failwith $"Invalid Vector4 data string: {dataString}"
        Vector4(float32 parts[0], float32 parts[1], float32 parts[2], float32 parts[3])

type Rgb with
    member this.ToDataString =
        $"%s{this.R.ToDataString} %s{this.G.ToDataString} %s{this.B.ToDataString}"

    static member FromDataString (dataString: string) =
        let parts = dataString.Split(' ')
        if parts.Length <> 3 then failwith $"Invalid Rgb data string: {dataString}"
        Rgb(byte parts[0], byte parts[1], byte parts[2])

type Rgba with
    member this.ToDataString =
        $"%s{this.R.ToDataString} %s{this.G.ToDataString} %s{this.B.ToDataString} %s{this.A.ToDataString}"

    static member FromDataString (dataString: string) =
        let parts = dataString.Split(' ')
        if parts.Length <> 4 then failwith $"Invalid Rgba data string: {dataString}"
        Rgba(byte parts[0], byte parts[1], byte parts[2], byte parts[3])
