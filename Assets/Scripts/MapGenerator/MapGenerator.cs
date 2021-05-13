using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

// Générateur de carte procédurale
// Inspiré du travail de Sebastian Lague : https://github.com/SebLague/Procedural-Landmass-Generation
public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    public const int mapChunkSize = 241;
    [Range(0, 6)]
    public int editorPreviewLOD;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void GenerateMapModel()
    {
        this.drawMode = DrawMode.Mesh;
        this.editorPreviewLOD = 0;
        this.noiseScale = 25f;
        this.octaves = 5;
        this.persistance = 0.5f;
        this.lacunarity = 2f;
        this.seed = 0;
        this.offset = new Vector2(0f, 0f);
        this.meshHeightMultiplier = 18f;

        var keyFrames = new List<Keyframe>();
        keyFrames.Add(new Keyframe(0, 0));
        keyFrames.Add(new Keyframe(0.2971967f, 0.01664277f));
        keyFrames.Add(new Keyframe(0.5789717f, 0.03839566f));
        keyFrames.Add(new Keyframe(0.6679901f, 0.08012913f));
        keyFrames.Add(new Keyframe(0.8058311f, 0.4265141f));
        keyFrames.Add(new Keyframe(1, 1));

        this.meshHeightCurve = new AnimationCurve(keyFrames.ToArray());
        this.meshHeightCurve.preWrapMode = WrapMode.Clamp;
        this.meshHeightCurve.postWrapMode = WrapMode.Clamp;

        var regionList = new List<TerrainType>();
        Color newCol;
        if (ColorUtility.TryParseHtmlString("#3263C3", out newCol))
        {
            regionList.Add(new TerrainType
            {
                colour = newCol,
                height = 0.1f,
                name = "WaterDeep"
            });
        }
        if (ColorUtility.TryParseHtmlString("#D3B071", out newCol))
        {
            regionList.Add(new TerrainType
            {
                colour = newCol,
                height = 0.45f,
                name = "Sand"
            });
        }
        if (ColorUtility.TryParseHtmlString("#996B26", out newCol))
        {
            regionList.Add(new TerrainType
            {
                colour = newCol,
                height = 0.6f,
                name = "Sand 2"
            });
        }
        if (ColorUtility.TryParseHtmlString("#B16A15", out newCol))
        {
            regionList.Add(new TerrainType
            {
                colour = newCol,
                height = 0.9f,
                name = "Sand 3"
            });
        }
        if (ColorUtility.TryParseHtmlString("#6C3801", out newCol))
        {
            regionList.Add(new TerrainType
            {
                colour = newCol,
                height = 1f,
                name = "Sand 4"
            });
        }
        this.regions = regionList.ToArray();
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureFactory.CreateTextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureFactory.CreateTextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD), TextureFactory.CreateTextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
    }

    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(centre, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    // Initialisation
    void Awake()
    {
        // GenerateMapModel();
    }

    // Démarrage
    void Start()
    {
        DrawMapInEditor();
    }

    // Boucle de jeu
    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, centre + offset);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }


        return new MapData(noiseMap, colourMap);
    }

    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}
