using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

public class DampenerNode : RuneNode
{
    private readonly Port _target;
    private readonly Port _suppressor;
    private readonly Port _out;

    public DampenerNode(string id) : base(id)
    {
        _target = AddInput("target");
        _suppressor = AddInput("suppressor");
        _out = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        var t = _target.CurrentValue;
        var s = _suppressor.CurrentValue;

        if (t.IsEmpty || s.IsEmpty) yield break;

        // Logic: Suppressor overcomes Target
        bool overcomes = InteractionManager.DoesOvercome(s.Type, t.Type);
        
        if (overcomes && t.Magnitude >= s.Magnitude)
        {
            int newMag = t.Magnitude - s.Magnitude;
            if (newMag > 0)
                _out.CurrentValue = new QiValue(t.Type, newMag, t.TTL);
            else
                 _out.CurrentValue = QiValue.Empty;

            yield return $"{Id}: Dampened {t} by {s} -> {_out.CurrentValue}";
        }
        else
        {
             yield return $"{Id}: Failed Dampen logic (T:{t}, S:{s})";
        }
    }
}
