namespace SimpleLevelEditor.Formats.Types.Level

open System.Numerics
open SimpleLevelEditor.Formats.Types
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

    static member CreateDefault() =
        { Id = 0
          Name = System.String.Empty
          Position = Vector3.Zero
          Shape = ShapeDescriptor.Point
          Properties = [] }

    static member FromData(id: int32, shapeData: string, name: string, position: string, properties: Map<string, string>) : Option<Entity> =
        let shape = ShapeDescriptor.FromShapeData shapeData

        match shape with
        | None -> None
        | Some shape ->
            let position = Vector3.FromDataString(Some position)

            match position with
            | None -> None
            | Some position ->
                let properties: Option<EntityProperty> list =
                    properties
                    |> Map.toList
                    |> List.map (fun (key, value) ->
                        let propertyValue = EntityPropertyValue.FromTypeData value

                        match propertyValue with
                        | None -> None
                        | Some propertyValue -> Some { Key = key; Value = propertyValue })

                if properties |> List.exists Option.isNone then
                    None
                else
                    let properties = properties |> List.choose id

                    Some
                        { Id = id
                          Name = name
                          Position = position
                          Shape = shape
                          Properties = properties }
