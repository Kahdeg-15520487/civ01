using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Simulation;

// Re-defining the DTOs here to avoid dependency on Compiler
// Ideally move to a Shared contract assembly, but for PoC this is fine.
public class GraphData
{
    [JsonPropertyName("nodes")]
    public List<NodeData> Nodes { get; set; } = new();

    [JsonPropertyName("edges")]
    public List<EdgeData> Edges { get; set; } = new();
}

public class NodeData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("params")]
    public Dictionary<string, JsonElement> Params { get; set; } = new();
}

public class EdgeData
{
    [JsonPropertyName("from")]
    public string From { get; set; } = "";

    [JsonPropertyName("to")]
    public string To { get; set; } = "";
}

public class GraphLoader
{
    public RuneGraph Load(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var data = JsonSerializer.Deserialize<GraphData>(json, options);
        if (data == null) throw new InvalidOperationException("Failed to load graph JSON");

        var graph = new RuneGraph();
        var nodeMap = new Dictionary<string, RuneNode>();

        foreach (var nodeData in data.Nodes)
        {
            // Skip Interface Nodes for the internal graph, 
            // OR create specific Source/Sink implementations?
            // "FormationInput" acts like a Source.
            // "FormationOutput" acts like a Sink/Emitter.
            
            var node = CreateNode(nodeData);
            if (node != null)
            {
                graph.AddNode(node);
                nodeMap[node.Id] = node;
            }
        }

        foreach (var edge in data.Edges)
        {
            Connect(graph, nodeMap, edge.From, edge.To);
        }

        return graph;
    }

    private RuneNode CreateNode(NodeData data)
    {
        return data.Type switch
        {
            "SpiritStoneSocket" => new SpiritStoneSource(data.Id, ParseQiValue(data.Params)),
            "Amplifier" => new AmplifierNode(data.Id),
            "QiCapacitor" => new QiCapacitor(data.Id, GetInt(data.Params, "capacity")),
            "Combiner" => new CombinerNode(data.Id),
            "Splitter" => new SplitterNode(data.Id),
            "FormationInput" => new SpiritStoneSource(data.Id, ParseQiValue(data.Params)), // Treat inputs as Sources
            "FormationOutput" => new StableEmitter(data.Id), // Treat outputs as Sinks
            "BurstTrigger" => new BurstTrigger(data.Id),
            "Transmuter" => new TransmuterNode(data.Id), // Needs param fix later
            "EffectEmitter" => new EffectEmitter(data.Id), 
            _ => throw new NotSupportedException($"Unknown node type: {data.Type}")
        };
    }

    private void Connect(RuneGraph graph, Dictionary<string, RuneNode> nodes, string from, string to)
    {
        // format: "nodeId.portId" OR "nodeId" (for implicits)
        var (fromNodeId, fromPort) = SplitId(from);
        var (toNodeId, toPort) = SplitId(to);

        // Handle Interface Nodes implicit ports
        // FormationInput ("ignition") has "out" implicitly
        // FormationOutput ("blast") has "in" implicitly
        
        // But if the IDs are just "ignition", SplitId returns ("ignition", null).
        // We need to resolve the ports.
        
        if (!nodes.TryGetValue(fromNodeId, out var srcNode)) throw new Exception($"Source node {fromNodeId} not found");
        if (!nodes.TryGetValue(toNodeId, out var dstNode)) throw new Exception($"Target node {toNodeId} not found");

        var srcPort = FindPort(srcNode, fromPort, isInput: false);
        var dstPort = FindPort(dstNode, toPort, isInput: true);

        if (srcPort == null) throw new Exception($"Output port {fromPort ?? "(default)"} not found on {fromNodeId}");
        if (dstPort == null) throw new Exception($"Input port {toPort ?? "(default)"} not found on {toNodeId}");
        
        graph.Connect(srcPort, dstPort);
    }

    private Port? FindPort(RuneNode node, string? portName, bool isInput)
    {
        var list = isInput ? node.Inputs : node.Outputs;
        if (portName == null)
        {
            // Default port logic
            // Simple heuristic: first port? Or "out"/"in"?
            return list.FirstOrDefault(p => p.Id == (isInput ? "in" : "out")) ?? list.FirstOrDefault();
        }
        return list.FirstOrDefault(p => p.Id == portName);
    }

    private (string, string?) SplitId(string id)
    {
        var parts = id.Split('.');
        if (parts.Length == 1) return (parts[0], null);
        return (parts[0], parts[1]);
    }

    private QiValue ParseQiValue(Dictionary<string, JsonElement> p)
    {
        var elemStr = p.ContainsKey("element") ? p["element"].ToString() : "Fire";
        if (!Enum.TryParse<ElementType>(elemStr, out var type)) type = ElementType.Fire;

        int mag = 1;
        if (p.TryGetValue("grade", out var gradeVal))
        {
             var g = gradeVal.ToString();
             if (g == "Medium") mag = 4;      // Spec: 4-7
             else if (g == "High") mag = 8;   // Spec: 8-12
             else if (g == "Supreme" || g == "Peak") mag = 16; // Spec: 16
        }
        // Also check "amplitude" if available (very basic parsing)
        if (p.TryGetValue("amplitude", out var ampVal))
        {
             // Try to find a number in the string "Value = X"
             var s = ampVal.ToString();
             var idx = s.IndexOf("Value = ");
             if (idx >= 0)
             {
                 var numStr = s.Substring(idx + 8).Split(' ', '}')[0];
                 int.TryParse(numStr, out mag);
             }
        }

        return new QiValue(type, mag); 
    }

    private int GetInt(Dictionary<string, JsonElement> p, string key, int def = 0)
    {
        if (p.TryGetValue(key, out var val))
        {
             if (val.ValueKind == JsonValueKind.Number) return val.GetInt32();
             if (int.TryParse(val.ToString(), out int res)) return res;
        }
        return def;
    }
}
