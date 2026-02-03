using RuneEngraver.Compiler.Semantics;
using RuneEngraver.Compiler.Syntax;
using Xunit;
using System.Linq;

namespace Tests.Compiler;

public class ValidatorTests
{
    private SymbolTable CreateSymbolTable() => new SymbolTable();

    [Fact]
    public void Validate_ValidFormation_ReturnsNoErrors()
    {
        var input = @"
package test;

formation Valid {
    node SpiritStoneSocket src;
    node QiCapacitor cap;
    src.out -> cap.in;
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_UnknownNodeType_ReturnsError()
    {
        var input = @"
package test;

formation Invalid {
    node NonExistentNode thing;
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        Assert.Single(errors);
        Assert.Equal(ErrorCode.UnknownNodeType, errors[0].Code);
        Assert.Contains("NonExistentNode", errors[0].Message);
    }

    [Fact]
    public void Validate_InvalidImport_ReturnsError()
    {
        var input = @"
package test;

import std.fire.FireStarter;

formation Valid {
    node SpiritStoneSocket src;
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        Assert.Single(errors);
        Assert.Equal(ErrorCode.UnresolvedImport, errors[0].Code);
        Assert.Contains("std.fire.FireStarter", errors[0].Message);
    }

    [Fact]
    public void Validate_InvalidWildcardImport_ReturnsError()
    {
        var input = @"
package test;

import std.earth.*;

formation Valid {
    node SpiritStoneSocket src;
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        Assert.Single(errors);
        Assert.Equal(ErrorCode.UnresolvedPackage, errors[0].Code);
        Assert.Contains("std.earth", errors[0].Message);
    }

    [Fact]
    public void Validate_InvalidPortReference_ReturnsError()
    {
        var input = @"
package test;

formation Invalid {
    node SpiritStoneSocket src;
    node QiCapacitor cap;
    src.nonexistent -> cap.in;
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        Assert.Single(errors);
        Assert.Contains("nonexistent", errors[0].Message);
    }

    [Fact]
    public void Validate_InvalidNodeReference_ReturnsError()
    {
        var input = @"
package test;

formation Invalid {
    node SpiritStoneSocket src;
    unknown_node.out -> src.out;
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        Assert.Contains(errors, e => e.Message.Contains("unknown_node"));
    }

    [Fact]
    public void Validate_AllCoreNodes_Succeed()
    {
        var coreNodes = new[]
        {
            "SpiritStoneSocket", "StoneArray", "CultivatorLink", "TunedResonator",
            "AmplitudeRegulator", "QiCapacitor", "BurstTrigger", "QiReceptacle",
            "EffectEmitter", "VoidDrain", "HeatSink", "GroundingRod",
            "UnstableVent", "BacklashNode", "CorruptionSeep", "SpiritVessel",
            "DualVessel", "ElementalPool", "Transmuter", "Amplifier",
            "Dampener", "Attenuator", "Splitter", "Combiner",
            "YinYangGate", "ThresholdGate", "ElementFilter"
        };

        foreach (var nodeName in coreNodes)
        {
            var input = $@"
package test;

formation Test{nodeName} {{
    node {nodeName} instance;
}}";
            var result = RunicParser.Parse(input);
            Assert.True(result.Success, $"Failed to parse {nodeName}");
            
            var validator = new RunicValidator(CreateSymbolTable());
            var errors = validator.Validate(result.Value).ToList();
            
            Assert.Empty(errors);
        }
    }

    [Fact]
    public void Validate_ComplexFormation_Succeeds()
    {
        var input = @"
package runic.examples;

formation CapacitorStrike {
    input Fire ignition [5+];
    output Earth magma_flow;

    node SpiritStoneSocket power_source ( element: Fire, grade: Medium );
    node Amplifier amp ( factor: 2 );
    node Transmuter trans ( from: Fire, to: Earth );
    node QiCapacitor cap ( capacity: 50 );
    node BurstTrigger trigger;
    node EffectEmitter strike ( type: ""Fireball"" );

    power_source.out -> amp.primary;
    amp.out -> trans.in;
    trans.out -> cap.in;
    cap.full -> trigger.trigger;
    cap.out -> trigger.capacitor;
    trigger.out -> strike.in;
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_MultipleErrors_ReturnsAll()
    {
        var input = @"
package test;

import std.fire.*;
import std.water.*;

formation Invalid {
    node NonExistent1 a;
    node NonExistent2 b;
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        // Should have 4 errors: 2 imports + 2 unknown nodes
        Assert.Equal(4, errors.Count);
    }

    [Fact]
    public void Validate_EmptyFormation_Succeeds()
    {
        var input = @"
package test;

formation Empty {
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_MultipleFormations_ValidatesAll()
    {
        var input = @"
package test;

formation First {
    node SpiritStoneSocket src;
}

formation Second {
    node NonExistentNode bad;
}";
        var result = RunicParser.Parse(input);
        Assert.True(result.Success);
        
        var validator = new RunicValidator(CreateSymbolTable());
        var errors = validator.Validate(result.Value).ToList();
        
        Assert.Single(errors);
        Assert.Contains("NonExistentNode", errors[0].Message);
    }
}
