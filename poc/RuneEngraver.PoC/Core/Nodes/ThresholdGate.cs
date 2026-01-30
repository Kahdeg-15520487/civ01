using System.Collections.Generic;
using RuneEngraver.PoC.Core.Elements;

namespace RuneEngraver.PoC.Core.Nodes;

public class ThresholdGate : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly int _threshold;

    public ThresholdGate(string id, int threshold) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _threshold = threshold;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            if (val.Magnitude >= _threshold)
            {
                _out.CurrentValue = val;
                yield return $"{Id}: {val} PASSED threshold {_threshold}";
            }
            else
            {
                 yield return $"{Id}: {val} BLOCKED (Threshold {_threshold})";
            }
        }
    }
}
