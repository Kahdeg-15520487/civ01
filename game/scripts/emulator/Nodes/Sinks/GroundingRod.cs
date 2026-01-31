using System.Collections.Generic;
using Civ.Emulator.Core.Elements;

namespace Civ.Emulator.Core.Nodes;

public class GroundingRod : RuneNode
{
    private readonly Port _in;

    public GroundingRod(string id) : base(id)
    {
        _in = AddInput("in");
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            if (val.Type == ElementType.Lightning || val.Type == ElementType.Tempest)
            {
                yield return $"{Id}: Grounding {val} into earth safely";
            }
            else
            {
                yield return $"{Id}: [FAILURE] Incompatible Element {val.Type}! Grounding Rod requires Lightning/Tempest.";
            }
        }
    }
}
