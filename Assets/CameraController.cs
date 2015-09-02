using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	/// <summary>
	/// The speed at which the camera translates based on user input.
	/// </summary>
	public float TranslateSpeed;
	/// <summary>
	/// The speed at which the camera pans based on user input.
	/// </summary>
	public float PanSpeed;
	/// <summary>
	/// The minimum pitch (rotation about the X axis) that the camera is permitted, in degrees.
	/// </summary>
	public float MinPitch;
	/// <summary>
	/// The minimum pitch (rotation about the X axis) that the camera is permitted, in degrees.
	/// </summary>
	public float MaxPitch;
	/// <summary>
	/// The amount by which the camera zooms based on the scroll wheel.
	/// </summary>
	public float ZoomMultiplier;
	/// <summary>
	/// The time, in seconds, that the zoom animation lasts for.
	/// </summary>
	public float ZoomEaseDuration;
	/// <summary>
	/// The minimum y value that the camera may reach by zooming in.
	/// </summary>
	public float MinY;
	/// <summary>
	/// The maximum y value that the camera may reach by zooming out.
	/// </summary>
	public float MaxY;

	/// <summary>
	/// The XZ bounds within which the camera may move.
	/// </summary>
	public Rect XZBounds;

	/// <summary>
	/// The vector that the camera will travel through over the course of the zoom animation.
	/// </summary>
	private Vector3 m_zoomVector = Vector3.zero;
	/// <summary>
	/// The fraction of the zoom animation that has been completed.
	/// </summary>
	private float m_zoomT = 1;

	/// <summary>
	/// Whether the camera is currently panning (ie. the middle mouse button is held down).
	/// </summary>
	private bool m_panning = false;


	void Update ()
	{
		panUpdate ();
		translationUpdate ();
		zoomUpdate ();
	}

	/// <summary>
	/// Applies any camera translations required in this frame.
	/// </summary>
	private void translationUpdate ()
	{
		//Translate the camera based on the user's axis inputs.
		Vector3 translation = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")) * TranslateSpeed;
		translation = Quaternion.Euler (0, transform.rotation.eulerAngles.y, 0) * translation;
		
		//Make sure that the camera focus cannot leave the borders of the map.
		Vector3 cameraFocus = getFocusPoint ();
		translation.x = Mathf.Clamp (translation.x, XZBounds.xMin - cameraFocus.x, XZBounds.xMax - cameraFocus.x);
		translation.z = Mathf.Clamp (translation.z, XZBounds.yMin - cameraFocus.z, XZBounds.yMax - cameraFocus.z);
		
		transform.position += translation;
	}

	/// <summary>
	/// Applies any camera panning (rotation) required in this frame.
	/// </summary>
	private void panUpdate ()
	{
		//Pan the camera based on middle-click dragging.
		if (Input.GetMouseButtonDown (2)) {
			//Begin panning on MMB down
			m_panning = true;
			Cursor.lockState = CursorLockMode.Locked;
		} 

		if (Input.GetMouseButtonUp (2)) {
			//End panning on MMB up
			m_panning = false;
			Cursor.lockState = CursorLockMode.None;
		}

		if (m_panning) {
			//The camera may yaw freely.
			transform.RotateAround (transform.position, Vector3.up, Input.GetAxis ("Mouse X") * PanSpeed);
			//Limit the camera's pitch.
			float pitchAngle = Mathf.Clamp (-Input.GetAxis ("Mouse Y") * PanSpeed, MinPitch - transform.rotation.eulerAngles.x, MaxPitch - transform.rotation.eulerAngles.x);
			transform.RotateAround (transform.position, transform.right, pitchAngle);
		}
	}

	/// <summary>
	/// Applies the zooming animation, if one is in progress.
	/// </summary>
	private void zoomUpdate ()
	{
		if (m_zoomT < 1) {
			// Calculate the fraction of the zoom animation to play this frame.
			float t0 = m_zoomT;
			float t1 = Mathf.Min (m_zoomT + (Time.deltaTime / ZoomEaseDuration), 1);
			
			//Add the eased fraction of the zoom vector to the camera's current position.
			transform.position += m_zoomVector * getEaseDelta (t0, t1);
			
			//Update the progress through the animation.
			m_zoomT = t1;
		}
	}

	/// <summary>
	/// Zooms the camera in by the specified amount.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void ZoomIn (float amount)
	{
		//Calculate the zoom vector to apply to the camera based on this input.
		Vector3 newZoomVector = transform.forward * amount * ZoomMultiplier;

		//Get any remainder of the existing zoom vector that hasn't been applied yet.
		Vector3 zoomRemainder = m_zoomVector * getEaseDelta (m_zoomT, 1);

		//Add the two vectors to get the total new zoom vector to apply, but ensure that it cannot cause the camera to move beyond its Y limits.
		m_zoomVector = newZoomVector + zoomRemainder;
		float permittedDeltaY = Mathf.Clamp (m_zoomVector.y, MinY - transform.position.y, MaxY - transform.position.y);
		m_zoomVector *= permittedDeltaY / m_zoomVector.y;

		//Reset the animation's progress
		m_zoomT = 0;
	}

	/// <summary>
	/// Gets the change in easing factor between times t0 and t1, where 0 <= t0 <= t1 <= 1.
	/// </summary>
	/// <returns>The easing factor delta.</returns>
	/// <param name="t0">The start time.</param>
	/// <param name="t1">The end time.</param>
	private float getEaseDelta (float t0, float t1)
	{
		return easeOutQuad (t1) - easeOutQuad (t0);
	}

	/// <summary>
	/// Get the fraction of the total displacement in a quadratically easing-out motion that should be achieved by time t, where 0 <= t <= 1.
	/// </summary>
	/// <returns>The easing factor.</returns>
	/// <param name="t">The fraction of the total animation duration that has elapsed.</param>
	private float easeOutQuad (float t)
	{
		return t * (2 - t);
	}

	/// <summary>
	/// Gets the point on the XZ plane that the camera is currently centered on.
	/// </summary>
	/// <returns>The camera focus point.</returns>
	private Vector3 getFocusPoint ()
	{
		Plane xz = new Plane (Vector3.up, Vector3.zero);
		Ray cameraRay = new Ray (transform.position, transform.forward);
		float distance = 0; 
		if (!xz.Raycast (cameraRay, out distance)) {
			throw new UnityException ("Camera isn't pointing at the XZ plane for some reason.");
		}

		return cameraRay.GetPoint (distance);
	}
}
