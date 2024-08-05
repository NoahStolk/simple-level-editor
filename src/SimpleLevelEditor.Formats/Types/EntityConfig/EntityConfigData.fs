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
