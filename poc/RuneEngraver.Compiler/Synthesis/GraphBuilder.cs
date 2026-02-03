using System;
using System.Collections.Generic;
using System.Linq;
using RuneEngraver.Compiler.Syntax;
using RuneEngraver.Compiler.Semantics;

namespace RuneEngraver.Compiler.Synthesis;

public class GraphBuilder
{
    private readonly SymbolTable _symbolTable;

    public GraphBuilder(SymbolTable symbolTable)
    {
        _symbolTable = symbolTable;
    }

    /// <summary>
    /// Builds a flat GraphDefinition from a Formation.
    /// Note: Currently only supports single-formation compilation (the "Entry Point").
    /// </summary>
    public GraphDefinition Build(FormationDefinition entryFormation, CompilationUnit unit)
    {
        var graph = new GraphDefinition();
        
        // 1. Process Nodes
        foreach (var nodeDef in entryFormation.Statements.OfType<NodeDefinition>())
        {
            var nodeInstance = new NodeInstance
            {
                Id = nodeInstanceName(nodeDef.InstanceName),
                Type = nodeDef.TypeName,
                Params = ExtractParams(nodeDef)
            };
            graph.Nodes.Add(nodeInstance);
        }

        // 2. Process Connections
        foreach (var connDef in entryFormation.Statements.OfType<ConnectionDefinition>())
        {
            var edge = new EdgeInstance
            {
                From = ResolvePortId(connDef.Source),
                To = ResolvePortId(connDef.Target)
            };
            graph.Edges.Add(edge);
        }

        // 3. Process Input/Output Ports of the Formation (as "Source" and "Sink" nodes?)
        // For PoC, the Formation IS the Graph. 
        // Inputs/Outputs of the formation usually interface with the Test Runner harness.
        // We might want to represent them as special nodes in the graph if we want the runner to bind to them.
        foreach (var portDef in entryFormation.Statements.OfType<PortDefinition>())
        {
            // e.g. "input Fire ignition [5+]" -> Treated as a SpiritStoneSource or similar for testing?
            // Or just metadata?
            // For now, let's create "Interface Nodes" so the graph is self-contained.
            
            if (portDef.Direction == PortDirection.Input)
            {
                // Create a Source Node
                var node = new NodeInstance
                {
                    Id = portDef.Name,
                    Type = "FormationInput",
                    Params = new Dictionary<string, object>
                    {
                         { "element", portDef.ElementType },
                         { "amplitude", portDef.Amplitude?.ToString() ?? "Exact(1)" } // Simplify amplitude for now
                    }
                };
                graph.Nodes.Add(node);
            }
            else
            {
                // Create a Sink Node
                var node = new NodeInstance
                {
                    Id = portDef.Name,
                    Type = "FormationOutput",
                    Params = new Dictionary<string, object>
                    {
                        { "element", portDef.ElementType }
                    }
                };
                graph.Nodes.Add(node);
            }
        }

        return graph;
    }

    private string nodeInstanceName(string originalName) => originalName;

    private Dictionary<string, object> ExtractParams(NodeDefinition node)
    {
        var dict = new Dictionary<string, object>();
        foreach (var param in node.Parameters)
        {
            dict[param.Name] = param.Value; // AST Value is already object (string, int, bool)
        }
        return dict;
    }

    private string ResolvePortId(PortReference portRef)
    {
        // source/target can be:
        // 1. "node.port" -> standard connection
        // 2. "port" -> connection to local Formation port (Input/Output)

        if (portRef.PortName != null)
        {
            return $"{portRef.NodeName}.{portRef.PortName}";
        }

        // It is a local port reference (e.g. "ignition" or "magma_flow")
        // We need to know if it's an Input or Output to attach the correct default port.
        // Input "ignition" behaves like a Source, so it provides "ignition.out"
        // Output "magma_flow" behaves like a Sink, so it accepts "magma_flow.in"
        
        // Lookup in SymbolTable not needed if we assume standard behavior:
        // If it is on the Left Side of -> (Source), it must be an Output Port of a Node OR an Input Port of the Formation.
        // If it is on the Right Side of -> (Target), it must be an Input Port of a Node OR an Output Port of the Formation.
        
        // But `ResolvePortId` doesn't know context (Src vs Target). 
        // We should fix this by looking at what `portRef.NodeName` refers to in the Formation.
        // BUT we don't have the formation context here easily unless we pass it or lookup.
        
        // Simple heuristic: 
        // We will suffix it in the GraphDefinition simply as "nodeId.portId".
        // The Loader knows "FormationInput" instances have an "out" port, and "FormationOutput" instances have an "in" port.
        // So we just need to know WHICH one it is.
        // Let's pass the direction in via a helper or just append based on usage if possible. Or cleaner:
        // We didn't save the Direction map. Let's assume the Runtime handles "ignition" -> "ignition.out" auto-resolution?
        // No, explicitness is better.
        
        return $"{portRef.NodeName}"; 
    }
}

