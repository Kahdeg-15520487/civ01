using System.Linq;
using Xunit;
using RuneEngraver.PoC.Core.Elements;
using RuneEngraver.PoC.Core.Nodes;

namespace RuneEngraver.PoC.Tests.Nodes.Sinks;

public class SinkTests
{
    [Fact]
    public void VoidDrain_AcceptsWithinLimit()
    {
        var node = new VoidDrain("Drain", 10);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 5);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("Dissipating"));
        Assert.DoesNotContain(logs, l => l.Contains("FAILURE"));
    }

    [Fact]
    public void VoidDrain_FailsOverLimit()
    {
        var node = new VoidDrain("Drain", 10);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 15);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("FAILURE"));
    }

    [Fact]
    public void HeatSink_StartFireOnly()
    {
        var node = new HeatSink("Sink");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 5);
        
        var logs = node.Process().ToList();
        Assert.Contains(logs, l => l.Contains("Radiating"));
    }

    [Fact]
    public void HeatSink_RejectCleanWater()
    {
        var node = new HeatSink("Sink");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 5);
        
        var logs = node.Process().ToList();
        Assert.Contains(logs, l => l.Contains("FAILURE"));
    }
}
