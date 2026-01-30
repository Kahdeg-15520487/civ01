using System;
using System.Collections.Generic;

namespace RuneEngraver.PoC.Core.Elements;

public static class InteractionManager
{
    // Generation Cycle (Primary)
    private static readonly Dictionary<ElementType, ElementType> GenerationCycle = new()
    {
        { ElementType.Wood, ElementType.Fire },
        { ElementType.Fire, ElementType.Earth },
        { ElementType.Earth, ElementType.Metal },
        { ElementType.Metal, ElementType.Water },
        { ElementType.Water, ElementType.Wood }
    };

    // Overcoming Cycle (Primary)
    private static readonly Dictionary<ElementType, ElementType> OvercomingCycle = new()
    {
        { ElementType.Wood, ElementType.Earth },
        { ElementType.Earth, ElementType.Water },
        { ElementType.Water, ElementType.Fire },
        { ElementType.Fire, ElementType.Metal },
        { ElementType.Metal, ElementType.Wood }
    };

    // Sub-Element Definitions (P1 + P2 -> Sub)
    // Key: (MinType, MaxType) to ensure order insensitivity
    private static readonly Dictionary<(ElementType, ElementType), (ElementType Sub, int TTL)> Combinations = new();

    // Decay Map (Sub -> Parent)
    private static readonly Dictionary<ElementType, ElementType> DecayMap = new();

    static InteractionManager()
    {
        RegisterCombinations();
        RegisterDecay();
    }

    private static void RegisterCombinations()
    {
        // Helper to register pair
        void Add(ElementType a, ElementType b, ElementType result, int ttl)
        {
            var k = a < b ? (a, b) : (b, a);
            Combinations[k] = (result, ttl);
        }

        // Primary + Primary
        Add(ElementType.Wood, ElementType.Fire, ElementType.Charcoal, 4);
        Add(ElementType.Wood, ElementType.Earth, ElementType.FertileSoil, 5);
        Add(ElementType.Wood, ElementType.Metal, ElementType.Splinter, 3);
        Add(ElementType.Wood, ElementType.Water, ElementType.LifeSap, 4);
        Add(ElementType.Fire, ElementType.Earth, ElementType.Magma, 4);
        Add(ElementType.Fire, ElementType.Metal, ElementType.Slag, 3);
        Add(ElementType.Fire, ElementType.Water, ElementType.Steam, 3);
        Add(ElementType.Earth, ElementType.Metal, ElementType.Ore, 6);
        Add(ElementType.Earth, ElementType.Water, ElementType.Mud, 5);
        Add(ElementType.Metal, ElementType.Water, ElementType.Ice, 5);

        // Primary + Aux
        Add(ElementType.Wood, ElementType.Wind, ElementType.Spore, 2);
        Add(ElementType.Wood, ElementType.Lightning, ElementType.ThornStorm, 2);
        Add(ElementType.Fire, ElementType.Wind, ElementType.Wildfire, 2);
        Add(ElementType.Fire, ElementType.Lightning, ElementType.Plasma, 2);
        Add(ElementType.Earth, ElementType.Wind, ElementType.Dust, 2);
        Add(ElementType.Earth, ElementType.Lightning, ElementType.Quartz, 4);
        Add(ElementType.Metal, ElementType.Wind, ElementType.BladeWind, 2);
        Add(ElementType.Metal, ElementType.Lightning, ElementType.Magnetism, 3);
        Add(ElementType.Water, ElementType.Wind, ElementType.Mist, 3);
        Add(ElementType.Water, ElementType.Lightning, ElementType.Storm, 2);

        // Aux + Aux
        Add(ElementType.Lightning, ElementType.Wind, ElementType.Tempest, 2);
    }

    private static void RegisterDecay()
    {
        // Mapping based on "Decays To" column
        DecayMap[ElementType.Charcoal] = ElementType.Fire;
        DecayMap[ElementType.FertileSoil] = ElementType.Earth;
        DecayMap[ElementType.Splinter] = ElementType.Wood;
        DecayMap[ElementType.LifeSap] = ElementType.Water;
        DecayMap[ElementType.Magma] = ElementType.Earth;
        DecayMap[ElementType.Slag] = ElementType.Metal;
        DecayMap[ElementType.Steam] = ElementType.Water;
        DecayMap[ElementType.Ore] = ElementType.Metal;
        DecayMap[ElementType.Mud] = ElementType.Earth;
        DecayMap[ElementType.Ice] = ElementType.Water;

        DecayMap[ElementType.Spore] = ElementType.Wood;
        DecayMap[ElementType.ThornStorm] = ElementType.Wood;
        DecayMap[ElementType.Wildfire] = ElementType.Fire;
        DecayMap[ElementType.Plasma] = ElementType.Fire;
        DecayMap[ElementType.Dust] = ElementType.Earth;
        DecayMap[ElementType.Quartz] = ElementType.Earth;
        DecayMap[ElementType.BladeWind] = ElementType.Metal;
        DecayMap[ElementType.Magnetism] = ElementType.Metal;
        DecayMap[ElementType.Mist] = ElementType.Water;
        DecayMap[ElementType.Storm] = ElementType.Water;
        DecayMap[ElementType.Tempest] = ElementType.Wind;
    }

    /// <summary>
    /// Combines two Qi values. 
    /// If Types match -> Additive merge.
    /// If Different -> Check Combination Matrix.
    /// </summary>
    public static QiValue Combine(QiValue a, QiValue b)
    {
        if (a.IsEmpty) return b;
        if (b.IsEmpty) return a;

        // Same Type Merge
        if (a.Type == b.Type)
        {
            // Max TTL validation as per spec
            int newTTL = Math.Max(a.TTL, b.TTL);
            return new QiValue(a.Type, a.Magnitude + b.Magnitude, newTTL);
        }

        // Different Type Combination
        // Rule: Inputs must be equal magnitude for stable combination
        if (a.Magnitude != b.Magnitude)
        {
            // Qi Deviation! For PoC just return Empty or error/void type
            // In real game: Explosion.
            Console.Error.WriteLine($"[Qi Deviation] Unequal magnitudes: {a} + {b}");
            return QiValue.Empty; 
        }

        var key = a.Type < b.Type ? (a.Type, b.Type) : (b.Type, a.Type);
        if (Combinations.TryGetValue(key, out var res))
        {
            // Magnitude is additive (e.g. 4+4=8)
            return new QiValue(res.Sub, a.Magnitude + b.Magnitude, res.TTL);
        }

        // No valid combination
        Console.Error.WriteLine($"[Incompatible] {a.Type} and {b.Type} do not combine.");
        return QiValue.Empty;
    }

    /// <summary>
    /// Generation Cycle: Input generates Output.
    /// Returns Empty if input is not part of the cycle.
    /// </summary>
    public static ElementType GetGenerationChild(ElementType parent)
    {
        return GenerationCycle.TryGetValue(parent, out var child) ? child : ElementType.None;
    }

    /// <summary>
    /// Overcoming Cycle: Suppressor overcomes Target.
    /// Returns true if suppressor overcomes target.
    /// </summary>
    public static bool DoesOvercome(ElementType suppressor, ElementType target)
    {
        return OvercomingCycle.TryGetValue(suppressor, out var t) && t == target;
    }

    /// <summary>
    /// Processes decay logic. Decrements TTL. 
    /// If TTL -> 0, reverts to Parent (with 80% magnitude).
    /// </summary>
    public static QiValue ProcessDecay(QiValue q)
    {
        if (q.IsStable || q.IsEmpty) return q;

        int newTTL = q.TTL - 1;
        if (newTTL > 0)
        {
            return new QiValue(q.Type, q.Magnitude, newTTL);
        }
        else
        {
            // Decay happened
            if (DecayMap.TryGetValue(q.Type, out var parent))
            {
                // Spec 5.3: 80% magnitude preserved
                int newMag = (int)(q.Magnitude * 0.8f);
                return new QiValue(parent, newMag, 0); // Parent is stable
            }
            return QiValue.Empty; // Should not happen if all subs mapped
        }
    }
}
