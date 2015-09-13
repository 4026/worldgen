using System;
using System.Collections.Generic;

public class BiomeNamer
{
    /// <summary>
    /// The sole instance of this singleton class.
    /// </summary>
    /// <value>The instance.</value>
    public static BiomeNamer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BiomeNamer();
            }
            return _instance;
        }
    }
    private static BiomeNamer _instance = null;

    /// <summary>
    /// Biome name definitions, as an array mapping biome types to lists of strings.
    /// </summary>
    private List<string>[] m_biomeNameDefinitions;

    private readonly char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
    private readonly char[] consonants = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j','k', 'l', 'm', 'n','p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };

    private BiomeNamer()
    {
        //Initialise the biome name definitions
        m_biomeNameDefinitions = new List < string >[Enum.GetValues(typeof(BiomeType)).Length];
        foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)))
        {
            m_biomeNameDefinitions[(int) biome] = new List<string>();
        }

        //Plains
        m_biomeNameDefinitions[(int)BiomeType.Plains].AddRange(new string[] {
            "Plain", "Steppe", "Grassland", "Prairie"
        });

        //Desert
        m_biomeNameDefinitions[(int)BiomeType.Desert].AddRange(new string[] {
            "Desert", "Sands", "Dunes"
        });

        //Cold Desert
        m_biomeNameDefinitions[(int)BiomeType.ColdDesert].AddRange(new string[] {
            "Desert", "Wastes", "Plain"
        });

        //Swamp
        m_biomeNameDefinitions[(int)BiomeType.Swamp].AddRange(new string[] {
            "Swamp", "Bog", "Marsh", "Wetlands"
        });

        //Snow
        m_biomeNameDefinitions[(int)BiomeType.Snow].AddRange(new string[] {
            "Glacier", "Drifts", "Plain", "Ice Shelf"
        });

        //Taiga
        m_biomeNameDefinitions[(int)BiomeType.Taiga].AddRange(new string[] {
            "Taiga"
        });

        //Forest
        m_biomeNameDefinitions[(int)BiomeType.Forest].AddRange(new string[] {
            "Forest", "Wood"
        });

        //Cold Forest
        m_biomeNameDefinitions[(int)BiomeType.ColdForest].AddRange(new string[] {
            "Forest", "Wood", "Pines"
        });

        //Cold Forest
        m_biomeNameDefinitions[(int)BiomeType.Jungle].AddRange(new string[] {
            "Jungle", "Rainforest"
        });
    }

    public string GenerateName(BiomeType biome)
    {
        string name = "The " + char.ToUpper(consonants[UnityEngine.Random.Range(0, 21)]).ToString();
        int nameLength = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < nameLength; ++i)
        {
            name += vowels[UnityEngine.Random.Range(0, 5)];
            name += consonants[UnityEngine.Random.Range(0, 21)];
        }

        List<string> names = m_biomeNameDefinitions[(int)biome];
        int nameIndex = UnityEngine.Random.Range(0, names.Count);
        name += " " + names[nameIndex];

        return name;
    }
}