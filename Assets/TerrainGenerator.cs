using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {

	enum TerrainTexture
	{
		Grass = 0,
		Rock = 1,
		Mud = 2,
		Cliff = 3,
		Sand = 4
	}

	void Awake() 
	{
		float[,] heights = generateHeightmap (512, 0.1f);
		Terrain.activeTerrain.terrainData.SetHeights (0, 0, heights);


		float[,,] alphas = new float[512, 512, System.Enum.GetValues(typeof(TerrainTexture)).Length];
		for (int y = 0; y < 512; ++y) {
			for (int x = 0; x < 512; ++x) {
				alphas[y, x, (int) TerrainTexture.Grass] = Mathf.Clamp(2.0f - Mathf.Pow(8.0f * heights[y, x] - 4.0f, 2), 0.0f, 1.0f);
				alphas[y, x, (int) TerrainTexture.Cliff] = 0;
				alphas[y, x, (int) TerrainTexture.Mud] = 0;
				alphas[y, x, (int) TerrainTexture.Rock] = Mathf.Clamp(-6.0f + 10.0f * heights[y, x], 0.0f, 1.0f);
				alphas[y, x, (int) TerrainTexture.Sand] = Mathf.Clamp(4.0f - 10.0f * heights[y, x], 0.0f, 1.0f);
				
			}
		}

		Terrain.activeTerrain.terrainData.SetAlphamaps (0, 0, alphas);
	}
	
	private float[,] generateHeightmap(int heightmapResolution, float scale) 
	{
		float[,] heights = new float[heightmapResolution, heightmapResolution];

		float mapRadius = heightmapResolution / 2;
		Vector2 mapCentre = new Vector2 (mapRadius, mapRadius);

		Vector2 perlinBase = Random.insideUnitCircle * 100000;
		float perlinX, perlinY, perlinValue, bias;

		Vector2 mapPosition = Vector2.zero;
		for (mapPosition.y = 0; mapPosition.y < heightmapResolution; ++mapPosition.y) {
			for (mapPosition.x = 0; mapPosition.x < heightmapResolution; ++mapPosition.x) {
				perlinX = perlinBase.x + mapPosition.x / (heightmapResolution * scale);
				perlinY = perlinBase.y + mapPosition.y / (heightmapResolution * scale);
				perlinValue = Mathf.PerlinNoise(perlinX, perlinY);

				bias = 1.0f - ((mapPosition - mapCentre).magnitude * 1.1f / mapRadius);

				heights[(int)mapPosition.y, (int)mapPosition.x] = Mathf.Clamp((perlinValue + bias) / 2.0f, 0.0f, 1.0f);
			}
		}

		return heights;
	}

}
