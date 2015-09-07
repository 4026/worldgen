using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{

	enum TerrainTexture
	{
		Grass = 0,
		Rock = 1,
		Mud = 2,
		Cliff = 3,
		Sand = 4
	}

	public float CliffFadeStartAngle;
	public float CliffFadeStopAngle;

	private AbstractValueMap m_heightmap;
	private AbstractValueMap m_rainmap;
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
		Regenerate ();
	}

	public void Regenerate ()
	{
		MinimapController minimap = FindObjectOfType<MinimapController> ();

		generateHeightmap ();
		m_terrainData.SetHeights (0, 0, m_heightmap.getValues ());
		minimap.setHeightmap (m_heightmap.getTexture ());

		generateRainMap ();
		minimap.setRainmap (m_rainmap.getTexture ());

		generateAlphamap ();
		m_terrainData.SetAlphamaps (0, 0, m_alphamap);
	}

	
	private void generateHeightmap ()
	{
		DiamondSquareNoiseMap map = new DiamondSquareNoiseMap (m_terrainData.heightmapResolution);

		//Set some seed values for the map:
		int quarterSize = (map.size - 1) / 4;
		// - Set some random heights in the middle of the map
		for (int x = 1; x <= 3; ++x) {
			for (int y = 1; y <= 3; ++y) {
				map.SetSeedValue (x * quarterSize, y * quarterSize, Random.value);
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

		m_heightmap = map;
	}

	private void generateRainMap ()
	{
		m_rainmap = new PerlinNoiseMap (m_terrainData.heightmapResolution, 0.2f);
		m_rainmap.generate ();
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

				//Get biome weights for the location.
				float[] biomeWeights = GetBiomeWeightsAtHeightmapPos (x, y);

				//Assign texture alphas based on biome weights
				m_alphamap [y, x, (int)TerrainTexture.Grass] = nonCliffAlpha * biomeWeights [(int)BiomeType.Plains];
				m_alphamap [y, x, (int)TerrainTexture.Mud] = nonCliffAlpha * biomeWeights [(int)BiomeType.Swamp];
				m_alphamap [y, x, (int)TerrainTexture.Rock] = nonCliffAlpha * biomeWeights [(int)BiomeType.Alpine];
				m_alphamap [y, x, (int)TerrainTexture.Sand] = nonCliffAlpha * biomeWeights [(int)BiomeType.Desert];
			}
		}
	}

	public Vector2 GetHeightmapPosFromWorldPos (Vector3 worldPos)
	{
		Vector3 localPos = worldPos - transform.position;
		return new Vector2 (
			Mathf.Floor (localPos.x * m_terrainData.heightmapWidth / m_terrainData.size.x),
			Mathf.Floor (localPos.z * m_terrainData.heightmapHeight / m_terrainData.size.z)
		);
	}

	public float[] GetBiomeWeightsAtHeightmapPos (int x, int y)
	{
		float temperature = 1 - m_heightmap.getValueAt (x, y);
		float precipitation = m_rainmap.getValueAt (x, y);
		return BiomeCalculator.Instance.getBiomeWeights (temperature, precipitation);
	}
}
