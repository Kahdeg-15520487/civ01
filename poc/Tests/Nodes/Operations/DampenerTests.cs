using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Operations;

public class DampenerTests
{
    [Fact]
    public void Process_ValidSuppression_ReducesTarget()
    {
        var node = new DampenerNode("Damp");
        
        // Water (8) is target. Earth (3) suppresses Water.
        // Result = 8 - 3 = 5 Water.
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 8);
        node.Inputs[1].CurrentValue = new QiValue(ElementType.Earth, 3);
        
        node.Process().ToList();
        
        var result = node.Outputs[0].CurrentValue;
        Assert.Equal(ElementType.Water, result.Type);
        Assert.Equal(5, result.Magnitude);
    }

    [Fact]
    public void Process_InvalidCycle_NoEffect()
    {
        var node = new DampenerNode("Damp");
        
        // Fire does not suppress Water (Water suppresses Fire).
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 8);
        node.Inputs[1].CurrentValue = new QiValue(ElementType.Fire, 3);
        
        node.Process().ToList();
        
        Assert.True(node.Outputs[0].CurrentValue.IsEmpty);
    }
}
