using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Operations;

public class CombinerTests
{
    [Fact]
    public void Process_EqualInputs_Combines()
    {
        var node = new CombinerNode("Com");
        
        // Water + Fire = Steam
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 5);
        node.Inputs[1].CurrentValue = new QiValue(ElementType.Fire, 5);
        
        node.Process().ToList();
        
        var result = node.Outputs[0].CurrentValue;
        Assert.Equal(ElementType.Steam, result.Type);
        Assert.Equal(10, result.Magnitude);
    }

    [Fact]
    public void Process_UnequalInputs_Fails()
    {
        var node = new CombinerNode("Com");
        
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 5);
        node.Inputs[1].CurrentValue = new QiValue(ElementType.Fire, 4); // Unequal
        
        var logs = node.Process().ToList();
        
        Assert.True(node.Outputs[0].CurrentValue.IsEmpty);
        Assert.Contains(logs, l => l.Contains("[Qi Deviation]"));
    }
}
