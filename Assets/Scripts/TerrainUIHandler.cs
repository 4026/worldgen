using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TerrainUIHandler : MonoBehaviour, IScrollHandler
{

	/// <summary>
	/// Called when the scroll wheel is used while the cursor is over the terrain.
	/// </summary>
	/// <param name="eventData">Pointer event data.</param>
	public void OnScroll (PointerEventData eventData)
	{
		Camera.main.GetComponent<CameraController> ().ZoomIn (eventData.scrollDelta.y);
	}
}
