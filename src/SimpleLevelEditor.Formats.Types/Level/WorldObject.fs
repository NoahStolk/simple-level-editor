namespace SimpleLevelEditor.Formats.Types.Level

open System
open SimpleLevelEditor.Formats.Types
open System.Numerics

type WorldObject =
    { Id: int32 // The id is only used to keep track of the object in the editor.
      mutable Mesh: string
      mutable Texture: string
      mutable Scale: Vector3
      mutable Rotation: Vector3
      mutable Position: Vector3
      mutable Flags: string list }

    member this.DeepCopy() =
        { this with
            Flags = this.Flags |> List.map id }

    member this.CloneAndPlaceAtPosition(entityId: int32, position: Vector3) =
        { Id = entityId
          Mesh = this.Mesh
          Texture = this.Texture
          Scale = this.Scale
          Rotation = this.Rotation
          Position = position
          Flags = this.Flags |> List.map id }

    // TODO: Test this member.
    member this.ChangeFlagAtIndex(flagIndex: int32, flag: string) =
        if flagIndex >= 0 && flagIndex < this.Flags.Length then
            this.Flags <- this.Flags |> List.mapi (fun i v -> if i = flagIndex then flag else v)

    // TODO: Test this member.
    member this.AddFlag(flag: string) =
        this.Flags <- this.Flags @ [flag]

    // TODO: Test this member.
    member this.RemoveFlagAtIndex(flagIndex: int32) =
        if flagIndex >= 0 && flagIndex < this.Flags.Length then
            this.Flags <- this.Flags |> List.mapi (fun i v -> if i = flagIndex then null else v) |> List.filter (fun v -> v <> null)

    static member CreateDefault() =
        { Id = 0
          Mesh = String.Empty
          Texture = String.Empty
          Scale = Vector3.One
          Rotation = Vector3.Zero
          Position = Vector3.Zero
          Flags = [] }

    static member FromData(worldObjectId: int32, mesh: string, texture: string, scale: string option, rotation: string option, position: string option, flags: string) : Option<WorldObject> =
        let scale = Vector3.FromDataString(scale)

        match scale with
        | None -> None
        | Some scale ->
            let rotation = Vector3.FromDataString(rotation)

            match rotation with
            | None -> None
            | Some rotation ->
                let position = Vector3.FromDataString(position)

                match position with
                | None -> None
                | Some position ->
                    let flags = flags.Split(',', StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries) |> List.ofArray

                    Some
                        { Id = worldObjectId
                          Mesh = mesh
                          Texture = texture
                          Scale = scale
                          Rotation = rotation
                          Position = position
                          Flags = flags }
