using System.Collections.Generic;
using System.Linq;
using RuneEngraver.Compiler.Semantics;
using RuneEngraver.Compiler.Synthesis;
using RuneEngraver.Compiler.Syntax;
using Xunit;

namespace Tests.Compiler;

public class SynthesisTests
{
    private SymbolTable CreateSymbolTable() => new SymbolTable();

    [Fact]
    public void Build_SimpleFormation_CreatesCorrectNodes()
    {
        var input = @"
package test;

formation Test {
    node SpiritStoneSocket src ( element: Fire );
    node Amplifier amp ( factor: 2 );
}";
        var unit = RunicParser.Parse(input).Value;
        var builder = new GraphBuilder(CreateSymbolTable());
        var graph = builder.Build(unit.Formations.First(), unit);

        Assert.Equal(2, graph.Nodes.Count);
        
        var src = graph.Nodes.First(n => n.Id == "src");
        Assert.Equal("SpiritStoneSocket", src.Type);
        Assert.Equal("Fire", src.Params["element"].ToString());

        var amp = graph.Nodes.First(n => n.Id == "amp");
        Assert.Equal("Amplifier", amp.Type);
        Assert.Equal(2, (int)amp.Params["factor"]);
    }

    [Fact]
    public void Build_Connections_CreatesCorrectEdges()
    {
        var input = @"
package test;

formation Test {
    node SpiritStoneSocket src;
    node Amplifier amp;
    src.out -> amp.primary;
}";
        var unit = RunicParser.Parse(input).Value;
        var builder = new GraphBuilder(CreateSymbolTable());
        var graph = builder.Build(unit.Formations.First(), unit);

        Assert.Single(graph.Edges);
        var edge = graph.Edges[0];
        Assert.Equal("src.out", edge.From);
        Assert.Equal("amp.primary", edge.To);
    }

    [Fact]
    public void Build_FormationPorts_CreatesInterfaceNodes()
    {
        var input = @"
package test;

formation Test {
    input Fire ignition;
    output Earth magma;
}";
        var unit = RunicParser.Parse(input).Value;
        var builder = new GraphBuilder(CreateSymbolTable());
        var graph = builder.Build(unit.Formations.First(), unit);

        Assert.Equal(2, graph.Nodes.Count);

        var inputNode = graph.Nodes.First(n => n.Id == "ignition");
        Assert.Equal("FormationInput", inputNode.Type);
        Assert.Equal("Fire", inputNode.Params["element"]);

        var outputNode = graph.Nodes.First(n => n.Id == "magma");
        Assert.Equal("FormationOutput", outputNode.Type);
        Assert.Equal("Earth", outputNode.Params["element"]);
    }

    [Fact]
    public void Build_LocalPortConnections_ResolvesCorrectly()
    {
        var input = @"
package test;

formation Test {
    input Fire ignition;
    node Amplifier amp;
    output Fire blast;

    ignition -> amp.primary;
    amp.out -> blast;
}";
        var result = RunicParser.ParseWithErrors(input);
        Assert.True(result.Success, $"Parse failed: {result.Error}");
        var unit = result.Value!;
        var builder = new GraphBuilder(CreateSymbolTable());
        var graph = builder.Build(unit.Formations.First(), unit);

        Assert.Equal(2, graph.Edges.Count);

        var inEdge = graph.Edges.First(e => e.From == "ignition");
        Assert.Equal("amp.primary", inEdge.To);

        var outEdge = graph.Edges.First(e => e.To == "blast");
        Assert.Equal("amp.out", outEdge.From);
    }
}
