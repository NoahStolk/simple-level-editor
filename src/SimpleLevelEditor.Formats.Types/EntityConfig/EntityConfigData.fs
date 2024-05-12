namespace SimpleLevelEditor.Formats.Types.EntityConfig

type EntityConfigData =
    { Version: int32
      Entities: EntityDescriptor list }

    static member CreateDefault() =
        { Version = 1
          Entities = [] }
