using Pidgin;

namespace RuneEngraver.Compiler.Syntax;

public record ParserError(
    int Line,
    int Column,
    string Message,
    string? Expected,
    string? Unexpected
)
{
    public override string ToString()
    {
        var location = $"({Line}:{Column})";
        var details = new List<string>();
        
        if (!string.IsNullOrEmpty(Expected))
            details.Add($"expected {Expected}");
        if (!string.IsNullOrEmpty(Unexpected))
            details.Add($"got '{Unexpected}'");
        
        var detailStr = details.Count > 0 ? $" [{string.Join(", ", details)}]" : "";
        return $"Parse error at {location}:{detailStr} {Message}";
    }

    public static ParserError FromPidginError<T>(Result<char, T> result, string input)
    {
        if (result.Success)
            throw new InvalidOperationException("Cannot create error from successful parse");

        var error = result.Error!;
        var pos = error.ErrorPos;
        
        // Extract the unexpected token (character at error position)
        string? unexpected = null;
        if (pos.Col > 0)
        {
            var lines = input.Split('\n');
            if (pos.Line > 0 && pos.Line <= lines.Length)
            {
                var line = lines[pos.Line - 1];
                if (pos.Col <= line.Length)
                {
                    // Get a few characters around the error
                    var startIdx = Math.Max(0, pos.Col - 1);
                    var length = Math.Min(10, line.Length - startIdx);
                    unexpected = line.Substring(startIdx, length).Trim();
                    if (unexpected.Length > 10)
                        unexpected = unexpected.Substring(0, 10) + "...";
                }
            }
        }

        // Build expected string from Pidgin's expected tokens
        string? expected = null;
        if (error.Expected != null && error.Expected.Any())
        {
            var expectations = error.Expected
                .Select(e => e.ToString())
                .Where(e => !string.IsNullOrEmpty(e))
                .Distinct()
                .Take(5);
            if (expectations.Any())
                expected = string.Join(" or ", expectations);
        }

        var message = error.Message ?? "Unexpected input";

        return new ParserError(
            pos.Line,
            pos.Col,
            message,
            expected,
            unexpected
        );
    }
}

public record ParseResult(
    bool Success,
    CompilationUnit? Value,
    ParserError? Error
)
{
    public static ParseResult Ok(CompilationUnit unit) => new(true, unit, null);
    public static ParseResult Fail(ParserError error) => new(false, null, error);
}
