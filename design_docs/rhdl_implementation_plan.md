# RunicHDL Implementation Plan

## Goal
Implement a compiler for RunicHDL that parses source code, validates semantics, calculates circuit requirements, and generates a JSON graph for the Runic Engine.

## Architecture

The compiler will be a C# Class Library (`RuneEngraver.Compiler`) with a CLI wrapper (`rhdl`).

### 1. Front-End (Parsing)
*   **Tokenizer**: Convert raw text into tokens (`Keyword`, `Identifier`, `Symbol`, `Literal`).
*   **Parser**: Recursive descent parser or Parser Combinator (e.g., `Pidgin`) to generate an Abstract Syntax Tree (AST).
*   **AST Nodes**:
    *   `FormationDef`
    *   `PortDef`
    *   `NodeDef`
    *   `ConnectionDef`

### 2. Semantic Analysis (Validation)
*   **Symbol Table**: Track defined Formations across packages.
*   **Type Checking**: Verify `Wood -> Fire` connections (valid) vs `Water -> Fire` (invalid/warning).
*   **Cycle Detection**: Ensure no infinite loops without delay elements (if applicable).
*   **Port Matching**: Ensure all required ports on nodes are connected.

### 3. Synthesis (Back-End)
*   **Graph Flattening**: Recursively instantiate "Node" formations into a flat graph of primitives for the engine.
*   **Requirement Analysis**:
    *   Calculate **Max Amplitude** (Peak Conductivity).
    *   Calculate **Complexity Score** (node count + weights).
    *   Determine **Element Affinity**.
*   **Code Generation**: Output `rune_graph.json` compatible with `RuneEngraver.PoC`.

## Proposed Tech Stack
*   **Language**: C# (.NET 8.0)
*   **Parsing**: `Pidgin` (Parser Combinator library).
*   **CLI**: `System.CommandLine`

## Step-by-Step Implementation

### Phase 1: Core AST & Lexer
1.  Define AST classes in `RuneEngraver.Compiler`.
2.  Implement `Lexer` logic (optionally using `Pidgin` for tokenization too).
3.  Implement `Parser` using `Pidgin` combinators to produce AST.
4.  **Verification**: Parse a basic `.rhdl` file and inspect the AST.

### Phase 2: Semantics & Imports
1.  Implement `PackageResolver` to handle `import`.
2.  Implement `TypeChecker` for element compatibility.
3.  **Verification**: Validation report for "broken" circuits.

### Phase 3: Synthesis
1.  Implement `GraphBuilder` to flatten AST.
2.  Implement `StatCalculator` for Tier/Conductivity.
3.  **Verification**: Generate JSON for the `CapacitorStrike` example.

## Question for User
*   Do you prefer a specific parsing library (ANTLR, Pidgin, Sprache) or a custom hand-written parser? (Custom is often easier for simple DSLs like this and allows better error messages).
