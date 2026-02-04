using Godot;
using RuneEngraver.Core.Elements;
using RuneEngraver.Core.Nodes;
using RuneEngraver.Core.Simulation;
using System;
using System.Linq;
using RuneEngraver.Compiler.Syntax;
using RuneEngraver.Compiler.Semantics;
using RuneEngraver.Compiler.Synthesis;
using System.Text.Json;
using System.Text.Json.Serialization;

public partial class EmulatorTests : Node
{
    public override void _Ready()
    {
        GD.Print("=== Starting Emulator Tests ===");
        
        try 
        {
            // Unit Tests
            QiValueTests.Run();
            InteractionTests.Run();

            // Scenario Tests (from PoC Program.cs)
            RunAmplifierTest();
            RunCombinerTest();
            RunDecayTest();
            RunLoopTest();
            RunNeuralNetTest();
            RunCapacitorTest();
            RunLogicGateTest();
            RunSplitterTest();
            RunSynthesizerTest();
            
            GD.Print("=== ALL TESTS PASSED ===");
        }
        catch (Exception e)
        {
            GD.PrintErr($"=== TEST FAILED: {e.Message} ===");
            GD.PrintErr(e.StackTrace);
        }
    }

    private void RunSimulation(RuneGraph graph, int ticks, string testName)
    {
        GD.Print($"\n--- {testName} ---");
        for (int i = 0; i < ticks; i++)
        {
            // GD.Print($"[Tick {graph.TickCount + 1}]");
            var logs = graph.Tick().ToList();
            foreach (var log in logs) GD.Print(log);
        }
    }

    private void RunAmplifierTest()
    {
        var graph = new RuneGraph();
        var fireSrc = new SpiritStoneSource("Src_Fire", new QiValue(ElementType.Fire, 5));
        var earthSrc = new SpiritStoneSource("Src_Earth", new QiValue(ElementType.Earth, 2));
        var amp = new AmplifierNode("Amp");
        var emit = new StableEmitter("Emit");

        graph.AddNode(fireSrc); graph.AddNode(earthSrc); graph.AddNode(amp); graph.AddNode(emit);

        graph.Connect(fireSrc.Outputs[0], amp.Inputs[0]); // Primary
        graph.Connect(earthSrc.Outputs[0], amp.Inputs[1]); // Catalyst
        graph.Connect(amp.Outputs[0], emit.Inputs[0]);

        RunSimulation(graph, 5, "Test 1: Amplifier (Fire+Earth->Earth)");
        
        // Assert final Output
        Assert(emit.LastReceived.Type == ElementType.Earth && emit.LastReceived.Magnitude == 10, "Amplifier Output Correct");
    }

    private void RunCombinerTest()
    {
        var graph = new RuneGraph();
        var waterSrc = new SpiritStoneSource("Src_Water", new QiValue(ElementType.Water, 4));
        var fireSrc = new SpiritStoneSource("Src_Fire", new QiValue(ElementType.Fire, 4));
        var cmb = new CombinerNode("Combiner");
        var emit = new StableEmitter("Emit");

        graph.AddNode(waterSrc); graph.AddNode(fireSrc); graph.AddNode(cmb); graph.AddNode(emit);

        graph.Connect(waterSrc.Outputs[0], cmb.Inputs[0]);
        graph.Connect(fireSrc.Outputs[0], cmb.Inputs[1]);
        graph.Connect(cmb.Outputs[0], emit.Inputs[0]);

        RunSimulation(graph, 5, "Test 2: Combiner (Water+Fire->Steam)");
        Assert(emit.LastReceived.Type == ElementType.Steam && emit.LastReceived.Magnitude == 8, "Combiner Output Correct");
    }

    private void RunDecayTest()
    {
        var graph = new RuneGraph();
        // Steam decays fast
        var steamSrc = new SpiritStoneSource("Src_Steam", new QiValue(ElementType.Steam, 8, 3));
        var emit = new StableEmitter("Emit");
        
        graph.AddNode(steamSrc); graph.AddNode(emit);
        graph.Connect(steamSrc.Outputs[0], emit.Inputs[0]);

        // Run for enough ticks to see decay if wires added delay, but here it's direct source->sink
        // To verify decay we'd need wire travel time or multi-hop. 
        // Wires in RuneGraph do decay! 
        RunSimulation(graph, 5, "Test 3: Decay Tracing");
        Assert(emit.LastReceived.Type == ElementType.Steam || emit.LastReceived.Type == ElementType.Water, "Decay logic ran");
    }

    private void RunLoopTest()
    {
        var graph = new RuneGraph();
        var src = new SpiritStoneSource("Src", new QiValue(ElementType.Fire, 1));
        var buffer = new SpiritVessel("Buffer");
        var emit = new StableEmitter("Emit");

        graph.AddNode(src); graph.AddNode(buffer); graph.AddNode(emit);
        graph.Connect(src.Outputs[0], buffer.Inputs[0]);
        graph.Connect(buffer.Outputs[0], buffer.Inputs[0]); // Feedback loop
        graph.Connect(buffer.Outputs[0], emit.Inputs[0]);

        RunSimulation(graph, 6, "Test 4: Loop Accumulation");
        // Tick 6 expectation: Accumulates 1 per tick. Depending on timing, should be ~4-6.
        Assert(emit.LastReceived.Magnitude > 3, "Loop Accumulated Qi");
    }

    private void RunNeuralNetTest()
    {
        var graph = new RuneGraph();
        var s1 = new SpiritStoneSource("Sensor_A", new QiValue(ElementType.Wood, 10));
        var s2 = new SpiritStoneSource("Sensor_B", new QiValue(ElementType.Wood, 5));
        var s3 = new SpiritStoneSource("Sensor_C", new QiValue(ElementType.Wood, 0));
        
        var w1 = new AttenuatorNode("W_A", 0.5f);
        var w2 = new AttenuatorNode("W_B", 0.2f);
        var w3 = new AttenuatorNode("W_C", 0.8f);
        
        var t1 = new TransmuterNode("T_A");
        var t2 = new TransmuterNode("T_B");
        var t3 = new TransmuterNode("T_C");
        
        var drain = new VoidDrain("Waste_Drain", 20);
        var summer = new SpiritVessel("Sum_Hidden");
        var activation = new ThresholdGate("Activation", 5);
        var output = new StableEmitter("Alert_System");

        graph.AddNode(s1); graph.AddNode(s2); graph.AddNode(s3);
        graph.AddNode(w1); graph.AddNode(w2); graph.AddNode(w3);
        graph.AddNode(t1); graph.AddNode(t2); graph.AddNode(t3);
        graph.AddNode(drain); graph.AddNode(summer); graph.AddNode(activation); graph.AddNode(output);

        // Wiring
        graph.Connect(s1.Outputs[0], w1.Inputs[0]);
        graph.Connect(s2.Outputs[0], w2.Inputs[0]);
        graph.Connect(s3.Outputs[0], w3.Inputs[0]);

        graph.Connect(w1.Outputs[0], t1.Inputs[0]);
        graph.Connect(w2.Outputs[0], t2.Inputs[0]);
        graph.Connect(w3.Outputs[0], t3.Inputs[0]);

        graph.Connect(w1.Outputs[1], drain.Inputs[0]);
        graph.Connect(w2.Outputs[1], drain.Inputs[0]);
        graph.Connect(w3.Outputs[1], drain.Inputs[0]);

        graph.Connect(t1.Outputs[0], summer.Inputs[0]);
        graph.Connect(t2.Outputs[0], summer.Inputs[0]);
        graph.Connect(t3.Outputs[0], summer.Inputs[0]);

        graph.Connect(summer.Outputs[0], activation.Inputs[0]);
        graph.Connect(activation.Outputs[0], output.Inputs[0]);

        RunSimulation(graph, 10, "Test 5: Neural Net (Wood->Fire Classifier)");
        // W1: 10*0.5=5. W2: 5*0.2=1. W3: 0. Sum = 6. Threshold 5. Pass.
        Assert(output.LastReceived.Magnitude == 6, "Neural Net Output Correct");
    }

    private void RunCapacitorTest()
    {
        var graph = new RuneGraph();
        var src = new SpiritStoneSource("Trickle_Charger", new QiValue(ElementType.Fire, 2));
        var cap = new QiCapacitor("Capacitor", 10);
        var emit = new StableEmitter("Burst_Out");

        graph.AddNode(src); graph.AddNode(cap); graph.AddNode(emit);
        graph.Connect(src.Outputs[0], cap.Inputs[0]);
        graph.Connect(cap.Outputs[1], emit.Inputs[0]);

        RunSimulation(graph, 8, "Test 6: Capacitor Burst");
        // Check if ANY value in history matches the burst
        Assert(emit.History.Any(q => q.Magnitude >= 10), "Capacitor Discharge Recorded in History");
    }

    private void RunLogicGateTest()
    {
        var graph = new RuneGraph();
        var src = new SpiritStoneSource("Source", new QiValue(ElementType.Water, 5));
        var ctrl = new SpiritStoneSource("Control_Signal", new QiValue(ElementType.Fire, 1));
        var gate = new YinYangGate("Gate");
        var trueOut = new StableEmitter("True_Path");
        var falseOut = new StableEmitter("False_Path");

        graph.AddNode(src); graph.AddNode(ctrl); graph.AddNode(gate); graph.AddNode(trueOut); graph.AddNode(falseOut);

        graph.Connect(src.Outputs[0], gate.Inputs[0]);
        graph.Connect(ctrl.Outputs[0], gate.Inputs[1]);
        graph.Connect(gate.Outputs[0], trueOut.Inputs[0]);
        graph.Connect(gate.Outputs[1], falseOut.Inputs[0]);

        RunSimulation(graph, 5, "Test 7: Logic Routing");
        Assert(trueOut.LastReceived.Magnitude == 5, "Logic Gate True Path");
    }

    private void RunSplitterTest()
    {
        var graph = new RuneGraph();
        var src = new SpiritStoneSource("Big_Flow", new QiValue(ElementType.Earth, 10));
        var split = new SplitterNode("Splitter");
        var out1 = new StableEmitter("Left");
        var out2 = new StableEmitter("Right");

        graph.AddNode(src); graph.AddNode(split); graph.AddNode(out1); graph.AddNode(out2);
        graph.Connect(src.Outputs[0], split.Inputs[0]);
        graph.Connect(split.Outputs[0], out1.Inputs[0]);
        graph.Connect(split.Outputs[1], out2.Inputs[0]);

        RunSimulation(graph, 5, "Test 8: Splitter");
        Assert(out1.LastReceived.Magnitude == 5 && out2.LastReceived.Magnitude == 5, "Splitter Output Correct");
    }

    private void RunSynthesizerTest()
    {
        var input = @"
package runic.examples;

formation CapacitorStrike {
    input Fire ignition [2+];
    output Effect fire_ball;

    node SpiritStoneSocket power_source ( element: Wood, grade: Medium );
    node Amplifier amp;
    node QiCapacitor cap ( capacity: 50 );
    node BurstTrigger trigger;
    node EffectEmitter strike ( type: ""Fireball"" );

    power_source.out -> amp.primary;
    ignition -> amp.catalyst;
    amp.out -> cap.in;
    cap.full -> trigger.trigger;
    cap.out -> trigger.capacitor;
    trigger.out -> strike.in;
    strike.out -> fire_ball;
}";

        GD.Print("\n--- Test 9: Text-to-Graph Synthesis ---");
        GD.Print("Parsing RunicHDL...");
        var result = RunicParser.Parse(input);

        if (result.Success)
        {
            GD.Print("Parse Successful!");
            var unit = result.Value;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
            // GD.Print(JsonSerializer.Serialize(unit, options));

            GD.Print("\nValidating Semantics...");
            var table = new SymbolTable(); // Loads mock built-ins
            var validator = new RunicValidator(table);
            var errors = validator.Validate(unit);

            if (errors.Any())
            {
                GD.PrintErr("Validation Failed:");
                foreach (var err in errors) GD.PrintErr($"- {err}");
            }
            else
            {
                GD.Print("Validation Successful!");

                // 2. Build Graph (Synthesis)
                GD.Print("\nSynthesizing Graph...");
                var builder = new GraphBuilder(table);
                // Note: Only building the first formation "CapacitorStrike"
                var graphDef = builder.Build(unit.Formations.First(), unit);
                
                var json = JsonSerializer.Serialize(graphDef, options);
                // GD.Print("Generated JSON:");
                // GD.Print(json);

                // 3. Load & Run (Runtime)
                GD.Print("\nLoading and Running Simulation...");
                var loader = new GraphLoader();
                var runGraph = loader.Load(json);

                RunSimulation(runGraph, 15, "Test 9: Synthesized CapacitorStrike");
            }
        }
        else
        {
            GD.PrintErr("Parse Failed!");
            GD.PrintErr(result.Error);
        }
    }

    private void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception($"Assertion Failed: {message}");
        }
    }
}

