using System;
using System.Collections.Generic;
using System.Linq;
using RuneEngraver.Compiler.Syntax;

namespace RuneEngraver.Compiler.Semantics;

public class RunicValidator
{
    private readonly SymbolTable _symbolTable;
    private readonly List<CompilerError> _errors = new();

    public RunicValidator(SymbolTable symbolTable)
    {
        _symbolTable = symbolTable;
    }

    public IEnumerable<CompilerError> Validate(CompilationUnit unit)
    {
        _errors.Clear();

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

        foreach (var node in formation.Statements.OfType<NodeDefinition>())
        {
            if (ResolveNode(node.TypeName, unit, out var def))
            {
                scope[node.InstanceName] = def;
            }
            else
            {
                _errors.Add(new CompilerError(
                    ErrorCode.UnknownNodeType,
                    ErrorSeverity.Error,
                    $"Unknown node type '{node.TypeName}' for instance '{node.InstanceName}'.",
                    node.Span.ToLocation(),
                    node.TypeName
                ));
            }
        }

        foreach (var conn in formation.Statements.OfType<ConnectionDefinition>())
        {
            ValidateConnection(conn, scope, formation);
        }
    }

    private void ValidateImport(ImportStatement import)
    {
        if (import.IsWildcard)
        {
            if (!_symbolTable.HasPackage(import.QualifiedId))
            {
                _errors.Add(new CompilerError(
                    ErrorCode.UnresolvedPackage,
                    ErrorSeverity.Error,
                    $"Imported package '{import.QualifiedId}' does not exist or has no visible members.",
                    import.Span.ToLocation(),
                    import.QualifiedId
                ));
            }
        }
        else
        {
            if (!_symbolTable.IsDefined(import.QualifiedId))
            {
                _errors.Add(new CompilerError(
                    ErrorCode.UnresolvedImport,
                    ErrorSeverity.Error,
                    $"Imported symbol '{import.QualifiedId}' could not be resolved.",
                    import.Span.ToLocation(),
                    import.QualifiedId
                ));
            }
        }
    }

    private bool ResolveNode(string typeName, CompilationUnit unit, out FormationDefinition? def)
    {
        if (_symbolTable.TryLookup(typeName, out def)) return true;
        if (_symbolTable.TryLookup($"core.{typeName}", out def)) return true;

        foreach (var import in unit.Imports)
        {
            if (import.IsWildcard)
            {
                if (_symbolTable.TryLookup($"{import.QualifiedId}.{typeName}", out def)) return true;
            }
            else
            {
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

        if (sourcePort == null || targetPort == null) return;

        if (!IsCompatible(sourcePort.ElementType, targetPort.ElementType))
        {
            _errors.Add(new CompilerError(
                ErrorCode.IncompatibleElements,
                ErrorSeverity.Error,
                $"Incompatible elements: {conn.Source.NodeName}.{conn.Source.PortName} ({sourcePort.ElementType}) -> {conn.Target.NodeName}.{conn.Target.PortName} ({targetPort.ElementType})",
                conn.Span.ToLocation(),
                $"{sourcePort.ElementType} -> {targetPort.ElementType}"
            ));
        }
    }

    private PortDefinition? GetPortDef(PortReference portRef, Dictionary<string, FormationDefinition> nodeScope, FormationDefinition context)
    {
        if (portRef.PortName == null)
        {
            var localPort = context.Statements.OfType<PortDefinition>().FirstOrDefault(p => p.Name == portRef.NodeName);
            if (localPort == null)
            {
                _errors.Add(new CompilerError(
                    ErrorCode.PortNotFound,
                    ErrorSeverity.Error,
                    $"Port '{portRef.NodeName}' not found in formation.",
                    portRef.Span.ToLocation(),
                    portRef.NodeName
                ));
                return null;
            }
            return localPort;
        }

        if (!nodeScope.TryGetValue(portRef.NodeName, out var container))
        {
            _errors.Add(new CompilerError(
                ErrorCode.UndefinedNodeInstance,
                ErrorSeverity.Error,
                $"Node instance '{portRef.NodeName}' not defined.",
                portRef.Span.ToLocation(),
                portRef.NodeName
            ));
            return null;
        }

        var portDef = container.Statements.OfType<PortDefinition>().FirstOrDefault(p => p.Name == portRef.PortName);
        if (portDef == null)
        {
            _errors.Add(new CompilerError(
                ErrorCode.UndefinedPort,
                ErrorSeverity.Error,
                $"Type '{container.Name}' does not have port '{portRef.PortName}'.",
                portRef.Span.ToLocation(),
                portRef.PortName
            ));
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
