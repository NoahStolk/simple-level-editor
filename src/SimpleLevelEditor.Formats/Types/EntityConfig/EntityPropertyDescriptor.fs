namespace SimpleLevelEditor.Formats.Types.EntityConfig

type EntityPropertyDescriptor =
    { Name: string
      Type: EntityPropertyTypeDescriptor
      Description: string option }

    member this.DeepCopy() =
        { this with
            Name = this.Name
            Type = this.Type.DeepCopy()
            Description = this.Description }
