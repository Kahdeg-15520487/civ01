using System.Collections.Generic;
using RuneEngraver.Core.Core.Elements;

namespace RuneEngraver.Core.Core.Nodes;

public class StableEmitter : RuneNode
{
    private readonly Port _in;
    
    public QiValue LastReceived { get; private set; }

    public StableEmitter(string id) : base(id)
    {
        _in = AddInput("in");
    }

    public override IEnumerable<string> Process()
    {
        LastReceived = _in.CurrentValue;
        if (!LastReceived.IsEmpty)
            yield return $"{Id}: EMIT {LastReceived}";
    }
}
