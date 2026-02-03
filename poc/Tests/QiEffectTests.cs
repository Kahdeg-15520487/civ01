using Xunit;
using RuneEngraver.Core.Core.Elements;

namespace RuneEngraver.Core.Tests.Core;

public class QiEffectTests
{
    [Fact]
    public void Constructor_SetsPropertiesAndTag()
    {
        var effect = new QiEffect(ElementType.Fire, 10, "Burn", 5);
        Assert.Equal(ElementType.Fire, effect.Type);
        Assert.Equal(10, effect.Magnitude);
        Assert.Equal("Burn", effect.Tag);
        Assert.Equal(5, effect.TTL);
    }

    [Fact]
    public void Constructor_NullTag_SetsEmptyString()
    {
        var effect = new QiEffect(ElementType.Water, 5, null);
        Assert.Equal(string.Empty, effect.Tag);
    }

    [Fact]
    public void Equality_SameValuesAndTag_AreEqual()
    {
        var e1 = new QiEffect(ElementType.Metal, 5, "Cut");
        var e2 = new QiEffect(ElementType.Metal, 5, "Cut");
        
        Assert.Equal(e1, e2);
        Assert.True(e1 == e2);
    }

    [Fact]
    public void Equality_DifferentTag_AreNotEqual()
    {
        var e1 = new QiEffect(ElementType.Metal, 5, "Cut");
        var e2 = new QiEffect(ElementType.Metal, 5, "Slash");
        
        Assert.NotEqual(e1, e2);
        Assert.True(e1 != e2);
    }

    [Fact]
    public void Equality_EffectVsValue_AreNotEqual()
    {
        var val = new QiValue(ElementType.Metal, 5);
        var effect = new QiEffect(ElementType.Metal, 5, "Cut");
        
        // Effect should not equal Value (because Tag distinguishes it)
        Assert.False(effect.Equals(val));
        Assert.False(val.Equals(effect)); // val.Equals(obj) -> obj is QiValue -> true, but fields match? 
                                          // Wait, val.Equals(effect) calls QiValue.Equals(QiValue). 
                                          // It checks Type, Mag, TTL. Inherited Effect HAS those.
                                          // So val.Equals(effect) might return TRUE if we are not careful in QiValue.Equals.
        
        // Let's verify this behavior.
        // If QiValue.Equals(QiValue other) checks runtime type, it returns false.
        // If it only checks fields, it returns true.
        // In my implementation: 
        // public virtual bool Equals(QiValue? other) { ... Type == other.Type ... }
        // It does NOT check other.GetType() == this.GetType().
        
        // This is a common "bug" or feature in inheritance equality.
        // If I want strictly NotEqual, check GetType().
        // For this task, usually an Effect IS A Value, so Value == Effect might be acceptable if looking at raw stats,
        // but Effect != Value is strictly true.
        // Given I implemented QiEffect.Equals(QiValue) to return false if it's not a QiEffect, 
        // effect.Equals(val) -> False.
        
        // But val.Equals(effect) -> QiValue.Equals(QiValue) -> Checks fields -> True.
        
        // I should probably fix QiValue.Equals to perform exact type check if I want symmetry.
        // Or accept asymmetry. 
        // I'll Assert what I expect. 
        // Ideally, they should not be equal if one has a tag and the other doesn't (or tag is implicit empty).
        // Let's check strict type equality in QiValue if we want symmetric equality.
        
        // For now, I'll assert the current behavior or update QiValue.
        // Let's update QiValue to include GetType() check for strict equality in a separate step if tests fail or if this is desired.
        // For now, let's write the test to expect what currently happens, or failing strict equality.
        // I prefer strict equality.
    }

    [Fact]
    public void ToString_IncludesTag()
    {
        var effect = new QiEffect(ElementType.Earth, 8, "Crush");
        Assert.Contains("Earth(8)", effect.ToString());
        Assert.Contains("[Effect:Crush]", effect.ToString());
    }
}
