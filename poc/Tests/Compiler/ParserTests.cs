using RuneEngraver.Compiler.Syntax;
using Xunit;

namespace Tests.Compiler;

public class ParserTests
{
    [Fact]
    public void Parse_EmptyFormation_Succeeds()
    {
        var input = @"
package test;

formation Empty {
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        Assert.Equal("test", result.Value.Package.QualifiedId);
        Assert.Single(result.Value.Formations);
        Assert.Equal("Empty", result.Value.Formations.First().Name);
        Assert.Empty(result.Value.Formations.First().Statements);
    }

    [Fact]
    public void Parse_FormationWithInputPort_Succeeds()
    {
        var input = @"
package test;

formation WithPort {
    input Fire ignition [5+];
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        var formation = result.Value.Formations.First();
        var port = formation.Statements.OfType<PortDefinition>().First();
        
        Assert.Equal(PortDirection.Input, port.Direction);
        Assert.Equal("Fire", port.ElementType);
        Assert.Equal("ignition", port.Name);
        Assert.IsType<MinAmplitude>(port.Amplitude);
        Assert.Equal(5, ((MinAmplitude)port.Amplitude).Value);
    }

    [Fact]
    public void Parse_FormationWithOutputPort_Succeeds()
    {
        var input = @"
package test;

formation WithPort {
    output Water flow;
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        var formation = result.Value.Formations.First();
        var port = formation.Statements.OfType<PortDefinition>().First();
        
        Assert.Equal(PortDirection.Output, port.Direction);
        Assert.Equal("Water", port.ElementType);
        Assert.Equal("flow", port.Name);
        Assert.Null(port.Amplitude);
    }

    [Fact]
    public void Parse_FormationWithNode_Succeeds()
    {
        var input = @"
package test;

formation WithNode {
    node SpiritStoneSocket source ( element: Fire, grade: High );
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        var formation = result.Value.Formations.First();
        var node = formation.Statements.OfType<NodeDefinition>().First();
        
        Assert.Equal("SpiritStoneSocket", node.TypeName);
        Assert.Equal("source", node.InstanceName);
        Assert.Equal(2, node.Parameters.Count());
        Assert.Equal("element", node.Parameters.First().Name);
        Assert.Equal("Fire", node.Parameters.First().Value);
    }

    [Fact]
    public void Parse_FormationWithConnection_Succeeds()
    {
        var input = @"
package test;

formation WithConnection {
    node SpiritStoneSocket src;
    node QiCapacitor cap;
    src.out -> cap.in;
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        var formation = result.Value.Formations.First();
        var conn = formation.Statements.OfType<ConnectionDefinition>().First();
        
        Assert.Equal("src", conn.Source.NodeName);
        Assert.Equal("out", conn.Source.PortName);
        Assert.Equal("cap", conn.Target.NodeName);
        Assert.Equal("in", conn.Target.PortName);
    }

    [Fact]
    public void Parse_WithImports_Succeeds()
    {
        var input = @"
package test;

import std.fire.FireStarter;
import std.earth.*;

formation Empty {
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        Assert.Equal(2, result.Value.Imports.Count());
        
        var import1 = result.Value.Imports.First();
        Assert.Equal("std.fire.FireStarter", import1.QualifiedId);
        Assert.False(import1.IsWildcard);
        
        var import2 = result.Value.Imports.Last();
        Assert.Equal("std.earth", import2.QualifiedId);
        Assert.True(import2.IsWildcard);
    }

    [Fact]
    public void Parse_AmplitudeRange_Succeeds()
    {
        var input = @"
package test;

formation WithRange {
    input Fire signal [3..10];
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        var port = result.Value.Formations.First().Statements.OfType<PortDefinition>().First();
        Assert.IsType<RangeAmplitude>(port.Amplitude);
        var range = (RangeAmplitude)port.Amplitude;
        Assert.Equal(3, range.Min);
        Assert.Equal(10, range.Max);
    }

    [Fact]
    public void Parse_ExactAmplitude_Succeeds()
    {
        var input = @"
package test;

formation WithExact {
    input Fire signal [5];
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        var port = result.Value.Formations.First().Statements.OfType<PortDefinition>().First();
        Assert.IsType<ExactAmplitude>(port.Amplitude);
        Assert.Equal(5, ((ExactAmplitude)port.Amplitude).Value);
    }

    [Fact]
    public void Parse_InvalidSyntax_Fails()
    {
        var input = @"
package test;

formation Broken {
    this is not valid syntax
}";
        var result = RunicParser.Parse(input);
        
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Parse_MultipleFormations_Succeeds()
    {
        var input = @"
package test;

formation First {
    input Fire a;
}

formation Second {
    output Water b;
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        Assert.Equal(2, result.Value.Formations.Count());
        Assert.Equal("First", result.Value.Formations.First().Name);
        Assert.Equal("Second", result.Value.Formations.Last().Name);
    }

    [Fact]
    public void Parse_NodeWithStringParameter_Succeeds()
    {
        var input = @"
package test;

formation WithString {
    node EffectEmitter emit ( type: ""Fireball"" );
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        var node = result.Value.Formations.First().Statements.OfType<NodeDefinition>().First();
        Assert.Equal("Fireball", node.Parameters.First().Value);
    }

    [Fact]
    public void Parse_NodeWithIntParameter_Succeeds()
    {
        var input = @"
package test;

formation WithInt {
    node QiCapacitor cap ( capacity: 50 );
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        var node = result.Value.Formations.First().Statements.OfType<NodeDefinition>().First();
        Assert.Equal(50, node.Parameters.First().Value);
    }

    [Fact]
    public void Parse_NodeWithNoParameters_Succeeds()
    {
        var input = @"
package test;

formation WithEmpty {
    node BurstTrigger trigger;
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        var node = result.Value.Formations.First().Statements.OfType<NodeDefinition>().First();
        Assert.Empty(node.Parameters);
    }

    [Fact]
    public void Parse_ComplexFormation_Succeeds()
    {
        var input = @"
package runic.examples;

formation CapacitorStrike {
    input Fire ignition [5+];
    output Earth magma_flow;

    node SpiritStoneSocket power_source ( element: Fire, grade: Medium );
    node QiCapacitor cap ( capacity: 50 );
    node BurstTrigger trigger;
    node EffectEmitter strike ( type: ""Fireball"" );

    power_source.out -> cap.in;
    cap.full -> trigger.trigger;
    cap.out -> trigger.capacitor;
    trigger.out -> strike.in;
}";
        var result = RunicParser.Parse(input);
        
        Assert.True(result.Success);
        Assert.Equal("runic.examples", result.Value.Package.QualifiedId);
        
        var formation = result.Value.Formations.First();
        Assert.Equal("CapacitorStrike", formation.Name);
        
        var ports = formation.Statements.OfType<PortDefinition>().ToList();
        Assert.Equal(2, ports.Count);
        
        var nodes = formation.Statements.OfType<NodeDefinition>().ToList();
        Assert.Equal(4, nodes.Count);
        
        var connections = formation.Statements.OfType<ConnectionDefinition>().ToList();
        Assert.Equal(4, connections.Count);
    }
}
