using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using RuneEngraver.Compiler.Syntax;
using RuneEngraver.Compiler.Semantics;

namespace RuneEngraver.Cli;

class Program
{
    static void Main(string[] args)
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

        Console.WriteLine("Parsing RunicHDL...");
        var result = RunicParser.Parse(input);

        if (result.Success)
        {
            Console.WriteLine("Parse Successful!");
            var unit = result.Value;
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
            Console.WriteLine(JsonSerializer.Serialize(unit, options));

            Console.WriteLine("\nValidating Semantics...");
            var table = new SymbolTable(); // Loads mock built-ins
            var validator = new RunicValidator(table);
            var errors = validator.Validate(unit);
            
            if (errors.Any())
            {
                Console.WriteLine("Validation Failed:");
                foreach(var err in errors) Console.WriteLine($"- {err}");
            }
            else
            {
                Console.WriteLine("Validation Successful!");
            }
        }
        else
        {
            Console.WriteLine("Parse Failed!");
            Console.WriteLine(result.Error);
        }
    }
}
