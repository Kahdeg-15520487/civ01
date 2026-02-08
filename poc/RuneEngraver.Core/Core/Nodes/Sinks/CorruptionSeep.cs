using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Corruption Seep - Disposes Qi by polluting the spiritual environment.
/// </summary>
public class CorruptionSeep : RuneNode
{
    private readonly Port _in;
    private int _totalPollution = 0;

    public CorruptionSeep(string id) : base(id)
    {
        _in = AddInput("in");
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            _totalPollution += val.Magnitude;
            yield return $"{Id}: [POLLUTION] Seeping {val} into environment. Total corruption: {_totalPollution}";
        }
    }
}
