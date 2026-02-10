# OpenSCAD Integration Design

> From Circuit to Artifact: Bridging logical design and physical creation

## Overview

This document defines how the OpenSCAD-based artifact modeling system integrates with the existing rune circuit programming system. The core concept: **Players first prove their circuit works, then give it physical form.**

---

## Current Gameplay Flow (As Implemented)

### What Exists Now

```
┌─────────────────────────────────────────────────────────────┐
│                    Main Menu                                 │
└──────────────────────┬──────────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────────┐
│              Engraving Table (Main IDE)                      │
│                                                               │
│  ┌──────────┐  ┌─────────────────────────────────┐          │
│  │ Palette  │  │                                 │          │
│  │ -Sources │  │    Grid (200x155 cells)          │          │
│  │ -Process │  │                                 │          │
│  │ -Control │  │    [Rune] [Rune]    [Rune]      │          │
│  │ -Storage │  │         │       │        │        │          │
│  │ -Sinks   │  │         └────Trace────┘        │          │
│  └──────────┘  │                                 │          │
│                └─────────────────────────────────┘          │
│  ┌─────────────────────────────────┐                          │
│  │ Properties Panel                │                          │
│  │ - Edit rune parameters           │                          │
│  │ - View connection info           │                          │
│  └─────────────────────────────────┘                          │
│  ┌─────────────────────────────────┐                          │
│  │ Toolbar                         │                          │
│  │ [SELECT] [PLACE] [ROUTE]        │                          │
│  │ [▶ Run Simulation] [■ Stop]      │                          │
│  └─────────────────────────────────┘                          │
└─────────────────────────────────────────────────────────────┘
                       │
                       ▼ (Player clicks "Run Simulation")
┌─────────────────────────────────────────────────────────────┐
│              Simulation Pipeline                              │
│                                                               │
│  1. NetlistExtractor extracts:                               │
│     - Rune positions and types                                │
│     - Trace connections                                       │
│     - Port mappings                                           │
│                                                               │
│  2. SimulationManager.BuildGraph():                           │
│     - Creates C# RuneGraph                                    │
│     - Instantiates nodes from factory                         │
│     - Connects ports via traces                               │
│                                                               │
│  3. Simulation Tick Loop:                                     │
│     - Reset inputs → Transfer Qi → Process nodes → Update outputs│
│     - Visual feedback: Glow on active traces                  │
│     - Log messages for Qi flow events                         │
│                                                               │
│  Result: Circuit WORKS or FAILS                               │
└─────────────────────────────────────────────────────────────┘
                       │
                       ▼ (Current end point)
                   [That's it!]
```

### What's Missing

```
❌ No quest system
❌ No tutorial/guidance
❌ No progression/unlocks
❌ No artifact creation (physical modeling)
❌ No save/load of designs
❌ No inventory or collection
❌ No reward for working circuits
❌ No purpose beyond "does it work?"
```

---

## Proposed Gameplay Flow (With OpenSCAD Integration)

### Enhanced Player Journey

```
┌─────────────────────────────────────────────────────────────┐
│                    Main Menu                                 │
│  [New Game]  [Load Game]  [Blueprint Library]  [Settings]   │
└──────────────────────┬──────────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────────┐
│              Sect/Hub Area (NEW)                            │
│                                                               │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐          │
│  │ Engraving   │  │  Artifact   │  │  Training   │          │
│  │   Table     │  │    Forge    │  │  Grounds    │          │
│  │             │  │             │  │             │          │
│  │ [Circuit    │  │ [Physical   │  │ [Test       │          │
│  │  Design]    │  │  Modeling]  │  │  Artifacts] │          │
│  └─────────────┘  └─────────────┘  └─────────────┘          │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ Quest/Journal Panel                                   │    │
│  │ Current: "Craft Your First Artifact"                 │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                       │
                       ▼ (Player clicks Engraving Table)
┌─────────────────────────────────────────────────────────────┐
│              Engraving Table Scene (Existing)               │
│                                                               │
│  Current Quest: "Craft Your First Artifact"                  │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ Elder Mo: "Ah, a new disciple. Let me teach you..." │    │
│  │                                                         │    │
│  │ Objectives:                                            │    │
│  │ ☐ 1. Learn the basics of rune inscribing              │    │
│  │ ☐ 2. Create a simple heating circuit                  │    │
│  │ ☐ 3. Forge your first artifact                        │    │
│  │ ☐ 4. Test it in the training grounds                  │    │
│  │                                                         │    │
│  │ Rewards:                                               │    │
│  │ - Unlock Spirit Stone Socket                           │    │
│  │ - Learn Heating Stone blueprint                        │    │
│  │ - +50 XP                                              │    │
│  └─────────────────────────────────────────────────────┘    │
└──────────────────────┬──────────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────────┐
│              Engraving Table Scene (Existing)               │
│                                                               │
│  [Player places runes and routes traces...]                 │
│                                                               │
│  Result: Circuit Simulation SUCCESS                          │
│           │                                                   │
│           ▼                                                   │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ "✓ Circuit Works!"                                 │    │
│  │                                                      │    │
│  │ Formation saved to library.                         │    │
│  │                                                      │    │
│  │ [Return to Sect Hub]  [Create Artifact]            │    │
│  │        ↑                    ↑                        │    │
│  │        │                    └──► Triggers:            │    │
│  │        │                         - Save formation    │    │
│  │        │                         - Award XP           │    │
│  │        │                         - Update quest      │    │
│  │        │                                            │    │
│  │        └── Just returns to hub (no artifact yet)    │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                       │
                       ▼ (Player returns to hub)
┌─────────────────────────────────────────────────────────────┐
│              Sect/Hub Area                                   │
│                                                               │
│  Artifact Forge button now glowing: "1 Formation Ready!"     │
│           │                                                   │
│           ▼                                                   │
│  [Click Artifact Forge]                                      │
└──────────────────────┬──────────────────────────────────────┘
                       ▼ (New Screen - Scene Transition)
┌─────────────────────────────────────────────────────────────┐
│         ARTIFACT FORGE SCENE (NEW - Separate Scene)          │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ "Your circuit works! What form shall it take?"       │    │
│  │                                                         │    │
│  │ Suggested Artifacts (based on circuit):                │    │
│  │  [🔥 Heating Stone]  [🏺 Tea Kettle]  [⚒ Furnace]  │    │
│  │  [📜 Custom Artifact...]                               │    │
│  └─────────────────────────────────────────────────────┘    │
│                          │                                  │
│                          ▼                                  │
│  ┌─────────────────────────────────────────────────────┐    │
│  │         Material Selection (NEW)                      │    │
│  │                                                         │    │
│  │ Available Materials (unlocked by tier):                 │    │
│  │  [💎 Spirit Stone] - Conductivity: 15, Cost: 100      │    │
│  │  [🪵 Spirit Wood]   - Conductivity: 10, Cost: 80      │    │
│  │  [🔒 Cold Iron]     - Requires Spirit Tier            │    │
│  │                                                         │    │
│  │ Selected: Spirit Stone ✓                               │    │
│  │                                                         │    │
│  │ Constraints:                                            │    │
│  │  - Circuit max amplitude: 25                            │    │
│  │  - Material conductivity: 15 ⚠️ (Over limit!)          │    │
│  │  - Add Stabilizer or reduce power                       │    │
│  └─────────────────────────────────────────────────────┘    │
│                          │                                  │
│                          ▼                                  │
│  ┌─────────────────────────────────────────────────────┐    │
│  │         Artifact Modeler (OpenSCAD) (NEW)             │    │
│  │                                                         │    │
│  │  ┌──────────┐  ┌─────────────────────────────────┐     │    │
│  │  │ Prefab   │  │         3D Preview               │     │    │
│  │  │ Library  │  │                                 │     │    │
│  │  │          │  │         [Artifact Mesh]         │     │    │
│  │  │ - Box    │  │                                 │     │    │
│  │  │ - Sphere │  │    (OpenSCAD-generated OBJ)      │     │    │
│  │  │ - Hilt   │  │                                 │     │    │
│  │  │ - Blade  │  │    ⬇ Rotate | ⬅ Pan | 🔍+ Zoom│     │    │
│  │  │          │  └─────────────────────────────────┘     │    │
│  │  │ [Custom]│                                         │    │
│  │  └──────────┘                                         │    │
│  │                                                       │    │
│  │  Current Mode: Prefab Stitching (Mortal Tier)        │    │
│  │  Components:                                          │    │
│  │    • Power Socket (must place 1)                      │    │
│  │    • Effect Emitter (heating)                         │    │
│  │    • Heat Sink (optional)                             │    │
│  │                                                       │    │
│  │  [Generate OpenSCAD] [Compile] [Test] [Save]        │    │
│  └─────────────────────────────────────────────────────┘    │
│                          │                                  │
│                          ▼                                  │
│  ┌─────────────────────────────────────────────────────┐    │
│  │         Validation & Assembly (NEW)                   │    │
│  │                                                         │    │
│  │ ✓ OpenSCAD compilation successful                     │    │
│  │ ✓ Polygon count: 850 / 5000 (OK)                      │    │
│  │ ✓ Components aligned with circuit ports              │    │
│  │ ✓ Material conductivity sufficient                    │    │
│  │ ✓ Heat generation: 5 / dissipation: 10 (OK)          │    │
│  │                                                         │    │
│  │ Name your artifact: [My First Heating Stone________]  │    │
│  │                                                         │    │
│  │ [FORGE ARTIFACT]                                      │    │
│  └─────────────────────────────────────────────────────┘    │
└──────────────────────┬──────────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────────┐
│                   Reward & Progression                       │
│                                                               │
│  🎉 Artifact Created!                                        │
│                                                               │
│  Item: My First Heating Stone                                │
│  Type: Utility / Mortal Tier                                  │
│  Circuit: Simple Heater Formation                            │
│  Material: Spirit Stone                                      │
│                                                               │
│  Stats:                                                       │
│  • Heat Output: 50 Qi/tick                                   │
│  • Duration: 1 hour per Spirit Stone                        │
│  • Complexity: 25 / 100                                      │
│                                                               │
│  Quest Progress:                                              │
│  ☐ 1. Learn basics ✓                                         │
│  ☐ 2. Create circuit ✓                                       │
│  ☐ 3. Forge artifact ✓                                       │
│  ☐ 4. Test in training grounds                                │
│                                                               │
│  Rewards Received:                                            │
│  • Blueprint saved to library                                │
│  • +75 XP                                                    │
│  • +100 Spirit Coins                                        │
│  • Unlock: Amplifier rune                                    │
│                                                               │
│  [Return to Engraving Table]  [Go to Training Grounds]      │
└─────────────────────────────────────────────────────────────┘
```

---

## Technical Integration Architecture

### System Components

```
┌──────────────────────────────────────────────────────────────┐
│                      Godot Game Layer                         │
│                                                               │
│  ┌────────────────────────────────────────────────────────┐  │
│  │           EngravingTable (GDScript)                    │  │
│  │  - Existing: Rune placement, trace routing             │  │
│  │  - NEW: "Forge Artifact" button handler                │  │
│  │  - NEW: Quest state integration                        │  │
│  └────────────────────────────────────────────────────────┘  │
│                           │                                   │
│                           ▼                                   │
│  ┌────────────────────────────────────────────────────────┐  │
│  │      SimulationManager.cs (Existing Bridge)            │  │
│  │  - BuildGraph() from netlist                          │  │
│  │  - RunTick() for simulation                           │  │
│  │  - NEW: GetCircuitStats() for validation              │  │
│  └────────────────────────────────────────────────────────┘  │
│                           │                                   │
│                           ▼                                   │
│  ┌────────────────────────────────────────────────────────┐  │
│  │    ArtifactCreationWorkflow.cs (NEW)                   │  │
│  │  - Orchestrates artifact creation process              │  │
│  │  - Validates circuit before forging                    │  │
│  │  - Manages material selection                          │  │
│  │  - Coordinates OpenSCAD bridge                          │  │
│  └────────────────────────────────────────────────────────┘  │
│                           │                                   │
│         ┌─────────────────┴─────────────────┐                 │
│         ▼                                   ▼                 │
│  ┌─────────────────────┐         ┌─────────────────────┐     │
│  │  OpenSCADBridge.cs  │         │  RHDLCompiler.cs    │     │
│  │  (NEW)              │         │  (Existing/Enhanced) │     │
│  │  - Execute OpenSCAD │         │  - Parse circuit     │     │
│  │  - Parse OBJ output │         │  - Validate bindings │     │
│  │  - Cache meshes     │         │  - Generate graph    │     │
│  └─────────────────────┘         └─────────────────────┘     │
│                           │                                   │
│                           ▼                                   │
│  ┌────────────────────────────────────────────────────────┐  │
│  │      ArtifactBlueprint.cs (Data Structure)             │  │
│  │  - Physical: OpenSCAD script, components               │  │
│  │  - Circuit: RHDL script, port bindings                 │  │
│  │  - Stats: Calculated properties                        │  │
│  └────────────────────────────────────────────────────────┘  │
│                           │                                   │
│                           ▼                                   │
│  ┌────────────────────────────────────────────────────────┐  │
│  │    ArtifactInstance.cs (Runtime Object)                │  │
│  │  - MeshInstance3D: Visual representation                │  │
│  │  - CircuitGraph: Embedded logic                        │  │
│  │  - ArtifactStats: Runtime behavior                     │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

---

## C# Implementation: ArtifactCreationWorkflow

```csharp
// ArtifactCreationWorkflow.cs
using Godot;
using System;
using System.Threading.Tasks;

namespace Civ.Artifacts;

/// <summary>
/// Orchestrates the complete artifact creation process.
/// Called when player clicks "Forge Artifact" after successful simulation.
/// </summary>
[GlobalClass]
public partial class ArtifactCreationWorkflow : Node
{
    // Dependencies
    private SimulationManager _simulationManager;
    private OpenSCADBridge _openscadBridge;
    private RHDLCompiler _rhdlCompiler;
    private MaterialDatabase _materialDatabase;
    private QuestManager _questManager;

    // State
    private ArtifactBlueprint _currentBlueprint;

    // Signals
    [Signal]
    public delegate void WorkflowStartedEventHandler();

    [Signal]
    public delegate void WorkflowCompletedEventHandler(ArtifactInstance artifact);

    [Signal]
    public delegate void WorkflowFailedEventHandler(string reason);

    [Signal]
    public delegate void WorkflowProgressEventHandler(string step, float progress);

    /// <summary>
    /// Entry point: Called from EngravingTable when "Forge Artifact" is clicked
    /// </summary>
    public async void StartWorkflow(Godot.Collections.Dictionary boardData)
    {
        EmitSignal(SignalName.WorkflowStarted);

        try
        {
            // Step 1: Validate Circuit
            EmitSignal(SignalName.WorkflowProgress, "Validating circuit...", 0.1f);
            var validationResult = await ValidateCircuitAsync(boardData);
            if (!validationResult.IsValid)
            {
                EmitSignal(SignalName.WorkflowFailed, validationResult.ErrorMessage);
                return;
            }

            // Step 2: Material Selection
            EmitSignal(SignalName.WorkflowProgress, "Checking materials...", 0.2f);
            var materialSelection = await ShowMaterialSelectionUI(validationResult);
            if (materialSelection == null)
            {
                EmitSignal(SignalName.WorkflowFailed, "Material selection cancelled");
                return;
            }

            // Step 3: Suggest Artifact Types
            EmitSignal(SignalName.WorkflowProgress, "Analyzing circuit...", 0.3f);
            var suggestions = SuggestArtifactTypes(validationResult.CircuitAnalysis);
            var artifactType = await ShowArtifactTypeSelectionUI(suggestions);

            // Step 4: Open Artifact Modeler UI
            EmitSignal(SignalName.WorkflowProgress, "Opening modeler...", 0.4f);
            var modelingResult = await OpenArtifactModeler(
                artifactType,
                materialSelection.Material,
                validationResult.CircuitAnalysis
            );

            if (!modelingResult.Success)
            {
                EmitSignal(SignalName.WorkflowFailed, modelingResult.ErrorMessage);
                return;
            }

            // Step 5: Compile OpenSCAD Script
            EmitSignal(SignalName.WorkflowProgress, "Compiling artifact mesh...", 0.6f);
            var meshResult = await CompileArtifactMesh(
                modelingResult.OpenSCADScript,
                materialSelection.Material
            );

            if (!meshResult.Success)
            {
                EmitSignal(SignalName.WorkflowFailed, meshResult.ErrorMessage);
                return;
            }

            // Step 6: Assemble Blueprint
            EmitSignal(SignalName.WorkflowProgress, "Assembling blueprint...", 0.8f);
            _currentBlueprint = AssembleBlueprint(
                modelingResult,
                meshResult,
                validationResult,
                materialSelection
            );

            // Step 7: Final Validation
            EmitSignal(SignalName.WorkflowProgress, "Final validation...", 0.9f);
            var finalValidation = ValidateBlueprint(_currentBlueprint);
            if (!finalValidation.IsValid)
            {
                EmitSignal(SignalName.WorkflowFailed, finalValidation.ErrorMessage);
                return;
            }

            // Step 8: Create Artifact Instance
            EmitSignal(SignalName.WorkflowProgress, "Creating artifact...", 1.0f);
            var artifact = CreateArtifactInstance(_currentBlueprint);

            // Step 9: Save Blueprint
            SaveBlueprint(_currentBlueprint);

            // Step 10: Update Quest Progress
            _questManager?.UpdateArtifactCreated(artifact);

            // Success!
            EmitSignal(SignalName.WorkflowCompleted, artifact);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Artifact creation workflow error: {ex.Message}");
            EmitSignal(SignalName.WorkflowFailed, $"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Step 1: Validate the circuit is ready for artifact creation
    /// </summary>
    private async Task<CircuitValidationResult> ValidateCircuitAsync(
        Godot.Collections.Dictionary boardData)
    {
        // Rebuild graph to get stats
        _simulationManager.BuildGraph(boardData);

        var stats = _simulationManager.GetCircuitStats();

        var result = new CircuitValidationResult
        {
            IsValid = true,
            CircuitStats = stats,
            CircuitAnalysis = AnalyzeCircuit(stats)
        };

        // Validation rules
        if (stats.MaxAmplitude == 0)
        {
            result.IsValid = false;
            result.ErrorMessage = "Circuit produces no output. Add an emitter.";
            return result;
        }

        if (stats.NodeCount < 2)
        {
            result.IsValid = false;
            result.ErrorMessage = "Circuit too simple. Add at least 2 nodes.";
            return result;
        }

        // Analyze circuit type for artifact suggestions
        result.CircuitAnalysis.PrimaryOutput = stats.PrimaryOutputType;
        result.CircuitAnalysis.MaxAmplitude = stats.MaxAmplitude;
        result.CircuitAnalysis.DominantElement = stats.DominantElement;

        return result;
    }

    /// <summary>
    /// Step 6: Compile OpenSCAD to OBJ mesh
    /// </summary>
    private async Task<MeshCompileResult> CompileArtifactMesh(
        string scadScript,
        MaterialData material)
    {
        // Check cache first
        string scriptHash = ComputeHash(scadScript);
        if (_openscadBridge.TryGetCached(scriptHash, out var cachedMesh))
        {
            return new MeshCompileResult
            {
                Success = true,
                MeshPath = cachedMesh,
                PolygonCount = _openscadBridge.GetPolygonCount(cachedMesh)
            };
        }

        // Compile with OpenSCAD
        var result = await _openscadBridge.CompileAsync(scadScript, material.Tier);

        if (!result.Success)
        {
            return new MeshCompileResult
            {
                Success = false,
                ErrorMessage = ParseOpenSCADError(result.ErrorLog)
            };
        }

        // Parse OBJ to Godot mesh
        var godotMesh = await ParseOBJToMesh(result.MeshPath);

        return new MeshCompileResult
        {
            Success = true,
            MeshPath = result.MeshPath,
            GodotMesh = godotMesh,
            PolygonCount = result.PolygonCount
        };
    }

    // ... additional helper methods ...
}
```

---

## C# Implementation: OpenSCADBridge

```csharp
// OpenSCADBridge.cs
using Godot;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Civ.Artifacts;

/// <summary>
/// Manages OpenSCAD binary execution and mesh compilation.
/// Executes OpenSCAD as separate process (GPL compliance).
/// </summary>
public partial class OpenSCADBridge : Node
{
    private string _openscadPath;
    private string _cacheDir;
    private CacheIndex _cacheIndex;

    public override void _Ready()
    {
        InitializePaths();
        LoadCacheIndex();
    }

    private void InitializePaths()
    {
        // Path to bundled OpenSCAD binary
        _openscadPath = Path.Combine(
            OS.GetExecutableDir().GetBaseDir(),
            "tools",
            "openscad",
            OS.HasFeature("windows") ? "openscad.exe" : "openscad"
        );

        if (!File.Exists(_openscadPath))
        {
            GD.PrintErr($"OpenSCAD not found at: {_openscadPath}");
            GD.PrintErr("Please ensure OpenSCAD is bundled with the game.");
        }

        // Cache directory for compiled meshes
        _cacheDir = Path.Combine(
            OS.GetUserDataDir(),
            "artifact_cache"
        );
        Directory.CreateDirectory(_cacheDir);
    }

    /// <summary>
    /// Compile OpenSCAD script to OBJ mesh (async)
    /// </summary>
    public async Task<CompileResult> CompileAsync(
        string scadScript,
        string artifactTier)
    {
        return await Task.Run(() => Compile(scadScript, artifactTier));
    }

    /// <summary>
    /// Compile OpenSCAD script to OBJ mesh (sync)
    /// </summary>
    public CompileResult Compile(
        string scadScript,
        string artifactTier)
    {
        var result = new CompileResult();

        try
        {
            // Validate OpenSCAD exists
            if (!File.Exists(_openscadPath))
            {
                result.Success = false;
                result.ErrorLog = "OpenSCAD binary not found";
                return result;
            }

            // Create temp directory for this compilation
            string tempDir = Path.Combine(_cacheDir, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            string scriptPath = Path.Combine(tempDir, "artifact.scad");
            string objPath = Path.Combine(tempDir, "artifact.obj");

            // Write script
            File.WriteAllText(scriptPath, scadScript);

            // Determine polygon budget based on tier
            int polyBudget = GetPolygonBudget(artifactTier);

            // Build OpenSCAD command
            var processInfo = new ProcessStartInfo
            {
                FileName = _openscadPath,
                Arguments = $"-o \"{objPath}\" \"{scriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Execute OpenSCAD
            using var process = Process.Start(processInfo);
            process.WaitForExit(); // Timeout could be added here

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode == 0 && File.Exists(objPath))
            {
                // Success
                result.Success = true;
                result.MeshPath = objPath;
                result.PolygonCount = CountPolygons(objPath);

                // Check polygon budget
                if (result.PolygonCount > polyBudget)
                {
                    result.Success = false;
                    result.ErrorLog = $"Polygon count {result.PolygonCount} exceeds budget {polyBudget}";
                }
            }
            else
            {
                // Failure
                result.Success = false;
                result.ErrorLog = error;
            }

            // Cleanup temp dir (or keep for cache)
            if (!result.Success)
            {
                Directory.Delete(tempDir, true);
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorLog = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Try to get cached mesh for a script
    /// </summary>
    public bool TryGetCached(string scriptHash, out string meshPath)
    {
        if (_cacheIndex.TryGetValue(scriptHash, out var entry))
        {
            if (File.Exists(entry.MeshPath))
            {
                meshPath = entry.MeshPath;
                return true;
            }
        }

        meshPath = null;
        return false;
    }

    /// <summary>
    /// Store compiled mesh in cache
    /// </summary>
    public void StoreInCache(string scriptHash, string meshPath, int polygonCount)
    {
        _cacheIndex.Add(scriptHash, meshPath, polygonCount);
        SaveCacheIndex();
    }

    private int GetPolygonBudget(string tier)
    {
        return tier switch
        {
            "Mortal" => 1000,
            "Spirit" => 5000,
            "Earth" => 20000,
            "Heaven" => 100000,
            _ => 1000
        };
    }

    private int CountPolygons(string objPath)
    {
        // Simple OBJ parser to count faces
        int faceCount = 0;
        foreach (var line in File.ReadLines(objPath))
        {
            if (line.TrimStart().StartsWith("f"))
            {
                faceCount++;
            }
        }
        return faceCount;
    }

    private string ComputeHash(string content)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLower();
    }

    // Cache management
    private class CacheIndex
    {
        private Dictionary<string, CacheEntry> _entries = new();

        public void Add(string scriptHash, string meshPath, int polygonCount)
        {
            _entries[scriptHash] = new CacheEntry
            {
                ScriptHash = scriptHash,
                MeshPath = meshPath,
                PolygonCount = polygonCount,
                Timestamp = DateTime.Now
            };
        }

        public bool TryGetValue(string scriptHash, out CacheEntry entry)
        {
            return _entries.TryGetValue(scriptHash, out entry);
        }

        public void CleanupOldEntries(int maxAgeDays = 30)
        {
            var cutoff = DateTime.Now.AddDays(-maxAgeDays);
            var toRemove = _entries.Where kvp =>
                kvp.Value.Timestamp < cutoff
            ).ToList();

            foreach (var (hash, entry) in toRemove)
            {
                if (File.Exists(entry.MeshPath))
                {
                    File.Delete(entry.MeshPath);
                }
                _entries.Remove(hash);
            }
        }
    }

    private class CacheEntry
    {
        public string ScriptHash;
        public string MeshPath;
        public int PolygonCount;
        public DateTime Timestamp;
    }
}

public struct CompileResult
{
    public bool Success;
    public string MeshPath;
    public int PolygonCount;
    public string ErrorLog;
}
```

---

## C# Implementation: OBJ Parser (Godot Integration)

```csharp
// OBJParser.cs
using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Civ.Artifacts;

/// <summary>
/// Parses OBJ files and generates Godot ArrayMesh.
/// Called after OpenSCAD compilation.
/// </summary>
public class OBJParser
{
    /// <summary>
    /// Parse OBJ file to Godot ArrayMesh (async)
    /// </summary>
    public static async Task<ArrayMesh> ParseAsync(string objPath)
    {
        return await Task.Run(() => Parse(objPath));
    }

    /// <summary>
    /// Parse OBJ file to Godot ArrayMesh (sync)
    /// </summary>
    public static ArrayMesh Parse(string objPath)
    {
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        var indices = new List<int>();

        // Parse OBJ file
        foreach (var line in File.ReadLines(objPath))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                continue;

            var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            switch (parts[0])
            {
                case "v":
                    // Vertex
                    vertices.Add(new Vector3(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture)
                    ));
                    break;

                case "vn":
                    // Normal
                    normals.Add(new Vector3(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture)
                    ));
                    break;

                case "vt":
                    // UV
                    uvs.Add(new Vector2(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture)
                    ));
                    break;

                case "f":
                    // Face (can be triangulated or quad)
                    ParseFace(parts, vertices.Count, indices);
                    break;
            }
        }

        return CreateGodotMesh(vertices, normals, uvs, indices);
    }

    private static void ParseFace(string[] parts, int vertexCount, List<int> indices)
    {
        // OBJ face format: f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3
        // Or: f v1 v2 v3 (vertex only)
        // Or: f v1//vn1 v2//vn2 v3//vn3 (vertex + normal)

        for (int i = 1; i < parts.Length; i++)
        {
            var indices_str = parts[i].Split('/');

            // Vertex index (OBJ is 1-based, convert to 0-based)
            int vIdx = int.Parse(indices_str[0]) - 1;
            indices.Add(vIdx);
        }
    }

    private static ArrayMesh CreateGodotMesh(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> indices)
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        // Vertices
        var vertArray = new Godot.Collections.Vector3Array();
        foreach (var v in vertices)
            vertArray.Add(v);
        arrays[(int)Mesh.ArrayType.Vertex] = vertArray;

        // Normals
        if (normals.Count > 0)
        {
            var normalArray = new Godot.Collections.Vector3Array();
            foreach (var n in normals)
                normalArray.Add(n);
            arrays[(int)Mesh.ArrayType.Normal] = normalArray;
        }

        // UVs
        if (uvs.Count > 0)
        {
            var uvArray = new Godot.Collections.Vector2Array();
            foreach (var uv in uvs)
                uvArray.Add(uv);
            arrays[(int)Mesh.ArrayType.TexUV] = uvArray;
        }

        // Indices
        var indexArray = new Godot.Collections.Int32Array();
        foreach (var idx in indices)
            indexArray.Add(idx);
        arrays[(int)Mesh.ArrayType.Index] = indexArray;

        // Create mesh
        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        return mesh;
    }
}
```

---

## UI Integration Points

### EngravingTable.gd Changes (Minimal)

**What It Does Now:**
- Circuit design and simulation
- Shows success/failure result
- "Return" button goes back to hub

**New Behavior:**
- When simulation succeeds, save formation to library
- Award XP immediately
- Update quest progress
- Return to hub (where Artifact Forge is now accessible)

```gdscript
# EngravingTable.gd - Enhanced

# Signals
signal formation_saved(formation_data)  # NEW: Emitted on success

# When simulation succeeds
func _on_simulation_success():
    # Save the working formation
    var formation_data = {
        "name": "Untitled Formation",
        "board_state": _get_current_board_state(),
        "stats": simulation_manager.get_circuit_stats(),
        "timestamp": Time.get_unix_time_from_system()
    }

    # Save to global formation library
    FormationLibrary.save_formation(formation_data)

    # Award XP
    XPManager.award_circuit_xp(formation_data.stats.complexity)

    # Update quest
    QuestManager.update_progress("circuit_completed", formation_data)

    # Show success dialog
    _show_success_dialog(formation_data)

    # Emit signal (for hub to know forge is available)
    formation_saved.emit(formation_data)

func _show_success_dialog(formation):
    var dialog = AcceptDialog.new()
    dialog.title = "✓ Circuit Works!"
    dialog.dialog_text = """
    Your formation is complete!

    Stats:
    • Nodes: {nodes}
    • Complexity: {complexity}
    • Max Amplitude: {amplitude}

    The formation has been saved to your library.
    Visit the Artifact Forge to create a physical artifact.
    """.format({
        "nodes": formation.stats.node_count,
        "complexity": formation.stats.complexity,
        "amplitude": formation.stats.max_amplitude
    })

    var return_btn = Button.new()
    return_btn.text = "Return to Sect Hub"
    return_btn.pressed.connect(_on_return_to_hub)
    dialog.add_child(return_btn)

    add_child(dialog)
    dialog.popup_centered()

func _on_return_to_hub():
    get_tree().change_scene_to_file("res://scenes/ui/SectHubScene.tscn")
```

---

## Scene Structure & Transitions

### Scene Architecture

```
MainMenu.tscn
    │
    ├─▶ New Game
    │       └─▶ SectHubScene.tscn (NEW - Central Hub)
    │               │
    │               ├─▶ EngravingTableScene.tscn (Existing)
    │               │       │
    │               │       ├─▶ Simulate Success
    │               │       │       └─▶ Return to SectHub
    │               │       │               └─▶ Artifact Forge now available
    │               │       │
    │               │       └─▶ Simulate Failure
    │               │               └─▶ Keep trying (no exit)
    │               │
    │               ├─▶ ArtifactForgeScene.tscn (NEW - Full Scene)
    │               │       │
    │               │       ├─▶ Select saved formation
    │               │       │       └─▶ Design artifact (OpenSCAD)
    │               │       │       └─▶ Forge artifact
    │               │       │               └─▶ Return to SectHub
    │               │       │
    │               │       └─▶ Cancel
    │               │               └─▶ Return to SectHub
    │               │
    │               ├─▶ TrainingGroundsScene.tscn (NEW)
    │               │       └─▶ Test artifacts in combat/action
    │               │
    │               └─▶ PlayerHousingScene.tscn (NEW)
    │                       └─▶ Enter cave dwelling
    │
    └─▶ Settings
```

### New Scene: SectHubScene.tscn

**Purpose**: Central hub where player navigates between activities

```
┌─────────────────────────────────────────────────────────────┐
│                  Sect Hub - Azure Peak                      │
│                                                               │
│                    [Quest Panel]                             │
│              Current: Craft Your First Artifact              │
│                  ☐☐☐☐ 2/4 complete                         │
│                                                               │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐          │
│  │ Engraving   │  │  Artifact   │  │  Training   │          │
│  │   Table     │  │    Forge    │  │  Grounds    │          │
│  │             │  │             │  │             │          │
│  │ [Enter]     │  │ [Enter]     │  │ [Enter]     │          │
│  │             │  │             │  │             │          │
│  │ Design      │  │ Glow when:  │  │ Test        │          │
│  │ circuits    │  │ formation   │  │ artifacts   │          │
│  │             │  │ ready!      │  │             │          │
│  └─────────────┘  └─────────────┘  └─────────────┘          │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ Player Stats                                         │    │
│  │ Level: 5  XP: 750/1000                               │    │
│  │ Artifacts Created: 1                                 │    │
│  │ Formations: 3                                        │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

---

## New Scene: ArtifactForgeScene.tscn

```
┌─────────────────────────────────────────────────────────────┐
│              Artifact Modeler                                │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  Top Toolbar                                         │    │
│  │  [Save] [Load] [Compile] [Test] [Cancel]            │    │
│  │  Mode: [Prefab Stitch ▾]                            │    │
│  │  Material: [Spirit Stone ▾]                         │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                               │
│  ┌──────────┐  ┌─────────────────────────────────┐          │
│  │ Prefab   │  │         3D Viewport              │          │
│  │ Palette  │  │                                 │          │
│  │          │  │                                 │          │
│  │ Basic:   │  │     [Artifact Mesh Preview]     │          │
│  │ ☐ Box    │  │                                 │          │
│  │ ☐ Sphere │  │         ⬇ Rotate               │          │
│  │ ☐ Cylinder│  │        ⬅ Pan ➡                │          │
│  │          │  │        🔍+ Zoom 🔍-             │          │
│  │ Weapon:  │  │                                 │          │
│  │ ☐ Blade  │  │   [Component Mount Points]      │          │
│  │ ☐ Hilt   │  │         🔵 🔵 🔵               │          │
│  │ ☐ Guard  │  │                                 │          │
│  │          │  │   [Engraving Zones]             │          │
│  │ Tool:    │  │         🟨 🟨 🟨               │          │
│  │ ☐ Handle │  │                                 │          │
│  │ ☐ Socket │  │                                 │          │
│  │          │  └─────────────────────────────────┘          │
│  │ [Custom] │                                                │
│  │          │  ┌─────────────────────────────────┐          │
│  └──────────┘  │ Code View (OpenSCAD)            │          │
│                │ ┌─────────────────────────────┐ │          │
│  ┌─────────────┤ │ artifact heating_stone() {   │ │          │
│  │ Properties  │ │   use Box(size=[5,5,3]);     │ │          │
│  │             │ │   use Dome(r=2.5);           │ │          │
│  │ Selected:   │ │   component_mount("socket",  │ │          │
│  │ Box         │ │     [2.5,2.5,3]);            │ │          │
│  │             │ │   engraving_zone("top",      │ │          │
│  │ Parameters: │ │     [0,0,3], [5,5,0.1]);     │ │          │
│  │ size: [5▾][3▾]│ │ }                            │ │          │
│  │             │ │ heating_stone();             │ │          │
│  │ [Add to      │ └─────────────────────────────┘ │          │
│  │  Design]     │ [Edit Code] [Generate]         │          │
│  └─────────────┘ └─────────────────────────────────┘          │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ Bottom Panel: Validation & Info                      │    │
│  │                                                      │    │
│  │ Components:                                          │    │
│  │ • Power Socket at [2.5, 2.5, 3] ✓                   │    │
│  │ • Effect Emitter at [2.5, 2.5, 5.5] ✓               │    │
│  │                                                      │    │
│  │ Stats:                                               │    │
│  │ • Polygon count: 850 / 5000 ✓                       │    │
│  │ • Engraving zones: 1 ✓                              │    │
│  │ • Material conductivity: 15 OK ✓                    │    │
│  │                                                      │    │
│  │ [COMPILE & TEST] [FORGE ARTIFACT]                   │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

---

## Quest Integration Example

```csharp
// QuestManager.cs
using Godot;
using System.Collections.Generic;

namespace Civ.Quests;

[GlobalClass]
public partial class QuestManager : Node
{
    private Dictionary<string, Quest> _activeQuests = new();
    private Dictionary<string, Quest> _completedQuests = new();

    public void UpdateArtifactCreated(ArtifactInstance artifact)
    {
        // Check all active quests for artifact creation objectives
        foreach (var (questId, quest) in _activeQuests)
        {
            quest.OnArtifactCreated(artifact);
        }
    }

    public void AwardArtifactCraftingXP(ArtifactInstance artifact)
    {
        // Calculate XP based on artifact complexity
        int xp = CalculateXP(artifact);
        AwardXP(xp);
    }

    private int CalculateXP(ArtifactInstance artifact)
    {
        // Base XP from complexity
        int baseXP = artifact.Stats.Complexity;

        // Multiplier for tier
        float tierMult = artifact.Tier switch
        {
            "Mortal" => 1.0f,
            "Spirit" => 2.0f,
            "Earth" => 5.0f,
            "Heaven" => 10.0f,
            _ => 1.0f
        };

        // Bonus for first artifact of type
        float firstBonus = IsFirstOfType(artifact) ? 1.5f : 1.0f;

        return (int)(baseXP * tierMult * firstBonus);
    }
}
```

---

## File Structure

```
game/
├── scripts/
│   ├── systems/
│   │   ├── EngravingTable.gd          # ENHANCED: Save formation, return to hub
│   │   └── FormationLibrary.gd        # NEW: Manage saved formations
│   ├── artifacts/                     # NEW: Artifact system directory
│   │   ├── ArtifactCreationWorkflow.cs
│   │   ├── OpenSCADBridge.cs
│   │   ├── OBJParser.cs
│   │   ├── ArtifactBlueprint.cs
│   │   ├── ArtifactInstance.cs
│   │   └── MaterialDatabase.cs
│   ├── quests/                        # NEW: Quest system
│   │   ├── QuestManager.cs
│   │   └── Quest.cs
│   ├── progression/                   # NEW: XP and leveling
│   │   ├── XPManager.cs
│   │   └── PlayerStats.cs
│   └── ui/
│       ├── SectHubController.gd       # NEW: Hub navigation
│       └── ArtifactForgeController.gd # NEW: Forge workflow controller
├── scenes/
│   ├── ui/
│   │   ├── MainMenu.tscn              # Existing
│   │   ├── SectHubScene.tscn          # NEW: Central hub
│   │   ├── ArtifactForgeScene.tscn    # NEW: Full artifact creation scene
│   │   └── MaterialSelectionDialog.tscn # NEW: Material picker
│   └── systems/
│       ├── EngravingTableScene.tscn   # Existing: Circuit design
│       └── TrainingGroundsScene.tscn  # NEW: Test artifacts
└── tools/
    └── openscad/                      # NEW: Bundled OpenSCAD
        ├── openscad.exe
        ├── openscad (Linux/Mac)
        ├── COPYING
        └── README.txt
```

---

## Summary: Integration Flow

```
Player Action                      System Response
─────────────                      ────────────────

1. Navigate from Hub             → Load EngravingTableScene
   [Click "Engraving Table"]       → Enter circuit design mode

2. Design working circuit          → Simulation validates circuit
   [Place runes, route traces]      → Show success dialog

3. Click "Return to Hub"           → Save formation to library
   [After successful simulation]    → Award circuit XP
                                     → Update quest progress
                                     → Load SectHubScene

4. Click "Artifact Forge"         → Load ArtifactForgeScene
   [Hub now shows glow effect]      → Show saved formations list

5. Select saved formation         → Analyze circuit stats
   [From formation library]         → Show compatible artifact types
                                     → Show material options

6. Select material & type          → Enter artifact modeling phase
   [Choose Spirit Stone, Heater]    → Load appropriate prefabs
                                     → Show 3D viewport

7. Design artifact shape           → Generate OpenSCAD script
   [Drag prefabs, set parameters]    → Compile to OBJ via OpenSCAD
                                     → Parse OBJ to Godot mesh
                                     → Validate polygon budget

8. Click "Forge Artifact"          → Create ArtifactInstance
   [In Artifact Forge scene]         → Save blueprint
                                     → Add to inventory
                                     → Update quest progress
                                     → Award artifact XP

9. Click "Return to Hub"           → Return to SectHubScene
   [After artifact creation]         → Show new artifact in inventory

10. Click "Training Grounds"       → Load TrainingGroundsScene
    [Test new artifact]              → Spawn artifact for testing
```

---

## Open Questions

- [ ] Should artifact creation be instant or take time (animation)?
- [ ] How to handle failed OpenSCAD compilation (show error line)?
- [ ] Multiplayer: Can players trade artifacts?
- [ ] Can artifacts be upgraded/reforged?
- [ ] How does player tier unlock artifact tiers?
