using UnityEngine;

public class EagleObject : MonoBehaviour
{
    [Header("Eagle Settings")]
    public float speed = 1.0f; // Vertical speed of Eagle obstacle
    public float destroyZ; // Z-coordinate at which the Eagle gets destroyed
    private float speedScale; // Scale for speed impacted by score of minigame

    KiteMinigameManager kiteMinigame;

    void Start()
    {
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();
        destroyZ = CameraBounds.MinZ - 1f; // Destroys Eagle just below camera boundaries

        this.speedScale = kiteMinigame.enemyManager.GetSpeedScale(); // Speed scale is determined by score
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * speedScale * Time.deltaTime, Space.World); // Movement

        if (transform.position.z < destroyZ) // Check to destroy if Eagle is below play area
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