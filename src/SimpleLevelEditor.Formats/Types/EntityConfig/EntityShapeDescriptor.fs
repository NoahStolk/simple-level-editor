namespace SimpleLevelEditor.Formats.Types.EntityConfig

open SimpleLevelEditor.Formats.Types
open SimpleLevelEditor.Formats.Types.Level

type EntityShapeDescriptor =
    | Point of Visualization: PointEntityVisualization
    | Sphere of Color: Rgb
    | Aabb of Color: Rgb

    member this.GetDefaultEntityShape() =
        match this with
        | Point _  -> EntityShape.Point
        | Sphere _ -> EntityShape.Sphere 2f
        | Aabb _   -> EntityShape.Aabb System.Numerics.Vector3.One

    member this.GetTypeId() =
        match this with
        | Point _  -> nameof Point
        | Sphere _ -> nameof Sphere
        | Aabb _   -> nameof Aabb

    member this.DeepCopy() =
        match this with
        | Point visualization -> Point (visualization.DeepCopy())
        | Sphere color        -> Sphere color
        | Aabb color          -> Aabb color
