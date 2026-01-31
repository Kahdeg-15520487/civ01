using System.Collections.Generic;
using Civ.Emulator.Core.Elements;

namespace Civ.Emulator.Core.Nodes;

public class YinYangGate : RuneNode
{
    private readonly Port _in;
    private readonly Port _cond;
    private readonly Port _trueOut;
    private readonly Port _falseOut;

    public YinYangGate(string id) : base(id)
    {
        _in = AddInput("in");
        _cond = AddInput("cond");
        _trueOut = AddOutput("true_out");
        _falseOut = AddOutput("false_out");
    }

    public override IEnumerable<string> Process()
    {
        var input = _in.CurrentValue;
        var condition = _cond.CurrentValue;

        if (!input.IsEmpty)
        {
            // Evaluate Condition: Yang (Msg > 0) vs Yin (Msg == 0 or Empty)
            // Even if Empty, IsEmpty returns true. 
            // So if !IsEmpty, Magnitude is likely > 0.
            if (!condition.IsEmpty && condition.Magnitude > 0)
            {
                // Yang Path
                _trueOut.CurrentValue = input;
                yield return $"{Id}: [YANG] {input} routed to True Out (Cond: {condition})";
            }
            else
            {
                // Yin Path
                _falseOut.CurrentValue = input;
                yield return $"{Id}: [YIN] {input} routed to False Out (Cond: {condition})";
            }
        }
    }
}
