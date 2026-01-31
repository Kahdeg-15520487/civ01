using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Civ.Emulator;

public partial class EmulatorTests : Node
{
    public override void _Ready()
    {
        GD.Print("=== Starting Emulator Tests ===");
        
        try 
        {
            TestQiValue();
            TestGraphConstruction();
            TestSpiritStoneSimulation();
            GD.Print("=== ALL TESTS PASSED ===");
        }
        catch (Exception e)
        {
            GD.PrintErr($"=== TEST FAILED: {e.Message} ===");
            GD.PrintErr(e.StackTrace);
        }
    }

    private void TestQiValue()
    {
        GD.Print("Running TestQiValue...");
        
        var v1 = new QiValue(ElementType.Fire, 10);
        var v2 = new QiValue(ElementType.Fire, 10);
        var v3 = new QiValue(ElementType.Water, 5);

        Assert(v1 == v2, "Equal QiValues should be equal");
        Assert(v1 != v3, "Different QiValues should be not equal");
        Assert(v1.Magnitude == 10, "Magnitude should be preserved");
        Assert(v1.Type == ElementType.Fire, "Type should be preserved");
    }

    private void TestGraphConstruction()
    {
        GD.Print("Running TestGraphConstruction...");
        
        var graph = new RuneGraph();
        var nodeA = new SpiritStoneSource("SourceA");
        var testSink = new TestSinkNode("Sink1");
        
        graph.AddNode(nodeA);
        graph.AddNode(testSink);

        Assert(graph.Nodes.Count == 2, "Graph should have 2 nodes");

        // Connect SourceA.out -> Sink1.in
        var outPort = nodeA.Outputs[0];
        var inPort = testSink.Inputs[0];
        
        graph.Connect(outPort, inPort);
        Assert(graph.Wires.Count == 1, "Graph should have 1 wire");
    }

    private void TestSpiritStoneSimulation()
    {
        GD.Print("Running TestSpiritStoneSimulation...");
        
        var graph = new RuneGraph();
        var source = new SpiritStoneSource("Source1");
        var sink = new TestSinkNode("Sink1");
        
        graph.AddNode(source);
        graph.AddNode(sink);
        graph.Connect(source.Outputs[0], sink.Inputs[0]);

        // Tick 1
        foreach(var _ in graph.Tick()) {} 
        
        // After Tick 1:
        // 1. Reset (Input Empty)
        // 2. Transfer (Wire moves Output -> Input). source.Output was Empty at start. So Sink Input is Empty.
        // 3. Process loop. Source generates Fire(5) into Output.
        
        Assert(sink.Inputs[0].CurrentValue.IsEmpty, "Tick 1: Sink should receive nothing yet (propagation delay)");
        Assert(source.Outputs[0].CurrentValue.Magnitude == 5, "Tick 1: Source should emit 5 Fire");

        // Tick 2
        foreach(var _ in graph.Tick()) {}
        
        // After Tick 2:
        // 2. Transfer: Moves source.Output (Fire 5) to Sink.Input
        Assert(sink.Inputs[0].CurrentValue.Magnitude == 5, "Tick 2: Sink should receive 5 Fire");
        Assert(sink.Inputs[0].CurrentValue.Type == ElementType.Fire, "Tick 2: Received correct element");
    }

    private void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception($"Assertion Failed: {message}");
        }
    }
}

// Helper for testing
public class TestSinkNode : RuneNode
{
    public Port Input { get; }

    public TestSinkNode(string id) : base(id)
    {
        Input = AddInput("in");
    }

    public override IEnumerable<string> Process()
    {
        yield break;
    }
}
