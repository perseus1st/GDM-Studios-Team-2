using UnityEngine;

// This code is used to ensure the player cannot move outside the camera's visible area
// It clamps the player's position to the edges of the screen

public class PlayerBoundsLimiter : MonoBehaviour
{
    float halfWidth;
    float halfDepth;

    // Flag to track whether we've calculated the player's size
    // To prevent uninitialized values
    bool sizeCached;

    void Start()
    {
        // When object starts, calculate and store player size
        CacheSize();
    }

    void CacheSize()
    {
        Renderer r = GetComponentInChildren<Renderer>();

        // World size of player
        Vector3 size = r.bounds.size;

        halfWidth = size.x * 0.5f;
        halfDepth = size.z * 0.5f;

        sizeCached = true;
    }

    void LateUpdate()
    {
        if (!CameraBounds.Initialized || !sizeCached) return;

        Vector3 pos = transform.position;

        // To limit the range of where the player can move to be within the camera's view
        pos.x = Mathf.Clamp(
            pos.x,
            CameraBounds.MinX + halfWidth,
            CameraBounds.MaxX - halfWidth
        );

        pos.z = Mathf.Clamp(
            pos.z,
            CameraBounds.MinZ + halfDepth,
            CameraBounds.MaxZ - halfDepth
        );

        transform.position = pos;
    }
}
