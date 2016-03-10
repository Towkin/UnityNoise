using UnityEngine;
using System.Collections.Generic;

public static class Noise
{

    private static int m_Seed = 0;
    public static int Seed
    {
        get { return m_Seed; }
        set
        {
            if(value != m_Seed)
            {
                Raw.Hash = GenerateHash(256, value);
                m_Seed = value;
            }
        }
    }
    
    // Fix to get seed stuffus.
    private static int[] GenerateHash(int a_Length, int a_Seed)
    {
        int OldSeed = Random.seed;
        Random.seed = a_Seed;

        List<int> NewHash = new List<int>(a_Length);
        for (int i = 0; i < a_Length; i++)
        {
            NewHash.Add(i);
        }
        for (int i = 0; i < a_Length; i++)
        {
            int SwitchIndex = Random.Range(0, a_Length);
            int SwitchValue = NewHash[SwitchIndex];
            NewHash[SwitchIndex] = NewHash[i];
            NewHash[i] = SwitchValue;
        }

        int[] ReturnHash = new int[a_Length * 2];
        for(int i = 0; i < 2; i++)
        {
            for(int j = 0; j < a_Length; j++)
            {
                ReturnHash[i * a_Length + j] = NewHash[j];
            }
        }
        Random.seed = OldSeed;

        return ReturnHash;
    }

    private static class Raw
    {
        public static int[] Hash =
        {
            151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
            140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
            247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
                57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
                74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
                60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
                65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
            200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
                52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
            207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
            119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
            129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
            218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
                81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
            184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
            222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

            151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
            140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
            247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
                57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
                74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
                60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
                65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
            200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
                52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
            207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
            119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
            129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
            218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
                81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
            184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
            222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
        };

        private const int HashMask = 255;
        private const float HashFactor = 1f / HashMask;

        private static float Smooth(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }
        
        public static float Value1D(float x)
        {
            int HashCoord = Mathf.FloorToInt(x);
            float LerpValue = Smooth(x - HashCoord);
            HashCoord &= HashMask;

            return
            Mathf.Lerp(
                Hash[HashCoord],
                Hash[HashCoord + 1],
                LerpValue
            ) * HashFactor;
        }
        public static float Value2D(float x, float y)
        {
            int HashCoordX0 = Mathf.FloorToInt(x);
            float LerpX = Smooth(x - HashCoordX0);
            HashCoordX0 &= HashMask;
            int HashCoordX1 = HashCoordX0 + 1;

            int Hash0 = Hash[HashCoordX0];
            int Hash1 = Hash[HashCoordX1];


            int HashCoordY0 = Mathf.FloorToInt(y);
            float LerpY = Smooth(y - HashCoordY0);
            HashCoordY0 &= HashMask;
            int HashCoordY1 = HashCoordY0 + 1;

            int Hash00 = Hash[Hash0 + HashCoordY0];
            int Hash10 = Hash[Hash1 + HashCoordY0];

            int Hash01 = Hash[Hash0 + HashCoordY1];
            int Hash11 = Hash[Hash1 + HashCoordY1];


            return
            Mathf.Lerp(
                Mathf.Lerp(
                    Hash00,
                    Hash10,
                    LerpX
                ),
                Mathf.Lerp(
                    Hash01,
                    Hash11,
                    LerpX
                ),
                LerpY
            ) * HashFactor;
        }
        public static float Value3D(float x, float y, float z)
        {
            int HashCoordX0 = Mathf.FloorToInt(x);
            float LerpX = Smooth(x - HashCoordX0);
            HashCoordX0 &= HashMask;
            int HashCoordX1 = HashCoordX0 + 1;

            int Hash0 = Hash[HashCoordX0];
            int Hash1 = Hash[HashCoordX1];


            int HashCoordY0 = Mathf.FloorToInt(y);
            float LerpY = Smooth(y - HashCoordY0);
            HashCoordY0 &= HashMask;
            int HashCoordY1 = HashCoordY0 + 1;

            int Hash00 = Hash[Hash0 + HashCoordY0];
            int Hash10 = Hash[Hash1 + HashCoordY0];

            int Hash01 = Hash[Hash0 + HashCoordY1];
            int Hash11 = Hash[Hash1 + HashCoordY1];


            int HashCoordZ0 = Mathf.FloorToInt(z);
            float LerpZ = Smooth(z - HashCoordZ0);
            HashCoordZ0 &= HashMask;
            int HashCoordZ1 = HashCoordZ0 + 1;

            int Hash000 = Hash[Hash00 + HashCoordZ0];
            int Hash100 = Hash[Hash10 + HashCoordZ0];

            int Hash010 = Hash[Hash01 + HashCoordZ0];
            int Hash110 = Hash[Hash11 + HashCoordZ0];

            int Hash001 = Hash[Hash00 + HashCoordZ1];
            int Hash101 = Hash[Hash10 + HashCoordZ1];

            int Hash011 = Hash[Hash01 + HashCoordZ1];
            int Hash111 = Hash[Hash11 + HashCoordZ1];
            
            return
            Mathf.Lerp(
                Mathf.Lerp(
                    Mathf.Lerp(
                        Hash000,
                        Hash100,
                        LerpX
                    ),
                    Mathf.Lerp(
                        Hash010,
                        Hash110,
                        LerpX
                    ),
                    LerpY
                ),
                Mathf.Lerp(
                    Mathf.Lerp(
                        Hash001,
                        Hash101,
                        LerpX
                    ),
                    Mathf.Lerp(
                        Hash011,
                        Hash111,
                        LerpX
                    ),
                    LerpY
                ),
                LerpZ
            ) * HashFactor;
        }
        
        private static float[] Gradients1D =
        {
            1f, -1f
        };
        private const int GradientsMask1D = 1;
        public static float Perlin1D(float x)
        {
            int HashCoord0 = Mathf.FloorToInt(x);
            float Decimal0 = x - HashCoord0;
            float Decimal1 = Decimal0 - 1f;
            float LerpValue = Smooth(Decimal0);

            HashCoord0 &= HashMask;
            int HashCoord1 = HashCoord0 + 1;

            float Gradient0 = Gradients1D[Hash[HashCoord0] & GradientsMask1D];
            float Gradient1 = Gradients1D[Hash[HashCoord1] & GradientsMask1D];

            float Velocity0 = Gradient0 * Decimal0;
            float Velocity1 = Gradient1 * Decimal1;

            

            return Mathf.Lerp(Velocity0, Velocity1, LerpValue) * 2f;
        }

        private static Vector2[] Gradients2D =
        {
            new Vector2( 1f, 0f),
            new Vector2(-1f, 0f),
            new Vector2( 0f, 1f),
            new Vector2( 0f,-1f),
            new Vector2( 1f, 1f).normalized,
            new Vector2(-1f, 1f).normalized,
            new Vector2( 1f,-1f).normalized,
            new Vector2(-1f,-1f).normalized
        };
        private const int GradientsMask2D = 7;
        private static float Sqrt2 = Mathf.Sqrt(2f);
        private static float Dot(Vector2 g, float x, float y)
        {
            return g.x * x + g.y * y;
        }
        public static float Perlin2D(float x, float y)
        {
            int HashCoordX0 = Mathf.FloorToInt(x);
            float DecimalX0 = x - HashCoordX0;
            float DecimalX1 = DecimalX0 - 1f;
            float LerpX = Smooth(DecimalX0);
            HashCoordX0 &= HashMask;
            int HashCoordX1 = HashCoordX0 + 1;

            int Hash0 = Hash[HashCoordX0];
            int Hash1 = Hash[HashCoordX1];


            int HashCoordY0 = Mathf.FloorToInt(y);
            float DecimalY0 = y - HashCoordY0;
            float DecimalY1 = DecimalY0 - 1f;
            float LerpY = Smooth(DecimalY0);
            HashCoordY0 &= HashMask;
            int HashCoordY1 = HashCoordY0 + 1;

            Vector2 Gradient00 = Gradients2D[Hash[Hash0 + HashCoordY0] & GradientsMask2D];
            Vector2 Gradient10 = Gradients2D[Hash[Hash1 + HashCoordY0] & GradientsMask2D];
            Vector2 Gradient01 = Gradients2D[Hash[Hash0 + HashCoordY1] & GradientsMask2D];
            Vector2 Gradient11 = Gradients2D[Hash[Hash1 + HashCoordY1] & GradientsMask2D];

            float Velocity00 = Dot(Gradient00, DecimalX0, DecimalY0);
            float Velocity10 = Dot(Gradient10, DecimalX1, DecimalY0);
            float Velocity01 = Dot(Gradient01, DecimalX0, DecimalY1);
            float Velocity11 = Dot(Gradient11, DecimalX1, DecimalY1);

            return
            Mathf.Lerp(
                Mathf.Lerp(
                    Velocity00,
                    Velocity10,
                    LerpX
                ),
                Mathf.Lerp(
                    Velocity01,
                    Velocity11,
                    LerpX
                ),
                LerpY
            ) * Sqrt2;
        }

        private static Vector3[] Gradients3D = 
        {
            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 1f,-1f, 0f),
            new Vector3(-1f,-1f, 0f),
            new Vector3( 1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f),
            new Vector3( 1f, 0f,-1f),
            new Vector3(-1f, 0f,-1f),
            new Vector3( 0f, 1f, 1f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f, 1f,-1f),
            new Vector3( 0f,-1f,-1f),

            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f,-1f,-1f)
        };

        private const int GradientsMask3D = 15;
        private static float Dot(Vector3 g, float x, float y, float z)
        {
            return g.x * x + g.y * y + g.z * z;
        }
        public static float Perlin3D(float x, float y, float z)
        {
            int HashCoordX0 = Mathf.FloorToInt(x);
            float DecimalX0 = x - HashCoordX0;
            float DecimalX1 = DecimalX0 - 1f;
            float LerpX = Smooth(DecimalX0);
            HashCoordX0 &= HashMask;
            int HashCoordX1 = HashCoordX0 + 1;

            int Hash0 = Hash[HashCoordX0];
            int Hash1 = Hash[HashCoordX1];


            int HashCoordY0 = Mathf.FloorToInt(y);
            float DecimalY0 = y - HashCoordY0;
            float DecimalY1 = DecimalY0 - 1f;
            float LerpY = Smooth(DecimalY0);
            HashCoordY0 &= HashMask;
            int HashCoordY1 = HashCoordY0 + 1;

            int Hash00 = Hash[Hash0 + HashCoordY0];
            int Hash10 = Hash[Hash1 + HashCoordY0];

            int Hash01 = Hash[Hash0 + HashCoordY1];
            int Hash11 = Hash[Hash1 + HashCoordY1];


            int HashCoordZ0 = Mathf.FloorToInt(z);
            float DecimalZ0 = z - HashCoordZ0;
            float DecimalZ1 = DecimalZ0 - 1f;
            float LerpZ = Smooth(DecimalZ0);
            HashCoordZ0 &= HashMask;
            int HashCoordZ1 = HashCoordZ0 + 1;

            Vector3 Gradient000 = Gradients3D[Hash[Hash00 + HashCoordZ0] & GradientsMask3D];
            Vector3 Gradient100 = Gradients3D[Hash[Hash10 + HashCoordZ0] & GradientsMask3D];
            Vector3 Gradient010 = Gradients3D[Hash[Hash01 + HashCoordZ0] & GradientsMask3D];
            Vector3 Gradient110 = Gradients3D[Hash[Hash11 + HashCoordZ0] & GradientsMask3D];

            Vector3 Gradient001 = Gradients3D[Hash[Hash00 + HashCoordZ1] & GradientsMask3D];
            Vector3 Gradient101 = Gradients3D[Hash[Hash10 + HashCoordZ1] & GradientsMask3D];
            Vector3 Gradient011 = Gradients3D[Hash[Hash01 + HashCoordZ1] & GradientsMask3D];
            Vector3 Gradient111 = Gradients3D[Hash[Hash11 + HashCoordZ1] & GradientsMask3D];

            float Velocity000 = Dot(Gradient000, DecimalX0, DecimalY0, DecimalZ0);
            float Velocity100 = Dot(Gradient100, DecimalX1, DecimalY0, DecimalZ0);
            float Velocity010 = Dot(Gradient010, DecimalX0, DecimalY1, DecimalZ0);
            float Velocity110 = Dot(Gradient110, DecimalX1, DecimalY1, DecimalZ0);
            float Velocity001 = Dot(Gradient001, DecimalX0, DecimalY0, DecimalZ1);
            float Velocity101 = Dot(Gradient101, DecimalX1, DecimalY0, DecimalZ1);
            float Velocity011 = Dot(Gradient011, DecimalX0, DecimalY1, DecimalZ1);
            float Velocity111 = Dot(Gradient111, DecimalX1, DecimalY1, DecimalZ1);

            return
            Mathf.Lerp(
                Mathf.Lerp(
                    Mathf.Lerp(
                        Velocity000,
                        Velocity100,
                        LerpX
                    ),
                    Mathf.Lerp(
                        Velocity010,
                        Velocity110,
                        LerpX
                    ),
                    LerpY
                ),
                Mathf.Lerp(
                    Mathf.Lerp(
                        Velocity001,
                        Velocity101,
                        LerpX
                    ),
                    Mathf.Lerp(
                        Velocity011,
                        Velocity111,
                        LerpX
                    ),
                    LerpY
                ),
                LerpZ
            );
        }
    }

    public static float Value(float a_Coordinate, float a_Frequency)
    {
        return Raw.Value1D(a_Coordinate * a_Frequency);
    }
    public static float Value(Vector2 a_Coordinate, float a_Frequency)
    {
        a_Coordinate *= a_Frequency;
        return Raw.Value2D(a_Coordinate.x, a_Coordinate.y);
    }
    public static float Value(Vector3 a_Coordinate, float a_Frequency)
    {
        a_Coordinate *= a_Frequency;
        return Raw.Value3D(a_Coordinate.x, a_Coordinate.y, a_Coordinate.z);
    }
    
    public static float Value(float a_Coordinate, float a_Frequency, int a_Octaves, float a_Lacunarity = 2f, float a_Persistance = 0.5f)
    {
        float Sum = Value(a_Coordinate, a_Frequency);
        float Amplitude = 1f;
        float Range = 1f;
        float Frequency = a_Frequency;
        for (int i = 1; i < a_Octaves; i++)
        {
            Frequency *= a_Lacunarity;
            Amplitude *= a_Persistance;
            Range += Amplitude;
            Sum += Value(a_Coordinate, Frequency) * Amplitude;
        }

        return Sum / Range;
    }
    public static float Value(Vector2 a_Coordinate, float a_Frequency, int a_Octaves, float a_Lacunarity = 2f, float a_Persistance = 0.5f)
    {
        float Sum = Value(a_Coordinate, a_Frequency);
        float Amplitude = 1f;
        float Range = 1f;
        float Frequency = a_Frequency;
        for (int i = 1; i < a_Octaves; i++)
        {
            Frequency *= a_Lacunarity;
            Amplitude *= a_Persistance;
            Range += Amplitude;
            Sum += Value(a_Coordinate, Frequency) * Amplitude;
        }

        return Sum / Range;
    }
    public static float Value(Vector3 a_Coordinate, float a_Frequency, int a_Octaves, float a_Lacunarity = 2f, float a_Persistance = 0.5f)
    {
        float Sum = Value(a_Coordinate, a_Frequency);
        float Amplitude = 1f;
        float Range = 1f;
        float Frequency = a_Frequency;
        for (int i = 1; i < a_Octaves; i++)
        {
            Frequency *= a_Lacunarity;
            Amplitude *= a_Persistance;
            Range += Amplitude;
            Sum += Value(a_Coordinate, Frequency) * Amplitude;
        }

        return Sum / Range;
    }


    public static float Perlin(float a_Coordinate, float a_Frequency)
    {
        return Raw.Perlin1D(a_Coordinate * a_Frequency);
    }
    public static float Perlin(Vector2 a_Coordinate, float a_Frequency)
    {
        a_Coordinate *= a_Frequency;
        return Raw.Perlin2D(a_Coordinate.x, a_Coordinate.y);
    }
    public static float Perlin(Vector3 a_Coordinate, float a_Frequency)
    {
        a_Coordinate *= a_Frequency;
        return Raw.Perlin3D(a_Coordinate.x, a_Coordinate.y, a_Coordinate.z);
    }

    public static float Perlin(float a_Coordinate, float a_Frequency, int a_Octaves, float a_Lacunarity = 2f, float a_Persistance = 0.5f)
    {
        float Sum = Perlin(a_Coordinate, a_Frequency);
        float Amplitude = 1f;
        float Range = 1f;
        float Frequency = a_Frequency;
        for (int i = 1; i < a_Octaves; i++)
        {
            Frequency *= a_Lacunarity;
            Amplitude *= a_Persistance;
            Range += Amplitude;
            Sum += Perlin(a_Coordinate, Frequency) * Amplitude;
        }

        return Sum / Range;
    }
    public static float Perlin(Vector2 a_Coordinate, float a_Frequency, int a_Octaves, float a_Lacunarity = 2f, float a_Persistance = 0.5f)
    {
        float Sum = Perlin(a_Coordinate, a_Frequency);
        float Amplitude = 1f;
        float Range = 1f;
        float Frequency = a_Frequency;
        for (int i = 1; i < a_Octaves; i++)
        {
            Frequency *= a_Lacunarity;
            Amplitude *= a_Persistance;
            Range += Amplitude;
            Sum += Perlin(a_Coordinate, Frequency) * Amplitude;
        }

        return Sum / Range;
    }
    public static float Perlin(Vector3 a_Coordinate, float a_Frequency, int a_Octaves, float a_Lacunarity = 2f, float a_Persistance = 0.5f)
    {
        float Sum = Perlin(a_Coordinate, a_Frequency);
        float Amplitude = 1f;
        float Range = 1f;
        float Frequency = a_Frequency;
        for (int i = 1; i < a_Octaves; i++)
        {
            Frequency *= a_Lacunarity;
            Amplitude *= a_Persistance;
            Range += Amplitude;
            Sum += Perlin(a_Coordinate, Frequency) * Amplitude;
        }

        return Sum / Range;
    }
}

