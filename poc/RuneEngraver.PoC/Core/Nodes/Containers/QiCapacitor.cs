using System.Collections.Generic;
using RuneEngraver.PoC.Core.Elements;

namespace RuneEngraver.PoC.Core.Nodes;

public class QiCapacitor : RuneNode
{
    private readonly Port _in;
    private readonly Port _out;
    private readonly Port _fullSig;
    private QiValue _storedQi = QiValue.Empty;
    private readonly int _threshold;

    public QiCapacitor(string id, int threshold) : base(id)
    {
        _in = AddInput("in");
        _out = AddOutput("out");
        _fullSig = AddOutput("full");
        _threshold = threshold;
    }

    public override IEnumerable<string> Process()
    {
        // Reset outputs
        _out.CurrentValue = QiValue.Empty;
        _fullSig.CurrentValue = QiValue.Empty;
        
        var input = _in.CurrentValue;

        // Charging
        if (!input.IsEmpty)
        {
             if (_storedQi.IsEmpty)
            {
                _storedQi = input;
            }
            else if (_storedQi.Type == input.Type)
            {
                int newMag = _storedQi.Magnitude + input.Magnitude;
                _storedQi = new QiValue(_storedQi.Type, newMag, _storedQi.TTL);
            }
        }

        // Discharge Check
        if (!_storedQi.IsEmpty && _storedQi.Magnitude >= _threshold)
        {
            // Discharge!
            _fullSig.CurrentValue = _storedQi;
            yield return $"{Id}: DISCHARGE! {_storedQi} (Threshold {_threshold} reached)";
            // Reset after discharge? Usually yes for a pulse capacitor.
            _storedQi = QiValue.Empty; 
        }
        else if (!_storedQi.IsEmpty)
        {
            // Status update only, no physical emission
             yield return $"{Id}: Charging... {_storedQi.Magnitude}/{_threshold}";
        }
    }
}
