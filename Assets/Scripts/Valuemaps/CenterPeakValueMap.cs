using UnityEngine;

public class CenterPeakValueMap : AbstractValueMap
{
	private float zeroRadius;
	private float oneRadius;

	public CenterPeakValueMap (int newSize, float newZeroRadius, float newOneRadius) : base(newSize)
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
				height = 1f - Mathf.Clamp ((distanceFromCenter - oneRadius) / (zeroRadius - oneRadius), 0f, 1f);
				m_values [(int)mapPosition.y, (int)mapPosition.x] = height;
			}
		}
	}
}
