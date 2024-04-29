namespace SimpleLevelEditor.Formats.Types.EntityConfig

open System
open SimpleLevelEditor.Formats.Types
open SimpleLevelEditor.Formats.Types.Level

type EntityShape =
    | Point of Visualization: PointEntityVisualization
    | Sphere of Color: Rgb
    | Aabb of Color: Rgb

    member this.GetDefaultDescriptor() =
        match this with
        | Point _  -> ShapeDescriptor.Point
        | Sphere _ -> ShapeDescriptor.Sphere 2f
        | Aabb _   -> ShapeDescriptor.Aabb (-System.Numerics.Vector3.One, System.Numerics.Vector3.One)

    member this.GetTypeId() =
        match this with
        | Point _  -> ShapeIds.PointId
        | Sphere _ -> ShapeIds.SphereId
        | Aabb _   -> ShapeIds.AabbId

    static member FromShapeText(shapeText: string) =
        let indexOfFirstDelimiter = shapeText.IndexOf(';', StringComparison.Ordinal)
        if indexOfFirstDelimiter = -1 then failwithf $"Invalid shape data: %s{shapeText}"

        let shapeId = shapeText[..(indexOfFirstDelimiter - 1)]
        let shapeData = shapeText[(indexOfFirstDelimiter + 1)..]

        match shapeId with
        | ShapeIds.PointId  -> EntityShape.ParsePointData shapeData
        | ShapeIds.SphereId -> Sphere(Rgb.FromDataString shapeData)
        | ShapeIds.AabbId   -> Aabb(Rgb.FromDataString shapeData)
        | _                 -> failwithf $"Unknown entity shape type: %s{shapeId}"

    static member private ParsePointData(data: string) : EntityShape =
        let indexOfFirstDelimiter = data.IndexOf(';')
        if indexOfFirstDelimiter = -1 then failwithf $"Invalid point data: %s{data}"
        let visualizationId = data[..(indexOfFirstDelimiter - 1)]
        let visualizationData = data[(indexOfFirstDelimiter + 1)..]

        match visualizationId with
        | PointEntityVisualizationIds.SimpleSphereId -> EntityShape.Point(EntityShape.ParsePointSimpleSphereData(visualizationData))
        | PointEntityVisualizationIds.BillboardSpriteId -> EntityShape.Point(EntityShape.ParsePointBillboardSpriteData(visualizationData))
        | PointEntityVisualizationIds.MeshId -> EntityShape.Point(EntityShape.ParsePointMeshData(visualizationData))
        | _ -> failwithf $"Unknown point visualization type: %s{visualizationId}"

    static member private ParsePointSimpleSphereData(data: string) : PointEntityVisualization =
        let parts = data.Split(';')
        if parts.Length <> 2 then failwithf $"Invalid point simple sphere data: %s{data}"
        let rgb = Rgb.FromDataString parts[0]
        let radius = Single.FromDataString(parts[1])
        PointEntityVisualization.SimpleSphere (rgb, radius)

    static member private ParsePointBillboardSpriteData(data: string) : PointEntityVisualization =
        let parts = data.Split(';')
        if parts.Length <> 2 then failwithf $"Invalid point billboard sprite data: %s{data}"
        let textureId = parts[0]
        let size = Single.FromDataString(parts[1])
        PointEntityVisualization.BillboardSprite (textureId, size)

    static member private ParsePointMeshData(data: string) : PointEntityVisualization =
        let parts = data.Split(';')
        if parts.Length <> 3 then failwithf $"Invalid point mesh data: %s{data}"
        let meshName = parts[0]
        let textureName = parts[1]
        let size = Single.FromDataString(parts[2])
        PointEntityVisualization.Mesh (meshName, textureName, size)
