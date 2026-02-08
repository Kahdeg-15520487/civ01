using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Backlash Node - Excess Qi reflects back to user as self-damage.
/// </summary>
public class BacklashNode : RuneNode
{
    private readonly Port _in;
    private readonly int _safeLimit;

    public BacklashNode(string id, int safeLimit = 30) : base(id)
    {
        _in = AddInput("in");
        _safeLimit = safeLimit;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            if (val.Magnitude <= _safeLimit)
            {
                yield return $"{Id}: Absorbing {val} - Minor backlash to user";
            }
            else
            {
                int damage = val.Magnitude - _safeLimit;
                yield return $"{Id}: [BACKLASH] Reflecting {damage} units of {val.Type} to user! Self-damage!";
            }
        }
    }
}
