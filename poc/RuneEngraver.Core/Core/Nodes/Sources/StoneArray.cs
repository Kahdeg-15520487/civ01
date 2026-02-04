using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

public class StoneArray : RuneNode
{
    private readonly Port _out;
    private readonly List<QiValue> _slots = new();

    public StoneArray(string id, params QiValue[] stones) : base(id)
    {
        _out = AddOutput("out");
        _slots.AddRange(stones);
    }

    public override IEnumerable<string> Process()
    {
        // Simple Logic: Sum all compatible stones. Fail on mix.
        // Assuming user puts correct stones in.
        
        if (_slots.Count == 0) yield break;

        var type = _slots[0].Type;
        int totalMag = 0;
        int maxTTL = 0;

        foreach (var s in _slots)
        {
            if (s.Type != type && !s.IsEmpty)
            {
                 yield return $"{Id}: [Qi Deviation] Stone Type Mismatch in Array!";
                 yield break;
            }
            totalMag += s.Magnitude;
            if (s.TTL > maxTTL) maxTTL = s.TTL;
        }

        if (totalMag > 0)
        {
            var output = new QiValue(type, totalMag, maxTTL);
            _out.CurrentValue = output;
            // Note: In real game, stones would decay (charges used). PoC assumes infinite sources for now.
        }
    }
}
