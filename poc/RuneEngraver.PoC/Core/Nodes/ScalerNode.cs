using System.Collections.Generic;
using RuneEngraver.PoC.Core.Elements;

namespace RuneEngraver.PoC.Core.Nodes;

public class ScalerNode : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly float _factor;

    public ScalerNode(string id, float factor) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _factor = factor;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            int newMag = (int)(val.Magnitude * _factor);
            if (newMag > 0)
            {
                _out.CurrentValue = new QiValue(val.Type, newMag, val.TTL);
                yield return $"{Id}: Scaled {val} x {_factor} -> {_out.CurrentValue}";
            }
        }
    }
}
