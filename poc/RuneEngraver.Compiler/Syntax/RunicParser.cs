using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RuneEngraver.Compiler.Syntax;

public static class RunicParser
{
    // Whitespace and Comments
    private static readonly Parser<char, Unit> _whitespaces = 
        Pidgin.Parser.SkipWhitespaces;

    private static readonly Parser<char, Unit> Comment =
        String("//").Then(AnyCharExcept('\n', '\r').SkipMany()).Then(_whitespaces)
        .Or(String("/*").Then(AnyCharExcept("*/").SkipMany()).Then(String("*/")).Then(_whitespaces));

    private static readonly Parser<char, Unit> Skip = 
        _whitespaces.Then(Comment.SkipMany()).Then(_whitespaces);

    private static Parser<char, T> Tok<T>(Parser<char, T> p) => 
        p.Before(Skip);
    
    private static Parser<char, string> Tok(string value) => 
        Tok(String(value));

    private static readonly Parser<char, char> Dot = Tok(Char('.'));

    // Identifiers
    private static readonly Parser<char, string> Identifier = 
        Tok(Map((first, rest) => first + rest, Letter.Or(Char('_')), LetterOrDigit.Or(Char('_')).ManyString()));

    private static readonly Parser<char, string> QualifiedIdentifier =
        Identifier.SeparatedAtLeastOnce(Dot).Select(ids => string.Join(".", ids));

    // Values
    private static readonly Parser<char, object> Number = 
        Tok(Pidgin.Parser.Num).Select(n => (object)n);

    private static readonly Parser<char, object> StringLiteral = 
        Tok(AnyCharExcept('"').ManyString().Between(Char('"'))).Select(s => (object)s);

    private static readonly Parser<char, object> Boolean = 
        Tok("true").ThenReturn((object)true)
        .Or(Tok("false").ThenReturn((object)false));

    private static readonly Parser<char, object> Value = 
        StringLiteral.Or(Number).Or(Boolean).Or(Identifier.Select(s => (object)s));

    // Package & Imports
    private static readonly Parser<char, PackageDeclaration> PackageDecl =
        Tok("package").Then(QualifiedIdentifier).Before(Tok(";"))
        .Select(id => new PackageDeclaration(id));

    private static readonly Parser<char, ImportStatement> ImportStmt =
        Tok("import").Then(
            Identifier.Or(Tok("*")).SeparatedAtLeastOnce(Dot)
            .Select(parts => {
                var list = parts.ToList();
                bool wild = list.Last() == "*";
                if (wild) list.RemoveAt(list.Count - 1); // Remove the "*"
                return new ImportStatement(string.Join(".", list), wild);
            })
        ).Before(Tok(";"));

    // Ports
    private static readonly Parser<char, AmplitudeSpec> Amplitude =
        Tok("[").Then(
            Pidgin.Parser.Num.Bind(n => 
                Tok("+").ThenReturn((AmplitudeSpec)new MinAmplitude(n))
                .Or(Tok("..").Then(Pidgin.Parser.Num).Select(max => (AmplitudeSpec)new RangeAmplitude(n, max)))
                .Or(Return((AmplitudeSpec)new ExactAmplitude(n)))
            )
        ).Before(Tok("]"));

    private static readonly Parser<char, PortDefinition> PortDef =
        Map((dir, type, name, amp) => new PortDefinition(dir, type, name, amp.GetValueOrDefault()),
            Tok("input").ThenReturn(PortDirection.Input).Or(Tok("output").ThenReturn(PortDirection.Output)),
            Identifier,
            Identifier,
            Amplitude.Optional()
        ).Before(Tok(";"));

    // Nodes
    private static readonly Parser<char, NodeParameter> NodeParam =
        Map((name, val) => new NodeParameter(name, val),
            Identifier.Before(Tok(":")),
            Value
        );

    private static readonly Parser<char, NodeDefinition> NodeDef =
        Map((type, name, @params) => new NodeDefinition(type, name, @params.GetValueOrDefault(Enumerable.Empty<NodeParameter>())),
            Tok("node").Then(Identifier),
            Identifier,
            Tok("(").Then(NodeParam.Separated(Tok(","))).Before(Tok(")")).Optional()
        ).Before(Tok(";"));

    // Connections
    private static readonly Parser<char, PortReference> PortRef =
        Map((node, port) => new PortReference(node, port.GetValueOrDefault()),
            Identifier,
            Char('.').Then(Identifier).Optional()
        );

    private static readonly Parser<char, ConnectionDefinition> Connection =
        Map((src, dst) => new ConnectionDefinition(src, dst),
            PortRef.Before(Tok("->")),
            PortRef
        ).Before(Tok(";"));

    // Statements
    private static readonly Parser<char, Statement> Statement =
        OneOf(
            PortDef.Cast<Statement>(),
            NodeDef.Cast<Statement>(),
            Connection.Cast<Statement>()
        );

    // Formation
    private static readonly Parser<char, FormationDefinition> Formation =
        Map((name, stmts) => new FormationDefinition(name, stmts),
            Tok("formation").Then(Identifier),
            Statement.Many().Between(Tok("{"), Tok("}"))
        );

    // Compilation Unit
    public static readonly Parser<char, CompilationUnit> CompilationUnitParser =
        Map((pkg, imports, formations) => new CompilationUnit(pkg, imports, formations),
            Skip.Then(PackageDecl),
            ImportStmt.Many(),
            Formation.Many()
        );

    public static Result<char, CompilationUnit> Parse(string input) =>
        CompilationUnitParser.Parse(input);
}
