using UnityEngine;
//using System.Collections;

public class TextureCreator : MonoBehaviour {
    [SerializeField] [Range(2, 1024)]
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

    [SerializeField] [Range(1, 8)]
    private int m_Octaves = 3;
    private int Octaves
    {
        get { return m_Octaves; }
    }

    [SerializeField] [Range(1f, 4f)] 
    private float m_Lacunarity = 2f;
    private float Lacunarity
    {
        get { return m_Lacunarity; }
    }
    [SerializeField] [Range(0f, 1f)]
    private float m_Persistance = 0.5f;
    private float Persistance
    {
        get { return m_Persistance; }
    }
    
    [SerializeField] [Range(1, 3)]
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

    private Texture2D m_Texture;
    private Texture2D Texture
    {
        get { return m_Texture; }
    }

    private void OnEnable()
    {
        if (Texture == null)
        {
            m_Texture = new Texture2D(Resolution, Resolution, TextureFormat.RGB24, true);
            Texture.name = "Procedural Texture";
            Texture.wrapMode = TextureWrapMode.Clamp;
            Texture.filterMode = FilterMode.Trilinear;
            Texture.anisoLevel = 9;
            GetComponent<MeshRenderer>().material.mainTexture = Texture;
        }
        FillTexture();
    }
    private void Update()
    {
        if(transform.hasChanged)
        {
            transform.hasChanged = false;
            FillTexture();
        }
    }
    public void FillTexture()
    {
        if(Texture.width != Resolution)
        {
            Texture.Resize(Resolution, Resolution);
        }

        Vector3[] Points = new Vector3[4];
        Points[0] = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        Points[1] = transform.TransformPoint(new Vector3( 0.5f, -0.5f));
        Points[2] = transform.TransformPoint(new Vector3(-0.5f,  0.5f));
        Points[3] = transform.TransformPoint(new Vector3( 0.5f,  0.5f));

        int iMax = Resolution * Resolution;
        float StepSize = 1f / Resolution;
        NoiseMethod Method = Noise.NoiseMethods[(int)NoiseType][Dimensions - 1];
        for (int i = 0; i < iMax; i++)
        {
            int x = i % Resolution;
            int y = i / Resolution;

            Vector3 LerpPoint = Vector3.Lerp(
                Vector3.Lerp(Points[0], Points[2], (y + 0.5f) * StepSize),
                Vector3.Lerp(Points[1], Points[3], (y + 0.5f) * StepSize),
                (x + 0.5f) * StepSize
            );
            float Sample = Noise.Sum(Method, LerpPoint, Frequency, Octaves, Lacunarity, Persistance);
            if(NoiseType == NoiseMethodType.Perlin)
            {
                Sample = Sample * 0.5f + 0.5f;
            }

            Texture.SetPixel(x, y, Coloring.Evaluate(Sample));
        }
        Texture.Apply();
    }
}
