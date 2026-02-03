using System.Collections.Generic;

namespace RuneEngraver.Compiler.Syntax;

public abstract record AstNode;

public record CompilationUnit(
    PackageDeclaration Package,
    IEnumerable<ImportStatement> Imports,
    IEnumerable<FormationDefinition> Formations
) : AstNode;

public record PackageDeclaration(string QualifiedId) : AstNode;
public record ImportStatement(string QualifiedId, bool IsWildcard) : AstNode;

public record FormationDefinition(
    string Name,
    IEnumerable<Statement> Statements
) : AstNode;



public abstract record Statement : AstNode;

public record PortDefinition(
    PortDirection Direction,
    string ElementType,
    string Name,
    AmplitudeSpec? Amplitude
) : Statement;

public enum PortDirection { Input, Output }

public abstract record AmplitudeSpec : AstNode;
public record ExactAmplitude(int Value) : AmplitudeSpec;
public record MinAmplitude(int Value) : AmplitudeSpec;
public record RangeAmplitude(int Min, int Max) : AmplitudeSpec;

public record NodeDefinition(
    string TypeName,
    string InstanceName,
    IEnumerable<NodeParameter> Parameters
) : Statement;

public record NodeParameter(string Name, object Value) : AstNode;

public record ConnectionDefinition(
    PortReference Source,
    PortReference Target
) : Statement;

public record PortReference(string NodeName, string? PortName) : AstNode;
