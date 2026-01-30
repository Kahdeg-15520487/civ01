using System.Linq;
using Xunit;
using RuneEngraver.PoC.Core.Elements;
using RuneEngraver.PoC.Core.Nodes;

namespace RuneEngraver.PoC.Tests.Nodes.Operations;

public class AttenuatorTests
{
    [Fact]
    public void Process_AttenuatesSignal_SplitsExcess()
    {
        var node = new AttenuatorNode("Att", 0.5f);
        
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Metal, 10);
        
        node.Process().ToList();
        
        // Out: 10 * 0.5 = 5
        var output = node.Outputs[0].CurrentValue;
        Assert.Equal(ElementType.Metal, output.Type);
        Assert.Equal(5, output.Magnitude);
        
        // Excess: 10 - 5 = 5
        var excess = node.Outputs[1].CurrentValue;
        Assert.Equal(ElementType.Metal, excess.Type);
        Assert.Equal(5, excess.Magnitude);
    }

    [Fact]
    public void Process_ZeroFactor_AllExcess()
    {
        var node = new AttenuatorNode("Att", 0.0f);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Metal, 10);
        
        node.Process().ToList();
        
        Assert.True(node.Outputs[0].CurrentValue.IsEmpty);
        Assert.Equal(10, node.Outputs[1].CurrentValue.Magnitude);
    }
}
