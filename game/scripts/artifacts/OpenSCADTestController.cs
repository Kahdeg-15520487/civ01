using Godot;
using System;
using System.Threading.Tasks;

namespace Civ.Artifacts;

/// <summary>
/// Test controller for OpenSCAD integration.
/// Simple pipeline: Script → OpenSCAD → OBJ → Godot Mesh
/// </summary>
public partial class OpenSCADTestController : Node3D
{
    // UI References
    private LineEdit _scriptInput;
    private Button _compileButton;
    private Label _statusLabel;
    private MeshInstance3D _meshDisplay;
    private Camera3D _camera;

    // OpenSCAD bridge
    private OpenSCADBridge _openscadBridge;

    // Default test script (simple cylinder)
    private const string DefaultScript = @"
// Simple cylinder test
cylinder(h=10, r=5, center=false);
";

    public override void _Ready()
    {
        GD.Print("OpenSCADTestController initialized");

        // Create UI
        _setup_ui();
        _setup_3d_scene();
        _initialize_openscad_bridge();
    }

    private void _setup_ui()
    {
        // Create a Control node for UI
        var uiRoot = new Control();
        uiRoot.SetAnchorsAndOffsetsPreset(Control.PresetFullRect);
        AddChild(uiRoot);

        // Title
        var titleLabel = new Label();
        titleLabel.Position = new Vector2(20, 20);
        titleLabel.Text = "OpenSCAD Integration Test";
        titleLabel.AddThemeFontSizeOverride("font_size", 24);
        uiRoot.AddChild(titleLabel);

        // Script input label
        var inputLabel = new Label();
        inputLabel.Position = new Vector2(20, 60);
        inputLabel.Text = "OpenSCAD Script:";
        uiRoot.AddChild(inputLabel);

        // Script input text edit
        var scriptInput = new TextEdit();
        scriptInput.Position = new Vector2(20, 90);
        scriptInput.Size = new Vector2(500, 200);
        scriptInput.Text = DefaultScript;
        scriptInput.WrapMode = TextEdit.LineWrappingMode.Accessible;
        scriptInput.PlaceholderText = "Enter OpenSCAD script here...";
        _scriptInput = scriptInput;
        uiRoot.AddChild(scriptInput);

        // Compile button
        var compileBtn = new Button();
        compileBtn.Position = new Vector2(20, 310);
        compileBtn.Text = "Compile & Render";
        compileBtn.CustomMinimumSize = new Vector2(150, 40);
        compileBtn.Pressed.Connect(_on_compile_pressed);
        _compileButton = compileBtn;
        uiRoot.AddChild(compileBtn);

        // Status label
        var statusLabel = new Label();
        statusLabel.Position = new Vector2(200, 310);
        statusLabel.Size = new Vector2(600, 40);
        statusLabel.Text = "Ready. Click Compile to test OpenSCAD integration.";
        statusLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _statusLabel = statusLabel;
        uiRoot.AddChild(statusLabel);

        // Instructions
        var infoLabel = new Label();
        infoLabel.Position = new Vector2(20, 370);
        infoLabel.Size = new Vector2(600, 100);
        infoLabel.Text = @"Instructions:
1. Enter OpenSCAD script in the text box above
2. Click 'Compile & Render' to process the script
3. Result will appear in the 3D viewport
4. Use WASD to move camera, mouse to look around
5. Right-click + drag to rotate camera

Note: OpenSCAD binary must be in tools/openscad/ directory";
        infoLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        uiRoot.AddChild(infoLabel);
    }

    private void _setup_3d_scene()
    {
        // Camera
        _camera = new Camera3D();
        _camera.Position = new Vector3(10, 10, 10);
        _camera.LookAt(Vector3.Zero);
        AddChild(_camera);

        // Make camera current
        _camera.Current = true;

        // Lighting
        var directionalLight = new DirectionalLight3D();
        directionalLight.Position = new Vector3(10, 10, 10);
        directionalLight.LookAt(Vector3.Zero);
        AddChild(directionalLight);

        var ambientLight = new AmbientLight3D();
        ambientLight.AmbientColor = new Color(0.3f, 0.3f, 0.3f);
        AddChild(ambientLight);

        // Grid helper (visualize ground plane)
        var grid = new GridMesh();
        grid.Size = new Vector2(20, 20);
        grid.Subdivisions = 10;
        var gridInstance = new MeshInstance3D();
        gridInstance.Mesh = grid;
        AddChild(gridInstance);

        // Mesh display
        _meshDisplay = new MeshInstance3D();
        AddChild(_meshDisplay);
    }

    private void _initialize_openscad_bridge()
    {
        // Create OpenSCAD bridge
        _openscadBridge = new OpenSCADBridge();
        AddChild(_openscadBridge);
    }

    private async void _on_compile_pressed()
    {
        string script = _scriptInput.Text;

        if (string.IsNullOrWhiteSpace(script))
        {
            _update_status("Error: Script is empty!", true);
            return;
        }

        // Update UI
        _update_status("Compiling...", false);
        _compileButton.Disabled = true;

        try
        {
            GD.Print("Starting OpenSCAD compilation...");

            // Step 1: Compile with OpenSCAD
            var compileResult = await _openscadBridge.CompileAsync(script, "Mortal");

            if (!compileResult.Success)
            {
                _update_status($"Compilation failed: {compileResult.ErrorLog}", true);
                _compileButton.Disabled = false;
                return;
            }

            GD.Print($"OpenSCAD compilation successful: {compileResult.MeshPath}");
            GD.Print($"  Polygons: {compileResult.PolygonCount}");

            // Step 2: Parse OBJ to Godot mesh
            _update_status($"Parsing OBJ ({compileResult.PolygonCount} polygons)...", false);

            var mesh = await OBJParser.ParseAsync(compileResult.MeshPath);

            if (mesh == null)
            {
                _update_status("Failed to parse OBJ file!", true);
                _compileButton.Disabled = false;
                return;
            }

            GD.Print("OBJ parsing successful");

            // Step 3: Display mesh in 3D viewport
            _meshDisplay.Mesh = mesh;

            // Center the mesh
            _center_mesh();

            _update_status($"Success! Rendered {compileResult.PolygonCount} polygons.", false);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error during compilation: {ex.Message}");
            _update_status($"Error: {ex.Message}", true);
        }
        finally
        {
            _compileButton.Disabled = false;
        }
    }

    private void _center_mesh()
    {
        if (_meshDisplay.Mesh == null)
            return;

        var mesh = _meshDisplay.Mesh as ArrayMesh;
        if (mesh == null)
            return;

        // Get mesh bounds
        var aabb = mesh.GetAabb();

        // Calculate center offset
        var center = aabb.Position + aabb.Size / 2;

        // Position mesh to center it at origin
        _meshDisplay.Position = -center;

        GD.Print($"Mesh centered. Bounds: {aabb.Size}");
    }

    private void _update_status(string message, bool isError)
    {
        _statusLabel.Text = message;
        _statusLabel.AddThemeColorOverride(
            "font_color",
            isError ? Colors.Red : Colors.Green
        );
    }

    public override void _Process(double delta)
    {
        // Simple camera controls
        var moveSpeed = 10.0f;
        var rotSpeed = 0.5f;

        // WASD movement
        if (Input.IsActionPressed("ui_up")) // W
            _camera.Position += _camera.Basis.Z * moveSpeed * (float)delta;
        if (Input.IsActionPressed("ui_down")) // S
            _camera.Position -= _camera.Basis.Z * moveSpeed * (float)delta;
        if (Input.IsActionPressed("ui_left")) // A
            _camera.Position -= _camera.Basis.X * moveSpeed * (float)delta;
        if (Input.IsActionPressed("ui_right")) // D
            _camera.Position += _camera.Basis.X * moveSpeed * (float)delta;

        // Mouse orbit (right click + drag)
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            var mouseMotion = Input.GetLastMouseScreenVelocity();
            _camera.Position = _camera.Position.Rotated(Vector3.Up, -mouseMotion.X * rotSpeed * (float)delta);
            _camera.LookAt(Vector3.Zero);
        }
    }
}
