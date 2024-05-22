namespace SimpleLevelEditor.Formats.Types.Level

type Level3dData =
    { mutable EntityConfigPath: string option
      mutable Models: string list
      mutable WorldObjects: WorldObject list
      mutable Entities: Entity list }

    static member CreateDefault() =
        { EntityConfigPath = None
          Models = []
          WorldObjects = []
          Entities = [] }

    member this.DeepCopy() =
        { this with
            Models = this.Models |> List.map id
            WorldObjects = this.WorldObjects |> List.map (_.DeepCopy())
            Entities = this.Entities |> List.map (_.DeepCopy()) }

    member this.AddMesh(mesh: string) =
        this.Models <- this.Models @ [mesh]

    member this.AddWorldObject(worldObject: WorldObject) =
        this.WorldObjects <- this.WorldObjects @ [worldObject]

    member this.AddEntity(entity: Entity) =
        this.Entities <- this.Entities @ [entity]

    member this.RemoveMesh(mesh: string) =
        this.Models <- this.Models |> List.filter (fun v -> v <> mesh)

    member this.RemoveWorldObject(worldObject: WorldObject) =
        this.WorldObjects <- this.WorldObjects |> List.filter (fun v -> v <> worldObject)

    member this.RemoveEntity(entity: Entity) =
        this.Entities <- this.Entities |> List.filter (fun v -> v <> entity)
