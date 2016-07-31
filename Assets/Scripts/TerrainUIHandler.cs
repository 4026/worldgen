using UnityEngine;
using UnityEngine.EventSystems;
using Biomes;
using UnityEngine.UI;
using System;

public class TerrainUIHandler : MonoBehaviour, IPointerClickHandler
{
    private Text m_locationText;
    private GameObject m_positionMarker;
    private GameObject m_positionMarkerPrefab;

    public void Start()
    {
        m_locationText = GameObject.Find("LocationText").GetComponent<Text>();
        m_positionMarkerPrefab = Resources.Load("Prefabs/PositionMarker") as GameObject;
    }

	/// <summary>
	/// Called when the pointer clicks on the terrain.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData)
	{
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        //Get info about clicked point.
        Vector3 clicked_point = eventData.pointerCurrentRaycast.worldPosition;
        TerrainGenerator generator = GetComponent<TerrainGenerator> ();
		Point heightmapPos = generator.GetHeightmapPosFromWorldPos (clicked_point);
		BiomeMapPixel biomeData = generator.BiomeMap.GetPixelAtPoint(heightmapPos);

        //Place location marker.
        if (m_positionMarker != null)
        {
            Destroy(m_positionMarker);
        }
        Vector3 camera_direction = transform.position - Camera.main.transform.position;
        camera_direction.y = 0;
        Quaternion look_rotation = Quaternion.LookRotation(camera_direction, Vector3.up);
        m_positionMarker = Instantiate(m_positionMarkerPrefab, clicked_point, look_rotation) as GameObject;

        //Animate it into position.
        m_positionMarker.transform.localScale = new Vector3(1, 0, 0);
        LeanTween.scaleY(m_positionMarker, 1, 0.5f).setEase(LeanTweenType.easeOutElastic);
        LeanTween.scaleZ(m_positionMarker, 1, 1f).setEase(LeanTweenType.easeOutElastic);

        //Update location text
        m_locationText.text = heightmapPos + Environment.NewLine + biomeData;
	}
}
