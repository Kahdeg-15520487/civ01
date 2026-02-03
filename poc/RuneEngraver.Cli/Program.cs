using System;
using RuneEngraver.Compiler.Syntax;

namespace RuneEngraver.Cli;

class Program
{
    static void Main(string[] args)
    {
        var input = @"
package runic.examples;

import std.fire.FireStarter;
import std.earth.*;

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

        Console.WriteLine("Parsing RunicHDL...");
        var result = RunicParser.Parse(input);

        if (result.Success)
        {
            Console.WriteLine("Parse Successful!");
            var unit = result.Value;
            Console.WriteLine($"Package: {unit.Package.QualifiedId}");
            Console.WriteLine($"Imports: {unit.Imports.Count()}");
            foreach(var formation in unit.Formations)
            {
                Console.WriteLine($"Formation: {formation.Name}");
                Console.WriteLine($"Statements: {formation.Statements.Count()}");
            }
        }
        else
        {
            Console.WriteLine("Parse Failed!");
            Console.WriteLine(result.Error);
        }
    }
}
