using System.Collections.Generic;
using RuneEngraver.Core.Core.Elements;

namespace RuneEngraver.Core.Core.Nodes;

public class SplitterNode : RuneNode
{
    private readonly Port _in;
    private readonly Port _out1;
    private readonly Port _out2;

    public SplitterNode(string id) : base(id)
    {
        _in = AddInput("in");
        _out1 = AddOutput("out1");
        _out2 = AddOutput("out2");
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            int splitMag = val.Magnitude / 2;
            if (splitMag > 0)
            {
                var splitVal = new QiValue(val.Type, splitMag, val.TTL);
                _out1.CurrentValue = splitVal;
                _out2.CurrentValue = splitVal;
                yield return $"{Id}: Split {val} -> {splitVal} & {splitVal}";
            }
            else
            {
                yield return $"{Id}: Input {val} too small to split";
            }
        }
    }
}
