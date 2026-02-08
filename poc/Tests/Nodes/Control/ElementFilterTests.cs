using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Control;

public class ElementFilterTests
{
    [Fact]
    public void Process_MatchesType_RoutesToMatch()
    {
        var node = new ElementFilter("Filter", ElementType.Fire);
        var input = new QiValue(ElementType.Fire, 10);
        node.Inputs[0].CurrentValue = input;
        
        node.Process().ToList();
        
        Assert.Equal(input, node.Outputs[0].CurrentValue); // Match
        Assert.True(node.Outputs[1].CurrentValue.IsEmpty); // Other
    }

    [Fact]
    public void Process_MismatchType_RoutesToOther()
    {
        var node = new ElementFilter("Filter", ElementType.Fire);
        var input = new QiValue(ElementType.Water, 10);
        node.Inputs[0].CurrentValue = input;
        
        node.Process().ToList();
        
        Assert.True(node.Outputs[0].CurrentValue.IsEmpty); // Match
        Assert.Equal(input, node.Outputs[1].CurrentValue); // Other
    }
}
