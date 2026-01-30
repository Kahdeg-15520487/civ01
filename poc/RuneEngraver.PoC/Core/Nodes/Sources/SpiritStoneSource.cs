using System.Collections.Generic;
using RuneEngraver.PoC.Core.Elements;

namespace RuneEngraver.PoC.Core.Nodes;

public class SpiritStoneSource : RuneNode
{
    private readonly QiValue _outputValue;
    private readonly Port _out;

    public SpiritStoneSource(string id, QiValue stoneValue) : base(id)
    {
        _outputValue = stoneValue;
        _out = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        _out.CurrentValue = _outputValue;
        yield break;
    }
}
