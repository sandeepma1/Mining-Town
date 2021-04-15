using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public TextAsset textAsset;
    public Material mat;
    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector2> uv = new List<Vector2>();
    public List<int> triangles = new List<int>();
    public float xSize;
    public float ySize;
    public string[,] mas;
    private const string nonHole = "1";
    private Vector4 tangent = new Vector4(1f, 0, 0, -1f);

    [Header("Texture")]
    [Space(10)]
    public float frequency = 6f;
    [Range(1, 8)] public int octaves = 8;
    [Range(1f, 4f)] public float lacunarity = 1.75f;
    [Range(0f, 1f)] public float persistence = 0.6f;
    [Range(1, 3)] public int dimensions = 2;
    public NoiseMethodType type = NoiseMethodType.Perlin;
    public Gradient coloring;
    private Texture2D texture;
    private int resolution;
    public int resolutionMultiplier = 8;

    private void Start()
    {
        CreateMesh();
        CreateTexture();
    }

    private void CreateMesh()
    {
        //mas = CSVReader_.SplitCsvGrid(textAsset.text);
        int vertexIndex = 0;
        GameObject go = new GameObject("Mesh");
        go.transform.SetParent(transform);
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        mesh.name = "Mas";
        xSize = mas.GetLength(0);
        ySize = mas.GetLength(1);
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                string res = mas[x, y];
                if (res == nonHole)
                {
                    Vector3[] currentFace = new Vector3[]{
                            new Vector3(0 + x, 0 + y, 0),
                            new Vector3(1 + x, 0 + y, 0),
                            new Vector3(0 + x, 1 + y, 0),
                            new Vector3(1 + x, 1 + y, 0)};
                    vertices.Add(currentFace[0]);
                    uv.Add(new Vector2(currentFace[0].x / xSize, currentFace[0].y / xSize));
                    vertices.Add(currentFace[1]);
                    uv.Add(new Vector2(currentFace[1].x / xSize, currentFace[1].y / xSize));
                    vertices.Add(currentFace[2]);
                    uv.Add(new Vector2(currentFace[2].x / xSize, currentFace[2].y / xSize));
                    vertices.Add(currentFace[3]);
                    uv.Add(new Vector2(currentFace[3].x / xSize, currentFace[3].y / xSize));
                    triangles.Add(0 + vertexIndex);
                    triangles.Add(2 + vertexIndex);
                    triangles.Add(1 + vertexIndex);
                    triangles.Add(1 + vertexIndex);
                    triangles.Add(2 + vertexIndex);
                    triangles.Add(3 + vertexIndex);
                    vertexIndex += 4;
                }
            }
        }
        Vector4[] tangents = new Vector4[uv.Count];
        for (int i = 0; i < uv.Count; i++) { tangents[i] = tangent; }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.tangents = tangents;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.Optimize();
        mf.mesh = mesh;
        mesh.RecalculateBounds();
        mr.material = mat;
    }

    private void CreateTexture()
    {
        resolution = (int)((xSize + ySize) / 2) * resolutionMultiplier;
        texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.anisoLevel = 0;
        mat.mainTexture = texture;
        FillTexture();
    }

    private void FillTexture()
    {
        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));

        NoiseMethod method = Noise.methods[(int)type][dimensions - 1];
        float stepSize = 1f / resolution;
        for (int y = 0; y < resolution; y++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            for (int x = 0; x < resolution; x++)
            {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
                if (type != NoiseMethodType.Value)
                {
                    sample = sample * 0.5f + 0.5f;
                }
                texture.SetPixel(x, y, coloring.Evaluate(sample));
            }
        }
        texture.Apply();
    }
}