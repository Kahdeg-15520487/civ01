using Godot;
using System;
using Civ.Emulator.Core.Elements;

// Helper class for Interaction Tests
public static class InteractionTests
{
    public static void Run()
    {
        GD.Print("Running InteractionTests...");
        TestGenerationCycle();
        TestCombine_EqualMagnitudes();
        TestCombine_SameType();
        TestProcessDecay();
    }

    private static void TestGenerationCycle()
    {
        GD.Print("  - TestGenerationCycle");
        Assert(InteractionManager.GetGenerationChild(ElementType.Wood) == ElementType.Fire, "Wood -> Fire");
        Assert(InteractionManager.GetGenerationChild(ElementType.Fire) == ElementType.Earth, "Fire -> Earth");
        Assert(InteractionManager.GetGenerationChild(ElementType.Earth) == ElementType.Metal, "Earth -> Metal");
        Assert(InteractionManager.GetGenerationChild(ElementType.Metal) == ElementType.Water, "Metal -> Water");
        Assert(InteractionManager.GetGenerationChild(ElementType.Water) == ElementType.Wood, "Water -> Wood");
    }

    private static void TestCombine_EqualMagnitudes()
    {
        GD.Print("  - TestCombine_EqualMagnitudes");
        var q1 = new QiValue(ElementType.Water, 5);
        var q2 = new QiValue(ElementType.Fire, 5);
        
        var result = InteractionManager.Combine(q1, q2);
        
        Assert(result.Type == ElementType.Steam, "Water + Fire -> Steam");
        Assert(result.Magnitude == 10, "Magnitude should sum to 10");
    }

    private static void TestCombine_SameType()
    {
        GD.Print("  - TestCombine_SameType");
        var q1 = new QiValue(ElementType.Fire, 5);
        var q2 = new QiValue(ElementType.Fire, 3);
        
        var result = InteractionManager.Combine(q1, q2);
        
        Assert(result.Type == ElementType.Fire, "Fire + Fire -> Fire");
        Assert(result.Magnitude == 8, "Magnitude should sum to 8");
    }

    private static void TestProcessDecay()
    {
        GD.Print("  - TestProcessDecay");
        var q = new QiValue(ElementType.Steam, 10, 5); // Sub-element decays fast
        var decayed = InteractionManager.ProcessDecay(q);
        
        Assert(decayed.TTL < 5, "TTL should decrease");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception($"Assertion Failed: {message}");
        }
    }
}
