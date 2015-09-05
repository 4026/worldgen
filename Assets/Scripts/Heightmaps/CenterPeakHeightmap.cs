using UnityEngine;

public class CenterPeakHeightmap : AbstractHeightmap
{
	private float zeroRadius;
	private float oneRadius;

	public CenterPeakHeightmap (int newSize, float newZeroRadius, float newOneRadius) : base(newSize)
	{
		zeroRadius = newZeroRadius;
		oneRadius = newOneRadius;
	}

	public override void generate ()
	{
		float mapRadius = size / 2;
		Vector2 mapCentre = new Vector2 (mapRadius, mapRadius);
		float distanceFromCenter, height;

		Vector2 mapPosition = Vector2.zero;
		for (mapPosition.y = 0; mapPosition.y < size; ++mapPosition.y) {
			for (mapPosition.x = 0; mapPosition.x < size; ++mapPosition.x) {
				distanceFromCenter = (mapPosition - mapCentre).magnitude / mapRadius;

				if (distanceFromCenter >= zeroRadius) {
					height = 0.0f;
				} else if (distanceFromCenter < oneRadius) {
					height = 1.0f;
				} else {
					height = 1.0f - ((distanceFromCenter - oneRadius) / (zeroRadius - oneRadius));
				}

				m_heights [(int)mapPosition.y, (int)mapPosition.x] = height;
			}
		}
	}
}
