using UnityEngine;
using Biomes;
using System.IO;
using UnityEngine.UI;
using System;

public class MinimapController : MonoBehaviour
{
	public enum MinimapMode
	{
		Heightmap,
		Rainmap,
        BiomeGraph
    }

    /// <summary>
    /// The size of texture to generate
    /// </summary>
    public int Size;

    private MinimapMode m_currentMode;
    public MinimapMode CurrentMode {
        get { return m_currentMode; }
        set {
            m_currentMode = value;
            updateDisplay();
        }
    }

    private Texture2D m_heightmap;
	private Texture2D m_rainmap;
	private Texture2D m_biomeGraph;

    private RawImage m_rawImage;

	void Awake ()
	{
        m_rawImage = this.GetComponent<RawImage>();

        //Draw and save the biome graph texture immediately, as it doesn't depend upon any generated data.
        m_biomeGraph = new Texture2D (Size, Size);
		Color[] pixels = m_biomeGraph.GetPixels ();
		for (int x = 0; x < m_biomeGraph.width; ++x) {
			for (int y = 0; y < m_biomeGraph.height; ++y) {
				float temperature = x / (float)m_biomeGraph.width;
				float precipitation = y / (float)m_biomeGraph.height;
				float[] biomeWeights = BiomeCalculator.Instance.GetBiomeWeights (temperature, precipitation, 1f);

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

        //Also, write out the biome map to a png file for reference.
        byte[] outfile = m_biomeGraph.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../BiomeMap.png", outfile);
    }

    public void Start()
    {
        this.CurrentMode = MinimapMode.Heightmap;
    }

    public void setModeFromDropdown (int option_index)
    {
        CurrentMode = (MinimapMode) System.Enum.GetValues(typeof(MinimapMode)).GetValue(option_index);
    }


	public void setHeightmap (Texture2D heightmap)
	{
		m_heightmap = heightmap;
        updateDisplay();
	}

	public void setRainmap (Texture2D rainmap)
	{
		m_rainmap = rainmap;
        updateDisplay();
    }

    /// <summary>
    /// Update this GameObject's RawImage component to show the correct minimap texture.
    /// </summary>
    private void updateDisplay()
    {
        switch (m_currentMode)
        {
            case MinimapMode.Heightmap:
                m_rawImage.texture = m_heightmap;
                break;
            case MinimapMode.Rainmap:
                m_rawImage.texture = m_rainmap;
                break;
            case MinimapMode.BiomeGraph:
                m_rawImage.texture = m_biomeGraph;
                break;
        }
        
    }
}
