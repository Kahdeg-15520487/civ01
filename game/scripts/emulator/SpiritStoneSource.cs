using System.Collections.Generic;

namespace Civ.Emulator;

public class SpiritStoneSource : RuneNode
{
    private readonly Port _output;
    private int _remainingCycles = 100;
    private int _amplitude = 5;

    public SpiritStoneSource(string id) : base(id)
    {
        _output = AddOutput("out");
    }

    public override IEnumerable<string> Process()
    {
        if (_remainingCycles > 0)
        {
            _remainingCycles--;
            _output.CurrentValue = new QiValue(ElementType.Fire, _amplitude);
            yield return $"Emitted Fire({_amplitude}). Remaining: {_remainingCycles}";
        }
        else
        {
            _output.CurrentValue = QiValue.Empty;
        }
    }
}
