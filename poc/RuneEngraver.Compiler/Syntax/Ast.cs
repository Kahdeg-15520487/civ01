using System.Collections.Generic;
using System.Text.Json.Serialization;
using RuneEngraver.Compiler.Semantics;

namespace RuneEngraver.Compiler.Syntax;

public abstract record AstNode
{
    [JsonIgnore]
    public SourceSpan Span { get; init; } = SourceSpan.Unknown;
}

public record SourceSpan(int StartLine, int StartColumn, int EndLine, int EndColumn)
{
    public static SourceSpan Unknown => new(0, 0, 0, 0);
    
    public SourceLocation ToLocation() => new(StartLine, StartColumn, EndColumn - StartColumn);
    
    public override string ToString() => $"({StartLine}:{StartColumn}-{EndLine}:{EndColumn})";
}

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



[JsonDerivedType(typeof(PortDefinition), typeDiscriminator: "Port")]
[JsonDerivedType(typeof(NodeDefinition), typeDiscriminator: "Node")]
[JsonDerivedType(typeof(ConnectionDefinition), typeDiscriminator: "Connection")]
public abstract record Statement : AstNode;

public record PortDefinition(
    PortDirection Direction,
    string ElementType,
    string Name,
    AmplitudeSpec? Amplitude
) : Statement;

public enum PortDirection { Input, Output }

[JsonDerivedType(typeof(ExactAmplitude), typeDiscriminator: "Exact")]
[JsonDerivedType(typeof(MinAmplitude), typeDiscriminator: "Min")]
[JsonDerivedType(typeof(RangeAmplitude), typeDiscriminator: "Range")]
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
