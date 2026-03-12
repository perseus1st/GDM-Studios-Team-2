using UnityEngine;

// NOTE: Because Telephone Pole and Branch have the exact same qualities,
// I am using the BranchObject class for Telephone Poles as well.

public class BranchObject : MonoBehaviour
{
    [Header("Branch Settings")]
    public float speed = 1.0f; // Vertical speed of Branch obstacle
    public float destroyZ; // Z-coordinate at which the Branch gets destroyed
    private float speedScale; // Scale for speed impacted by score of minigame

    KiteMinigameManager kiteMinigame;

    void Start()
    {
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();
        destroyZ = CameraBounds.MinZ - 1f; // Destroys Branch just below camera boundaries

        this.speedScale = kiteMinigame.enemyManager.GetSpeedScale(); // Speed scale is determined by score
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * speedScale * Time.deltaTime, Space.World); // Movement

        if (transform.position.z < destroyZ) // Check to destroy if Branch is below play area
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!kiteMinigame.isRunning) return;

        if (other.CompareTag("PlayerKite") && !other.GetComponent<KitePlayerController>().isInvincible) // Check invincibility of Kite
        {
            kiteMinigame.LoseLife(); // Signal to display lost life
            other.GetComponent<KitePlayerController>().isHit(); // Hit player and begin invincibility period
        }
    }
}
