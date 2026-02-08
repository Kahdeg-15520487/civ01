using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Modifiers;

public class ModifierTests
{
    #region StabilizerNode Tests
    
    [Fact]
    public void StabilizerNode_PassesThroughValue()
    {
        var node = new StabilizerNode("Stabilizer");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Steam, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Equal(10, node.Outputs[0].CurrentValue.Magnitude);
        Assert.Equal(ElementType.Steam, node.Outputs[0].CurrentValue.Type);
    }

    [Fact]
    public void StabilizerNode_LogsStabilization()
    {
        var node = new StabilizerNode("Stabilizer", 3);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Magma, 5);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("Stabilizing"));
        Assert.Contains(logs, l => l.Contains("+3 TTL"));
    }

    [Fact]
    public void StabilizerNode_EmptyInput_NoOutput()
    {
        var node = new StabilizerNode("Stabilizer");
        
        var logs = node.Process().ToList();
        
        Assert.Empty(logs);
        Assert.True(node.Outputs[0].CurrentValue.IsEmpty);
    }
    
    #endregion

    #region CoolingChamber Tests
    
    [Fact]
    public void CoolingChamber_ColdElement_GetsTTLBonus()
    {
        var node = new CoolingChamber("Cooler");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Ice, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("+50% TTL"));
        Assert.Equal(10, node.Outputs[0].CurrentValue.Magnitude);
    }

    [Fact]
    public void CoolingChamber_WaterElement_GetsTTLBonus()
    {
        var node = new CoolingChamber("Cooler");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 8);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("+50% TTL"));
    }

    [Fact]
    public void CoolingChamber_NonColdElement_NoBonus()
    {
        var node = new CoolingChamber("Cooler");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("no TTL bonus"));
    }
    
    #endregion

    #region HeatingChamber Tests
    
    [Fact]
    public void HeatingChamber_FireElement_GetsTTLBonus()
    {
        var node = new HeatingChamber("Heater");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("+50% TTL"));
    }

    [Fact]
    public void HeatingChamber_MagmaElement_GetsTTLBonus()
    {
        var node = new HeatingChamber("Heater");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Magma, 12);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("+50% TTL"));
    }

    [Fact]
    public void HeatingChamber_NonHeatElement_NoBonus()
    {
        var node = new HeatingChamber("Heater");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("no TTL bonus"));
    }

    [Fact]
    public void HeatingChamber_PassesThroughValue()
    {
        var node = new HeatingChamber("Heater");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Plasma, 15);
        
        node.Process().ToList();
        
        Assert.Equal(15, node.Outputs[0].CurrentValue.Magnitude);
        Assert.Equal(ElementType.Plasma, node.Outputs[0].CurrentValue.Type);
    }
    
    #endregion

    #region CatalystNode Tests
    
    [Fact]
    public void CatalystNode_PassesThroughValue()
    {
        var node = new CatalystNode("Catalyst");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Steam, 8);
        
        node.Process().ToList();
        
        Assert.Equal(8, node.Outputs[0].CurrentValue.Magnitude);
        Assert.Equal(ElementType.Steam, node.Outputs[0].CurrentValue.Type);
    }

    [Fact]
    public void CatalystNode_LogsAcceleratedDecay()
    {
        var node = new CatalystNode("Catalyst", 4);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Magma, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("Catalyzing"));
        Assert.Contains(logs, l => l.Contains("-4 TTL"));
    }

    [Fact]
    public void CatalystNode_DefaultTTLReduction()
    {
        var node = new CatalystNode("Catalyst"); // Default -2
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Ice, 5);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("-2 TTL"));
    }

    [Fact]
    public void CatalystNode_EmptyInput_NoOutput()
    {
        var node = new CatalystNode("Catalyst");
        
        var logs = node.Process().ToList();
        
        Assert.Empty(logs);
    }
    
    #endregion
}
