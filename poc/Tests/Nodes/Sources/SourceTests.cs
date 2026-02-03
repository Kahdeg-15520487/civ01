using System.Linq;
using Xunit;
using RuneEngraver.Core.Core.Elements;
using RuneEngraver.Core.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Sources;

public class SourceTests
{
    [Fact]
    public void SpiritStoneSource_EmitsConstant()
    {
        var val = new QiValue(ElementType.Metal, 10);
        var node = new SpiritStoneSource("Src", val);
        
        node.Process().ToList();
        
        Assert.Equal(val, node.Outputs[0].CurrentValue);
    }

    [Fact]
    public void StoneArray_SumsInputs()
    {
        var s1 = new QiValue(ElementType.Metal, 10);
        var s2 = new QiValue(ElementType.Metal, 5);
        var node = new StoneArray("Array", s1, s2);
        
        node.Process().ToList();
        
        var outVal = node.Outputs[0].CurrentValue;
        Assert.Equal(ElementType.Metal, outVal.Type);
        Assert.Equal(15, outVal.Magnitude);
    }
}
