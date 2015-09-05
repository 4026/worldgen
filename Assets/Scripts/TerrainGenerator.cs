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

	public int MapSize;


	void Awake ()
	{
		Regenerate ();
	}

	public void Regenerate ()
	{
		float[,] heights = generateHeightmap (MapSize, 0.1f, 0.6f);
		Terrain.activeTerrain.terrainData.SetHeights (0, 0, heights);
		
		
		float[,,] alphas = new float[MapSize, MapSize, System.Enum.GetValues (typeof(TerrainTexture)).Length];
		for (int y = 0; y < MapSize; ++y) {
			for (int x = 0; x < MapSize; ++x) {
				alphas [y, x, (int)TerrainTexture.Grass] = Mathf.Clamp (3.0f - Mathf.Abs (10.0f * heights [y, x] - 5.0f), 0.0f, 1.0f);
				alphas [y, x, (int)TerrainTexture.Cliff] = 0;
				alphas [y, x, (int)TerrainTexture.Mud] = 0;
				alphas [y, x, (int)TerrainTexture.Rock] = Mathf.Clamp (-7.0f + 10.0f * heights [y, x], 0.0f, 1.0f);
				alphas [y, x, (int)TerrainTexture.Sand] = Mathf.Clamp (3.0f - 10.0f * heights [y, x], 0.0f, 1.0f);
				
			}
		}
		
		Terrain.activeTerrain.terrainData.SetAlphamaps (0, 0, alphas);
	}

	
	private float[,] generateHeightmap (int heightmapResolution, float scale, float cragStartAltitude)
	{
		float height, cragHeight;
		float[,] heights = new float[heightmapResolution, heightmapResolution];

		DiamondSquareHeightmap baseMap = new DiamondSquareHeightmap (heightmapResolution);
		//Set some seed values for the map:
		// - Force a mountain in the middle
		baseMap.SetSeedValue (baseMap.size / 2, baseMap.size / 2, 1.0f);

		// - Force sea around the outside.
		for (int coordinate = 0; coordinate < baseMap.size; ++coordinate) {
			baseMap.SetSeedValue (coordinate, 0, 0.0f);
			baseMap.SetSeedValue (coordinate, baseMap.size, 0.0f);
			baseMap.SetSeedValue (0, coordinate, 0.0f);
			baseMap.SetSeedValue (baseMap.size, coordinate, 0.0f);
		}
		baseMap.generate ();

		for (int y = 0; y < heightmapResolution; ++y) {
			for (int x = 0; x < heightmapResolution; ++x) {
				height = baseMap.getHeightAt (x, y);
				heights [y, x] = Mathf.Clamp (height, 0.0f, 1.0f);
			}
		}

		return heights;
	}

}
