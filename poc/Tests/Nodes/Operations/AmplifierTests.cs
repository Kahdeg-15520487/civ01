using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Operations;

public class AmplifierTests
{
    [Fact]
    public void Process_ValidGeneration_AmplifiesCatalyst()
    {
        // Fire generates Earth. Primary(Fire) > Catalyst(Earth).
        var node = new AmplifierNode("Amp");
        
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 5); // Primary
        node.Inputs[1].CurrentValue = new QiValue(ElementType.Earth, 2); // Catalyst

        // Act
        node.Process().ToList(); // Trigger processing
        
        var result = node.Outputs[0].CurrentValue;
        
        Assert.Equal(ElementType.Earth, result.Type);
        Assert.Equal(10, result.Magnitude); // 5 * 2
    }

    [Fact]
    public void Process_InvalidCycle_NoOutput()
    {
        var node = new AmplifierNode("Amp");
        // Water does NOT generate Earth.
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 5);
        node.Inputs[1].CurrentValue = new QiValue(ElementType.Earth, 2);

        node.Process().ToList();

        Assert.True(node.Outputs[0].CurrentValue.IsEmpty);
    }

    [Fact]
    public void Process_InsufficientPrimary_NoOutput()
    {
        var node = new AmplifierNode("Amp");
        // Fire generates Earth, but Primary(2) is not > Catalyst(5).
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 2);
        node.Inputs[1].CurrentValue = new QiValue(ElementType.Earth, 5);

        node.Process().ToList();

        Assert.True(node.Outputs[0].CurrentValue.IsEmpty);
    }
}
