using UnityEngine;
using System.Collections.Generic;
using Biomes;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour
{

    //Parameters
	public float CliffFadeStartAngle;
	public float CliffFadeStopAngle;
    public float SeaLevel;

    //Data
    public AbstractValueMap Heightmap;
	public AbstractValueMap Rainmap;
    public BiomeMap BiomeMap;

    private float[,,] m_alphamap;
	private Terrain m_terrain;
	private TerrainData m_terrainData;

	void Start ()
	{

	}

	void Awake ()
	{
		m_terrain = Terrain.activeTerrain;
		m_terrainData = m_terrain.terrainData;
        Regenerate();
	}

	public void Regenerate ()
	{
        float startTime = Time.realtimeSinceStartup;

		MinimapController minimap = FindObjectOfType<MinimapController> ();

        float stepStartTime = Time.realtimeSinceStartup;
        generateHeightmap ();
        Debug.Log("Generate Heightmap: " + (Time.realtimeSinceStartup - stepStartTime) + "s");
		m_terrainData.SetHeights (0, 0, Heightmap.getValues ());
		minimap.setHeightmap (Heightmap.getTexture ());

        stepStartTime = Time.realtimeSinceStartup;
        generateRainMap ();
        Debug.Log("Generate Rainmap: " + (Time.realtimeSinceStartup - stepStartTime) + "s");
        minimap.setRainmap (Rainmap.getTexture ());

        stepStartTime = Time.realtimeSinceStartup;
        generateBiomeMap();
        Debug.Log("Generate Biome map: " + (Time.realtimeSinceStartup - stepStartTime) + "s");

        stepStartTime = Time.realtimeSinceStartup;
        generateAlphamap ();
        Debug.Log("Generate Alphamap: " + (Time.realtimeSinceStartup - stepStartTime) + "s");
        m_terrainData.SetAlphamaps (0, 0, m_alphamap);

        stepStartTime = Time.realtimeSinceStartup;
        placeTrees();
        Debug.Log("Place trees: " + (Time.realtimeSinceStartup - stepStartTime) + "s");

        m_terrain.Flush();

        Debug.Log("Total: " + (Time.realtimeSinceStartup - startTime) + "s");
    }    

    private void generateHeightmap ()
	{
		DiamondSquareNoiseMap map = new DiamondSquareNoiseMap (m_terrainData.heightmapResolution);

		//Set some seed values for the map:
		int quarterSize = (map.size - 1) / 4;
		// - Set some random heights in the middle of the map
		for (int x = 1; x <= 3; ++x) {
			for (int y = 1; y <= 3; ++y) {
				map.SetSeedValue (x * quarterSize, y * quarterSize, UnityEngine.Random.value);
			}
		}

		// - Force sea around the outside.
		for (int coordinate = 0; coordinate < map.size; ++coordinate) {
			map.SetSeedValue (coordinate, 0, 0.0f);
			map.SetSeedValue (coordinate, map.size - 1, 0.0f);
			map.SetSeedValue (0, coordinate, 0.0f);
			map.SetSeedValue (map.size - 1, coordinate, 0.0f);
		}
		map.generate ();

		Heightmap = map;
	}

	private void generateRainMap ()
	{
		Rainmap = new PerlinNoiseMap (m_terrainData.heightmapResolution, 0.2f);
		Rainmap.generate ();
	}

    private void generateBiomeMap()
    {
        BiomeMap = new BiomeMap(this);
    }

    private void generateAlphamap ()
	{
		m_alphamap = new float[m_terrainData.alphamapWidth, m_terrainData.alphamapHeight, m_terrainData.alphamapLayers];
		float normX, normY, steepness, cliffAlpha, nonCliffAlpha;
		for (int y = 0; y < m_terrainData.alphamapHeight; ++y) {
			for (int x = 0; x < m_terrainData.alphamapWidth; ++x) {

				//Calculate normalised x and y co-ordinates, and use them to get the steepness of the terrain at this point.
				normX = x * 1f / (m_terrainData.alphamapWidth - 1);
				normY = y * 1f / (m_terrainData.alphamapHeight - 1);
				steepness = m_terrainData.GetSteepness (normX, normY);

				//Use steepness to calculate the alpha of the cliff texture.
				cliffAlpha = (steepness - CliffFadeStartAngle) / (CliffFadeStopAngle - CliffFadeStartAngle);
				cliffAlpha = Mathf.Clamp (cliffAlpha, 0f, 1f);
				m_alphamap [y, x, (int)TerrainTexture.Cliff] = cliffAlpha;
				nonCliffAlpha = 1 - cliffAlpha;

				//Get biome data for the location.
				BiomeMapPixel biomeData = BiomeMap.GetPixelAtPoint(x, y);

                //Assign texture alphas based on biome weights
                m_alphamap[y, x, (int)TerrainTexture.Seabed] = nonCliffAlpha * biomeData.GetBiomeWeight(BiomeType.Ocean);
                m_alphamap[y, x, (int)TerrainTexture.Grass] = nonCliffAlpha * biomeData.GetBiomeWeight(BiomeType.Plains);
				m_alphamap [y, x, (int)TerrainTexture.Mud] = nonCliffAlpha * biomeData.GetBiomeWeight(BiomeType.Swamp);
				m_alphamap [y, x, (int)TerrainTexture.Sand] = nonCliffAlpha * biomeData.GetBiomeWeight(BiomeType.Desert);
				m_alphamap [y, x, (int)TerrainTexture.ParchedGround] = nonCliffAlpha * biomeData.GetBiomeWeight(BiomeType.ColdDesert);
				m_alphamap [y, x, (int)TerrainTexture.Forest] = nonCliffAlpha * biomeData.GetBiomeWeight(BiomeType.Forest);
				m_alphamap [y, x, (int)TerrainTexture.PineForest] = nonCliffAlpha * biomeData.GetBiomeWeight(BiomeType.Taiga);
				m_alphamap [y, x, (int)TerrainTexture.Jungle] = nonCliffAlpha * biomeData.GetBiomeWeight(BiomeType.Jungle);
				m_alphamap [y, x, (int)TerrainTexture.Snow] = nonCliffAlpha * biomeData.GetBiomeWeight(BiomeType.Snow);
			}
		}
	}

    private void placeTrees()
    {
        m_terrainData.treeInstances = new TreeInstance[0];

        List<TreeInstance> treeList = new List<TreeInstance>();
        TreeInstance tree;
        Point heightmapPosition;
        BiomeMapPixel biomeData;
        BiomeType biome;
        int prototypeIndex;
        Vector3 position;
        float scale;

        for (int i = 0; i < 50000; ++i)
        {
            heightmapPosition = new Point(Random.Range(0, Heightmap.size), Random.Range(0, Heightmap.size));
            biomeData = BiomeMap.GetPixelAtPoint(heightmapPosition);
            biome = biomeData.GetPrimaryBiome();

            if (biome == BiomeType.Forest)
            {
                prototypeIndex = 0;
                scale = Random.Range(0.1f, 0.2f) * biomeData.GetBiomeWeight(biome);
            }
            else if (biome == BiomeType.Taiga)
            {
                prototypeIndex = 1;
                scale = Random.Range(0.1f, 0.2f) * biomeData.GetBiomeWeight(biome);
            }
            else if (biome == BiomeType.Jungle)
            {
                prototypeIndex = 2;
                scale = Random.Range(0.3f, 0.4f) * biomeData.GetBiomeWeight(biome);
            }
            else
            {
                continue;
            }

            tree = new TreeInstance();
            tree.prototypeIndex = prototypeIndex;
            tree.heightScale = scale;
            tree.widthScale = scale;
            tree.rotation = Random.Range(0f, 2 * Mathf.PI);

            position = new Vector3(heightmapPosition.x / (float)Heightmap.size, 0f, heightmapPosition.y / (float)Heightmap.size);
            position.y = m_terrainData.GetInterpolatedHeight(position.x, position.z) / m_terrainData.size.y;
            tree.position = position;

            treeList.Add(tree);
        }

        m_terrainData.treeInstances = treeList.ToArray();
    }

    public Point GetHeightmapPosFromWorldPos (Vector3 worldPos)
	{
		Vector3 localPos = worldPos - transform.position;
		return new Point (
			Mathf.FloorToInt (localPos.x * m_terrainData.heightmapWidth / m_terrainData.size.x),
			Mathf.FloorToInt (localPos.z * m_terrainData.heightmapHeight / m_terrainData.size.z)
		);
	}

    /// <summary>
    /// Get a value between 1 and -1 representing the terrain's height above sea level at the specified point on the heightmap.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public float GetHeightAboveSeaLevelAt(Point pos)
    {
        return Mathf.Clamp((Heightmap.GetValueAt(pos) - SeaLevel) / (1 - SeaLevel), -1f, 1f);
    }
}
