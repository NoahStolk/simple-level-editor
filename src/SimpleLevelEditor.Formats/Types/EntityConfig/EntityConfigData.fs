namespace SimpleLevelEditor.Formats.Types.EntityConfig

type EntityConfigData =
    { mutable ModelPaths: string list
      mutable TexturePaths: string list
      mutable Entities: EntityDescriptor list }

    static member CreateDefault() =
        { ModelPaths = []
          TexturePaths = []
          Entities = [] }

    member this.DeepCopy() =
        { this with
            ModelPaths = this.ModelPaths |> List.map id
            TexturePaths = this.TexturePaths |> List.map id
            Entities = this.Entities |> List.map (_.DeepCopy()) }

    member this.AddModel(model: string) =
        if not (this.ModelPaths |> List.exists (fun v -> v = model)) then
            this.ModelPaths <- this.ModelPaths @ [ model ]

    member this.AddTexture(texture: string) =
        if not (this.TexturePaths |> List.exists (fun v -> v = texture)) then
            this.TexturePaths <- this.TexturePaths @ [ texture ]

    member this.AddEntity(entity: EntityDescriptor) =
        this.Entities <- this.Entities @ [ entity ] // TODO: Check for duplicates

    member this.RemoveModel(model: string) =
        this.ModelPaths <- this.ModelPaths |> List.filter (fun v -> v <> model)

    member this.RemoveTexture(texture: string) =
        this.TexturePaths <- this.TexturePaths |> List.filter (fun v -> v <> texture)

    member this.RemoveEntity(entity: EntityDescriptor) =
        this.Entities <- this.Entities |> List.filter (fun v -> v <> entity)
