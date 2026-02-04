namespace RuneEngraver.Core.Elements;

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

public class QiValue : IEquatable<QiValue>
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

    public virtual bool IsEmpty => Magnitude <= 0 || Type == ElementType.None;
    public bool IsStable => TTL == 0;

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Magnitude, TTL);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as QiValue);
    }

    public virtual bool Equals(QiValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Type == other.Type && Magnitude == other.Magnitude && TTL == other.TTL;
    }

    public static bool operator ==(QiValue? left, QiValue? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(QiValue? left, QiValue? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{Type}({Magnitude}){(TTL > 0 ? $"[TTL:{TTL}]" : "")}";
    }

    public static readonly QiValue Empty = new QiValue(ElementType.None, 0);
}
