using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaker : MonoBehaviour
{
    [SerializeField] private Material mat;
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uv = new List<Vector2>();
    private List<int> triangles = new List<int>();
    private float xSize;
    private float ySize;
    private int resolution;
    private GameObject meshMakerGo;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private int vertexIndex = 0;
    private Vector4 tangent = new Vector4(1f, 0, 0, -1f);

    [Header("Texture")]
    [Space(10)]
    public float frequency = 6f;
    [Range(1, 8)] [SerializeField] private int octaves = 8;
    [Range(1f, 4f)] [SerializeField] private float lacunarity = 1.75f;
    [Range(0f, 1f)] [SerializeField] private float persistence = 0.6f;
    [Range(1, 3)] [SerializeField] private int dimensions = 2;
    [SerializeField] private int resolutionMultiplier = 8;
    [SerializeField] private Gradient coloring;
    private NoiseMethodType type = NoiseMethodType.Perlin;
    private Texture2D texture;

    public void InitMesh(int xSize, int ySize)
    {
        meshMakerGo = new GameObject("MinesGroundMesh");
        meshMakerGo.transform.SetParent(transform);
        meshFilter = meshMakerGo.AddComponent<MeshFilter>();
        meshRenderer = meshMakerGo.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        this.xSize = xSize;
        this.ySize = ySize;
    }

    public void AddVertex(int x, int y)
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

    public void FinilizeMesh()
    {
        Vector4[] tangents = new Vector4[uv.Count];
        for (int i = 0; i < uv.Count; i++) { tangents[i] = tangent; }
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.tangents = tangents;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.Optimize();
        meshFilter.mesh = mesh;
        mesh.RecalculateBounds();
        meshRenderer.material = mat;
        CreateTexture();
        meshMakerGo.transform.position = new Vector3(-0.5f, 0, -0.5f);
        meshMakerGo.transform.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
    }

    private void CreateTexture()
    {
        resolution = (int)Mathf.Max(xSize, ySize) * resolutionMultiplier;// (int)((xSize + ySize) / 2) * resolutionMultiplier;
        texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
        texture.wrapMode = TextureWrapMode.Repeat;
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