using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Containers;

public class VesselTests
{
    [Fact]
    public void SpiritVessel_BuffersValue()
    {
        var node = new SpiritVessel("Vessel");
        var input = new QiValue(ElementType.Wood, 10);
        node.Inputs[0].CurrentValue = input;
        
        node.Process().ToList();
        
        Assert.Equal(input, node.Outputs[0].CurrentValue);
    }

    [Fact]
    public void DualVessel_BuffersIndependentChannels()
    {
        var node = new DualVessel("Dual");
        var in1 = new QiValue(ElementType.Wood, 10);
        var in2 = new QiValue(ElementType.Fire, 5);
        
        node.Inputs[0].CurrentValue = in1;
        node.Inputs[1].CurrentValue = in2;
        
        node.Process().ToList();
        
        Assert.Equal(in1, node.Outputs[0].CurrentValue);
        Assert.Equal(in2, node.Outputs[1].CurrentValue);
    }
}
