using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

public class CultivatorLink : RuneNode
{
    private readonly Port _out;
    public QiValue CurrentInput { get; set; }

    public CultivatorLink(string id, QiValue initialInput) : base(id)
    {
        _out = AddOutput("out");
        CurrentInput = initialInput;
    }

    public override IEnumerable<string> Process()
    {
        if (!CurrentInput.IsEmpty)
        {
            _out.CurrentValue = CurrentInput;
            yield return $"{Id}: Cultivator channeling {CurrentInput}";
        }
    }
}
