using System;

namespace Biomes
{
    public class BiomeMapPixel
    {

        public readonly float Latitude;
        public readonly float HeightAboveSeaLevel;
        public readonly float Temperature;
        public readonly float Precipitation;

        private float[] m_biomeWeights;
        public Biome ParentBiome { get; set; }

        public BiomeMapPixel(float newLatitude, float newHeightAboveSeaLevel, float newPrecipitation)
        {
            Latitude = newLatitude;
            HeightAboveSeaLevel = newHeightAboveSeaLevel;
            Precipitation = newPrecipitation;
            Temperature = BiomeCalculator.Instance.GetTemperature(Latitude, HeightAboveSeaLevel);
            m_biomeWeights = BiomeCalculator.Instance.GetBiomeWeights(Temperature, Precipitation, HeightAboveSeaLevel);
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
        public float GetBiomeWeight(BiomeType biome)
        {
            return m_biomeWeights[(int)biome];
        }

        /// <summary>
        /// Get a string representation of the Biome.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string parentBiomeString = (ParentBiome != null) ? ParentBiome.Name + " (Area: " + ParentBiome.Size + ")" : "Unnamed Area";

            string locationDataString = String.Format("Latitude: {0:f3}, Altitude: {1:f3}, Temperature: {2:f3}, Precipitation: {3:f3}", Latitude, HeightAboveSeaLevel, Temperature, Precipitation);

            string biomeWeightsString = "";
            foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)))
            {
                if (GetBiomeWeight(biome) != 0)
                {
                    biomeWeightsString += String.Format("{0}: {1:f2}, ", biome.ToString(), GetBiomeWeight(biome));
                }
            }

            return String.Format("{0} - {1} - {2}", parentBiomeString, locationDataString, biomeWeightsString);
        }
    }
}