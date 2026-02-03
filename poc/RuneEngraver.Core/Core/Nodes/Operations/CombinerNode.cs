using System.Collections.Generic;
using RuneEngraver.Core.Core.Elements;

namespace RuneEngraver.Core.Core.Nodes;

public class CombinerNode : RuneNode
{
    private readonly Port _in1;
    private readonly Port _in2;
    private readonly Port _out;

    public CombinerNode(string id) : base(id)
    {
        _in1 = AddInput("in1");
        _in2 = AddInput("in2");
        _out = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        var result = InteractionManager.Combine(_in1.CurrentValue, _in2.CurrentValue);
        _out.CurrentValue = result;
        
        if (!result.IsEmpty)
            yield return $"{Id}: Combined {_in1.CurrentValue} + {_in2.CurrentValue} -> {result}";
        else if (!_in1.CurrentValue.IsEmpty && !_in2.CurrentValue.IsEmpty)
             yield return $"{Id}: Combination Failed [Qi Deviation]";
    }
}
