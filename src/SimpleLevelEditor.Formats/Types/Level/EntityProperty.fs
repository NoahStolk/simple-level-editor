namespace SimpleLevelEditor.Formats.Types.Level

type EntityProperty =
    { Key: string
      mutable Value: EntityPropertyValue }

    member this.DeepCopy() =
        { this with
            Value = this.Value.DeepCopy() }
