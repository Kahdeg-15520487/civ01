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

    // Singleton instance
    private static OpenSCADBridge _instance;
    public static OpenSCADBridge Instance => _instance;

    // Track if paths have been initialized
    private bool _pathsInitialized = false;

    public override void _Ready()
    {
        _instance = this;
        InitializePaths();
        GD.Print("OpenSCADBridge initialized");
    }

    /// <summary>
    /// Ensure paths are initialized (for use when created dynamically)
    /// </summary>
    private void EnsureInitialized()
    {
        if (!_pathsInitialized)
        {
            InitializePaths();
        }
    }

    private void InitializePaths()
    {
        // Path to bundled OpenSCAD binary
        var basePath = ProjectSettings.GlobalizePath("res://tools/openscad");

        // Cache directory for compiled meshes
        _cacheDir = Path.Combine(
            OS.GetUserDataDir(),
            "artifact_cache"
        );

        // Create cache directory if it doesn't exist
        if (!DirAccess.DirExistsAbsolute(_cacheDir))
        {
            DirAccess.MakeDirAbsolute(_cacheDir);
        }

        _openscadPath = Path.Combine(
            basePath,
            OS.HasFeature("windows") ? "openscad.exe" : "openscad"
        );

        GD.Print($"OpenSCAD path: {_openscadPath}");
        GD.Print($"Cache directory: {_cacheDir}");

        // Validate OpenSCAD exists
        if (!File.Exists(_openscadPath))
        {
            GD.PrintErr($"OpenSCAD binary not found at: {_openscadPath}");
            GD.PrintErr("OpenSCAD features will not work. Please bundle OpenSCAD with the game.");
        }

        _pathsInitialized = true;
    }

    /// <summary>
    /// Compile OpenSCAD script to OBJ mesh (async)
    /// </summary>
    /// <param name="scadScript">OpenSCAD script content</param>
    /// <param name="artifactTier">Artifact tier for polygon budget</param>
    /// <returns>Compilation result with mesh path</returns>
    public async Task<CompileResult> CompileAsync(
        string scadScript,
        string artifactTier = "Mortal")
    {
        return await Task.Run(() => Compile(scadScript, artifactTier));
    }

    /// <summary>
    /// Compile OpenSCAD script to OBJ mesh (sync)
    /// </summary>
    /// <param name="scadScript">OpenSCAD script content</param>
    /// <param name="artifactTier">Artifact tier for polygon budget</param>
    /// <returns>Compilation result with mesh path</returns>
    public CompileResult Compile(
        string scadScript,
        string artifactTier = "Mortal")
    {
        // Ensure paths are initialized (might not be if created dynamically)
        EnsureInitialized();

        var result = new CompileResult
        {
            Success = false
        };

        // Validate OpenSCAD exists
        if (string.IsNullOrEmpty(_openscadPath) || !File.Exists(_openscadPath))
        {
            result.ErrorLog = $"OpenSCAD binary not found. Path: {_openscadPath ?? "null"}. Please ensure OpenSCAD is bundled with the game at: tools/openscad/";
            return result;
        }

        // Create temp directory for this compilation
        string tempDir = Path.Combine(_cacheDir, Guid.NewGuid().ToString());
        try
        {
            Directory.CreateDirectory(tempDir);
        }
        catch (Exception ex)
        {
            result.ErrorLog = $"Failed to create temp directory: {ex.Message}";
            return result;
        }

        string scriptPath = Path.Combine(tempDir, "artifact.scad");
        string objPath = Path.Combine(tempDir, "artifact.obj");

        // Write script
        try
        {
            File.WriteAllText(scriptPath, scadScript);
        }
        catch (Exception ex)
        {
            result.ErrorLog = $"Failed to write script: {ex.Message}";
            return result;
        }

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

        GD.Print($"Executing: {processInfo.FileName} {processInfo.Arguments}");

        // Execute OpenSCAD
        Process process = null;
        try
        {
            process = Process.Start(processInfo);

            // Wait for completion (with timeout)
            if (process.WaitForExit(30000)) // 30 second timeout
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(output))
                    GD.Print($"OpenSCAD stdout: {output}");

                if (process.ExitCode == 0 && File.Exists(objPath))
                {
                    // Success
                    result.Success = true;
                    result.MeshPath = objPath;
                    result.PolygonCount = CountPolygons(objPath);

                    GD.Print($"OpenSCAD compilation successful: {objPath}");
                    GD.Print($"  Polygons: {result.PolygonCount} / {polyBudget}");

                    // Check polygon budget
                    if (result.PolygonCount > polyBudget)
                    {
                        result.Success = false;
                        result.ErrorLog = $"Polygon count {result.PolygonCount} exceeds budget {polyBudget}";
                        GD.PrintErr(result.ErrorLog);
                    }
                }
                else
                {
                    // Failure
                    result.Success = false;
                    result.ErrorLog = error;
                    GD.PrintErr($"OpenSCAD compilation failed (exit {process.ExitCode}): {error}");
                }
            }
            else
            {
                result.Success = false;
                result.ErrorLog = "Compilation timed out after 30 seconds";
                GD.PrintErr(result.ErrorLog);

                // Kill the process
                try
                {
                    process.Kill();
                }
                catch { }
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorLog = $"Exception during compilation: {ex.Message}";
            GD.PrintErr(result.ErrorLog);
        }
        finally
        {
            process?.Dispose();
        }

        return result;
    }

    /// <summary>
    /// Count polygon faces in an OBJ file
    /// </summary>
    private int CountPolygons(string objPath)
    {
        try
        {
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
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to count polygons: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Get polygon budget based on artifact tier
    /// </summary>
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

    /// <summary>
    /// Compute SHA256 hash of content
    /// </summary>
    private string ComputeHash(string content)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLower();
    }
}

/// <summary>
/// Result of OpenSCAD compilation
/// </summary>
public struct CompileResult
{
    /// <summary>True if compilation succeeded</summary>
    public bool Success;

    /// <summary>Path to generated OBJ file</summary>
    public string MeshPath;

    /// <summary>Number of polygons in mesh</summary>
    public int PolygonCount;

    /// <summary>Error message if failed</summary>
    public string ErrorLog;
}
