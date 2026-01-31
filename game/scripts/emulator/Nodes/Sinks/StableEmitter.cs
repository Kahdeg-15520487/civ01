using System.Collections.Generic;
using Civ.Emulator.Core.Elements;

namespace Civ.Emulator.Core.Nodes;

public class StableEmitter : RuneNode
{
    private readonly Port _in;
    
    public QiValue LastReceived { get; private set; }
    public List<QiValue> History { get; } = new();

    public StableEmitter(string id) : base(id)
    {
        _in = AddInput("in");
    }

    public override IEnumerable<string> Process()
    {
        LastReceived = _in.CurrentValue;
        if (!LastReceived.IsEmpty)
        {
            History.Add(LastReceived);
            yield return $"{Id}: EMIT {LastReceived}";
        }
    }
}
