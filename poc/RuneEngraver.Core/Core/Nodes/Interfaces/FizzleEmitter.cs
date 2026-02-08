using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Fizzle Emitter - Momentary pulse interface that dissipates quickly.
/// </summary>
public class FizzleEmitter : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private bool _hasFired = false;

    public FizzleEmitter(string id) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty && !_hasFired)
        {
            // Single pulse output, then fizzle
            _out.CurrentValue = val;
            _hasFired = true;
            yield return $"{Id}: [FIZZLE] Momentary pulse of {val} - dissipating...";
        }
        else if (_hasFired)
        {
            // Already fizzled, absorb input
            yield return $"{Id}: Fizzled out, absorbing {val}";
        }
    }

    public void Reset()
    {
        _hasFired = false;
    }
}
