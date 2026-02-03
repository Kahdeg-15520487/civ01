using System.Collections.Generic;
using RuneEngraver.Core.Core.Elements;

namespace RuneEngraver.Core.Core.Nodes;

public class DualVessel : RuneNode
{
    private readonly Port _in1;
    private readonly Port _in2;
    private readonly Port _out1;
    private readonly Port _out2;

    public DualVessel(string id) : base(id)
    {
        _in1 = AddInput("in1");
        _in2 = AddInput("in2");
        _out1 = AddOutput("out1");
        _out2 = AddOutput("out2");
    }

    public override IEnumerable<string> Process()
    {
        var v1 = _in1.CurrentValue;
        var v2 = _in2.CurrentValue;

        if (!v1.IsEmpty)
        {
            _out1.CurrentValue = v1;
            yield return $"{Id}: Channel 1 Buffered {v1}";
        }
        if (!v2.IsEmpty)
        {
            _out2.CurrentValue = v2;
            yield return $"{Id}: Channel 2 Buffered {v2}";
        }
    }
}
