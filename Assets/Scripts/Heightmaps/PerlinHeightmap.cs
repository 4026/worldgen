using UnityEngine;

public class PerlinHeightmap : AbstractHeightmap
{
	private float scale;
	private Vector2 perlinBase;

	public PerlinHeightmap (int newSize, float newScale) : base(newSize)
	{
		scale = newScale;
		perlinBase = Random.insideUnitCircle * 100000;
	}

	public override void generate ()
	{
		float perlinX, perlinY, perlinValue;

		for (int y = 0; y < size; ++y) {
			for (int x = 0; x < size; ++x) {
				perlinX = perlinBase.x + x / (size * scale);
				perlinY = perlinBase.y + y / (size * scale);
				perlinValue = Mathf.PerlinNoise (perlinX, perlinY);

				m_heights [y, x] = perlinValue;
			}
		}
	}
}
