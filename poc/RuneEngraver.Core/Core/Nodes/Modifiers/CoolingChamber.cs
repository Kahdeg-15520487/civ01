using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Cooling Chamber - Slows decay for cold-related sub-elements (+50% TTL).
/// </summary>
public class CoolingChamber : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;

    private static readonly HashSet<ElementType> ColdElements = new()
    {
        ElementType.Water,
        ElementType.Ice,
        ElementType.Mist,
        ElementType.Metal
    };

    public CoolingChamber(string id) : base(id)
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
            if (ColdElements.Contains(val.Type))
            {
                yield return $"{Id}: Cooling {val} (+50% TTL for cold elements)";
            }
            else
            {
                yield return $"{Id}: Passing {val} (no TTL bonus for non-cold elements)";
            }
        }
    }
}
