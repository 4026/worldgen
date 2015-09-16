using UnityEngine;
using System.Collections.Generic;

namespace Biomes
{
    public class BiomeDatabase
    {

        private Dictionary<BiomeType, BiomeData> m_database;

        /// <summary>
        /// The sole instance of this singleton class.
        /// </summary>
        /// <value>The instance.</value>
        public static BiomeDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BiomeDatabase();
                }
                return _instance;
            }
        }
        private static BiomeDatabase _instance = null;


        public BiomeDatabase()
        {
            //Initialise biome database
            m_database = new Dictionary<BiomeType, BiomeData>();

            //Ocean
            m_database.Add(
                BiomeType.Ocean, 
                new BiomeData(
                    BiomeType.Ocean,
                    TerrainTexture.Seabed,
                    Color.blue,
                    new string[] { "Ocean" },
                    new string[] { "Sea" },
                    new string[] { "Lake", "Pool", "Lagoon", "Loch" }
                )
            );

            //Plains
            m_database.Add(
                BiomeType.Plains,
                new BiomeData(
                    BiomeType.Plains,
                    TerrainTexture.Grass,
                    new Color(0.5f, 0.5f, 0.2f),
                    new string[] { "Plain", "Steppe", "Grassland", "Prairie", "Expanse" },
                    new string[] { "Plain", "Steppe", "Grassland", "Prairie" },
                    new string[] { "Pasture", "Meadow", "Scrub" }
                )
            );

            //Desert
            m_database.Add(
                BiomeType.Desert,
                new BiomeData(
                    BiomeType.Desert,
                    TerrainTexture.Sand,
                    Color.yellow,
                    new string[] { "Desert", "Sands" },
                    new string[] { "Desert", "Sands" },
                    new string[] { "Dunes" }
                )
            );

            //Cold Desert
            m_database.Add(
                BiomeType.ColdDesert,
                new BiomeData(
                    BiomeType.ColdDesert,
                    TerrainTexture.ParchedGround,
                    Color.grey,
                    new string[] { "Desert", "Wastelands", "Badlands" },
                    new string[] { "Desert", "Wasteland", "Badlands" },
                    new string[] { "Waste", "Shale" }
                )
            );

            //Swamp
            m_database.Add(
                BiomeType.Swamp,
                new BiomeData(
                    BiomeType.Swamp,
                    TerrainTexture.Mud,
                    Color.black,
                    new string[] { "Bog", "Marsh", "Mire", "Quagmire", "Swamp", "Morass", "Moor", "Fen" },
                    new string[] { "Bog", "Marsh", "Mire", "Quagmire", "Swamp", "Morass", "Moor", "Fen" },
                    new string[] { "Bog", "Marsh", "Mire", "Quagmire", "Wetlands", "Glade" }
                )
            );

            //Snow
            m_database.Add(
                BiomeType.Snow,
                new BiomeData(
                    BiomeType.Snow,
                    TerrainTexture.Snow,
                    Color.white,
                    new string[] { "Plain", "Glacier", "Ice Shelf" },
                    new string[] { "Plain", "Glacier" },
                    new string[] { "Drifts", "Floe", "Field" }
                )
            );

            //Forest
            m_database.Add(
                BiomeType.Forest,
                new BiomeData(
                    BiomeType.Forest,
                    TerrainTexture.Forest,
                    new Color(0f, 0.5f, 0f),
                    new string[] { "Forest" },
                    new string[] { "Forest", "Wood" },
                    new string[] { "Wood", "Thicket", "Copse", "Coppice", "Clump", "Grove" }
                )
            );

            //Taiga
            m_database.Add(
                BiomeType.Taiga,
                new BiomeData(
                    BiomeType.Taiga,
                    TerrainTexture.PineForest,
                    new Color(0f, 0.5f, 0.5f),
                    new string[] { "Forest", "Taiga" },
                    new string[] { "Forest", "Taiga" },
                    new string[] { "Thicket", "Copse", "Clump", "Pines" }
                )
            );

            //Jungle
            m_database.Add(
                BiomeType.Jungle,
                new BiomeData(
                    BiomeType.Jungle,
                    TerrainTexture.Jungle,
                    Color.green,
                    new string[] { "Rainforest", "Jungle", "Bush" },
                    new string[] { "Rainforest", "Jungle", "Bush" },
                    new string[] { "Rainforest", "Jungle", "Bush" }
                )
            );
        }

        public BiomeData Get(BiomeType type)
        {
            return m_database[type];
        }
    }
}