using System;
using System.Collections.Generic;
using RuneEngraver.Core.Core.Elements;
using RuneEngraver.Core.Core.Nodes;

namespace RuneEngraver.Core.Core.Simulation;

public class Wire
{
    public Port Source { get; }
    public Port Target { get; }

    public Wire(Port source, Port target)
    {
        // Validation could go here
        Source = source;
        Target = target;
    }

    public void Transfer()
    {
        var val = Source.CurrentValue;
        
        // Apply Decay during transit
        var decayedVal = InteractionManager.ProcessDecay(val);
        
        // Merge with existing value at Target (Additive)
        Target.CurrentValue = InteractionManager.Combine(Target.CurrentValue, decayedVal);
    }
}

public class RuneGraph
{
    public List<RuneNode> Nodes { get; } = new();
    public List<Wire> Wires { get; } = new();
    public int TickCount { get; private set; } = 0;

    public void AddNode(RuneNode node)
    {
        Nodes.Add(node);
    }

    public void Connect(Port source, Port target)
    {
        if (source.IsInput || !target.IsInput)
        {
            throw new ArgumentException("Must connect Output -> Input");
        }
        Wires.Add(new Wire(source, target));
    }

    /// <summary>
    /// Executes one system tick.
    /// Order: Reset Inputs -> Transfer (Decay) -> Process Nodes
    /// </summary>
    public IEnumerable<string> Tick()
    {
        TickCount++;
        // yield return $"--- Tick {TickCount} ---";

        // 1. Reset Inputs (Important so unconnected ports are empty)
        foreach (var node in Nodes)
        {
            node.ResetInputs();
        }

        // 2. Transfer Qi (Wires push Output -> Input)
        // Note: Wires read from Source Output (which holds value from PREVIOUS tick processing)
        foreach (var wire in Wires)
        {
            wire.Transfer();
        }

        // 3. Process Nodes (Calculate new Outputs based on fresh Inputs)
        foreach (var node in Nodes)
        {
            foreach (var log in node.Process())
            {
                yield return log;
            }
        }

        // 4. dissipation check (Handle unrouted outputs)
        foreach (var node in Nodes)
        {
            foreach (var output in node.Outputs)
            {
                if (!output.CurrentValue.IsEmpty && !IsConnected(output))
                {
                    yield return $"[Qi Deviation] UNCONTAINED ENERGY! {output.CurrentValue} at {node.Id}.{output.Id} has no route! Node taking damage.";
                }
            }
        }
    }

    private bool IsConnected(Port output)
    {
        // Naive check; optimization: cache connections
        foreach (var wire in Wires)
        {
            if (wire.Source == output) return true;
        }
        return false;
    }
}
