using UnityEngine;
using System.Collections;

/// <summary>
/// Component that makes the GameObject always face towards the camera along the xz plane (that is, 
/// it ignores the camera's altitude).
/// </summary>
public class Billboard : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //Face the camera
        Vector3 camera_direction = transform.position - Camera.main.transform.position;
        camera_direction.y = 0;
        transform.rotation = Quaternion.LookRotation(camera_direction, Vector3.up);
    }
}
