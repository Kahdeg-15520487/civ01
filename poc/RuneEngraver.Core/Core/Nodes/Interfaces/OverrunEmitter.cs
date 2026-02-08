using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Overrun Emitter - High capacity emitter with risk of explosion.
/// </summary>
public class OverrunEmitter : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly Port _feedback;
    private readonly int _overrunThreshold;

    public OverrunEmitter(string id, int overrunThreshold = 50) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _feedback = AddOutput("feedback");
        _overrunThreshold = overrunThreshold;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            if (val.Magnitude <= _overrunThreshold)
            {
                _out.CurrentValue = val;
                yield return $"{Id}: Emitting {val} (Overrun Mode - Stable)";
            }
            else
            {
                // Overrun! Emit partial and send feedback
                var emitted = new QiValue(val.Type, _overrunThreshold);
                var overflow = new QiValue(val.Type, val.Magnitude - _overrunThreshold);
                _out.CurrentValue = emitted;
                _feedback.CurrentValue = overflow;
                yield return $"{Id}: [OVERRUN] Emitting {emitted}, overflow {overflow} to feedback!";
            }
        }
    }
}
