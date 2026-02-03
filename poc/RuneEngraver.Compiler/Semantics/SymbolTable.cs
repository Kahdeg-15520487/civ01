using System.Collections.Generic;
using RuneEngraver.Compiler.Syntax;

namespace RuneEngraver.Compiler.Semantics;

public class SymbolTable
{
    private readonly Dictionary<string, FormationDefinition> _formations = new();
    
    // Core built-in formations (mocked for now)
    public SymbolTable()
    {
        RegisterBuiltins();
    }

    public void Define(string qualifiedName, FormationDefinition formation)
    {
        _formations[qualifiedName] = formation;
    }

    public bool TryLookup(string qualifiedName, out FormationDefinition formation)
    {
        return _formations.TryGetValue(qualifiedName, out formation);
    }

    public bool IsDefined(string qualifiedName)
    {
        return _formations.ContainsKey(qualifiedName);
    }

    public bool HasPackage(string packagePrefix)
    {
        // Simple prefix check for now
        foreach (var key in _formations.Keys)
        {
            if (key.StartsWith(packagePrefix + ".")) return true;
        }
        return false;
    }

    private void RegisterBuiltins()
    {
        // Source Nodes
        _formations["core.SpiritStoneSocket"] = CreateMockNode("SpiritStoneSocket", new[] { "out" });
        _formations["core.StoneArray"] = CreateMockNode("StoneArray", new[] { "out" });
        _formations["core.CultivatorLink"] = CreateMockNode("CultivatorLink", new[] { "out" });
        _formations["core.TunedResonator"] = CreateMockNode("TunedResonator", new[] { "in", "out" });
        _formations["core.AmplitudeRegulator"] = CreateMockNode("AmplitudeRegulator", new[] { "in", "out" });
        
        // Capacitor Systems
        _formations["core.QiCapacitor"] = CreateMockNode("QiCapacitor", new[] { "in", "out", "full" });
        _formations["core.BurstTrigger"] = CreateMockNode("BurstTrigger", new[] { "capacitor", "trigger", "out" });

        // Sink Nodes
        _formations["core.QiReceptacle"] = CreateMockNode("QiReceptacle", new[] { "in" });
        _formations["core.EffectEmitter"] = CreateMockNode("EffectEmitter", new[] { "in" });
        _formations["core.VoidDrain"] = CreateMockNode("VoidDrain", new[] { "in" });
        _formations["core.HeatSink"] = CreateMockNode("HeatSink", new[] { "in" });
        _formations["core.GroundingRod"] = CreateMockNode("GroundingRod", new[] { "in" });
        _formations["core.UnstableVent"] = CreateMockNode("UnstableVent", new[] { "in" });
        _formations["core.BacklashNode"] = CreateMockNode("BacklashNode", new[] { "in" });
        _formations["core.CorruptionSeep"] = CreateMockNode("CorruptionSeep", new[] { "in" });
        
        // Container Nodes
        _formations["core.SpiritVessel"] = CreateMockNode("SpiritVessel", new[] { "in", "out" });
        _formations["core.DualVessel"] = CreateMockNode("DualVessel", new[] { "in1", "in2", "out" });
        _formations["core.ElementalPool"] = CreateMockNode("ElementalPool", new[] { "in", "out" });

        // Operation Nodes
        _formations["core.Transmuter"] = CreateMockNode("Transmuter", new[] { "in", "out" });
        _formations["core.Amplifier"] = CreateMockNode("Amplifier", new[] { "primary", "catalyst", "out" });
        _formations["core.Dampener"] = CreateMockNode("Dampener", new[] { "target", "suppressor", "out" });
        _formations["core.Attenuator"] = CreateMockNode("Attenuator", new[] { "in", "out", "excess" });
        _formations["core.Splitter"] = CreateMockNode("Splitter", new[] { "in", "out1", "out2" });
        _formations["core.Combiner"] = CreateMockNode("Combiner", new[] { "in1", "in2", "out" });

        // Control Nodes
        _formations["core.YinYangGate"] = CreateMockNode("YinYangGate", new[] { "in", "cond", "true_out", "false_out" });
        _formations["core.ThresholdGate"] = CreateMockNode("ThresholdGate", new[] { "in", "pass", "block" });
        _formations["core.ElementFilter"] = CreateMockNode("ElementFilter", new[] { "in", "match", "other" });
    }

    private FormationDefinition CreateMockFormation(string name, string portDir, string elemType)
    {
        var stmt = new PortDefinition(
            portDir == "input" ? PortDirection.Input : PortDirection.Output,
            elemType,
            "main_port",
            null
        );
        return new FormationDefinition(name, new List<Statement> { stmt });
    }

    private FormationDefinition CreateMockNode(string name, string[] ports)
    {
        var stmts = new List<Statement>();
        foreach (var p in ports)
        {
            stmts.Add(new PortDefinition(
                // Simple heuristic: 'in' is input, else output for mocks
                p.Contains("in") || p == "capacitor" || p == "trigger" ? PortDirection.Input : PortDirection.Output,
                "Any", // Core nodes often accept Any
                p,
                null
            ));
        }
        return new FormationDefinition(name, stmts);
    }
}
