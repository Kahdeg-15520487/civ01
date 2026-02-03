using System;
using System.Collections.Generic;
using System.Linq;
using RuneEngraver.Compiler.Syntax;

namespace RuneEngraver.Compiler.Semantics;

public class RunicValidator
{
    private readonly SymbolTable _symbolTable;
    private readonly List<string> _errors = new();

    public RunicValidator(SymbolTable symbolTable)
    {
        _symbolTable = symbolTable;
    }

    public IEnumerable<string> Validate(CompilationUnit unit)
    {
        _errors.Clear();

        // 0. Validate Imports
        foreach (var import in unit.Imports)
        {
            ValidateImport(import);
        }

        foreach (var formation in unit.Formations)
        {
            ValidateFormation(formation, unit);
        }

        return _errors;
    }

    private void ValidateFormation(FormationDefinition formation, CompilationUnit unit)
    {
        var scope = new Dictionary<string, FormationDefinition>();

        // 1. Register local nodes
        foreach (var node in formation.Statements.OfType<NodeDefinition>())
        {
            if (ResolveNode(node.TypeName, unit, out var def))
            {
                scope[node.InstanceName] = def;
            }
            else
            {
                _errors.Add($"Error: Unknown node type '{node.TypeName}' for instance '{node.InstanceName}'.");
            }
        }

        // 2. Validate Connections
        foreach (var conn in formation.Statements.OfType<ConnectionDefinition>())
        {
            ValidateConnection(conn, scope, formation); // pass formation for local ports
        }
    }

    private void ValidateImport(ImportStatement import)
    {
        if (import.IsWildcard)
        {
            // Check if package exists
            if (!_symbolTable.HasPackage(import.QualifiedId))
            {
                _errors.Add($"Error: Imported package '{import.QualifiedId}' does not exist or has no visible members.");
            }
        }
        else
        {
            // Check if specific symbol exists
            if (!_symbolTable.IsDefined(import.QualifiedId))
            {
                 _errors.Add($"Error: Imported symbol '{import.QualifiedId}' could not be resolved.");
            }
        }
    }

    private bool ResolveNode(string typeName, CompilationUnit unit, out FormationDefinition def)
    {
        // 1. Try fully qualified lookup (if already fully qualified)
        if (_symbolTable.TryLookup(typeName, out def)) return true;

        // 2. Try implicit 'core' package
        if (_symbolTable.TryLookup($"core.{typeName}", out def)) return true;

        // 3. Try imported packages
        // Simple logic: if typeName matches a suffix of an import
        foreach (var import in unit.Imports)
        {
            if (import.IsWildcard)
            {
                // check pkg.TypeName
                if (_symbolTable.TryLookup($"{import.QualifiedId}.{typeName}", out def)) return true;
            }
            else
            {
                // check if import IS the type
                if (import.QualifiedId.EndsWith($".{typeName}"))
                {
                     if (_symbolTable.TryLookup(import.QualifiedId, out def)) return true;
                }
            }
        }

        def = null;
        return false;
    }

    private void ValidateConnection(ConnectionDefinition conn, Dictionary<string, FormationDefinition> nodeScope, FormationDefinition context)
    {
        var sourcePort = GetPortDef(conn.Source, nodeScope, context);
        var targetPort = GetPortDef(conn.Target, nodeScope, context);

        if (sourcePort == null || targetPort == null) return; // Error already logged

        // Element Validation
        if (!IsCompatible(sourcePort.ElementType, targetPort.ElementType))
        {
            _errors.Add($"Error: Incompatible Elements. {conn.Source.NodeName}.{conn.Source.PortName} ({sourcePort.ElementType}) -> {conn.Target.NodeName}.{conn.Target.PortName} ({targetPort.ElementType})");
        }
    }

    private PortDefinition? GetPortDef(PortReference portRef, Dictionary<string, FormationDefinition> nodeScope, FormationDefinition context)
    {
        FormationDefinition container = null;
        
        // Is it a local port of the formation itself? (e.g. "input Fire ignition")
        // represented as "ignition" (NodeName="ignition", PortName=null)
        if (portRef.PortName == null)
        {
             // For self-ports, we look in the context's statements
             var localPort = context.Statements.OfType<PortDefinition>().FirstOrDefault(p => p.Name == portRef.NodeName);
             if (localPort == null) 
             {
                 // Or maybe it is a Node without a port? No, connections are Node.Port
                 _errors.Add($"Error: Port '{portRef.NodeName}' not found in formation.");
                 return null; 
             }
             return localPort;
        }

        // It is a Node Instance port (e.g. "power_source.out")
        if (!nodeScope.TryGetValue(portRef.NodeName, out container))
        {
            _errors.Add($"Error: Node instance '{portRef.NodeName}' not defined.");
            return null;
        }

        var portDef = container.Statements.OfType<PortDefinition>().FirstOrDefault(p => p.Name == portRef.PortName);
        if (portDef == null)
        {
            _errors.Add($"Error: Type '{container.Name}' does not have port '{portRef.PortName}'.");
            return null;
        }
        return portDef;
    }

    private bool IsCompatible(string sourceElement, string targetElement)
    {
        if (sourceElement == "Any" || targetElement == "Any") return true;
        return sourceElement == targetElement;
    }
}
