namespace SimpleLevelEditor.Formats.Types.EntityConfig

open SimpleLevelEditor.Formats.Types.Level

type EntityShape =
    | Point
    | Sphere
    | Aabb

    member this.GetDefaultDescriptor() =
        match this with
        | Point  -> ShapeDescriptor.Point
        | Sphere -> ShapeDescriptor.Sphere 2f
        | Aabb   -> ShapeDescriptor.Aabb (-System.Numerics.Vector3.One, System.Numerics.Vector3.One)
