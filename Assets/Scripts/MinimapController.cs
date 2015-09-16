using UnityEngine;
using Biomes;

public class MinimapController : MonoBehaviour
{
	public enum MinimapMode
	{
		Off,
		BiomeGraph,
		Heightmap,
		Rainmap
	}

	public MinimapMode CurrentMode = MinimapMode.Off;
	public int size;

	private Texture2D m_heightmap;
	private Texture2D m_rainmap;
	private Texture2D m_biomeGraph;

	void Awake ()
	{


		m_biomeGraph = new Texture2D (size, size);
		Color[] pixels = m_biomeGraph.GetPixels ();
		for (int x = 0; x < m_biomeGraph.width; ++x) {
			for (int y = 0; y < m_biomeGraph.height; ++y) {
				float temperature = x / (float)m_biomeGraph.width;
				float precipitation = y / (float)m_biomeGraph.height;
				float[] biomeWeights = BiomeCalculator.Instance.getBiomeWeights (temperature, precipitation, 1f);

                BiomeType[] allBiomes = System.Enum.GetValues(typeof(BiomeType)) as BiomeType[];
                Color pixel = new Color(0f, 0f, 0f, 0f);
                foreach (BiomeType biome in allBiomes)
                {
                    pixel += BiomeDatabase.Instance.Get(biome).MinimapColor * biomeWeights[(int) biome];
                }

                pixels [y * m_biomeGraph.width + x] = pixel;
			}
		}

		m_biomeGraph.SetPixels (pixels);
		m_biomeGraph.Apply ();
	}
	
	void OnGUI ()
	{
		switch (CurrentMode) {
		case MinimapMode.BiomeGraph:
			GUI.DrawTexture (new Rect (10, 10, 10 + size, 10 + size), m_biomeGraph, ScaleMode.ScaleToFit);
			break;
		case MinimapMode.Heightmap:
			GUI.DrawTexture (new Rect (10, 10, 10 + size, 10 + size), m_heightmap, ScaleMode.ScaleToFit);
			break;
		case MinimapMode.Rainmap:
			GUI.DrawTexture (new Rect (10, 10, 10 + size, 10 + size), m_rainmap, ScaleMode.ScaleToFit);
			break;
		}
	}

	public void setHeightmap (Texture2D heightmap)
	{
		m_heightmap = heightmap;
	}

	public void setRainmap (Texture2D rainmap)
	{
		m_rainmap = rainmap;
	}
}
