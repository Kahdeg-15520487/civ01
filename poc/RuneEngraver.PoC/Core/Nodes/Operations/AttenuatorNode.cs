using System.Collections.Generic;
using RuneEngraver.PoC.Core.Elements;

namespace RuneEngraver.PoC.Core.Nodes;

public class AttenuatorNode : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly Port _excess;
    private readonly float _factor; // 0.0 to 1.0

    public AttenuatorNode(string id, float factor) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _excess = AddOutput("excess");
        
        // Clamp factor to valid physical range [0, 1]
        _factor = factor < 0 ? 0 : (factor > 1 ? 1 : factor);
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            int outMag = (int)(val.Magnitude * _factor);
            int excessMag = val.Magnitude - outMag;

            // Primary Output (Attenuated Signal)
            if (outMag > 0)
            {
                _out.CurrentValue = new QiValue(val.Type, outMag, val.TTL);
            }

            // Waste/Excess Output (Conservation of Qi)
            if (excessMag > 0)
            {
                _excess.CurrentValue = new QiValue(val.Type, excessMag, val.TTL);
            }

            yield return $"{Id}: Attenuated {val} by factor {_factor:F2} -> Out:{_out.CurrentValue}, Excess:{_excess.CurrentValue}";
        }
    }
}
