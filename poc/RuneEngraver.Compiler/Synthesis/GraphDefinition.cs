using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RuneEngraver.Compiler.Synthesis;

public class GraphDefinition
{
    [JsonPropertyName("nodes")]
    public List<NodeInstance> Nodes { get; set; } = new();

    [JsonPropertyName("edges")]
    public List<EdgeInstance> Edges { get; set; } = new();
}

public class NodeInstance
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("params")]
    public Dictionary<string, object> Params { get; set; } = new();
}

public class EdgeInstance
{
    [JsonPropertyName("from")]
    public string From { get; set; } = ""; // "nodeName.portName"

    [JsonPropertyName("to")]
    public string To { get; set; } = "";   // "nodeName.portName"
}
