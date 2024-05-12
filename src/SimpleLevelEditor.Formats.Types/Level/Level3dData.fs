namespace SimpleLevelEditor.Formats.Types.Level

type Level3dData =
    { mutable EntityConfigPath: string option
      mutable Meshes: string list
      mutable Textures: string list
      mutable WorldObjects: WorldObject list
      mutable Entities: Entity list }

    static member CreateDefault() =
        { EntityConfigPath = None
          Meshes = []
          Textures = []
          WorldObjects = []
          Entities = [] }

    member this.DeepCopy() =
        { this with
            Meshes = this.Meshes |> List.map id
            Textures = this.Textures |> List.map id
            WorldObjects = this.WorldObjects |> List.map (_.DeepCopy())
            Entities = this.Entities |> List.map (_.DeepCopy()) }

    member this.AddMesh(mesh: string) =
        this.Meshes <- this.Meshes @ [mesh]

    member this.AddTexture(texture: string) =
        this.Textures <- this.Textures @ [texture]

    member this.AddWorldObject(worldObject: WorldObject) =
        this.WorldObjects <- this.WorldObjects @ [worldObject]

    member this.AddEntity(entity: Entity) =
        this.Entities <- this.Entities @ [entity]

    member this.RemoveMesh(mesh: string) =
        this.Meshes <- this.Meshes |> List.filter (fun v -> v <> mesh)

    member this.RemoveTexture(texture: string) =
        this.Textures <- this.Textures |> List.filter (fun v -> v <> texture)

    member this.RemoveWorldObject(worldObject: WorldObject) =
        this.WorldObjects <- this.WorldObjects |> List.filter (fun v -> v <> worldObject)

    member this.RemoveEntity(entity: Entity) =
        this.Entities <- this.Entities |> List.filter (fun v -> v <> entity)
