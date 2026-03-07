using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    public static bool Initialized;

    public static float MinX, MaxX, MinZ, MaxZ;

    void OnEnable()
    {
        Initialize();
    }

    void Initialize()
    {
        Camera cam = Camera.main;
        if (!cam || !cam.orthographic) return;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        Vector3 pos = cam.transform.position;

        MinX = pos.x - halfWidth;
        MaxX = pos.x + halfWidth;

        MinZ = pos.z - halfHeight;
        MaxZ = pos.z + halfHeight;

        Initialized = true;
    }
}
