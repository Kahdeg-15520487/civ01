using System;
using System.Collections.Generic;
using System.Linq;
using RuneEngraver.PoC.Core.Elements;
using RuneEngraver.PoC.Core.Nodes;
using RuneEngraver.PoC.Core.Simulation;

namespace RuneEngraver.PoC;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Rune Engraver PoC ===");
        
        if (args.Length > 0)
        {
            RunTest(args[0]);
            return;
        }

        while (true)
        {
            Console.WriteLine("\nSelect Test Scenario:");
            Console.WriteLine("1. Amplifier Logic (Fire + Earth -> Earth)");
            Console.WriteLine("2. Combiner Logic (Water + Fire -> Steam)");
            Console.WriteLine("3. Decay Tracing (Steam Decay)");
            Console.WriteLine("4. Loop Accumulation (Feedback Loop)");
            Console.WriteLine("5. Neural Net (3-1-1 Sensor Classifier)");
            Console.WriteLine("Q. Quit");
            Console.Write("> ");
            
            var key = Console.ReadLine()?.Trim().ToUpper();
            if (key == "Q") break;
            
            RunTest(key);
        }
    }

    static void RunTest(string key)
    {
        switch (key)
        {
            case "1": RunAmplifierTest(); break;
            case "2": RunCombinerTest(); break;
            case "3": RunDecayTest(); break;
            case "4": RunLoopTest(); break;
            case "5": RunMatrixMultiplicationTest(); break;
            default: Console.WriteLine("Invalid selection."); break;
        }
    }

    static void RunMatrixMultiplicationTest() {
        Console.WriteLine("\n--- Test 5: Neural Net (3 Sensors -> 1 Hidden -> 1 Output) ---");
        // Concept: Classify if environment is "Dangerous" based on 3 sensors
        // Inputs: Heat(10), Wind(5), Gas(0)
        // Weights: Heat(0.5), Wind(0.2), Gas(0.8)
        // Hidden Layer Sum: 10*0.5 + 5*0.2 + 0*0.8 = 5 + 1 + 0 = 6
        // Activation: Threshold 5 (If Sum >= 5, Emit Signal)
        
        var graph = new RuneGraph();

        // 1. Input Layer (Sensors)
        var s1 = new SpiritStoneSource("Heat_Sensor", new QiValue(ElementType.Fire, 10));
        var s2 = new SpiritStoneSource("Wind_Sensor", new QiValue(ElementType.Wind, 5));
        var s3 = new SpiritStoneSource("Gas_Sensor",  new QiValue(ElementType.Wood, 0)); // No Gas

        // 2. Weights (Scalers)
        var w1 = new ScalerNode("W_Heat", 0.5f);
        var w2 = new ScalerNode("W_Wind", 0.2f);
        var w3 = new ScalerNode("W_Gas",  0.8f);

        // 3. Normalization (Transmuters) - Convert all to Lightning for summation
        var t1 = new TransmuterNode("T_Heat", ElementType.Lightning);
        var t2 = new TransmuterNode("T_Wind", ElementType.Lightning);
        var t3 = new TransmuterNode("T_Gas", ElementType.Lightning);

        // 4. Summation (Hidden Neuron) - Using SpiritVessel as adder
        var summer = new SpiritVessel("Sum_Hidden");

        // 5. Activation (Relu/Threshold)
        var activation = new ThresholdGate("Activation", 5);
        
        // 6. Output
        var output = new StableEmitter("Alert_System");

        graph.AddNode(s1); graph.AddNode(s2); graph.AddNode(s3);
        graph.AddNode(w1); graph.AddNode(w2); graph.AddNode(w3);
        graph.AddNode(t1); graph.AddNode(t2); graph.AddNode(t3);
        graph.AddNode(summer);
        graph.AddNode(activation);
        graph.AddNode(output);

        // Wiring
        // Input -> Weight
        graph.Connect(s1.Outputs[0], w1.Inputs[0]);
        graph.Connect(s2.Outputs[0], w2.Inputs[0]);
        graph.Connect(s3.Outputs[0], w3.Inputs[0]);

        // Weight -> Transmuter
        graph.Connect(w1.Outputs[0], t1.Inputs[0]);
        graph.Connect(w2.Outputs[0], t2.Inputs[0]);
        graph.Connect(w3.Outputs[0], t3.Inputs[0]);

        // Transmuter -> Summer (All are Lightning now, so they merge additively)
        graph.Connect(t1.Outputs[0], summer.Inputs[0]);
        graph.Connect(t2.Outputs[0], summer.Inputs[0]);
        graph.Connect(t3.Outputs[0], summer.Inputs[0]);

        // Summer -> Activation
        graph.Connect(summer.Outputs[0], activation.Inputs[0]);

        // Activation -> Output
        graph.Connect(activation.Outputs[0], output.Inputs[0]);
        
        RunSimulation(graph, 8); // Need more ticks for extra layer
    }

    static void RunAmplifierTest()
    {
        Console.WriteLine("\n--- Test 1: Amplifier (Gen Cycle) ---");
        var graph = new RuneGraph();

        // Nodes
        var fireSrc = new SpiritStoneSource("Src_Fire", new QiValue(ElementType.Fire, 5));
        var earthSrc = new SpiritStoneSource("Src_Earth", new QiValue(ElementType.Earth, 2));
        var amp = new AmplifierNode("Amp");
        var emit = new StableEmitter("Emit");

        graph.AddNode(fireSrc);
        graph.AddNode(earthSrc);
        graph.AddNode(amp);
        graph.AddNode(emit);

        // Wiring
        // Fire generates Earth. Fire(5) > Earth(2). Should amp Earth.
        graph.Connect(fireSrc.Outputs[0], amp.Inputs[0]); // Primary
        graph.Connect(earthSrc.Outputs[0], amp.Inputs[1]); // Catalyst
        graph.Connect(amp.Outputs[0], emit.Inputs[0]);

        RunSimulation(graph, 5);
    }

    static void RunCombinerTest()
    {
        Console.WriteLine("\n--- Test 2: Combiner (Water + Fire) ---");
        var graph = new RuneGraph();

        var waterSrc = new SpiritStoneSource("Src_Water", new QiValue(ElementType.Water, 4));
        var fireSrc = new SpiritStoneSource("Src_Fire", new QiValue(ElementType.Fire, 4));
        var cmb = new CombinerNode("Combiner");
        var emit = new StableEmitter("Emit");

        graph.AddNode(waterSrc);
        graph.AddNode(fireSrc);
        graph.AddNode(cmb);
        graph.AddNode(emit);

        graph.Connect(waterSrc.Outputs[0], cmb.Inputs[0]);
        graph.Connect(fireSrc.Outputs[0], cmb.Inputs[1]);
        graph.Connect(cmb.Outputs[0], emit.Inputs[0]);

        RunSimulation(graph, 5);
    }

    static void RunDecayTest()
    {
        Console.WriteLine("\n--- Test 3: Decay Tracing ---");
        var graph = new RuneGraph();

        // Source emits Steam directly (cheating for test) to see it decay
        var steamSrc = new SpiritStoneSource("Src_Steam", new QiValue(ElementType.Steam, 8, 3));
        
        // Chain of "Wire Segments" (using dummy Repeaters/Emitters) to simulate time/distance
        // Actually, just looping it through logic nodes adds tick delay.
        // Let's just have Source -> Emit. 
        // Tick 1: Source emits Steam(TTL3).
        // Tick 2: Wire transfers -> decays to TTL2 -> Emitter receives TTL2.
        
        var emit = new StableEmitter("Emit");
        graph.AddNode(steamSrc);
        graph.AddNode(emit);
        
        graph.Connect(steamSrc.Outputs[0], emit.Inputs[0]);

        // We run for more ticks to see steady state
        RunSimulation(graph, 5);
    }

    static void RunLoopTest()
    {
        Console.WriteLine("\n--- Test 4: Loop Accumulation ---");
        var graph = new RuneGraph();

        // Source: Constant Fire(1)
        var src = new SpiritStoneSource("Src", new QiValue(ElementType.Fire, 1));
        
        // Node: Spirit Vessel acting as buffer/adder
        // It receives Input (from Source) AND its own Output (via feedback)
        var buffer = new SpiritVessel("Buffer");
        
        var emit = new StableEmitter("Emit");

        graph.AddNode(src);
        graph.AddNode(buffer);
        graph.AddNode(emit);

        // Wiring
        graph.Connect(src.Outputs[0], buffer.Inputs[0]);      // Source -> Buffer
        graph.Connect(buffer.Outputs[0], buffer.Inputs[0]);   // Feedback: Buffer Out -> Buffer In
        graph.Connect(buffer.Outputs[0], emit.Inputs[0]);     // Buffer Out -> Emit

        /* Expected:
           Tick 1: Src outputs 1. Buffer empty.
           Tick 2: Wire transfers Src(1) to Buffer. Buffer processes -> Output(1).
           Tick 3: 
             - Wire Src -> Buffer (1)
             - Wire Buffer -> Buffer (1) -> MERGE -> Buffer In (2)
             - Buffer processes -> Output(2)
           Tick 4: In(1 + 2) -> Out(3)
        */

        RunSimulation(graph, 6);
    }

    static void RunSimulation(RuneGraph graph, int ticks)
    {
        for (int i = 0; i < ticks; i++)
        {
            Console.WriteLine($"\n[Tick {graph.TickCount + 1}]");
            var logs = graph.Tick().ToList();
            if (logs.Count == 0) Console.WriteLine("(No Activity)");
            else foreach (var log in logs) Console.WriteLine(log);
            
            // Wait for user or auto
            // System.Threading.Thread.Sleep(500);
        }
    }
}
