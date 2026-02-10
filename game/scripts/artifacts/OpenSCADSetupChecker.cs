using Godot;
using System;
using System.IO;
using System.Diagnostics;

namespace Civ.Artifacts;

/// <summary>
/// Helper to check OpenSCAD installation and provide setup guidance.
/// Run this script from the Godot editor console or attach to an AutoLoad node.
/// </summary>
public partial class OpenSCADSetupChecker : Node
{
    public override void _Ready()
    {
        CheckOpenSCADSetup();
    }

    public static void CheckOpenSCADSetup()
    {
        GD.Print("=== OpenSCAD Setup Checker ===");

        // Expected paths
        var basePath = ProjectSettings.GlobalizePath("res://tools/openscad");

        // For exported builds, check the executable directory
        if (!DirAccess.DirExistsAbsolute(basePath))
        {
            basePath = Path.Combine(OS.GetExecutableDir().GetBaseDir(), "tools", "openscad");
        }

        GD.Print($"Checking path: {basePath}");

        // Check if directory exists
        if (!DirAccess.DirExistsAbsolute(basePath))
        {
            GD.PrintErr("❌ OpenSCAD directory not found!");
            GD.Print($"   Expected: {basePath}");
            GD.Print("");
            PrintSetupInstructions();
            return;
        }

        GD.Print("✓ OpenSCAD directory exists");

        // Check for OpenSCAD binary
        string binaryName = OS.HasFeature("windows") ? "openscad.exe" : "openscad";
        string binaryPath = Path.Combine(basePath, binaryName);

        if (!File.Exists(binaryPath))
        {
            GD.PrintErr($"❌ OpenSCAD binary not found: {binaryPath}");
            GD.Print("");
            PrintSetupInstructions();
            return;
        }

        GD.Print($"✓ OpenSCAD binary found: {binaryPath}");

        // Try to get version
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = binaryPath,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            process.WaitForExit(5000); // 5 second timeout

            if (process.ExitCode == 0)
            {
                string output = process.StandardOutput.ReadToEnd();
                GD.Print($"✓ OpenSCAD version: {output.Trim()}");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"⚠ Could not get OpenSCAD version: {ex.Message}");
        }

        GD.Print("");
        GD.Print("=== Setup Complete ===");
        GD.Print("OpenSCAD integration is ready to use!");
        GD.Print("Run the OpenSCADTestScene to test the integration.");
    }

    private static void PrintSetupInstructions()
    {
        GD.Print("=== Setup Instructions ===");
        GD.Print("");
        GD.Print("1. Download OpenSCAD:");
        GD.Print("   https://openscad.org/downloads.html");
        GD.Print("");
        GD.Print("2. Install OpenSCAD to your system");
        GD.Print("");
        GD.Print("3. Copy OpenSCAD binary to your game project:");
        GD.Print("");

        if (OS.HasFeature("windows"))
        {
            GD.Print("   Copy from installation:");
            GD.Print("     C:\\Program Files\\OpenSCAD\\openscad.exe");
            GD.Print("     And all .dll files");
            GD.Print("");
            GD.Print("   To your game:");
            GD.Print($"     {ProjectSettings.GlobalizePath("res://tools/openscad")}");
            GD.Print("");
            GD.Print("   Or for exported builds:");
            GD.Print($"     {Path.Combine(OS.GetExecutableDir().GetBaseDir(), "tools", "openscad")}");
        }
        else if (OS.HasFeature("x11"))
        {
            GD.Print("   Linux: Copy /usr/bin/openscad to:");
            GD.Print($"     {ProjectSettings.GlobalizePath("res://tools/openscad/openscad")}");
        }
        else if (OS.HasFeature("osx"))
        {
            GD.Print("   macOS: Copy /Applications/OpenSCAD.app/Contents/MacOS/OpenSCAD to:");
            GD.Print($"     {ProjectSettings.GlobalizePath("res://tools/openscad/openscad")}");
        }

        GD.Print("");
        GD.Print("4. Restart Godot editor");
        GD.Print("5. Run this checker again to verify");
    }

    /// <summary>
    /// Quick check - returns true if OpenSCAD is properly set up
    /// </summary>
    public static bool IsOpenSCADAvailable()
    {
        var basePath = ProjectSettings.GlobalizePath("res://tools/openscad");
        if (!DirAccess.DirExistsAbsolute(basePath))
        {
            basePath = Path.Combine(OS.GetExecutableDir().GetBaseDir(), "tools", "openscad");
        }

        string binaryName = OS.HasFeature("windows") ? "openscad.exe" : "openscad";
        string binaryPath = Path.Combine(basePath, binaryName);

        return File.Exists(binaryPath);
    }
}
