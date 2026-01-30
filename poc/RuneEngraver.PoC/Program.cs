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
            Console.WriteLine("6. Capacitor Burst (Charge -> Discharge)");
            Console.WriteLine("7. Logic Routing (Yin-Yang & Filter)");
            Console.WriteLine("8. Splitter Distribution");
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
            case "6": RunCapacitorTest(); break;
            case "7": RunLogicGateTest(); break;
            case "8": RunSplitterTest(); break;
            default: Console.WriteLine("Invalid selection."); break;
        }
    }

    static void RunMatrixMultiplicationTest() {
        Console.WriteLine("\n--- Test 5: Neural Net (3 Sensors -> 1 Hidden -> 1 Output) ---");
        // Concept: Classify if environment is "Dangerous" based on 3 sensors
        // Inputs: Wood(10), Wood(5), Wood(0)
        // Weights: 0.5, 0.2, 0.8
        // Normalization: Transmute Wood -> Fire to prove we can process the signal
        // Sum: Fire(5) + Fire(1) + Fire(0) = Fire(6)
        
        var graph = new RuneGraph();

        // 1. Input Layer (Sensors - All Wood for this test to allow simple normalization)
        var s1 = new SpiritStoneSource("Sensor_A", new QiValue(ElementType.Wood, 10));
        var s2 = new SpiritStoneSource("Sensor_B", new QiValue(ElementType.Wood, 5));
        var s3 = new SpiritStoneSource("Sensor_C", new QiValue(ElementType.Wood, 0));

        // 2. Weights (Attenuators) - Representing resistance/filtering
        // Note: Factor must be <= 1.0. 
        var w1 = new AttenuatorNode("W_A", 0.5f);
        var w2 = new AttenuatorNode("W_B", 0.2f);
        var w3 = new AttenuatorNode("W_C", 0.8f);

        // 3. Normalization (Transmuters) - Convert Wood -> Fire
        // This proves we can manipulate the signal type before processing
        var t1 = new TransmuterNode("T_A");
        var t2 = new TransmuterNode("T_B");
        var t3 = new TransmuterNode("T_C");

        // 3b. Waste Management (Void Drains) - Handle excess from Attenuators
        var drain = new VoidDrain("Waste_Drain", 20);

        // 4. Summation (Hidden Neuron) - Using SpiritVessel as adder
        var summer = new SpiritVessel("Sum_Hidden");

        // 5. Activation (Relu/Threshold)
        var activation = new ThresholdGate("Activation", 5);
        
        // 6. Output
        var output = new StableEmitter("Alert_System");

        graph.AddNode(s1); graph.AddNode(s2); graph.AddNode(s3);
        graph.AddNode(w1); graph.AddNode(w2); graph.AddNode(w3);
        graph.AddNode(t1); graph.AddNode(t2); graph.AddNode(t3);
        graph.AddNode(drain);
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

        // Weight Excess -> Drain (Critical to prevent Qi Deviation)
        graph.Connect(w1.Outputs[1], drain.Inputs[0]); // Using index 1 (excess)
        graph.Connect(w2.Outputs[1], drain.Inputs[0]);
        graph.Connect(w3.Outputs[1], drain.Inputs[0]);

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

    static void RunCapacitorTest()
    {
        Console.WriteLine("\n--- Test 6: Capacitor Burst ---");
        var graph = new RuneGraph();

        // Source: Trickle charge (Fire 2)
        var src = new SpiritStoneSource("Trickle_Charger", new QiValue(ElementType.Fire, 2));
        
        // Capacitor: Threshold 10. Should take ~5 ticks to fill.
        var cap = new QiCapacitor("Capacitor", 10);
        
        // Burst Output
        var emit = new StableEmitter("Burst_Out");
        
        graph.AddNode(src);
        graph.AddNode(cap);
        graph.AddNode(emit);

        // Wiring
        graph.Connect(src.Outputs[0], cap.Inputs[0]);
        // Connect 'full' port to emitter
        graph.Connect(cap.Outputs[1], emit.Inputs[0]); // Index 1 is 'full'

        RunSimulation(graph, 8);
    }

    static void RunLogicGateTest()
    {
        Console.WriteLine("\n--- Test 7: Logic Routing ---");
        var graph = new RuneGraph();

        // Source
        var src = new SpiritStoneSource("Source", new QiValue(ElementType.Water, 5));
        
        // Control Signal (Yang/Positive)
        var ctrl = new SpiritStoneSource("Control_Signal", new QiValue(ElementType.Fire, 1));
        
        // Yin-Yang Gate
        var gate = new YinYangGate("Gate");
        
        // Output Paths
        var trueOut = new StableEmitter("True_Path");
        var falseOut = new StableEmitter("False_Path");

        graph.AddNode(src); graph.AddNode(ctrl);
        graph.AddNode(gate);
        graph.AddNode(trueOut); graph.AddNode(falseOut);

        // Wiring
        graph.Connect(src.Outputs[0], gate.Inputs[0]);      // In
        graph.Connect(ctrl.Outputs[0], gate.Inputs[1]);     // Cond
        
        graph.Connect(gate.Outputs[0], trueOut.Inputs[0]);  // True
        graph.Connect(gate.Outputs[1], falseOut.Inputs[0]); // False

        RunSimulation(graph, 3);
    }

    static void RunSplitterTest()
    {
        Console.WriteLine("\n--- Test 8: Splitter ---");
        var graph = new RuneGraph();

        var src = new SpiritStoneSource("Big_Flow", new QiValue(ElementType.Earth, 10));
        var split = new SplitterNode("Splitter");
        var out1 = new StableEmitter("Left");
        var out2 = new StableEmitter("Right");

        graph.AddNode(src);
        graph.AddNode(split);
        graph.AddNode(out1);
        graph.AddNode(out2);

        graph.Connect(src.Outputs[0], split.Inputs[0]);
        graph.Connect(split.Outputs[0], out1.Inputs[0]);
        graph.Connect(split.Outputs[1], out2.Inputs[0]);

        RunSimulation(graph, 3);
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
