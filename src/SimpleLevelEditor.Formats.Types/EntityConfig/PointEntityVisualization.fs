namespace SimpleLevelEditor.Formats.Types.EntityConfig

open System
open SimpleLevelEditor.Formats.Types

type PointEntityVisualization =
    | SimpleSphere    of Color: Rgb          * Radius: float32
    | BillboardSprite of TextureName: String * Size: float32 // TODO: Rename property to TexturePath.
    | Mesh            of MeshName: String    * TextureName: String * Size: float32 // TODO: Rename case to Model. Rename property to ModelPath. Remove TextureName property.

    member this.GetTypeId() : string =
        match this with
        | SimpleSphere _    -> nameof SimpleSphere
        | BillboardSprite _ -> nameof BillboardSprite
        | Mesh _            -> nameof Mesh
