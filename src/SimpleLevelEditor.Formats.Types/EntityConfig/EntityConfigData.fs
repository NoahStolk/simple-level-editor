namespace SimpleLevelEditor.Formats.Types.EntityConfig

type EntityConfigData =
    { Meshes: string list
      Textures: string list
      Entities: EntityDescriptor list }

    static member CreateDefault() =
        { Meshes = []
          Textures = []
          Entities = [] }
