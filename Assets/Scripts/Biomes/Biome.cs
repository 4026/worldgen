
namespace Biomes
{
    public class Biome
    {
        public string Name;
        public BiomeType Type;
        public int Size = 0;

        public Biome(BiomeType newType, string newName)
        {
            Type = newType;
            Name = newName;
        }

        public Biome(BiomeType newType)
        {
            Type = newType;
        }

        public void GenerateName()
        {
            Name = BiomeNamer.Instance.GenerateName(this);
        }
    }
}