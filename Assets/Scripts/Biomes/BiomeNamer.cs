using System;
using System.Collections.Generic;

namespace Biomes
{
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

        private readonly string[] vowels = new string[] { "a", "e", "i", "o", "u" };
        private readonly string[] consonants = new string[] { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "qu", "r", "s", "t", "v", "w", "x", "y", "z" };
        private readonly string[] bigrams = new string[] { "th", "he", "in", "er", "an", "re", "on", "at", "en", "nd", "ti", "es", "or", "te", "of", "ed", "is", "it", "al", "ar", "st", "to", "nt", "ng", "se", "ha", "as", "ou", "io", "le", "ve", "co", "me", "de", "hi", "ri", "ro", "ic", "ne", "ea", "ra", "ce", "li", "ch", "ll", "be", "ma", "si", "om", "ur" };

        private BiomeNamer()
        {
            
        }

        public string GenerateName(Biome biome)
        {
            BiomeData biomeData = BiomeDatabase.Instance.Get(biome.Type);

            return "The " + firstCharToUpper(generateRandomString()) + " " + generateBiomeTypeName(biome, biomeData);
        }

        private string generateRandomString()
        {
            string output = "";
            int nameLength = UnityEngine.Random.Range(2, 6);
            for (int i = 0; i < nameLength; ++i)
            {
                int mode = UnityEngine.Random.Range(0, 3);

                switch (mode)
                {
                    case 0:
                        output += consonants[UnityEngine.Random.Range(0, consonants.Length)];
                        output += vowels[UnityEngine.Random.Range(0, vowels.Length)];
                        break;
                    case 1:
                        output += bigrams[UnityEngine.Random.Range(0, bigrams.Length)];
                        break;
                    case 2:
                        output += consonants[UnityEngine.Random.Range(0, consonants.Length)];
                        break;
                }
                
            }

            return output;
        }

        private string generateBiomeTypeName(Biome biome, BiomeData biomeData)
        {
            string[] names;
            if (biome.Size > 10000)
            {
                //Large biome
                names = biomeData.BigNames;
            }
            else if (biome.Size > 1000)
            {
                //Medium biome
                names = biomeData.MediumNames;
            }
            else
            {
                //Small biome
                names = biomeData.SmallNames;
            }

            int nameIndex = UnityEngine.Random.Range(0, names.Length);
            return names[nameIndex];
        }

        private static string firstCharToUpper(string str)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length > 1)
            {
                return char.ToUpper(str[0]) + str.Substring(1);
            }

            return str.ToUpper();
        }

    }
}