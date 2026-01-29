using UnityEngine;

public class NoteMover : MonoBehaviour
{
    public float speed = 5f; // Units per second
    public float targetY = -6f; // Y position of the hitzone

    void Update()
    {
        // Move downward
        transform.position += Vector3.down * speed * Time.deltaTime;

        // Optional: Destroy note if it goes past the hitzone
        if (transform.position.y < targetY -1f)
        {
            Destroy(gameObject);
        }
    }
}
