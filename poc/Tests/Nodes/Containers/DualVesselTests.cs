using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Containers;

public class DualVesselTests
{
    [Fact]
    public void DualVessel_StoresTwoInputs()
    {
        var node = new DualVessel("Dual");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 10);
        node.Inputs[1].CurrentValue = new QiValue(ElementType.Water, 5);
        
        var logs = node.Process().ToList();
        
        // DualVessel should store both values
        Assert.NotEmpty(logs);
    }

    [Fact]
    public void DualVessel_OutputsCombinedOrSequential()
    {
        var node = new DualVessel("Dual");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Metal, 8);
        node.Inputs[1].CurrentValue = new QiValue(ElementType.Metal, 7);
        
        node.Process().ToList();
        
        // Output should have been written
        Assert.False(node.Outputs[0].CurrentValue.IsEmpty);
    }
}
