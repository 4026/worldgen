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
		//  Dry																										Wet
			{BiomeType.Alpine, BiomeType.Alpine, BiomeType.Alpine, BiomeType.Alpine, BiomeType.Alpine, BiomeType.Alpine}, //Cold
			{BiomeType.Desert, BiomeType.Alpine, BiomeType.Alpine, BiomeType.Alpine, BiomeType.Alpine, BiomeType.Alpine},
			{BiomeType.Desert, BiomeType.Desert, BiomeType.Plains, BiomeType.Plains, BiomeType.Plains, BiomeType.Swamp},
			{BiomeType.Desert, BiomeType.Desert, BiomeType.Plains, BiomeType.Plains, BiomeType.Plains, BiomeType.Swamp},
			{BiomeType.Desert, BiomeType.Desert, BiomeType.Plains, BiomeType.Plains, BiomeType.Swamp, BiomeType.Swamp},
			{BiomeType.Desert, BiomeType.Desert, BiomeType.Plains, BiomeType.Plains, BiomeType.Swamp, BiomeType.Swamp},   //Hot
		};
	}

	/// <summary>
	/// Gets the biome weights for given temperature and precipitation values.
	/// </summary>
	/// <returns>An array of values, totalling one, that correspond to the weights for each biome type for the given parameters.</returns>
	/// <param name="temperature">Temperature.</param>
	/// <param name="precipitation">Precipitation.</param>
	public float[] getBiomeWeights (float temperature, float precipitation)
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
		return biomeWeights;
	}
}
