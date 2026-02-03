using System.Collections.Generic;
using RuneEngraver.Core.Core.Elements;

namespace RuneEngraver.Core.Core.Nodes;

public class EffectEmitter : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;

    public EffectEmitter(string id) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
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

            // Create QiEffect with the tag
            var qiEffect = new QiEffect(val.Type, val.Magnitude, effect, val.TTL);
            _out.CurrentValue = qiEffect;

            yield return $"{Id}: ACTIVATED > {effect} [Power: {val.Magnitude}]";
        }
        else
        {
             _out.CurrentValue = QiValue.Empty;
        }
    }
}
