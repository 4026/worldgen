using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TerrainUIHandler : MonoBehaviour, IScrollHandler, IPointerClickHandler
{
	/// <summary>
	/// Called when the pointer clicks on the terrain.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData)
	{
		TerrainGenerator generator = GetComponent<TerrainGenerator> ();
		Vector2 heightmapPos = generator.GetHeightmapPosFromWorldPos (eventData.pointerCurrentRaycast.worldPosition);
		float[] biomeWeights = generator.GetBiomeWeightsAtHeightmapPos ((int)heightmapPos.x, (int)heightmapPos.y);
		Debug.Log (
			heightmapPos + " - " +
			"Plains: " + biomeWeights [(int)BiomeType.Plains] + ", " +
			"Desert: " + biomeWeights [(int)BiomeType.Desert] + ", " +
			"Swamp: " + biomeWeights [(int)BiomeType.Swamp] + ", " +
			"Alpine: " + biomeWeights [(int)BiomeType.Snow]
		);
	}

	/// <summary>
	/// Called when the scroll wheel is used while the cursor is over the terrain.
	/// </summary>
	/// <param name="eventData">Pointer event data.</param>
	public void OnScroll (PointerEventData eventData)
	{
		Camera.main.GetComponent<CameraController> ().ZoomIn (eventData.scrollDelta.y);
	}
}
