using System.Linq;
using Xunit;
using RuneEngraver.Core.Core.Elements;
using RuneEngraver.Core.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Control;

public class ThresholdGateTests
{
    [Fact]
    public void Process_AboveThreshold_Passes()
    {
        var node = new ThresholdGate("Gate", 5);
        // Input 10 > 5
        var input = new QiValue(ElementType.Water, 10);
        node.Inputs[0].CurrentValue = input;
        
        node.Process().ToList();
        
        Assert.Equal(input, node.Outputs[0].CurrentValue); // Pass port
        Assert.True(node.Outputs[1].CurrentValue.IsEmpty); // Block port
    }

    [Fact]
    public void Process_BelowThreshold_Blocks()
    {
        var node = new ThresholdGate("Gate", 5);
        // Input 3 < 5
        var input = new QiValue(ElementType.Water, 3);
        node.Inputs[0].CurrentValue = input;
        
        node.Process().ToList();
        
        Assert.True(node.Outputs[0].CurrentValue.IsEmpty); // Pass port
        Assert.Equal(input, node.Outputs[1].CurrentValue); // Block port
    }
}
