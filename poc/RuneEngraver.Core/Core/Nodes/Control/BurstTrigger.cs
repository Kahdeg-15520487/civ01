using System.Collections.Generic;
using RuneEngraver.Core.Core.Elements;

namespace RuneEngraver.Core.Core.Nodes;

public class BurstTrigger : RuneNode
{
    private readonly Port _trigger;
    private readonly Port _capacitor;
    private readonly Port _out;

    public BurstTrigger(string id) : base(id)
    {
        _trigger = AddInput("trigger");
        _capacitor = AddInput("capacitor");
        _out = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        var trig = _trigger.CurrentValue;
        var cap = _capacitor.CurrentValue;
        
        // If trigger is active (non-empty), pass capacitor input to output
        if (!trig.IsEmpty)
        {
            if (!cap.IsEmpty)
            {
                _out.CurrentValue = cap;
                yield return $"{Id}: TRIGGERED! Bursting {cap}";
            }
            else
            {
                // Triggered but no load
                yield return $"{Id}: Triggered (Empty Load)";
            }
        }
        else
        {
            // Gate closed
        }
    }
}
