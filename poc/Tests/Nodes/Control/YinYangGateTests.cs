using System.Linq;
using Xunit;
using RuneEngraver.PoC.Core.Elements;
using RuneEngraver.PoC.Core.Nodes;

namespace RuneEngraver.PoC.Tests.Nodes.Control;

public class YinYangGateTests
{
    [Theory]
    [InlineData(5, true)]  // Magnitude > 0 -> Yang -> True Path
    [InlineData(0, false)] // Magnitude 0 -> Yin -> False Path
    public void Process_RoutesBasedOnCondition(int condMag, bool expectTrue)
    {
        var node = new YinYangGate("Gate");
        
        var input = new QiValue(ElementType.Water, 10);
        var cond = new QiValue(ElementType.Fire, condMag);
        
        node.Inputs[0].CurrentValue = input;
        node.Inputs[1].CurrentValue = cond;
        
        node.Process().ToList();
        
        if (expectTrue)
        {
            Assert.Equal(input.Magnitude, node.Outputs[0].CurrentValue.Magnitude); // True Out
            Assert.True(node.Outputs[1].CurrentValue.IsEmpty); // False Out Empty
        }
        else
        {
            Assert.True(node.Outputs[0].CurrentValue.IsEmpty); // True Out Empty
            Assert.Equal(input.Magnitude, node.Outputs[1].CurrentValue.Magnitude); // False Out
        }
    }
}
