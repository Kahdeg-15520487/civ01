using RuneEngraver.Compiler.Semantics;
using RuneEngraver.Compiler.Syntax;
using Xunit;

namespace Tests.Compiler;

public class SymbolTableTests
{
    [Fact]
    public void SymbolTable_ContainsCoreNodes()
    {
        var table = new SymbolTable();
        
        Assert.True(table.IsDefined("core.SpiritStoneSocket"));
        Assert.True(table.IsDefined("core.QiCapacitor"));
        Assert.True(table.IsDefined("core.BurstTrigger"));
        Assert.True(table.IsDefined("core.EffectEmitter"));
    }

    [Fact]
    public void SymbolTable_ContainsSourceNodes()
    {
        var table = new SymbolTable();
        
        Assert.True(table.IsDefined("core.SpiritStoneSocket"));
        Assert.True(table.IsDefined("core.StoneArray"));
        Assert.True(table.IsDefined("core.CultivatorLink"));
        Assert.True(table.IsDefined("core.TunedResonator"));
        Assert.True(table.IsDefined("core.AmplitudeRegulator"));
    }

    [Fact]
    public void SymbolTable_ContainsSinkNodes()
    {
        var table = new SymbolTable();
        
        Assert.True(table.IsDefined("core.QiReceptacle"));
        Assert.True(table.IsDefined("core.EffectEmitter"));
        Assert.True(table.IsDefined("core.VoidDrain"));
        Assert.True(table.IsDefined("core.HeatSink"));
        Assert.True(table.IsDefined("core.GroundingRod"));
        Assert.True(table.IsDefined("core.UnstableVent"));
        Assert.True(table.IsDefined("core.BacklashNode"));
        Assert.True(table.IsDefined("core.CorruptionSeep"));
    }

    [Fact]
    public void SymbolTable_ContainsContainerNodes()
    {
        var table = new SymbolTable();
        
        Assert.True(table.IsDefined("core.SpiritVessel"));
        Assert.True(table.IsDefined("core.DualVessel"));
        Assert.True(table.IsDefined("core.ElementalPool"));
    }

    [Fact]
    public void SymbolTable_ContainsOperationNodes()
    {
        var table = new SymbolTable();
        
        Assert.True(table.IsDefined("core.Transmuter"));
        Assert.True(table.IsDefined("core.Amplifier"));
        Assert.True(table.IsDefined("core.Dampener"));
        Assert.True(table.IsDefined("core.Attenuator"));
        Assert.True(table.IsDefined("core.Splitter"));
        Assert.True(table.IsDefined("core.Combiner"));
    }

    [Fact]
    public void SymbolTable_ContainsControlNodes()
    {
        var table = new SymbolTable();
        
        Assert.True(table.IsDefined("core.YinYangGate"));
        Assert.True(table.IsDefined("core.ThresholdGate"));
        Assert.True(table.IsDefined("core.ElementFilter"));
    }

    [Fact]
    public void SymbolTable_TryLookup_ReturnsFormation()
    {
        var table = new SymbolTable();
        
        Assert.True(table.TryLookup("core.SpiritStoneSocket", out var formation));
        Assert.NotNull(formation);
        Assert.Equal("SpiritStoneSocket", formation.Name);
    }

    [Fact]
    public void SymbolTable_TryLookup_ReturnsFalseForUnknown()
    {
        var table = new SymbolTable();
        
        Assert.False(table.TryLookup("core.NonExistentNode", out _));
        Assert.False(table.TryLookup("std.fire.FireStarter", out _));
    }

    [Fact]
    public void SymbolTable_HasPackage_ReturnsTrueForCore()
    {
        var table = new SymbolTable();
        
        Assert.True(table.HasPackage("core"));
    }

    [Fact]
    public void SymbolTable_HasPackage_ReturnsFalseForUnknown()
    {
        var table = new SymbolTable();
        
        Assert.False(table.HasPackage("std"));
        Assert.False(table.HasPackage("std.fire"));
        Assert.False(table.HasPackage("unknown"));
    }

    [Fact]
    public void SymbolTable_Define_AddsNewFormation()
    {
        var table = new SymbolTable();
        var formation = new FormationDefinition("CustomNode", new List<Statement>());
        
        table.Define("custom.MyNode", formation);
        
        Assert.True(table.IsDefined("custom.MyNode"));
        Assert.True(table.TryLookup("custom.MyNode", out var result));
        Assert.Equal("CustomNode", result.Name);
    }

    [Fact]
    public void SymbolTable_CoreNodes_HaveCorrectPorts()
    {
        var table = new SymbolTable();
        
        // QiCapacitor should have 'in', 'out', 'full'
        table.TryLookup("core.QiCapacitor", out var cap);
        var ports = cap.Statements.OfType<PortDefinition>().ToList();
        Assert.Contains(ports, p => p.Name == "in");
        Assert.Contains(ports, p => p.Name == "out");
        Assert.Contains(ports, p => p.Name == "full");
        
        // Amplifier should have 'primary', 'catalyst', 'out'
        table.TryLookup("core.Amplifier", out var amp);
        var ampPorts = amp.Statements.OfType<PortDefinition>().ToList();
        Assert.Contains(ampPorts, p => p.Name == "primary");
        Assert.Contains(ampPorts, p => p.Name == "catalyst");
        Assert.Contains(ampPorts, p => p.Name == "out");
    }
}
