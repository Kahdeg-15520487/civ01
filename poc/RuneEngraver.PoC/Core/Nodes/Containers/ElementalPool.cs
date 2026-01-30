using System.Collections.Generic;
using RuneEngraver.PoC.Core.Elements;

namespace RuneEngraver.PoC.Core.Nodes;

public class ElementalPool : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private QiValue _storedQi = QiValue.Empty;
    private const int CAPACITY = 100;

    public ElementalPool(string id) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        var input = _in.CurrentValue;
        
        // Add Input to Storage
        if (!input.IsEmpty)
        {
            if (_storedQi.IsEmpty)
            {
                _storedQi = input;
            }
            else if (_storedQi.Type == input.Type)
            {
                // Additive
                int newMag = _storedQi.Magnitude + input.Magnitude;
                if (newMag > CAPACITY) newMag = CAPACITY; // Cap
                _storedQi = new QiValue(_storedQi.Type, newMag, _storedQi.TTL);
            }
            else
            {
                yield return $"{Id}: [Qi Deviation] Pool pollution! {_storedQi.Type} vs {input.Type}";
                // In game, this might explode. PoC: Just fail addition.
            }
        }

        // Emit stored
        if (!_storedQi.IsEmpty)
        {
            _out.CurrentValue = _storedQi;
            yield return $"{Id}: Pool Level {_storedQi}";
        }
    }
}
