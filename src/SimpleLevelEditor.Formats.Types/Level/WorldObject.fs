namespace SimpleLevelEditor.Formats.Types.Level

open System
open System.Numerics

type WorldObject =
    { Id: int32 // The id is only used to keep track of the object in the editor.
      mutable Mesh: string // TODO: Rename to ModelPath.
      mutable Texture: string // TODO: Remove.
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
