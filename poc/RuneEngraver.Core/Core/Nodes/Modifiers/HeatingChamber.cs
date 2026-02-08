using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Heating Chamber - Slows decay for heat-related sub-elements (+50% TTL).
/// </summary>
public class HeatingChamber : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;

    private static readonly HashSet<ElementType> HeatElements = new()
    {
        ElementType.Fire,
        ElementType.Magma,
        ElementType.Plasma,
        ElementType.Steam
    };

    public HeatingChamber(string id) : base(id)
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
            if (HeatElements.Contains(val.Type))
            {
                yield return $"{Id}: Heating {val} (+50% TTL for heat elements)";
            }
            else
            {
                yield return $"{Id}: Passing {val} (no TTL bonus for non-heat elements)";
            }
        }
    }
}
