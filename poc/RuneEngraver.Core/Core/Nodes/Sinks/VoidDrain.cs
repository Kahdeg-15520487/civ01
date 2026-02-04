using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

public class VoidDrain : RuneNode
{
    private readonly Port _in;
    private readonly int _bandwidthLimit;

    public VoidDrain(string id, int bandwidthLimit = 10) : base(id)
    {
        _in = AddInput("in");
        _bandwidthLimit = bandwidthLimit;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            if (val.Magnitude <= _bandwidthLimit)
            {
                yield return $"{Id}: Dissipating {val} into Void (Safe)";
            }
            else
            {
                yield return $"{Id}: [FAILURE] Void Overflow! Input {val.Magnitude} > Limit {_bandwidthLimit}";
            }
        }
    }
}
