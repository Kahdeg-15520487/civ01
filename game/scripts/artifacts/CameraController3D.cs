using Godot;
using System;

namespace Civ.Artifacts;

/// <summary>
/// Orbit camera controller for 3D artifact modeling.
/// Controls: Left-click drag to orbit, Right-click drag to pan, Mouse wheel to zoom.
/// Middle-click to reset camera.
/// </summary>
public partial class CameraController3D : Node3D
{
    // Camera reference
    private Camera3D _camera;

    // Orbit parameters
    private float _orbitRadius = 20.0f;
    private float _orbitAngleHorizontal = Mathf.Pi / 4.0f;  // 45 degrees
    private float _orbitAngleVertical = Mathf.Pi / 6.0f;    // 30 degrees
    private Vector3 _focusPoint = Vector3.Zero;

    // Input state
    private bool _isOrbiting = false;
    private bool _isPanning = false;
    private Vector2 _lastMousePos;

    // Camera settings
    [Export]
    public float MinZoomDistance = 2.0f;

    [Export]
    public float MaxZoomDistance = 100.0f;

    [Export]
    public float ZoomSpeed = 0.1f;

    [Export]
    public float OrbitSensitivity = 0.01f;

    [Export]
    public float PanSpeed = 0.002f;

    [Export]
    public float MinVerticalAngle = -Mathf.Pi / 2 + 0.1f;

    [Export]
    public float MaxVerticalAngle = Mathf.Pi / 2 - 0.1f;

    /// <summary>
    /// Current focus point of the camera
    /// </summary>
    public Vector3 FocusPoint
    {
        get => _focusPoint;
        set
        {
            _focusPoint = value;
            UpdateCameraPosition();
        }
    }

    /// <summary>
    /// Current orbit radius (distance from focus point)
    /// </summary>
    public float OrbitRadius
    {
        get => _orbitRadius;
        set
        {
            _orbitRadius = Mathf.Clamp(value, MinZoomDistance, MaxZoomDistance);
            UpdateCameraPosition();
        }
    }

    public override void _Ready()
    {
        // Find or create camera
        _camera = GetNode<Camera3D>("Camera");
        if (_camera == null)
        {
            _camera = new Camera3D();
            _camera.Name = "Camera";
            AddChild(_camera);
        }

        _camera.Current = true;
        UpdateCameraPosition();
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse button events
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed)
            {
                _lastMousePos = mouseButton.Position;

                if (mouseButton.ButtonIndex == MouseButton.Left)
                {
                    _isOrbiting = true;
                }
                else if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    _isPanning = true;
                }
                else if (mouseButton.ButtonIndex == MouseButton.Middle)
                {
                    // Reset camera
                    ResetCamera();
                }
            }
            else
            {
                if (mouseButton.ButtonIndex == MouseButton.Left)
                {
                    _isOrbiting = false;
                }
                else if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    _isPanning = false;
                }
            }
        }

        // Mouse motion events
        if (@event is InputEventMouseMotion mouseMotion)
        {
            var delta = mouseMotion.Position - _lastMousePos;
            _lastMousePos = mouseMotion.Position;

            // Orbit (left click drag)
            if (_isOrbiting)
            {
                _orbitAngleHorizontal -= delta.X * OrbitSensitivity;
                _orbitAngleVertical = Mathf.Clamp(
                    _orbitAngleVertical + delta.Y * OrbitSensitivity,
                    MinVerticalAngle,
                    MaxVerticalAngle
                );
                UpdateCameraPosition();
            }

            // Pan (right click drag)
            if (_isPanning && _camera != null)
            {
                var right = _camera.Basis.X;
                var up = Vector3.Up;  // Use world up for consistent panning

                _focusPoint -= right * delta.X * PanSpeed * _orbitRadius;
                _focusPoint += up * delta.Y * PanSpeed * _orbitRadius;
                UpdateCameraPosition();
            }
        }

        // Mouse wheel for zoom
        if (@event is InputEventMouseButton wheel)
        {
            if (wheel.ButtonIndex == MouseButton.WheelUp)
            {
                _orbitRadius = Mathf.Max(MinZoomDistance, _orbitRadius * (1.0f - ZoomSpeed));
                UpdateCameraPosition();
            }
            else if (wheel.ButtonIndex == MouseButton.WheelDown)
            {
                _orbitRadius = Mathf.Min(MaxZoomDistance, _orbitRadius * (1.0f + ZoomSpeed));
                UpdateCameraPosition();
            }
        }
    }

    /// <summary>
    /// Update camera position based on orbit parameters
    /// </summary>
    private void UpdateCameraPosition()
    {
        if (_camera == null) return;

        // Convert spherical coordinates to Cartesian
        float x = _orbitRadius * Mathf.Cos(_orbitAngleVertical) * Mathf.Sin(_orbitAngleHorizontal);
        float y = _orbitRadius * Mathf.Sin(_orbitAngleVertical);
        float z = _orbitRadius * Mathf.Cos(_orbitAngleVertical) * Mathf.Cos(_orbitAngleHorizontal);

        _camera.Position = new Vector3(x, y, z);
        _camera.LookAt(_focusPoint);
    }

    /// <summary>
    /// Reset camera to default position
    /// </summary>
    public void ResetCamera()
    {
        _orbitRadius = 20.0f;
        _orbitAngleHorizontal = Mathf.Pi / 4.0f;
        _orbitAngleVertical = Mathf.Pi / 6.0f;
        _focusPoint = Vector3.Zero;
        UpdateCameraPosition();
    }

    /// <summary>
    /// Focus camera on a specific world position
    /// </summary>
    public void FocusOnPosition(Vector3 position)
    {
        _focusPoint = position;
        UpdateCameraPosition();
    }

    /// <summary>
    /// Get the camera's forward direction (towards focus)
    /// </summary>
    public Vector3 GetForwardDirection()
    {
        if (_camera == null) return Vector3.Forward;
        return -_camera.Basis.Z;
    }

    /// <summary>
    /// Get the camera's up direction
    /// </summary>
    public Vector3 GetUpDirection()
    {
        if (_camera == null) return Vector3.Up;
        return _camera.Basis.Y;
    }

    /// <summary>
    /// Get the camera's right direction
    /// </summary>
    public Vector3 GetRightDirection()
    {
        if (_camera == null) return Vector3.Right;
        return _camera.Basis.X;
    }

    /// <summary>
    /// Set camera angles directly
    /// </summary>
    public void SetOrbitAngles(float horizontal, float vertical)
    {
        _orbitAngleHorizontal = horizontal;
        _orbitAngleVertical = Mathf.Clamp(vertical, MinVerticalAngle, MaxVerticalAngle);
        UpdateCameraPosition();
    }
}
