namespace SimpleLevelEditor.Formats.Types.EntityConfig

type EntityDescriptor =
    { Name: string
      Shape: EntityShapeDescriptor
      Properties: EntityPropertyDescriptor list }

    member this.DeepCopy() =
        { this with
            Name = this.Name
            Shape = this.Shape.DeepCopy()
            Properties = this.Properties |> List.map (_.DeepCopy()) }
