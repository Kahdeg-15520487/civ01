using Godot;
using System;
using Civ.Emulator.Core.Elements;

public static class QiValueTests
{
    public static void Run()
    {
        GD.Print("Running QiValueTests...");
        TestConstructor();
        TestEquality();
    }

    private static void TestConstructor()
    {
        GD.Print("  - TestConstructor");
        var qi = new QiValue(ElementType.Fire, 10, 5);
        Assert(qi.Type == ElementType.Fire, "Type match");
        Assert(qi.Magnitude == 10, "Mag match");
        Assert(qi.TTL == 5, "TTL match");
    }

    private static void TestEquality()
    {
        GD.Print("  - TestEquality");
        var q1 = new QiValue(ElementType.Water, 5);
        var q2 = new QiValue(ElementType.Water, 5);
        var q3 = new QiValue(ElementType.Water, 6);

        Assert(q1 == q2, "Equality check");
        Assert(q1 != q3, "Inequality check");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception($"Assertion Failed: {message}");
        }
    }
}
