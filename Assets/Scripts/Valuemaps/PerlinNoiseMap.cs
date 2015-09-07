using UnityEngine;

public class PerlinNoiseMap : AbstractValueMap
{
	private float scale;
	private Vector2 perlinBase;

	public PerlinNoiseMap (int newSize, float newScale) : base(newSize)
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

				m_values [y, x] = Mathf.Clamp (perlinValue, 0f, 1f);
			}
		}
	}
}
