using System.Collections.Generic;
using RuneEngraver.Core.Core.Elements;

namespace RuneEngraver.Core.Core.Nodes;

public class HeatSink : RuneNode
{
    private readonly Port _in;

    public HeatSink(string id) : base(id)
    {
        _in = AddInput("in");
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            if (val.Type == ElementType.Fire)
            {
                yield return $"{Id}: Radiating {val} as harmless heat";
            }
            else
            {
                yield return $"{Id}: [FAILURE] Incompatible Element {val.Type}! Heat Sink requires Fire.";
            }
        }
    }
}
