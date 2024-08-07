namespace SimpleLevelEditor.Formats.Types.EntityConfig

open SimpleLevelEditor.Formats.Types

type PointEntityVisualization =
    | SimpleSphere    of Color: Rgb          * Radius: float32
    | BillboardSprite of TexturePath: string * Size: float32
    | Model           of ModelPath: string   * Size: float32

    member this.GetTypeId() : string =
        match this with
        | SimpleSphere _    -> nameof SimpleSphere
        | BillboardSprite _ -> nameof BillboardSprite
        | Model _           -> nameof Model

    member this.DeepCopy() : PointEntityVisualization =
        match this with
        | SimpleSphere (color, radius)        -> SimpleSphere (color, radius)
        | BillboardSprite (texturePath, size) -> BillboardSprite (texturePath, size)
        | Model (modelPath, size)             -> Model (modelPath, size)
