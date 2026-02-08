using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Interfaces;

public class InterfaceTests
{
    #region StableEmitter Tests
    
    [Fact]
    public void StableEmitter_PassesThroughValue()
    {
        var node = new StableEmitter("Emitter");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("EMIT"));
    }
    
    #endregion

    #region OverrunEmitter Tests
    
    [Fact]
    public void OverrunEmitter_WithinThreshold_EmitsNormally()
    {
        var node = new OverrunEmitter("Overrun", 50);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 30);
        
        var logs = node.Process().ToList();
        
        Assert.Equal(30, node.Outputs[0].CurrentValue.Magnitude);
        Assert.Contains(logs, l => l.Contains("Stable"));
    }

    [Fact]
    public void OverrunEmitter_OverThreshold_SplitsToFeedback()
    {
        var node = new OverrunEmitter("Overrun", 50);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 70);
        
        var logs = node.Process().ToList();
        
        // Main output capped at threshold
        Assert.Equal(50, node.Outputs[0].CurrentValue.Magnitude);
        // Overflow goes to feedback
        Assert.Equal(20, node.Outputs[1].CurrentValue.Magnitude); // 70 - 50 = 20
        Assert.Contains(logs, l => l.Contains("OVERRUN"));
    }

    [Fact]
    public void OverrunEmitter_PreservesElementType()
    {
        var node = new OverrunEmitter("Overrun", 50);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Metal, 60);
        
        node.Process().ToList();
        
        Assert.Equal(ElementType.Metal, node.Outputs[0].CurrentValue.Type);
        Assert.Equal(ElementType.Metal, node.Outputs[1].CurrentValue.Type);
    }
    
    #endregion

    #region FizzleEmitter Tests
    
    [Fact]
    public void FizzleEmitter_FirstPulse_Emits()
    {
        var node = new FizzleEmitter("Fizzle");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Lightning, 15);
        
        var logs = node.Process().ToList();
        
        Assert.Equal(15, node.Outputs[0].CurrentValue.Magnitude);
        Assert.Contains(logs, l => l.Contains("FIZZLE"));
    }

    [Fact]
    public void FizzleEmitter_SecondPulse_Absorbed()
    {
        var node = new FizzleEmitter("Fizzle");
        
        // First pulse
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 10);
        node.Process().ToList();
        
        // Second pulse - should be absorbed
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 20);
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("Fizzled out"));
    }

    [Fact]
    public void FizzleEmitter_CanReset()
    {
        var node = new FizzleEmitter("Fizzle");
        
        // First pulse
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 10);
        node.Process().ToList();
        
        // Reset
        node.Reset();
        
        // Should emit again after reset
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 5);
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("FIZZLE"));
        Assert.Equal(5, node.Outputs[0].CurrentValue.Magnitude);
    }
    
    #endregion

    #region SkyAntenna Tests
    
    [Fact]
    public void SkyAntenna_RequiresTempest_Fails()
    {
        var node = new SkyAntenna("Antenna");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("ignored"));
    }

    [Fact]
    public void SkyAntenna_WithTempest_Transmits()
    {
        var node = new SkyAntenna("Antenna");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Tempest, 20);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("BROADCASTING"));
    }
    
    #endregion
}
