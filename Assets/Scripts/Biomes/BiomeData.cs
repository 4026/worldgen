using UnityEngine;

namespace Biomes
{
    public class BiomeData
    {
        public readonly BiomeType Type;
        public readonly TerrainTexture TextureID;
        public readonly Color MinimapColor;

        public readonly string[] BigNames;
        public readonly string[] MediumNames;
        public readonly string[] SmallNames;

        public BiomeData(
            BiomeType newType, 
            TerrainTexture newTexture, 
            Color newColor, 
            string[] newBigNames, 
            string[] newMediumNames, 
            string[] newSmallNames
        )
        {
            Type = newType;
            TextureID = newTexture;
            MinimapColor = newColor;

            BigNames = newBigNames;
            MediumNames = newMediumNames;
            SmallNames = newSmallNames;
        }
    }
}
