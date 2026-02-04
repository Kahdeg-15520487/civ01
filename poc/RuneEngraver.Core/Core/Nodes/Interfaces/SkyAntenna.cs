using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

public class SkyAntenna : RuneNode
{
    private readonly Port _in;
    // PoC: Just logs if Tempest is received
    
    public SkyAntenna(string id) : base(id)
    {
        _in = AddInput("in");
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (val.Type == ElementType.Tempest)
            yield return $"{Id}: BROADCASTING SIGNAL (Tempest Received)";
        else if (!val.IsEmpty)
            yield return $"{Id}: Input {val} ignored (Requires Tempest)";
    }
}
