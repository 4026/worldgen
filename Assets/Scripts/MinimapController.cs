using UnityEngine;
using System.Collections;

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
		m_biomeGraph = new Texture2D (128, 128);
		Color[] pixels = m_biomeGraph.GetPixels ();
		for (int x = 0; x < m_biomeGraph.width; ++x) {
			for (int y = 0; y < m_biomeGraph.height; ++y) {
				float temperature = x / (float)m_biomeGraph.width;
				float precipitation = y / (float)m_biomeGraph.height;
				float[] biomeWeights = BiomeCalculator.Instance.getBiomeWeights (temperature, precipitation);
				pixels [y * m_biomeGraph.width + x] = new Color (
					biomeWeights [(int)BiomeType.Desert],
					biomeWeights [(int)BiomeType.Plains], 
					biomeWeights [(int)BiomeType.Alpine]
				);
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
