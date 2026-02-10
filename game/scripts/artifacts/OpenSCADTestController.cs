using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Civ.Artifacts;

/// <summary>
/// Test controller for OpenSCAD integration.
/// Simple pipeline: Script → OpenSCAD → OBJ → Godot Mesh
/// </summary>
public partial class OpenSCADTestController : Node3D
{
    // UI References
    private TextEdit _scriptInput;
    private Button _compileButton;
    private Label _statusLabel;
    private MeshInstance3D _meshDisplay;
    private Camera3D _camera;

    // Camera orbit state
    private Vector3 _cameraOffset = new Vector3(10, 10, 10);
    private float _orbitRadius = 17.32f; // Distance from origin
    private float _orbitAngleHorizontal = 0.785f; // 45 degrees
    private float _orbitAngleVertical = 0.615f; // ~35 degrees
    private bool _isDragging = false;
    private bool _isPanning = false;
    private Vector2 _lastMousePos;

    // OpenSCAD bridge
    private OpenSCADBridge _openscadBridge;

    // Default test script (simple cylinder)
    private const string DefaultScript = @"
// Simple cylinder test
cylinder(h=10, r=5, center=false);
";

    public override void _Ready()
    {
        GD.Print("OpenSCADTestController _Ready() called");

        // Create UI
        _setup_ui();
        _setup_3d_scene();

        // Initialize bridge early
        _initialize_openscad_bridge();

        GD.Print($"OpenSCAD bridge after init: {_openscadBridge != null}");
    }

    private void _setup_ui()
    {
        // Create a Control node for UI
        var uiRoot = new Control();
        // Anchor to all corners - set directly
        uiRoot.AnchorLeft = 0.0f;
        uiRoot.AnchorTop = 0.0f;
        uiRoot.AnchorRight = 1.0f;
        uiRoot.AnchorBottom = 1.0f;
        uiRoot.OffsetLeft = 0;
        uiRoot.OffsetTop = 0;
        uiRoot.OffsetRight = 0;
        uiRoot.OffsetBottom = 0;
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
        // Note: WrapMode left at default
        _scriptInput = scriptInput;
        uiRoot.AddChild(scriptInput);

        // Compile button
        var compileBtn = new Button();
        compileBtn.Position = new Vector2(20, 310);
        compileBtn.Text = "Compile & Render";
        compileBtn.CustomMinimumSize = new Vector2(150, 40);
        compileBtn.Pressed += _on_compile_pressed;
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
        UpdateCameraPosition();
        AddChild(_camera);

        // Make camera current
        _camera.Current = true;

        // Lighting
        var directionalLight = new DirectionalLight3D();
        directionalLight.Position = new Vector3(10, 10, 10);
        directionalLight.LookAt(Vector3.Zero);
        AddChild(directionalLight);

        // Ambient light using WorldEnvironment
        var env = new WorldEnvironment();
        var godotEnv = new Godot.Environment();
        godotEnv.AmbientLightSource = Godot.Environment.AmbientSource.Color;
        godotEnv.AmbientLightColor = new Color(0.3f, 0.3f, 0.3f);
        env.Environment = godotEnv;
        AddChild(env);

        // Simple grid using primitive mesh
        _create_grid();

        // Mesh display
        _meshDisplay = new MeshInstance3D();
        AddChild(_meshDisplay);
    }

    private void _create_grid()
    {
        // Create a simple grid using primitive mesh
        var grid = new ArrayMesh();
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        // Vertices for 20x20 grid
        var verts = new Vector3[80];
        var idx = 0;
        var size = 10;

        // Draw grid lines
        for (int i = -10; i <= 10; i++)
        {
            // X-axis line
            verts[idx++] = new Vector3(i * size, 0, -10 * size);
            verts[idx++] = new Vector3(i * size, 0, 10 * size);
            // Z-axis line
            verts[idx++] = new Vector3(-10 * size, 0, i * size);
            verts[idx++] = new Vector3(10 * size, 0, i * size);
        }

        arrays[(int)Mesh.ArrayType.Vertex] = verts;
        // Create index array (0, 1, 2, 3, ...)
        var indices = new int[idx];
        for (int i = 0; i < idx; i++)
        {
            indices[i] = i;
        }
        arrays[(int)Mesh.ArrayType.Index] = indices;

        grid.AddSurfaceFromArrays(Mesh.PrimitiveType.Lines, arrays);

        var gridInstance = new MeshInstance3D();
        gridInstance.Mesh = grid;
        AddChild(gridInstance);
    }

    private void _initialize_openscad_bridge()
    {
        GD.Print("_initialize_openscad_bridge() called");
        // Create OpenSCAD bridge
        _openscadBridge = new OpenSCADBridge();
        GD.Print($"Created bridge, is null? {_openscadBridge == null}");
        AddChild(_openscadBridge);
        GD.Print("Bridge added as child");
    }

    private async void _on_compile_pressed()
    {
        // Ensure bridge is initialized
        if (_openscadBridge == null)
        {
            GD.PrintErr("OpenSCAD bridge is null! Re-initializing...");
            _initialize_openscad_bridge();
        }

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
            var totalStopwatch = Stopwatch.StartNew();
            GD.Print("═══ Starting compilation pipeline ═══");
            GD.Print($"Bridge instance: {_openscadBridge != null}");

            // Step 1: Compile with OpenSCAD
            var compileStopwatch = Stopwatch.StartNew();
            var compileResult = await _openscadBridge.CompileAsync(script, "Mortal");
            compileStopwatch.Stop();

            if (!compileResult.Success)
            {
                _update_status($"Compilation failed: {compileResult.ErrorLog}", true);
                _compileButton.Disabled = false;
                return;
            }

            GD.Print($"✓ OpenSCAD compilation: {compileStopwatch.ElapsedMilliseconds}ms");
            GD.Print($"  Output: {compileResult.MeshPath}");
            GD.Print($"  Polygons: {compileResult.PolygonCount}");

            // Step 2: Parse OBJ to Godot mesh
            var parseStopwatch = Stopwatch.StartNew();
            GD.Print("Starting OBJ parse...");
            _update_status($"Parsing OBJ ({compileResult.PolygonCount} polygons)...", false);

            var mesh = await OBJParser.ParseAsync(compileResult.MeshPath);
            parseStopwatch.Stop();

            GD.Print($"✓ OBJ parsing: {parseStopwatch.ElapsedMilliseconds}ms");
            GD.Print($"  Mesh is null: {mesh == null}");

            if (mesh == null)
            {
                GD.PrintErr("Mesh is null after parsing!");
                _update_status("Failed to parse OBJ file!", true);
                _compileButton.Disabled = false;
                return;
            }

            GD.Print("OBJ parsing successful - setting mesh to display");

            // Step 3: Display mesh in 3D viewport
            var displayStopwatch = Stopwatch.StartNew();

            if (_meshDisplay == null)
            {
                GD.PrintErr("_meshDisplay is null! Re-creating...");
                _meshDisplay = new MeshInstance3D();
                AddChild(_meshDisplay);
            }

            _meshDisplay.Mesh = mesh;
            displayStopwatch.Stop();
            GD.Print($"✓ Mesh display assignment: {displayStopwatch.ElapsedMilliseconds}ms");

            // Center the mesh
            var centerStopwatch = Stopwatch.StartNew();
            _center_mesh();
            centerStopwatch.Stop();
            GD.Print($"✓ Mesh centering: {centerStopwatch.ElapsedMilliseconds}ms");

            totalStopwatch.Stop();

            GD.Print("═══ Pipeline complete ═══");
            GD.Print($"Total time: {totalStopwatch.ElapsedMilliseconds}ms");
            GD.Print($"  - OpenSCAD compilation: {compileStopwatch.ElapsedMilliseconds}ms ({100f * compileStopwatch.ElapsedMilliseconds / totalStopwatch.ElapsedMilliseconds:F1}%)");
            GD.Print($"  - OBJ parsing: {parseStopwatch.ElapsedMilliseconds}ms ({100f * parseStopwatch.ElapsedMilliseconds / totalStopwatch.ElapsedMilliseconds:F1}%)");
            GD.Print($"  - Mesh display: {displayStopwatch.ElapsedMilliseconds}ms ({100f * displayStopwatch.ElapsedMilliseconds / totalStopwatch.ElapsedMilliseconds:F1}%)");
            GD.Print($"  - Mesh centering: {centerStopwatch.ElapsedMilliseconds}ms ({100f * centerStopwatch.ElapsedMilliseconds / totalStopwatch.ElapsedMilliseconds:F1}%)");

            _update_status($"Success! {compileResult.PolygonCount} polys in {totalStopwatch.ElapsedMilliseconds}ms", false);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error during compilation: {ex.GetType().Name}: {ex.Message}");
            GD.PrintErr($"Stack trace: {ex.StackTrace}");
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
        {
            GD.PrintErr("_center_mesh: _meshDisplay.Mesh is null");
            return;
        }

        var mesh = _meshDisplay.Mesh as ArrayMesh;
        if (mesh == null)
        {
            GD.PrintErr("_center_mesh: mesh is null or not ArrayMesh");
            return;
        }

        try
        {
            // Get mesh bounds
            var aabb = mesh.GetAabb();
            GD.Print($"Mesh AABB: position={aabb.Position}, size={aabb.Size}");

            // Calculate center offset
            var center = aabb.Position + aabb.Size / 2;

            // Position mesh to center it at origin
            _meshDisplay.Position = -center;

            GD.Print($"Mesh centered. New position: {_meshDisplay.Position}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error centering mesh: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void _update_status(string message, bool isError)
    {
        _statusLabel.Text = message;
        _statusLabel.AddThemeColorOverride(
            "font_color",
            isError ? Colors.Red : Colors.Green
        );
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse button press
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed)
            {
                _lastMousePos = mouseButton.Position;

                if (mouseButton.ButtonIndex == MouseButton.Left)
                {
                    _isDragging = true;
                }
                else if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    _isPanning = true;
                }
                else if (mouseButton.ButtonIndex == MouseButton.Middle)
                {
                    // Reset camera
                    _orbitRadius = 17.32f;
                    _orbitAngleHorizontal = 0.785f;
                    _orbitAngleVertical = 0.615f;
                    UpdateCameraPosition();
                }
            }
            else
            {
                if (mouseButton.ButtonIndex == MouseButton.Left)
                {
                    _isDragging = false;
                }
                else if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    _isPanning = false;
                }
            }
        }

        // Mouse motion
        if (@event is InputEventMouseMotion mouseMotion)
        {
            var delta = mouseMotion.Position - _lastMousePos;
            _lastMousePos = mouseMotion.Position;

            // Orbit (left click drag)
            if (_isDragging)
            {
                _orbitAngleHorizontal -= delta.X * 0.01f;
                _orbitAngleVertical = Mathf.Clamp(
                    _orbitAngleVertical + delta.Y * 0.01f,
                    -Mathf.Pi / 2 + 0.1f,
                    Mathf.Pi / 2 - 0.1f
                );
                UpdateCameraPosition();
            }

            // Pan (right click drag)
            if (_isPanning)
            {
                var panSpeed = _orbitRadius * 0.002f;
                var right = _camera.Basis.X;
                var up = new Vector3(0, 1, 0);

                _camera.Position -= right * delta.X * panSpeed;
                _camera.Position += up * delta.Y * panSpeed;
            }
        }

        // Mouse wheel for zoom
        if (@event is InputEventMouseButton wheel && wheel.ButtonIndex == MouseButton.WheelUp)
        {
            _orbitRadius = Mathf.Max(2.0f, _orbitRadius * 0.9f);
            UpdateCameraPosition();
        }
        if (@event is InputEventMouseButton wheelDown && wheelDown.ButtonIndex == MouseButton.WheelDown)
        {
            _orbitRadius = Mathf.Min(100.0f, _orbitRadius * 1.1f);
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        // Convert spherical coordinates to Cartesian
        float x = _orbitRadius * Mathf.Cos(_orbitAngleVertical) * Mathf.Sin(_orbitAngleHorizontal);
        float y = _orbitRadius * Mathf.Sin(_orbitAngleVertical);
        float z = _orbitRadius * Mathf.Cos(_orbitAngleVertical) * Mathf.Cos(_orbitAngleHorizontal);

        _camera.Position = new Vector3(x, y, z);
        _camera.LookAt(Vector3.Zero);
    }
}
