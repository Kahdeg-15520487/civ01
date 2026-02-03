using System.Collections.Generic;
using RuneEngraver.Core.Core.Elements;

namespace RuneEngraver.Core.Core.Nodes;

public class SpiritVessel : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;

    public SpiritVessel(string id) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            _out.CurrentValue = val;
            yield return $"{Id}: Buffered {val}";
        }
    }
}
