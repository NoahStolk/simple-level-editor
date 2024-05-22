namespace SimpleLevelEditor.Formats.Types.EntityConfig

open System
open SimpleLevelEditor.Formats.Types

type PointEntityVisualization =
    | SimpleSphere    of Color: Rgb          * Radius: float32
    | BillboardSprite of TexturePath: String * Size: float32
    | Model           of ModelPath: String   * Size: float32

    member this.GetTypeId() : string =
        match this with
        | SimpleSphere _    -> nameof SimpleSphere
        | BillboardSprite _ -> nameof BillboardSprite
        | Model _           -> nameof Model
