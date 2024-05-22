namespace SimpleLevelEditor.Formats.Types.EntityConfig

type EntityConfigData =
    { Models: string list
      Textures: string list
      Entities: EntityDescriptor list }

    static member CreateDefault() =
        { Models = []
          Textures = []
          Entities = [] }
