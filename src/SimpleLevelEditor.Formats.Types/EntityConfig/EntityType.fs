namespace SimpleLevelEditor.Formats.Types.EntityConfig

open System
open System.Globalization
open SimpleLevelEditor.Formats.Types
open SimpleLevelEditor.Formats.Types.Level

type EntityShape =
    | Point of PointEntityVisualization
    | Sphere of Rgb
    | Aabb of Rgb

    member this.GetDefaultDescriptor() =
        match this with
        | Point _  -> ShapeDescriptor.Point
        | Sphere _ -> ShapeDescriptor.Sphere 2f
        | Aabb _   -> ShapeDescriptor.Aabb (-System.Numerics.Vector3.One, System.Numerics.Vector3.One)

    static member FromIdData(id: string, data: string) =
        match id with
        | ShapeIds.PointId  -> EntityShape.ParsePointData data
        | ShapeIds.SphereId -> Sphere(Rgb.FromString data)
        | ShapeIds.AabbId   -> Aabb(Rgb.FromString data)
        | _                 -> failwithf $"Unknown entity shape type: %s{id}"

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
        let rgb = Rgb.FromString parts[0]
        let radius = Single.Parse(parts[1], CultureInfo.InvariantCulture)
        PointEntityVisualization.SimpleSphere (rgb, radius)

    static member private ParsePointBillboardSpriteData(data: string) : PointEntityVisualization =
        let parts = data.Split(';')
        if parts.Length <> 2 then failwithf $"Invalid point billboard sprite data: %s{data}"
        let textureId = parts[0]
        let size = Single.Parse(parts[1], CultureInfo.InvariantCulture)
        PointEntityVisualization.BillboardSprite (textureId, size)

    static member private ParsePointMeshData(data: string) : PointEntityVisualization =
        let parts = data.Split(';')
        if parts.Length <> 3 then failwithf $"Invalid point mesh data: %s{data}"
        let meshName = parts[0]
        let textureName = parts[1]
        let size = Single.Parse(parts[2], CultureInfo.InvariantCulture)
        PointEntityVisualization.Mesh (meshName, textureName, size)
