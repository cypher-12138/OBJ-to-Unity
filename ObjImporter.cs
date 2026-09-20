using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjImporter : MonoBehaviour
{
    // 解析出来的原始顶点（模型本地顶点，可直接访问）
    public List<Vector3> rawVertices { get; private set; }
    public List<Vector2> rawUVs { get; private set; }
    public List<Vector3> rawNormals { get; private set; }

    private List<Vector3> meshVertices;
    private List<Vector2> meshUVs;
    private List<Vector3> meshNormals;
    private List<int> triangles;

    public GameObject ImportObj(string filePath)
    {
        rawVertices = new List<Vector3>();
        rawUVs = new List<Vector2>();
        rawNormals = new List<Vector3>();

        meshVertices = new List<Vector3>();
        meshUVs = new List<Vector2>();
        meshNormals = new List<Vector3>();
        triangles = new List<int>();

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            switch (parts[0])
            {
                case "v":
                    rawVertices.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])
                    ));
                    break;
                case "vt":
                    rawUVs.Add(new Vector2(
                        float.Parse(parts[1]),
                        float.Parse(parts[2])
                    ));
                    break;
                case "vn":
                    rawNormals.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])
                    ));
                    break;
                case "f":
                    ParseFace(parts);
                    break;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = meshVertices.ToArray();
        mesh.uv = meshUVs.ToArray();
        mesh.normals = meshNormals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();

        GameObject obj = new GameObject("ImportedOBJ");
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
        return obj;
    }

    void ParseFace(string[] parts)
    {
        for (int i = 1; i < parts.Length - 2; i++)
        {
            AddFaceVertex(parts[1]);
            AddFaceVertex(parts[i + 1]);
            AddFaceVertex(parts[i + 2]);
        }
    }

    void AddFaceVertex(string faceStr)
    {
        string[] indices = faceStr.Split('/');
        int vIdx = int.Parse(indices[0]) - 1;
        int vtIdx = indices[1] == "" ? 0 : int.Parse(indices[1]) - 1;
        int vnIdx = indices[2] == "" ? 0 : int.Parse(indices[2]) - 1;

        meshVertices.Add(rawVertices[vIdx]);
        if (rawUVs.Count > 0) meshUVs.Add(rawUVs[vtIdx]);
        if (rawNormals.Count > 0) meshNormals.Add(rawNormals[vnIdx]);
        triangles.Add(meshVertices.Count - 1);
    }

    // 对外接口：获取OBJ原始顶点
    public Vector3[] GetAllVertices()
    {
        return rawVertices.ToArray();
    }

    private void OnDrawGizmosSelected()
    {
        if (rawVertices == null) return;
        Gizmos.color = Color.red;
        foreach (var v in rawVertices)
        {
            Gizmos.DrawSphere(v, 0.05f);
        }
    }
}
