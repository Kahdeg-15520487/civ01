using System.Linq;
using Xunit;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;

namespace RuneEngraver.Core.Tests.Nodes.Sources;

public class NewSourceTests
{
    #region TunedResonator Tests
    
    [Fact]
    public void TunedResonator_AppliesEfficiencyBonus()
    {
        var node = new TunedResonator("Resonator", 1.5); // 50% bonus
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 10);
        
        node.Process().ToList();
        
        var output = node.Outputs[0].CurrentValue;
        Assert.Equal(ElementType.Fire, output.Type);
        Assert.Equal(15, output.Magnitude); // 10 * 1.5 = 15
    }

    [Fact]
    public void TunedResonator_DefaultEfficiency()
    {
        var node = new TunedResonator("Resonator"); // Default 1.2x
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Water, 10);
        
        node.Process().ToList();
        
        var output = node.Outputs[0].CurrentValue;
        Assert.Equal(12, output.Magnitude); // 10 * 1.2 = 12
    }

    [Fact]
    public void TunedResonator_PreservesElement()
    {
        var node = new TunedResonator("Resonator");
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Metal, 5);
        
        node.Process().ToList();
        
        Assert.Equal(ElementType.Metal, node.Outputs[0].CurrentValue.Type);
    }
    
    #endregion

    #region AmplitudeRegulator Tests
    
    [Fact]
    public void AmplitudeRegulator_NormalizesToTarget()
    {
        var node = new AmplitudeRegulator("Regulator", targetAmplitude: 10);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 50);
        
        var logs = node.Process().ToList();
        
        var output = node.Outputs[0].CurrentValue;
        Assert.Equal(10, output.Magnitude);
        Assert.Contains(logs, l => l.Contains("normalized"));
    }

    [Fact]
    public void AmplitudeRegulator_PreservesElement()
    {
        var node = new AmplitudeRegulator("Regulator", targetAmplitude: 10);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Earth, 30);
        
        node.Process().ToList();
        
        Assert.Equal(ElementType.Earth, node.Outputs[0].CurrentValue.Type);
    }

    [Fact]
    public void AmplitudeRegulator_OverloadFails()
    {
        var node = new AmplitudeRegulator("Regulator", targetAmplitude: 10, maxInput: 50);
        node.Inputs[0].CurrentValue = new QiValue(ElementType.Fire, 60);
        
        var logs = node.Process().ToList();
        
        Assert.Contains(logs, l => l.Contains("OVERLOAD"));
    }

    [Fact]
    public void AmplitudeRegulator_EmptyInput_NoOutput()
    {
        var node = new AmplitudeRegulator("Regulator");
        // No input set
        
        var logs = node.Process().ToList();
        
        Assert.Empty(logs);
    }
    
    #endregion

    #region CultivatorLink Tests
    
    [Fact]
    public void CultivatorLink_EmitsCurrentInput()
    {
        var input = new QiValue(ElementType.Lightning, 20);
        var node = new CultivatorLink("Link", input);
        
        node.Process().ToList();
        
        Assert.Equal(input, node.Outputs[0].CurrentValue);
    }

    [Fact]
    public void CultivatorLink_EmptyConstructor_NoOutput()
    {
        var node = new CultivatorLink("Link");
        
        var logs = node.Process().ToList();
        
        Assert.True(node.Outputs[0].CurrentValue.IsEmpty);
    }

    [Fact]
    public void CultivatorLink_CanChangeInput()
    {
        var node = new CultivatorLink("Link");
        node.CurrentInput = new QiValue(ElementType.Wind, 15);
        
        node.Process().ToList();
        
        Assert.Equal(ElementType.Wind, node.Outputs[0].CurrentValue.Type);
        Assert.Equal(15, node.Outputs[0].CurrentValue.Magnitude);
    }
    
    #endregion
}
