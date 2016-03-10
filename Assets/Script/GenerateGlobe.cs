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
    private int m_Seed = 1000;
    private int Seed
    {
        get { return m_Seed; }
        set { m_Seed = value; }
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
        GlobeMesh = Filter.mesh;
        Generate();
	}

    public void Generate()
    {
        //GlobeMesh = CreateGlobeMesh();
        //Filter.mesh = GlobeMesh;
        Noise.Seed = Seed;
        Renderer.material.mainTexture = CreateMeshNoiseTexture(GlobeMesh, Resolution, NoiseOld.NoiseMethods[(int)NoiseType][Dimensions - 1], Frequency, Octaves, Lacunarity, Persistance, Coloring);
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

    private Texture2D CreateMeshNoiseTexture(Mesh a_Mesh, int a_Resolution, NoiseMethod a_Method, float a_Frequency, int a_Octaves, float a_Lacunarity, float a_Persistance, Gradient a_Coloring)
    {
        Texture2D MeshTexture = new Texture2D(a_Resolution, a_Resolution, TextureFormat.RGB24, true);

        for(int i = 0; i < a_Resolution; i++)
        {
            for(int j = 0; j < a_Resolution; j++)
            {
                MeshTexture.SetPixel(i, j, Color.black);
            }
        }
        
        MeshTexture.Apply();

        MeshTexture.name = "Procedural Noise Texture";
        MeshTexture.wrapMode = TextureWrapMode.Clamp;
        MeshTexture.filterMode = FilterMode.Trilinear;
        MeshTexture.anisoLevel = 9;

        int CounterInside = 0;
        int CounterOutside = 0;
        int CounterOverlap = 0;

        int Triangles = 0;
        int TriangleVertex = 0;
        while (TriangleVertex < a_Mesh.triangles.Length)
        {
            int[] TriangleVertexIndicies = new int[]
            {
                a_Mesh.triangles[TriangleVertex + 0],
                a_Mesh.triangles[TriangleVertex + 1],
                a_Mesh.triangles[TriangleVertex + 2]
            };

            TriangleVertex += 3;
            Triangles++;

            Vector3[] Verticies = new Vector3[]
            {
                a_Mesh.vertices[TriangleVertexIndicies[0]],
                a_Mesh.vertices[TriangleVertexIndicies[1]],
                a_Mesh.vertices[TriangleVertexIndicies[2]]
            };
            Vector2[] VertexTextureUV = new Vector2[]
            {
                a_Mesh.uv[TriangleVertexIndicies[0]] * a_Resolution,
                a_Mesh.uv[TriangleVertexIndicies[1]] * a_Resolution,
                a_Mesh.uv[TriangleVertexIndicies[2]] * a_Resolution
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

            float UVArea = TriangleArea(VertexTextureUV[0], VertexTextureUV[1], VertexTextureUV[2]);

            for(int x = minX; x < maxX; x++)
            {
                for(int y = minY; y < maxY; y++)
                {
                    if(MeshTexture.GetPixel(x, y) != Color.black)
                    {
                        CounterOverlap++;
                        continue;
                    }

                    Vector2 Point2D = new Vector2();
                    bool Inside = false;
                    for (int k = 0; k < 4; k++)
                    {
                        Point2D.x = x + k / 2;
                        Point2D.y = y + k % 2;

                        if (PointInTriangle(Point2D, VertexTextureUV))
                        {
                            Inside = true;
                            break;
                        }
                    }
                    if(!Inside)
                    {
                        CounterOutside++;
                        continue;
                    }
                    
                    CounterInside++;

                    float[] Weights = new float[]
                    {
                        TriangleArea(Point2D, VertexTextureUV[1], VertexTextureUV[2]),
                        TriangleArea(Point2D, VertexTextureUV[0], VertexTextureUV[2]),
                        TriangleArea(Point2D, VertexTextureUV[0], VertexTextureUV[1])
                    };
                    float Sum = Weights[0] + Weights[1] + Weights[2];
                    Vector3 Point3D = (Verticies[0] * Weights[0] + Verticies[1] * Weights[1] + Verticies[2] * Weights[2]) / Sum;
                    
                    float Sample = (Noise.Perlin(Point3D, a_Frequency, a_Octaves, a_Lacunarity, a_Persistance) + 1f) / 2f;
                    MeshTexture.SetPixel(x, y, a_Coloring.Evaluate(Sample));
                }
            }
        }
        Debug.Log(CounterOverlap.ToString() + " overlapped (and ignored).");
        Debug.Log(CounterOutside.ToString() + " outside (and ignored).");
        Debug.Log(CounterInside.ToString() + " inside.");

        MeshTexture.Apply();

        Debug.Log("Mesh triangles: " + Triangles.ToString());

        return MeshTexture;
    }

    // Fix shit
    private bool PointInTriangle(Vector2 a_Point, Vector2[] a_Triangle)
    {
        float p0x = a_Triangle[0].x;
        float p1x = a_Triangle[1].x;
        float p2x = a_Triangle[2].x;
        float p0y = a_Triangle[0].y;
        float p1y = a_Triangle[1].y;
        float p2y = a_Triangle[2].y;

        float Area = 1f / 2 * (-p1y * p2x + p0y * (-p1x + p2x) + p0x * (p1y - p2y) + p1x * p2y);

        float s = 1f / (2 * Area) * (p0y * p2x - p0x * p2y + (p2y - p0y) * a_Point.x + (p0x - p2x) * a_Point.y);
        float t = 1f / (2 * Area) * (p0x * p1y - p0y * p1x + (p0y - p1y) * a_Point.x + (p1x - p0x) * a_Point.y);


        return s >= 0 && s <= 1 && t >= 0 && t <= 1 && s + t <= 1; 
    }
    private float TriangleArea(Vector2 a_PointA, Vector2 a_PointB, Vector2 a_PointC)
    {
        return Mathf.Abs(
                (
                a_PointA.x * (a_PointB.y - a_PointC.y) +
                a_PointB.x * (a_PointC.y - a_PointA.y) +
                a_PointC.x * (a_PointA.y - a_PointB.y)
            ) / 2f);
    }
}
