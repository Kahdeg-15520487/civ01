using Xunit;
using RuneEngraver.Core.Elements;

namespace RuneEngraver.Core.Tests.Core;

public class QiValueTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var qi = new QiValue(ElementType.Fire, 10, 5);
        Assert.Equal(ElementType.Fire, qi.Type);
        Assert.Equal(10, qi.Magnitude);
        Assert.Equal(5, qi.TTL);
    }

    [Fact]
    public void IsEmpty_ReturnsTrue_UseDefault()
    {
        var qi = QiValue.Empty;
        Assert.True(qi.IsEmpty);
        Assert.Equal(ElementType.None, qi.Type);
        Assert.Equal(0, qi.Magnitude);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var q1 = new QiValue(ElementType.Water, 5);
        var q2 = new QiValue(ElementType.Water, 5);
        Assert.Equal(q1, q2);
        Assert.True(q1 == q2);
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var q1 = new QiValue(ElementType.Water, 5);
        var q2 = new QiValue(ElementType.Water, 6);
        Assert.NotEqual(q1, q2);
        Assert.True(q1 != q2);
    }
}
