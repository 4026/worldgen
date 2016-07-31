using UnityEngine;
using UnityEngine.EventSystems;
using Biomes;
using UnityEngine.UI;
using System;

public class TerrainUIHandler : MonoBehaviour, IScrollHandler, IPointerClickHandler
{
    Text m_locationText;

    public void Start()
    {
        m_locationText = GameObject.Find("LocationText").GetComponent<Text>();
    }

	/// <summary>
	/// Called when the pointer clicks on the terrain.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData)
	{
		TerrainGenerator generator = GetComponent<TerrainGenerator> ();
		Point heightmapPos = generator.GetHeightmapPosFromWorldPos (eventData.pointerCurrentRaycast.worldPosition);
		BiomeMapPixel biomeData = generator.BiomeMap.GetPixelAtPoint(heightmapPos);

        m_locationText.text = heightmapPos + Environment.NewLine + biomeData;
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
