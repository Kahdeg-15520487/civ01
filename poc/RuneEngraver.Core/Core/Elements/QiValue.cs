namespace RuneEngraver.Core.Core.Elements;

public enum ElementType
{
    None = 0,
    // Primary (5)
    Wood, Fire, Earth, Metal, Water,
    // Auxiliary (2)
    Lightning, Wind,
    
    // Sub-Elements (Primary + Primary)
    Charcoal,       // Wood + Fire
    FertileSoil,    // Wood + Earth
    Splinter,       // Wood + Metal
    LifeSap,        // Wood + Water
    Magma,          // Fire + Earth
    Slag,           // Fire + Metal
    Steam,          // Fire + Water
    Ore,            // Earth + Metal
    Mud,            // Earth + Water
    Ice,            // Metal + Water
    
    // Sub-Elements (Primary + Aux)
    Spore,          // Wood + Wind
    ThornStorm,     // Wood + Lightning
    Wildfire,       // Fire + Wind
    Plasma,         // Fire + Lightning
    Dust,           // Earth + Wind
    Quartz,         // Earth + Lightning
    BladeWind,      // Metal + Wind
    Magnetism,      // Metal + Lightning
    Mist,           // Water + Wind
    Storm,          // Water + Lightning
    
    // Aux + Aux
    Tempest         // Lightning + Wind
}

public struct QiValue : IEquatable<QiValue>
{
    public ElementType Type { get; }
    public int Magnitude { get; }
    public int TTL { get; } // Time To Live: 0 = Stable (Primary), >0 = Unstable (Sub)

    public QiValue(ElementType type, int magnitude, int ttl = 0)
    {
        Type = type;
        Magnitude = magnitude;
        TTL = ttl;
    }

    public bool IsEmpty => Magnitude <= 0 || Type == ElementType.None;
    public bool IsStable => TTL == 0;

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Magnitude, TTL);
    }

    public override bool Equals(object? obj)
    {
        return obj is QiValue other && Equals(other);
    }

    public bool Equals(QiValue other)
    {
        return Type == other.Type && Magnitude == other.Magnitude && TTL == other.TTL;
    }

    public static bool operator ==(QiValue left, QiValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(QiValue left, QiValue right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{Type}({Magnitude}){(TTL > 0 ? $"[TTL:{TTL}]" : "")}";
    }

    public static QiValue Empty => new QiValue(ElementType.None, 0);
}
