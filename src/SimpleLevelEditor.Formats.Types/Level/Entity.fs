namespace SimpleLevelEditor.Formats.Types.Level

open System.Numerics
open SimpleLevelEditor.Formats.Types.Level

type Entity =
    { Id: int32 // The id is only used to keep track of the object in the editor.
      mutable Name: string
      mutable Position: Vector3
      mutable Shape: ShapeDescriptor
      mutable Properties: EntityProperty list }

    member this.DeepCopy() =
        { this with
            Shape = this.Shape.DeepCopy()
            Properties = this.Properties |> List.map (_.DeepCopy()) }

    member this.CloneAndPlaceAtPosition(entityId: int32, position: Vector3) =
        { Id = entityId
          Name = this.Name
          Position = position
          Shape = this.Shape.DeepCopy()
          Properties = this.Properties |> List.map (_.DeepCopy()) }

    static member CreateDefault() =
        { Id = 0
          Name = System.String.Empty
          Position = Vector3.Zero
          Shape = ShapeDescriptor.Point
          Properties = [] }
