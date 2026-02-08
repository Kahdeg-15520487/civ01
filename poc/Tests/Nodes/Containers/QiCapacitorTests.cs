using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Containers;

public class QiCapacitorTests
{
    [Fact]
    public void Process_ChargesAndBursts()
    {
        var node = new QiCapacitor("Cap", 10);
        var input = new QiValue(ElementType.Fire, 4);
        
        // Tick 1: 4/10
        node.Inputs[0].CurrentValue = input;
        node.Process().ToList();
        Assert.Equal(4, node.Outputs[0].CurrentValue.Magnitude); // Trickle
        Assert.True(node.Outputs[1].CurrentValue.IsEmpty); // Burst Empty
        
        // Tick 2: 8/10
        node.Inputs[0].CurrentValue = input;
        node.Process().ToList();
        Assert.Equal(8, node.Outputs[0].CurrentValue.Magnitude);
        Assert.True(node.Outputs[1].CurrentValue.IsEmpty);
        
        // Tick 3: 12/10 -> Burst
        node.Inputs[0].CurrentValue = input;
        node.Process().ToList();
        
        // Should Burst 12
        var burst = node.Outputs[1].CurrentValue;
        Assert.Equal(12, burst.Magnitude);
        Assert.Equal(ElementType.Fire, burst.Type);
        
        // Next tick should be empty (Reset)
        // If we don't feed it input
        node.Inputs[0].CurrentValue = QiValue.Empty;
        node.Process().ToList();
        Assert.True(node.Outputs[1].CurrentValue.IsEmpty);
        // And stored should be 0? 
        // Logic says storedQi reset to Empty.
    }
}
