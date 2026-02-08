using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Operations;

public class TransmuterTests
{
    [Fact]
    public void Process_ValidTransmutation_ConvertsType()
    {
        var node = new TransmuterNode("Trans");
        
        // Wood -> Fire
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Wood, 10);
        
        node.Process().ToList();
        
        var result = node.Outputs[0].CurrentValue;
        Assert.Equal(ElementType.Fire, result.Type);
        Assert.Equal(10, result.Magnitude);
    }

    [Fact]
    public void Process_NoChild_NoOutput()
    {
        var node = new TransmuterNode("Trans");
        
        // None -> None (or invalid type behavior)
        // Testing a type that might not have a generation child if any?
        // Currently all 5 elements have a child.
        // Let's test Steam (Sub-element). Does Steam generate anything?
        // InteractionManager.GetGenerationChild(Steam) -> likely None.
        
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Steam, 10);
        
        node.Process().ToList();
        
        Assert.True(node.Outputs[0].CurrentValue.IsEmpty);
    }
}
