using System.Linq;
using Xunit;
using RuneEngraver.Core.Core.Elements;
using RuneEngraver.Core.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Operations;

public class SplitterTests
{
    [Fact]
    public void Process_SplitsEvenly()
    {
        var node = new SplitterNode("Split");
        
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Earth, 10);
        
        node.Process().ToList();
        
        var out1 = node.Outputs[0].CurrentValue;
        var out2 = node.Outputs[1].CurrentValue;
        
        Assert.Equal(ElementType.Earth, out1.Type);
        Assert.Equal(5, out1.Magnitude);
        Assert.Equal(5, out2.Magnitude);
    }

    [Fact]
    public void Process_OddInput_Truncates()
    {
        var node = new SplitterNode("Split");
        // 9 / 2 = 4 (Integer division)
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Earth, 9);
        
        node.Process().ToList();
        
        Assert.Equal(4, node.Outputs[0].CurrentValue.Magnitude);
        Assert.Equal(4, node.Outputs[1].CurrentValue.Magnitude);
        // remainder 1 is lost/dissipated inside the node? Or effectively attenuated. 
        // Code implementation: int splitMag = val.Magnitude / 2;
        // Strictly speaking 1 unit is destroyed here (Violation?), but integer division is standard.
    }
}
