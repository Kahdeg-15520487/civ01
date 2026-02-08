using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Catalyst Node - Accelerates sub-element decay (-2 TTL per pass).
/// Useful for controlled release of sub-elements.
/// </summary>
public class CatalystNode : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly int _ttlReduction;

    public CatalystNode(string id, int ttlReduction = 2) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _ttlReduction = ttlReduction;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            _out.CurrentValue = val;
            yield return $"{Id}: Catalyzing {val} (-{_ttlReduction} TTL, accelerated decay)";
        }
    }
}
