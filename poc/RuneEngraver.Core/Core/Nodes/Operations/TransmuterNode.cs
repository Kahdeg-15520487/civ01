using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

public class TransmuterNode : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    public TransmuterNode(string id) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            var nextType = InteractionManager.GetGenerationChild(val.Type);
            if (nextType != ElementType.None)
            {
                _out.CurrentValue = new QiValue(nextType, val.Magnitude, val.TTL);
                yield return $"{Id}: Transmuted {val} -> {_out.CurrentValue}";
            }
            else
            {
                yield return $"{Id}: Component {val.Type} cannot be transmuted (No Generation Child)";
            }
        }
    }
}
