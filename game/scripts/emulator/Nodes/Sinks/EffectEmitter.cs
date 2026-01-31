using System.Collections.Generic;
using Civ.Emulator.Core.Elements;

namespace Civ.Emulator.Core.Nodes;

public class EffectEmitter : RuneNode
{
    private readonly Port _in;

    public EffectEmitter(string id) : base(id)
    {
        _in = AddInput("in");
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            string effect = "Unknown Effect";
            switch (val.Type)
            {
                case ElementType.Fire: effect = "Thermal Projection (Burn)"; break;
                case ElementType.Water: effect = "Hydro Barrier (Shield)"; break;
                case ElementType.Wood: effect = "Regeneration Aura (Heal)"; break;
                case ElementType.Metal: effect = "Kinetic Blade (Cut)"; break;
                case ElementType.Earth: effect = "Mass Solidification (Block)"; break;
                case ElementType.Lightning: effect = "Arc Discharge (Shock)"; break;
                default: effect = $"Raw {val.Type} Release"; break;
            }
            yield return $"{Id}: ACTIVATED > {effect} [Power: {val.Magnitude}]";
        }
    }
}
