module SimpleLevelEditor.Formats.SimpleLevelEditorJsonSerializer

open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open SimpleLevelEditor.Formats.Types.EntityConfig
open SimpleLevelEditor.Formats.Types.Level

let defaultSerializerOptions = JsonFSharpOptions.Default().ToJsonSerializerOptions()
defaultSerializerOptions.WriteIndented <- true;
defaultSerializerOptions.IncludeFields <- true

let DeserializeEntityConfigFromString(json: string) : Option<EntityConfigData> =
    try
        Some(JsonSerializer.Deserialize<EntityConfigData>(json, defaultSerializerOptions))
    with
        | :? JsonException -> None

let DeserializeEntityConfigFromStream(stream: Stream) : Option<EntityConfigData> =
    try
        Some(JsonSerializer.Deserialize<EntityConfigData>(stream, defaultSerializerOptions))
    with
        | :? JsonException -> None

let SerializeEntityConfigToString(entityConfig: EntityConfigData) : string =
    JsonSerializer.Serialize(entityConfig, defaultSerializerOptions)

let SerializeEntityConfigToStream(stream: Stream, entityConfig: EntityConfigData) : unit =
    JsonSerializer.Serialize(stream, entityConfig, defaultSerializerOptions)

let DeserializeLevelFromString(json: string) : Option<Level3dData> =
    try
        Some(JsonSerializer.Deserialize<Level3dData>(json, defaultSerializerOptions))
    with
        | :? JsonException -> None

let DeserializeLevelFromStream(stream: Stream) : Option<Level3dData> =
    try
        Some(JsonSerializer.Deserialize<Level3dData>(stream, defaultSerializerOptions))
    with
        | :? JsonException -> None

let SerializeLevelToString(level: Level3dData) : string =
    JsonSerializer.Serialize(level, defaultSerializerOptions)

let SerializeLevelToStream(stream: Stream, level: Level3dData) : unit =
    JsonSerializer.Serialize(stream, level, defaultSerializerOptions)
