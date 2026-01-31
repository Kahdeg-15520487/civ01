using System.Collections.Generic;
using Civ.Emulator.Core.Elements;

namespace Civ.Emulator.Core.Nodes;

public class ElementFilter : RuneNode
{
    private readonly Port _in;
    private readonly Port _matchOut;
    private readonly Port _otherOut;
    private readonly ElementType _targetType;

    public ElementFilter(string id, ElementType targetType) : base(id)
    {
        _in = AddInput("in");
        _matchOut = AddOutput("match");
        _otherOut = AddOutput("other");
        _targetType = targetType;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            if (val.Type == _targetType)
            {
                _matchOut.CurrentValue = val;
                yield return $"{Id}: Matched {_targetType} -> {val}";
            }
            else
            {
                _otherOut.CurrentValue = val;
                yield return $"{Id}: Rejected {val} (Expected {_targetType}) -> Other";
            }
        }
    }
}
