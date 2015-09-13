public class Biome
{
	public readonly string Name;
	public readonly BiomeType Type;

	public Biome (BiomeType newType, string newName)
	{
		Type = newType;
		Name = newName;
	}

    public Biome(BiomeType newType)
    {
        Type = newType;
        Name = BiomeNamer.Instance.GenerateName(newType);
    }

    
}
