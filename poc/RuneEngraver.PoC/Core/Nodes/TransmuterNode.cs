using System.Collections.Generic;
using RuneEngraver.PoC.Core.Elements;

namespace RuneEngraver.PoC.Core.Nodes;

public class TransmuterNode : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly ElementType _targetType;

    public TransmuterNode(string id, ElementType targetType) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _targetType = targetType;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            _out.CurrentValue = new QiValue(_targetType, val.Magnitude, val.TTL);
            yield return $"{Id}: Transmuted {val} -> {_out.CurrentValue}";
        }
    }
}
