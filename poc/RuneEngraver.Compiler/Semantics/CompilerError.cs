namespace RuneEngraver.Compiler.Semantics;

public enum ErrorSeverity
{
    Warning,
    Error
}

public enum ErrorCode
{
    // Import Errors (1xx)
    UnresolvedImport = 101,
    UnresolvedPackage = 102,

    // Node Errors (2xx)
    UnknownNodeType = 201,
    UndefinedNodeInstance = 202,

    // Port Errors (3xx)
    UndefinedPort = 301,
    PortNotFound = 302,

    // Type Errors (4xx)
    IncompatibleElements = 401,
    AmplitudeMismatch = 402
}

public record SourceLocation(int Line, int Column, int Length = 1)
{
    public static SourceLocation Unknown => new(0, 0, 0);
    
    public override string ToString() => $"({Line}:{Column})";
}

public record CompilerError(
    ErrorCode Code,
    ErrorSeverity Severity,
    string Message,
    SourceLocation Location,
    string? Token = null)
{
    public override string ToString()
    {
        var prefix = Severity == ErrorSeverity.Error ? "error" : "warning";
        var tokenInfo = Token != null ? $" '{Token}'" : "";
        return $"{prefix} {(int)Code}{Location}:{tokenInfo} {Message}";
    }
}
