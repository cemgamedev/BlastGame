using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform gridCenter;
    public float cameraDistance = 10f;

    void Start()
    {
        if (gridCenter != null)
        {
            // Kameray� grid'in merkezinden bakacak �ekilde konumland�r
            transform.position = gridCenter.position + new Vector3(cameraDistance, cameraDistance, -cameraDistance);
            transform.rotation = Quaternion.Euler(45f, 45f, 0f);  // D�zg�n izometrik a��
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 5; // Grid boyutuna g�re ayarlay�n
        }
    }
}
