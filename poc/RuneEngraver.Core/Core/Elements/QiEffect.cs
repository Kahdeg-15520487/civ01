using System;

namespace RuneEngraver.Core.Elements;

public class QiEffect : QiValue, IEquatable<QiEffect>
{
    public string Tag { get; }

    public QiEffect(ElementType type, int magnitude, string tag, int ttl = 0) 
        : base(type, magnitude, ttl)
    {
        Tag = tag ?? string.Empty;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as QiEffect);
    }

    public bool Equals(QiEffect? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        // Check base equality manually or assume base.Equals checks Type/Mag/TTL?
        // base.Equals checks Type/Mag/TTL.
        return base.Equals(other) && Tag == other.Tag;
    }

    // Since we override Equals taking object, we should probably ensure base.Equals(QiValue) also handles this correctly if we care about Liskov substitution for equality w.r.t QiValue,
    // but typically Effect != Value even if same stats if one has a tag. 
    // However, our base.Equals(QiValue) only checks Type/Mag/TTL. 
    // If we have strict equality requirements where QiEffect("Fire", 10, "Burn") != QiValue("Fire", 10), 
    // then the base class needs to be aware of type or we override Equals(QiValue) here too.
    
    public override bool Equals(QiValue? other)
    {
        // If other is exactly QiValue, it has no Tag, so it's not equal to this QiEffect (which implies a tag).
        // Or strictly: logic depends on domain. 
        // For now: An Effect is NOT equal to a raw Value.
        return other is QiEffect effect && Equals(effect);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Tag);
    }

    public override string ToString()
    {
        return $"{base.ToString()} [Effect:{Tag}]";
    }
}
