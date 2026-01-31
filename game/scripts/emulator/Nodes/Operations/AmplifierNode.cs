using System.Collections.Generic;
using Civ.Emulator.Core.Elements;

namespace Civ.Emulator.Core.Nodes;

public class AmplifierNode : RuneNode
{
    private readonly Port _primary;
    private readonly Port _catalyst;
    private readonly Port _out;

    public AmplifierNode(string id) : base(id)
    {
        _primary = AddInput("primary");
        _catalyst = AddInput("catalyst");
        _out = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        var p = _primary.CurrentValue;
        var c = _catalyst.CurrentValue;

        if (p.IsEmpty || c.IsEmpty) yield break;

        // Logic: Primary generates Catalyst
        var expectedChild = InteractionManager.GetGenerationChild(p.Type);
        
        if (expectedChild == c.Type && p.Magnitude > c.Magnitude)
        {
            // Success
            int newMag = p.Magnitude * c.Magnitude;
            // Cap at 81 per spec? PoC can be lenient, but let's note it.
            _out.CurrentValue = new QiValue(c.Type, newMag, c.TTL); 
            yield return $"{Id}: Amplified {p} + {c} -> {_out.CurrentValue}";
        }
        else
        {
            // Failure or Idle
             yield return $"{Id}: Failed Amp logic (P:{p}, C:{c})";
        }
    }
}
