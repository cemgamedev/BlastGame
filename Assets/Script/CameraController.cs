using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform gridCenter;
    public float cameraDistance = 10f;

    void Start()
    {
        if (gridCenter != null)
        {
            // Kamerayý grid'in merkezinden bakacak þekilde konumlandýr
            transform.position = gridCenter.position + new Vector3(cameraDistance, cameraDistance, -cameraDistance);
            transform.rotation = Quaternion.Euler(45f, 45f, 0f);  // Düzgün izometrik açý
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 5; // Grid boyutuna göre ayarlayýn
        }
    }
}
