using Godot;
using System.Collections.Generic;
using System.Linq;
using Civ.Emulator.Core.Nodes;
using Civ.Emulator.Core.Simulation;
using Civ.Emulator.Core.Nodes.Sources;
using Civ.Emulator.Core.Elements;

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
        // Simple parsing or default params for now. 
        // In a real scenario, we'd pass a 'params' dictionary too.
        
        switch (type)
        {
            // Sources
            case "SpiritStoneSource": 
                // Defaulting to Fire(5) if not specified, 
                // or we could assume the netlist provides params.
                return new SpiritStoneSource(id);
            case "CultivatorLink":
                return new CultivatorLink(id, new QiValue(ElementType.None, 0)); // Placeholder

            // Operations
            case "AmplifierNode": return new AmplifierNode(id);
            case "CombinerNode": return new CombinerNode(id);
            case "AttenuatorNode": return new AttenuatorNode(id, 0.5f); // Todo: Param
            case "TransmuterNode": return new TransmuterNode(id);
            case "SplitterNode": return new SplitterNode(id);
            case "DampenerNode": return new DampenerNode(id);

            // Containers
            case "SpiritVessel": return new SpiritVessel(id);
            case "QiCapacitor": return new QiCapacitor(id, 10); // Todo: Param
            case "DualVessel": return new DualVessel(id);
            case "ElementalPool": return new ElementalPool(id);

            // Control
            case "ThresholdGate": return new ThresholdGate(id, 5); // Todo: Param
            case "YinYangGate": return new YinYangGate(id);
            case "ElementFilter": return new ElementFilter(id, ElementType.Fire); // Todo: Param

            // Sinks
            case "StableEmitter": return new StableEmitter(id);
            case "VoidDrain": return new VoidDrain(id);
            case "EffectEmitter": return new EffectEmitter(id);
            case "GroundingRod": return new GroundingRod(id);
            case "HeatSink": return new HeatSink(id);
            case "SkyAntenna": return new SkyAntenna(id);

            default:
                GD.PrintErr($"Unknown Node Type: {type}");
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
