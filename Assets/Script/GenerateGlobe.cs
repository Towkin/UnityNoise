using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GenerateGlobe : MonoBehaviour {

    private const float TAU = Mathf.PI * 2f; 

    private MeshRenderer m_Renderer;
    private MeshRenderer Renderer
    {
        get { return m_Renderer; }
        set { m_Renderer = value; }
    }

    private MeshFilter m_Filter;
    private MeshFilter Filter
    {
        get { return m_Filter; }
        set { m_Filter = value; }
    }

    [SerializeField] [Range(3, 1024)]
    private int m_VertexPrecision = 16;
    private int VertexPrecision
    {
        get { return m_VertexPrecision; }
    }
    [SerializeField] [Range(0.01f, 100.0f)]
    private float m_SphereRadius = 1;
    private float SphereRadius
    {
        get { return m_SphereRadius; }
    }

    private Mesh m_GlobeMesh;
    private Mesh GlobeMesh
    {
        get { return m_GlobeMesh; }
        set { m_GlobeMesh = value; }
    }

    [SerializeField]
    [Range(2, 1024)]
    private int m_Resolution = 256;
    private int Resolution
    {
        get { return m_Resolution; }
    }
    [SerializeField]
    private float m_Frequency = 1f;
    private float Frequency
    {
        get { return m_Frequency; }
    }

    [SerializeField]
    [Range(1, 8)]
    private int m_Octaves = 3;
    private int Octaves
    {
        get { return m_Octaves; }
    }

    [SerializeField]
    [Range(1f, 4f)]
    private float m_Lacunarity = 2f;
    private float Lacunarity
    {
        get { return m_Lacunarity; }
    }
    [SerializeField]
    [Range(0f, 1f)]
    private float m_Persistance = 0.5f;
    private float Persistance
    {
        get { return m_Persistance; }
    }

    [SerializeField]
    [Range(1, 3)]
    private int m_Dimensions = 1;
    private int Dimensions
    {
        get { return m_Dimensions; }
    }



    [SerializeField]
    private NoiseMethodType m_NoiseType;
    private NoiseMethodType NoiseType
    {
        get { return m_NoiseType; }
    }

    [SerializeField]
    private Gradient m_Coloring;
    private Gradient Coloring
    {
        get { return m_Coloring; }
    }

    void OnEnable () {
        Renderer = GetComponent<MeshRenderer>();
        Filter = GetComponent<MeshFilter>();
        Generate();
	}

    public void Generate()
    {
        GlobeMesh = CreateGlobeMesh();
        Filter.mesh = GlobeMesh;
        Renderer.material.mainTexture = CreateMeshNoiseTexture(GlobeMesh, Resolution, Noise.NoiseMethods[(int)NoiseType][Dimensions], Frequency, Octaves, Lacunarity, Persistance, Coloring);
    }

    private Mesh CreateGlobeMesh()
    {
        Mesh Globe = new Mesh();

        List<Vector3> VertexList = new List<Vector3>();
        List<int> PolyList = new List<int>();
        List<Vector2> UVList = new List<Vector2>();

        int iMax = VertexPrecision * VertexPrecision / 2;
        float TileSize = 360f / VertexPrecision;
        for (int i = 0; i < iMax; i++)
        {
            float[] x = new float[]
            {
                (i / VertexPrecision) * TileSize,
                (i / VertexPrecision) * TileSize + TileSize
            };

            float[] y = new float[]
            {
                (i % VertexPrecision) * TileSize,
                (i % VertexPrecision) * TileSize + TileSize
            };

            Vector3[] TileVerticies = new Vector3[4];

            for(int j = 0; j < 4; j++)
            {
                float vx = x[j / 2];
                float vy = y[j % 2];
                TileVerticies[j] = Quaternion.Euler(vx, vy, 0f) * Vector3.up;
            }
            
            int p = VertexList.Count;

            VertexList.AddRange(TileVerticies);
            UVList.AddRange(
                new Vector2[]
                {
                    new Vector2(x[0], y[0]) / 360f,
                    new Vector2(x[0], y[1]) / 360f,
                    new Vector2(x[1], y[0]) / 360f,
                    new Vector2(x[1], y[1]) / 360f
                }
            );
            PolyList.AddRange(
                new int[]
                {
                    p + 1, p + 0, p + 2,
                    p + 1, p + 2, p + 3
                }
            );
        }

        Globe.vertices = VertexList.ToArray();
        Globe.triangles = PolyList.ToArray();
        Globe.uv = UVList.ToArray();

        Globe.RecalculateBounds();
        Globe.RecalculateNormals();

        return Globe;
    }

    private Texture2D CreateMeshNoiseTexture(Mesh aMesh, int aResolution, NoiseMethod aMethod, float aFrequency, int aOctaves, float aLacunarity, float aPersistance, Gradient aColoring)
    {
        Texture2D MeshTexture = new Texture2D(aResolution, aResolution, TextureFormat.RGB24, true);

        MeshTexture.name = "Procedural Noise Texture";
        MeshTexture.wrapMode = TextureWrapMode.Clamp;
        MeshTexture.filterMode = FilterMode.Trilinear;
        MeshTexture.anisoLevel = 9;

        int TriangleVertex = 0;
        while (TriangleVertex < aMesh.triangles.Length)
        {
            int[] TriangleVertexIndicies = new int[]
            {
                aMesh.triangles[TriangleVertex + 0],
                aMesh.triangles[TriangleVertex + 1],
                aMesh.triangles[TriangleVertex + 2]
            };

            TriangleVertex += 3;

            Vector3[] Verticies = new Vector3[]
            {
                aMesh.vertices[TriangleVertexIndicies[0]],
                aMesh.vertices[TriangleVertexIndicies[1]],
                aMesh.vertices[TriangleVertexIndicies[2]]
            };
            Vector2[] VertexTextureUV = new Vector2[]
            {
                aMesh.uv[TriangleVertexIndicies[0]] * aResolution,
                aMesh.uv[TriangleVertexIndicies[1]] * aResolution,
                aMesh.uv[TriangleVertexIndicies[2]] * aResolution
            };

            int maxX = Mathf.Max(
                new int[]
                {
                    Mathf.CeilToInt(VertexTextureUV[0].x),
                    Mathf.CeilToInt(VertexTextureUV[1].x),
                    Mathf.CeilToInt(VertexTextureUV[2].x)
                }
            );
            int minX = Mathf.Min(
                new int[]
                {
                    Mathf.FloorToInt(VertexTextureUV[0].x),
                    Mathf.FloorToInt(VertexTextureUV[1].x),
                    Mathf.FloorToInt(VertexTextureUV[2].x)
                }
            );
            int maxY = Mathf.Max(
                new int[]
                {
                    Mathf.CeilToInt(VertexTextureUV[0].y),
                    Mathf.CeilToInt(VertexTextureUV[1].y),
                    Mathf.CeilToInt(VertexTextureUV[2].y)
                }
            );
            int minY = Mathf.Min(
                new int[]
                {
                    Mathf.FloorToInt(VertexTextureUV[0].y),
                    Mathf.FloorToInt(VertexTextureUV[1].y),
                    Mathf.FloorToInt(VertexTextureUV[2].y)
                }
            );

            for(int x = minX; x < maxX; x++)
            {
                for(int y = minY; y < maxY; y++)
                {
                    float Sample = Noise.Sum(aMethod, Verticies[0], aFrequency, aOctaves, aLacunarity, aPersistance);

                    MeshTexture.SetPixel(x, y, aColoring.Evaluate(Sample));
                }
            }

            
        }
        MeshTexture.Apply();

        return MeshTexture;
    }

    // Fix shit
    private bool PointInTriangle(Vector2 aPoint, Vector2[] aTriangle)
    {
        return true; 
    }
}
