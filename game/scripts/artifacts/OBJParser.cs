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
public static class OBJParser
{
    /// <summary>
    /// Parse OBJ file to Godot ArrayMesh (async)
    /// </summary>
    /// <param name="objPath">Path to OBJ file</param>
    /// <returns>Godot ArrayMesh</returns>
    public static async Task<ArrayMesh> ParseAsync(string objPath)
    {
        return await Task.Run(() => Parse(objPath));
    }

    /// <summary>
    /// Parse OBJ file to Godot ArrayMesh (sync)
    /// </summary>
    /// <param name="objPath">Path to OBJ file</param>
    /// <returns>Godot ArrayMesh</returns>
    public static ArrayMesh Parse(string objPath)
    {
        if (!File.Exists(objPath))
        {
            GD.PrintErr($"OBJ file not found: {objPath}");
            return null;
        }

        GD.Print($"Parsing OBJ file: {objPath}");

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();

        // Parse OBJ file
        try
        {
            foreach (var line in File.ReadLines(objPath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    continue;

                switch (parts[0])
                {
                    case "v":
                        // Vertex: v x y z
                        if (parts.Length >= 4)
                        {
                            vertices.Add(new Vector3(
                                float.Parse(parts[1], CultureInfo.InvariantCulture),
                                float.Parse(parts[2], CultureInfo.InvariantCulture),
                                float.Parse(parts[3], CultureInfo.InvariantCulture)
                            ));
                        }
                        break;

                    case "vn":
                        // Normal: vn x y z
                        if (parts.Length >= 4)
                        {
                            normals.Add(new Vector3(
                                float.Parse(parts[1], CultureInfo.InvariantCulture),
                                float.Parse(parts[2], CultureInfo.InvariantCulture),
                                float.Parse(parts[3], CultureInfo.InvariantCulture)
                            ));
                        }
                        break;

                    case "vt":
                        // UV coordinate: vt u v
                        if (parts.Length >= 3)
                        {
                            uvs.Add(new Vector2(
                                float.Parse(parts[1], CultureInfo.InvariantCulture),
                                float.Parse(parts[2], CultureInfo.InvariantCulture)
                            ));
                        }
                        break;
                }
            }

            // Now parse faces to generate indices
            var indices = new List<int>();
            foreach (var line in File.ReadLines(objPath))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("f"))
                {
                    ParseFace(trimmed, indices);
                }
            }

            GD.Print($"  Parsed: {vertices.Count} vertices, {indices.Count / 3} triangles");

            return CreateGodotMesh(vertices, normals, uvs, indices);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to parse OBJ: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Parse face definition and add indices
    /// </summary>
    /// <param name="faceLine">Face line from OBJ file</param>
    /// <param name="indices">Index list to add to</param>
    private static void ParseFace(string faceLine, List<int> indices)
    {
        // OBJ face format:
        // f v1 v2 v3           (vertex only)
        // f v1/vt1 v2/vt2 v3/vt3           (vertex/uv)
        // f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3 (vertex/uv/normal)
        // f v1//vn1 v2//vn2 v3//vn3         (vertex//normal)

        var parts = faceLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Skip the "f" prefix
        for (int i = 1; i < parts.Length; i++)
        {
            var indices_str = parts[i].Split('/');

            // Vertex index is always first (OBJ is 1-based, convert to 0-based)
            if (int.TryParse(indices_str[0], out int vIdx))
            {
                indices.Add(vIdx - 1);
            }
        }
    }

    /// <summary>
    /// Create Godot ArrayMesh from parsed data
    /// </summary>
    private static ArrayMesh CreateGodotMesh(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> indices)
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        // Convert lists to arrays for Godot
        var vertArray = vertices.ToArray();
        arrays[(int)Mesh.ArrayType.Vertex] = vertArray;

        // Normals (optional)
        if (normals.Count > 0 && normals.Count == vertices.Count)
        {
            var normalArray = normals.ToArray();
            arrays[(int)Mesh.ArrayType.Normal] = normalArray;
        }

        // UVs (optional)
        if (uvs.Count > 0)
        {
            var uvArray = uvs.ToArray();
            arrays[(int)Mesh.ArrayType.TexUV] = uvArray;
        }

        // Indices (required)
        var indexArray = indices.ToArray();
        arrays[(int)Mesh.ArrayType.Index] = indexArray;

        // Create mesh
        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        return mesh;
    }

    /// <summary>
    /// Calculate smooth normals for a mesh (if OBJ doesn't have them)
    /// </summary>
    public static Vector3[] CalculateNormals(
        Vector3[] vertices,
        int[] indices)
    {
        var normals = new Vector3[vertices.Length];

        // Initialize normals to zero
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.Zero;
        }

        // Sum normals for each face
        for (int i = 0; i < indices.Length; i += 3)
        {
            int i0 = indices[i];
            int i1 = indices[i + 1];
            int i2 = indices[i + 2];

            if (i0 >= normals.Length || i1 >= normals.Length || i2 >= normals.Length)
                continue;

            var v0 = vertices[i0];
            var v1 = vertices[i1];
            var v2 = vertices[i2];

            // Calculate face normal using cross product
            var edge1 = v1 - v0;
            var edge2 = v2 - v0;
            var normal = edge1.Cross(edge2).Normalized();

            // Add to each vertex
            normals[i0] += normal;
            normals[i1] += normal;
            normals[i2] += normal;
        }

        // Normalize all normals
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = normals[i].Normalized();
        }

        return normals;
    }
}
