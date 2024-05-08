[<AutoOpen>]
module SimpleLevelEditor.Formats.Types.DataFormatExtensions

open System
open System.Globalization
open System.Numerics

type Boolean with
    member this.ToDataString =
        this.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()

    static member FromDataString (dataString: Option<string>) : Option<bool> =
        match dataString with
        | None -> Option.None
        | Some dataString ->
            match bool.TryParse dataString with
            | true, value -> Option.Some value
            | false, _ -> Option.None

type Byte with
    member this.ToDataString =
        this.ToString(CultureInfo.InvariantCulture)

    static member FromDataString (dataString: Option<string>) : Option<byte> =
        match dataString with
        | None -> Option.None
        | Some dataString ->
            match Byte.TryParse(dataString, CultureInfo.InvariantCulture) with
            | true, value -> Option.Some value
            | false, _ -> Option.None

type Int32 with
    member this.ToDataString =
        this.ToString(CultureInfo.InvariantCulture)

    static member FromDataString (dataString: Option<string>) : Option<int> =
        match dataString with
        | None -> Option.None
        | Some dataString ->
            match Int32.TryParse(dataString, CultureInfo.InvariantCulture) with
            | true, value -> Option.Some value
            | false, _ -> Option.None

type Single with
    member this.ToDataString =
        this.ToString(CultureInfo.InvariantCulture)

    static member FromDataString (dataString: Option<string>) : Option<float32> =
        match dataString with
        | None -> Option.None
        | Some dataString ->
            match Single.TryParse(dataString, CultureInfo.InvariantCulture) with
            | true, value -> Option.Some value
            | false, _ -> Option.None

type Vector2 with
    member this.ToDataString =
        $"%s{this.X.ToDataString} %s{this.Y.ToDataString}"

    static member FromDataString (dataString: Option<string>) : Option<Vector2> =
        match dataString with
        | None -> Option.None
        | Some dataString ->
            let parts = dataString.Split(' ')
            match parts.Length with
            | 2 ->
                match Single.TryParse(parts[0], CultureInfo.InvariantCulture), Single.TryParse(parts[1], CultureInfo.InvariantCulture) with
                | (true, x), (true, y) -> Option.Some (Vector2(x, y))
                | _ -> Option.None
            | _ -> Option.None

type Vector3 with
    member this.ToDataString =
        $"%s{this.X.ToDataString} %s{this.Y.ToDataString} %s{this.Z.ToDataString}"

    static member FromDataString (dataString: Option<string>) : Option<Vector3> =
        match dataString with
        | None -> Option.None
        | Some dataString ->
            let parts = dataString.Split(' ')
            match parts.Length with
            | 3 ->
                match Single.TryParse(parts[0], CultureInfo.InvariantCulture), Single.TryParse(parts[1], CultureInfo.InvariantCulture), Single.TryParse(parts[2], CultureInfo.InvariantCulture) with
                | (true, x), (true, y), (true, z) -> Option.Some (Vector3(x, y, z))
                | _ -> Option.None
            | _ -> Option.None

type Vector4 with
    member this.ToDataString =
        $"%s{this.X.ToDataString} %s{this.Y.ToDataString} %s{this.Z.ToDataString} %s{this.W.ToDataString}"

    static member FromDataString (dataString: Option<string>) : Option<Vector4> =
        match dataString with
        | None -> Option.None
        | Some dataString ->
            let parts = dataString.Split(' ')
            match parts.Length with
            | 4 ->
                match Single.TryParse(parts[0], CultureInfo.InvariantCulture), Single.TryParse(parts[1], CultureInfo.InvariantCulture), Single.TryParse(parts[2], CultureInfo.InvariantCulture), Single.TryParse(parts[3], CultureInfo.InvariantCulture) with
                | (true, x), (true, y), (true, z), (true, w) -> Option.Some (Vector4(x, y, z, w))
                | _ -> Option.None
            | _ -> Option.None

type Rgb with
    member this.ToDataString =
        $"%s{this.R.ToDataString} %s{this.G.ToDataString} %s{this.B.ToDataString}"

    static member FromDataString (dataString: Option<string>) : Option<Rgb> =
        match dataString with
        | None -> Option.None
        | Some dataString ->
            let parts = dataString.Split(' ')
            match parts.Length with
            | 3 ->
                match Byte.TryParse(parts[0], CultureInfo.InvariantCulture), Byte.TryParse(parts[1], CultureInfo.InvariantCulture), Byte.TryParse(parts[2], CultureInfo.InvariantCulture) with
                | (true, r), (true, g), (true, b) -> Option.Some (Rgb(r, g, b))
                | _ -> Option.None
            | _ -> Option.None

type Rgba with
    member this.ToDataString =
        $"%s{this.R.ToDataString} %s{this.G.ToDataString} %s{this.B.ToDataString} %s{this.A.ToDataString}"

    static member FromDataString (dataString: Option<string>) : Option<Rgba> =
        match dataString with
        | None -> Option.None
        | Some dataString ->
            let parts = dataString.Split(' ')
            match parts.Length with
            | 4 ->
                match Byte.TryParse(parts[0], CultureInfo.InvariantCulture), Byte.TryParse(parts[1], CultureInfo.InvariantCulture), Byte.TryParse(parts[2], CultureInfo.InvariantCulture), Byte.TryParse(parts[3], CultureInfo.InvariantCulture) with
                | (true, r), (true, g), (true, b), (true, a) -> Option.Some (Rgba(r, g, b, a))
                | _ -> Option.None
            | _ -> Option.None
