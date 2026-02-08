using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Amplitude Regulator - Normalizes variable amplitude input to stable output.
/// Essential for multi-user artifacts.
/// </summary>
public class AmplitudeRegulator : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly int _targetAmplitude;
    private readonly int _maxInput;

    public AmplitudeRegulator(string id, int targetAmplitude = 10, int maxInput = 81) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _targetAmplitude = targetAmplitude;
        _maxInput = maxInput;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            if (val.Magnitude > _maxInput)
            {
                yield return $"{Id}: [OVERLOAD] Input {val.Magnitude} exceeds max {_maxInput}!";
            }
            else
            {
                // Normalize to target amplitude
                var output = new QiValue(val.Type, _targetAmplitude);
                _out.CurrentValue = output;
                yield return $"{Id}: Regulating {val} -> {output} (normalized)";
            }
        }
    }
}
