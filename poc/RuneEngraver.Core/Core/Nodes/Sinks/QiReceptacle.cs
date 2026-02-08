using System.Collections.Generic;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Nodes;

/// <summary>
/// Qi Receptacle - Validates output against expected values for puzzle verification.
/// </summary>
public class QiReceptacle : RuneNode
{
    private readonly Port _in;
    private ElementType? _expectedElement;
    private int? _expectedMagnitude;

    public QiReceptacle(string id, ElementType? expectedElement = null, int? expectedMagnitude = null) : base(id)
    {
        _in = AddInput("in");
        _expectedElement = expectedElement;
        _expectedMagnitude = expectedMagnitude;
    }

    public override IEnumerable<string> Process()
    {
        var val = _in.CurrentValue;
        if (!val.IsEmpty)
        {
            bool valid = true;
            
            if (_expectedElement.HasValue && val.Type != _expectedElement.Value)
            {
                valid = false;
                yield return $"{Id}: [VALIDATION FAILED] Expected {_expectedElement}, got {val.Type}";
            }
            
            if (_expectedMagnitude.HasValue && val.Magnitude != _expectedMagnitude.Value)
            {
                valid = false;
                yield return $"{Id}: [VALIDATION FAILED] Expected magnitude {_expectedMagnitude}, got {val.Magnitude}";
            }
            
            if (valid)
            {
                yield return $"{Id}: [VALIDATED] Received {val} - Puzzle condition met!";
            }
        }
    }
}
