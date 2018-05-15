using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class TerrainChunk : MonoBehaviour
    {

        public int seed = 0;
        [Range(1, 100)]
        public float noiseScale = 30.0f;
        [Range(3, 6)]
        public int octaves = 5;
        public float persistance = 0.5f;
        public float lacunaryty = 2.0f;
        public Vector2 offset = Vector2.zero;
        public NoiseGenerator.NormalizedModel model = NoiseGenerator.NormalizedModel.Local;
        public TerrainType[] regions;
        int verticesSize = 241;

        public AnimationCurve meshHeightCurve;
        [Range(10, 50)]
        public float HeightScale = 10.0f;

        [Range(0, 6)]
        public int LOD = 0;
        public Material mat;
        float[,] fallOfMap;
        public bool ApplyFallOf = true;
        public bool autoUpdate = false;

        // [ContextMenu("Generate Terrain Chunk")]
        public void GenerateTerrainChunk()
        {
            if (mat == null) { Debug.LogError("Material not signed yet"); return; }
            if (fallOfMap == null) { fallOfMap = FalloffGenerator.GenerateFallofMap(verticesSize); }
            Vector2 center = new Vector2(transform.position.x, transform.position.z);
            TerrainMapData mapData;
            if (ApplyFallOf)
            {
                mapData = TerrainMapDataGenerator.GenerateTerrainMapData(center, verticesSize, seed, noiseScale, octaves, persistance, lacunaryty, offset, model, regions, fallOfMap);
            }
            else
            {
                mapData = TerrainMapDataGenerator.GenerateTerrainMapData(center, verticesSize, seed, noiseScale, octaves, persistance, lacunaryty, offset, model, regions);
            }
            MeshData terrainMesh = TerrainMeshGenerator.GenerateMeshFromHeightMapData(mapData.heightMap, HeightScale, meshHeightCurve, LOD);

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            meshFilter.sharedMesh = terrainMesh.CreateMesh();
            meshRenderer.sharedMaterial = mat;
            mat.mainTexture = TextureGenerator.TextureFromColourMap(mapData.colourMap, verticesSize, verticesSize);
        }
    }
}
