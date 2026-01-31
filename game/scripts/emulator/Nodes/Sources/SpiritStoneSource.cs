using System.Collections.Generic;
using Civ.Emulator.Core.Elements;

namespace Civ.Emulator.Core.Nodes.Sources;

public class SpiritStoneSource : RuneNode
{
    private readonly Port _output;
    private int _remainingCycles = 100;
    private QiValue _emissionValue;

    public SpiritStoneSource(string id, QiValue emissionValue) : base(id)
    {
        _output = AddOutput("out");
        _emissionValue = emissionValue;
    }
    
    // Default for old tests if needed, or remove? 
    // The errors suggest tests use the 2-arg constructor.
    // Let's overload or simpler: just change the existing one?
    // Old tests used 1-arg. I should keep 1-arg as default or update tests.
    // Actually, line 56, 57 etc call 2-args. The old tests (TestSpiritStoneSimulation) call 1-arg.
    // So I need both constructors.

    public SpiritStoneSource(string id) : this(id, new QiValue(ElementType.Fire, 5)) {}

    public override IEnumerable<string> Process()
    {
        if (_remainingCycles > 0)
        {
            _remainingCycles--;
            _output.CurrentValue = _emissionValue;
            yield return $"Emitted {_emissionValue}. Remaining: {_remainingCycles}";
        }
        else
        {
            _output.CurrentValue = QiValue.Empty;
        }
    }
}
