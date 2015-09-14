using System;
using UnityEngine;
using System.Collections.Generic;

public class BiomeCalculator
{
	/// <summary>
	/// The sole instance of this singleton class.
	/// </summary>
	/// <value>The instance.</value>
	public static BiomeCalculator Instance { 
		get {
			if (_instance == null) {
				_instance = new BiomeCalculator (); 
			}  
			return _instance;
		} 
	}
	private static BiomeCalculator _instance = null;  

	/// <summary>
	/// Biome definitions, as a 2-dimensional array mapping (temperature, precipitation) pairs to biome types.
	/// </summary>
	private BiomeType[,] m_biomeDefinitions;

	private BiomeCalculator ()
	{
		//Initialise the biome definitions
		m_biomeDefinitions = new BiomeType[6, 6] {
			{BiomeType.Snow, BiomeType.Snow, BiomeType.Snow, BiomeType.Snow, BiomeType.Snow, BiomeType.Snow}, 		  				
			{BiomeType.Snow, BiomeType.Snow, BiomeType.Taiga, BiomeType.Taiga, BiomeType.Taiga, BiomeType.Taiga}, 
			{BiomeType.ColdDesert,BiomeType.Plains,BiomeType.Plains,BiomeType.Forest,BiomeType.Forest,BiomeType.Swamp},
			{BiomeType.ColdDesert, BiomeType.Plains, BiomeType.Plains, BiomeType.Forest, BiomeType.Forest, BiomeType.Swamp},
			{BiomeType.Desert, BiomeType.Plains, BiomeType.Plains, BiomeType.Jungle, BiomeType.Jungle, BiomeType.Swamp},
			{BiomeType.Desert, BiomeType.Desert, BiomeType.Plains, BiomeType.Jungle, BiomeType.Jungle, BiomeType.Jungle}
		};
	}

    /// <summary>
    /// Gets the biome weights for given temperature and precipitation values.
    /// </summary>
    /// <returns>An array of values, totalling one, that correspond to the weights for each biome type for the given parameters.</returns>
    /// <param name="temperature">Temperature.</param>
    /// <param name="precipitation">Precipitation.</param>
    /// <param name="heightAboveSealevel">Height above sea level.</param>
    public float[] getBiomeWeights (float temperature, float precipitation, float heightAboveSealevel)
	{
		//Get the values of the parameters as they map to the dimenstions of our definitions array.
		float normalisedTemperature = temperature * (m_biomeDefinitions.GetLength (0) - 1);
		float normalisedPrecipitation = precipitation * (m_biomeDefinitions.GetLength (1) - 1);

		//Find the indices of the four elements in the array that the parameter values fall between.
		int temperature0 = Mathf.FloorToInt (normalisedTemperature);
		int temperature1 = Mathf.CeilToInt (normalisedTemperature);
		int precipitation0 = Mathf.FloorToInt (normalisedPrecipitation);
		int precipitation1 = Mathf.CeilToInt (normalisedPrecipitation);

		//Get the four biome types that are at each corner of the unit square that the paramters fall into.
		BiomeType biome00 = m_biomeDefinitions [temperature0, precipitation0];
		BiomeType biome10 = m_biomeDefinitions [temperature1, precipitation0];
		BiomeType biome01 = m_biomeDefinitions [temperature0, precipitation1];
		BiomeType biome11 = m_biomeDefinitions [temperature1, precipitation1];
	
		//Get the values of the parameters within that unit square.
		float normalisedTemperatureOffset = normalisedTemperature - temperature0;
		float normalisedPrecipitationOffset = normalisedPrecipitation - precipitation0;

		//Build an array of biome weights based on bilinear interpolation between the biomes at each corner of the unit square.
		float[] biomeWeights = new float[Enum.GetValues (typeof(BiomeType)).Length];
		biomeWeights [(int)biome00] += (1 - normalisedTemperatureOffset) * (1 - normalisedPrecipitationOffset);
		biomeWeights [(int)biome10] += normalisedTemperatureOffset * (1 - normalisedPrecipitationOffset);
		biomeWeights [(int)biome01] += (1 - normalisedTemperatureOffset) * normalisedPrecipitationOffset;
		biomeWeights [(int)biome11] += normalisedTemperatureOffset * normalisedPrecipitationOffset;

        //If we're close to the sea, start fading in the ocean biome.
        if (heightAboveSealevel < 0.02f)
        {
            float t = Mathf.Clamp(heightAboveSealevel / 0.02f, 0f, 1f);
            float[] oceanWeights = getFullWeightBiome(BiomeType.Ocean);
            biomeWeights = lerpWeights(oceanWeights, biomeWeights, t);
        }

		return biomeWeights;
	}

    /// <summary>
    /// Get an array of biome weights that allocate 100% weight to a single biome type.
    /// </summary>
    /// <param name="biome"></param>
    /// <returns></returns>
    public float[] getFullWeightBiome(BiomeType biome)
    {
        float[] biomeWeights = new float[Enum.GetValues(typeof(BiomeType)).Length];
        biomeWeights[(int)biome] = 1f;

        return biomeWeights;
    }

    /// <summary>
    /// Get an array of biome weights, lerped between two input weightings.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public float[] lerpWeights(float[] from, float[] to, float t)
    {
        BiomeType[] allBiomes = Enum.GetValues(typeof(BiomeType)) as BiomeType[];
        float[] output = new float[allBiomes.Length];

        foreach (BiomeType biome in allBiomes)
        {
            output[(int)biome] = ((1-t) * from[(int)biome]) + (t * to[(int)biome]);
        }

        return output;
    }
}
