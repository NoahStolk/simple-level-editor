namespace SimpleLevelEditor.Formats.Types.EntityConfig

type EntityConfigData =
    { ModelPaths: string list
      TexturePaths: string list
      Entities: EntityDescriptor list }

    static member CreateDefault() =
        { ModelPaths = []
          TexturePaths = []
          Entities = [] }
