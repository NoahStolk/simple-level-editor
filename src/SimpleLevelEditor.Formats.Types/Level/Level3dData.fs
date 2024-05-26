namespace SimpleLevelEditor.Formats.Types.Level

type Level3dData =
    { mutable EntityConfigPath: string option
      mutable ModelPaths: string list
      mutable WorldObjects: WorldObject list
      mutable Entities: Entity list }

    static member CreateDefault() =
        { EntityConfigPath = None
          ModelPaths = []
          WorldObjects = []
          Entities = [] }

    member this.DeepCopy() =
        { this with
            ModelPaths = this.ModelPaths |> List.map id
            WorldObjects = this.WorldObjects |> List.map (_.DeepCopy())
            Entities = this.Entities |> List.map (_.DeepCopy()) }

    member this.AddModel(model: string) =
        if not (this.ModelPaths |> List.exists (fun v -> v = model)) then
            this.ModelPaths <- this.ModelPaths @ [model]

    member this.AddWorldObject(worldObject: WorldObject) =
        this.WorldObjects <- this.WorldObjects @ [worldObject]

    member this.AddEntity(entity: Entity) =
        this.Entities <- this.Entities @ [entity]

    member this.RemoveModel(model: string) =
        this.ModelPaths <- this.ModelPaths |> List.filter (fun v -> v <> model)

    member this.RemoveWorldObject(worldObject: WorldObject) =
        this.WorldObjects <- this.WorldObjects |> List.filter (fun v -> v <> worldObject)

    member this.RemoveEntity(entity: Entity) =
        this.Entities <- this.Entities |> List.filter (fun v -> v <> entity)
