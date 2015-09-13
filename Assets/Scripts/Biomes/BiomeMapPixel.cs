using System;

public class BiomeMapPixel
{
	private float[] m_biomeWeights;
	public Biome ParentBiome { get; set; }

	public BiomeMapPixel (float[] biomeWeights)
	{
		m_biomeWeights = biomeWeights;
	}

    /// <summary>
    /// Get the biome type with the greatest weight in this pixel.
    /// </summary>
    /// <returns></returns>
    public BiomeType GetPrimaryBiome()
    {
        float weight;
        float maxWeight = 0f;
        BiomeType primaryBiomeType = BiomeType.Plains;
        
        foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)))
        {
            weight = GetBiomeWeight(biome);
            if (weight > maxWeight)
            {
                primaryBiomeType = biome;
                maxWeight = weight;
            }
        }

        return primaryBiomeType;
    }

    /// <summary>
    /// Get the weight of the specified biome type in this pixel.
    /// </summary>
    /// <param name="biome"></param>
    /// <returns></returns>
	public float GetBiomeWeight (BiomeType biome)
	{
		return m_biomeWeights [(int)biome];
	}

    public override string ToString()
    {
        string biomeData = "";
        foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)))
        {
            if (GetBiomeWeight(biome) != 0)
            {
                biomeData += String.Format("{0}: {1}, ", biome.ToString(), GetBiomeWeight(biome));
            }
        }

        return ParentBiome.Name + " (" + biomeData + ")";
    }
}
