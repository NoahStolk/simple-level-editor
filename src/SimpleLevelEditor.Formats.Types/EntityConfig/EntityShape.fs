namespace SimpleLevelEditor.Formats.Types.EntityConfig

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
        | Aabb _   -> ShapeDescriptor.Aabb System.Numerics.Vector3.One

    member this.GetTypeId() =
        match this with
        | Point _  -> nameof Point
        | Sphere _ -> nameof Sphere
        | Aabb _   -> nameof Aabb
