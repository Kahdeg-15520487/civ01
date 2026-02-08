using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Stabilizer Node - Reinforces sub-element bonds, extending TTL.
/// </summary>
public class StabilizerNode : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly int _ttlBonus;

    public StabilizerNode(string id, int ttlBonus = 2) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _ttlBonus = ttlBonus;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            // Pass through with extended TTL (TTL handled at QiValue level if implemented)
            _out.CurrentValue = val;
            yield return $"{Id}: Stabilizing {val} (+{_ttlBonus} TTL)";
        }
    }
}
