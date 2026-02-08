using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Unstable Vent - Quick Qi disposal but damages surroundings.
/// </summary>
public class UnstableVent : RuneNode
{
    private readonly Port _in;
    private readonly int _bandwidthLimit;

    public UnstableVent(string id, int bandwidthLimit = 50) : base(id)
    {
        _in = AddInput("in");
        _bandwidthLimit = bandwidthLimit;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            if (val.Magnitude <= _bandwidthLimit)
            {
                yield return $"{Id}: [SIDE EFFECT] Venting {val} - Damaging surroundings!";
            }
            else
            {
                yield return $"{Id}: [EXPLOSION] Vent overflow! Input {val.Magnitude} > Limit {_bandwidthLimit}. Catastrophic damage!";
            }
        }
    }
}
