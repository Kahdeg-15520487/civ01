using System.Collections.Generic;
using RuneEngraver.PoC.Core.Elements;

namespace RuneEngraver.PoC.Core.Nodes;

public class Port
{
    public string Id { get; }
    public bool IsInput { get; }
    public QiValue CurrentValue { get; set; } = QiValue.Empty;

    public Port(string id, bool isInput)
    {
        Id = id;
        IsInput = isInput;
    }
}

public abstract class RuneNode
{
    public string Id { get; }
    public List<Port> Inputs { get; } = new();
    public List<Port> Outputs { get; } = new();

    protected RuneNode(string id)
    {
        Id = id;
    }

    protected Port AddInput(string name)
    {
        var p = new Port(name, true);
        Inputs.Add(p);
        return p;
    }

    protected Port AddOutput(string name)
    {
        var p = new Port(name, false);
        Outputs.Add(p);
        return p;
    }

    /// <summary>
    /// Reads inputs and sets outputs.
    /// Returns any side-effects (logs, errors) as strings.
    /// </summary>
    public abstract IEnumerable<string> Process();

    /// <summary>
    /// Clears inputs for the next tick (outputs persist until transferred).
    /// </summary>
    public void ResetInputs()
    {
        foreach (var p in Inputs) p.CurrentValue = QiValue.Empty;
    }
}
