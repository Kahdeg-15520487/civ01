using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Civ.Artifacts;

/// <summary>
/// Parses ASCII STL files and generates Godot ArrayMesh.
/// Called after OpenSCAD compilation.
/// </summary>
public static class STLParser
{
    /// <summary>
    /// Parse STL file to Godot ArrayMesh (async)
    /// </summary>
    /// <param name="stlPath">Path to STL file</param>
    /// <returns>Godot ArrayMesh</returns>
    public static async Task<ArrayMesh> ParseAsync(string stlPath)
    {
        return await Task.Run(() => Parse(stlPath));
    }

    /// <summary>
    /// Parse STL file to Godot ArrayMesh (sync)
    /// </summary>
    /// <param name="stlPath">Path to STL file</param>
    /// <returns>Godot ArrayMesh</returns>
    public static ArrayMesh Parse(string stlPath)
    {
        if (!File.Exists(stlPath))
        {
            GD.PrintErr($"STL file not found: {stlPath}");
            return null;
        }

        GD.Print($"Parsing STL file: {stlPath}");

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();

        try
        {
            string[] lines = File.ReadAllLines(stlPath);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                    continue;

                switch (parts[0])
                {
                    case "facet":
                        // facet normal x y z
                        if (parts.Length >= 5)
                        {
                            var normal = new Vector3(
                                float.Parse(parts[2], CultureInfo.InvariantCulture),
                                float.Parse(parts[3], CultureInfo.InvariantCulture),
                                float.Parse(parts[4], CultureInfo.InvariantCulture)
                            );
                            // STL stores one normal per triangle, add it 3 times (once per vertex)
                            normals.Add(normal);
                            normals.Add(normal);
                            normals.Add(normal);
                        }
                        break;

                    case "vertex":
                        // vertex x y z
                        if (parts.Length >= 4)
                        {
                            vertices.Add(new Vector3(
                                float.Parse(parts[1], CultureInfo.InvariantCulture),
                                float.Parse(parts[2], CultureInfo.InvariantCulture),
                                float.Parse(parts[3], CultureInfo.InvariantCulture)
                            ));
                        }
                        break;
                }
            }

            GD.Print($"  Parsed: {vertices.Count} vertices, {vertices.Count / 3} triangles");

            return CreateGodotMesh(vertices, normals);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to parse STL: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Create Godot ArrayMesh from parsed STL data
    /// </summary>
    private static ArrayMesh CreateGodotMesh(
        List<Vector3> vertices,
        List<Vector3> normals)
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        // Convert lists to arrays for Godot
        var vertArray = vertices.ToArray();
        arrays[(int)Mesh.ArrayType.Vertex] = vertArray;

        // Normals
        if (normals.Count > 0)
        {
            var normalArray = normals.ToArray();
            arrays[(int)Mesh.ArrayType.Normal] = normalArray;
        }

        // Create indices (0, 1, 2, 3, 4, 5, ...)
        var indices = new int[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            indices[i] = i;
        }
        arrays[(int)Mesh.ArrayType.Index] = indices;

        // Create mesh
        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        return mesh;
    }
}
