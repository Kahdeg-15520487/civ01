using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Containers;

public class ElementalPoolTests
{
    [Fact]
    public void Process_AccumulatesUntilCap()
    {
        var node = new ElementalPool("Pool");
        var input = new QiValue(ElementType.Water, 50);
        
        // Tick 1
        node.Inputs[0].CurrentValue = input;
        node.Process().ToList();
        Assert.Equal(50, node.Outputs[0].CurrentValue.Magnitude); // Stored 50, Emit 50
        
        // Tick 2 (Accumulate)
        node.Inputs[0].CurrentValue = input;
        node.Process().ToList();
        Assert.Equal(100, node.Outputs[0].CurrentValue.Magnitude); // Stored 100
        
        // Tick 3 (Cap Check, capacity 100)
        node.Inputs[0].CurrentValue = input;
        node.Process().ToList();
        Assert.Equal(100, node.Outputs[0].CurrentValue.Magnitude); // Capped at 100
    }
}
