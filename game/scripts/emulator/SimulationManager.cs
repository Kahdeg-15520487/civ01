using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Civ.Emulator;

/// <summary>
/// The Bridge between Godot (Visuals) and the C# Simulation Core.
/// </summary>
[GlobalClass]
public partial class SimulationManager : Node
{
    private RuneGraph _graph;

    private bool _isRunning = false;

    [Signal]
    public delegate void SimulationTickEventHandler(int tickCount);

    [Signal]
    public delegate void LogMessageEventHandler(string message);

    public override void _Ready()
    {
        GD.Print("SimulationManager (C#) Initialized.");
    }

    /// <summary>
    /// Rebuilds the graph from the provided dictionary representation.
    /// Expected format:
    /// {
    ///    "nodes": [ { "id": "MainSource", "type": "SpiritStoneSource", "pos": Vector2i } ],
    ///    "traces": [ { "from_node": "id", "from_port": "out", "to_node": "id", "to_port": "in" } ]
    /// }
    /// </summary>
    public void BuildGraph(Godot.Collections.Dictionary boardData)
    {
        _graph = new RuneGraph();
        GD.Print("Building Simulation Graph...");

        // 1. Create Nodes
        var nodes = boardData["nodes"].AsGodotArray<Godot.Collections.Dictionary>();
        var nodeMap = new Dictionary<string, RuneNode>();

        foreach (var nodeData in nodes)
        {
            string id = nodeData["id"].AsString();
            string type = nodeData["type"].AsString();
            
            RuneNode newNode = CreateNodeFactory(type, id);
            if (newNode != null)
            {
                _graph.AddNode(newNode);
                nodeMap[id] = newNode;
                GD.Print($"  - Created Node: {id} ({type})");
            }
            else
            {
                GD.PrintErr($"  - Unknown Node Type: {type}");
            }
        }

        // 2. Create Connections (Traces)
        var traces = boardData["traces"].AsGodotArray<Godot.Collections.Dictionary>();
        foreach (var traceData in traces)
        {
            string fromId = traceData["from_node"].AsString();
            string fromPort = traceData["from_port"].AsString();
            string toId = traceData["to_node"].AsString();
            string toPort = traceData["to_port"].AsString();

            if (nodeMap.ContainsKey(fromId) && nodeMap.ContainsKey(toId))
            {
                var sourceNode = nodeMap[fromId];
                var targetNode = nodeMap[toId];

                var sourcePort = sourceNode.Outputs.FirstOrDefault(p => p.Id == fromPort);
                var targetPort = sourceNode.Inputs.FirstOrDefault(p => p.Id == toPort); 
                // Wait, targetPort should come from targetNode! FIXING logic below.
                
                var realTargetPort = targetNode?.Inputs.FirstOrDefault(p => p.Id == toPort);

                if (sourcePort != null && realTargetPort != null)
                {
                    _graph.Connect(sourcePort, realTargetPort);
                    GD.Print($"  - Connected {fromId}.{fromPort} -> {toId}.{toPort}");
                }
                else
                {
                    GD.PrintErr($"  - Failed to connect {fromId} -> {toId}: Port not found.");
                }
            }
        }
        
        GD.Print("Graph Built Successfully.");
    }

    private RuneNode CreateNodeFactory(string type, string id)
    {
        switch (type)
        {
            case "SpiritStoneSource":
                 return new SpiritStoneSource(id);
             // TODO: Add other types here
            default:
                return null;
        }
    }

    public void RunTick()
    {
        if (_graph == null) return;

        foreach (var log in _graph.Tick())
        {
            EmitSignal(SignalName.LogMessage, log);
            GD.Print($"[Sim] {log}");
        }
        
        EmitSignal(SignalName.SimulationTick, _graph.TickCount);
    }
}
