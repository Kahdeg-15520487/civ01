using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Tuned Resonator - Calibrates to a specific user's Qi signature for improved efficiency.
/// </summary>
public class TunedResonator : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly double _efficiencyBonus;

    public TunedResonator(string id, double efficiencyBonus = 1.2) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _efficiencyBonus = efficiencyBonus;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            // Apply efficiency bonus to magnitude
            int boostedMag = (int)(val.Magnitude * _efficiencyBonus);
            var output = new QiValue(val.Type, boostedMag);
            _out.CurrentValue = output;
            yield return $"{Id}: Resonating {val} -> {output} (x{_efficiencyBonus} efficiency)";
        }
    }
}
