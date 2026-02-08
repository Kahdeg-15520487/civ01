using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Sinks;

public class NewSinkTests
{
    #region UnstableVent Tests
    
    [Fact]
    public void UnstableVent_VentsWithinLimit_ShowsSideEffect()
    {
        var node = new UnstableVent("Vent", 50);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 30);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("SIDE EFFECT"));
        Assert.Contains(logs, l => l.Contains("Venting"));
    }

    [Fact]
    public void UnstableVent_OverLimit_CausesExplosion()
    {
        var node = new UnstableVent("Vent", 50);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 60);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("EXPLOSION"));
    }

    [Fact]
    public void UnstableVent_EmptyInput_NoOutput()
    {
        var node = new UnstableVent("Vent", 50);
        // No input set
        
        var logs = node.Process().ToList();
        
        Assert.Empty(logs);
    }
    
    #endregion

    #region BacklashNode Tests
    
    [Fact]
    public void BacklashNode_WithinSafeLimit_MinorBacklash()
    {
        var node = new BacklashNode("Backlash", 30);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 20);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("Minor backlash"));
    }

    [Fact]
    public void BacklashNode_OverSafeLimit_ReflectsDamage()
    {
        var node = new BacklashNode("Backlash", 30);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 50);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("BACKLASH"));
        Assert.Contains(logs, l => l.Contains("Reflecting 20")); // 50 - 30 = 20
    }
    
    #endregion

    #region CorruptionSeep Tests
    
    [Fact]
    public void CorruptionSeep_AccumulatesPollution()
    {
        var node = new CorruptionSeep("Seep");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Earth, 10);
        
        var logs1 = node.Process().ToList();
        Assert.Contains(logs1, l => l.Contains("POLLUTION"));
        Assert.Contains(logs1, l => l.Contains("Total corruption: 10"));
        
        // Process again
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 15);
        var logs2 = node.Process().ToList();
        Assert.Contains(logs2, l => l.Contains("Total corruption: 25")); // 10 + 15
    }
    
    #endregion

    #region QiReceptacle Tests
    
    [Fact]
    public void QiReceptacle_NoExpectation_Validates()
    {
        var node = new QiReceptacle("Receptacle");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Metal, 5);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("VALIDATED"));
    }

    [Fact]
    public void QiReceptacle_MatchesExpectedElement_Validates()
    {
        var node = new QiReceptacle("Receptacle", expectedElement: ElementType.Fire);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("VALIDATED"));
    }

    [Fact]
    public void QiReceptacle_WrongElement_FailsValidation()
    {
        var node = new QiReceptacle("Receptacle", expectedElement: ElementType.Fire);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("VALIDATION FAILED"));
    }

    [Fact]
    public void QiReceptacle_MatchesMagnitude_Validates()
    {
        var node = new QiReceptacle("Receptacle", expectedMagnitude: 10);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Wood, 10);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("VALIDATED"));
    }

    [Fact]
    public void QiReceptacle_WrongMagnitude_FailsValidation()
    {
        var node = new QiReceptacle("Receptacle", expectedMagnitude: 10);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Wood, 5);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("VALIDATION FAILED"));
    }
    
    #endregion
}
