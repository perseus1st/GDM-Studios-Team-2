using UnityEngine;

public class CardinalObject : MonoBehaviour
{
    [Header("Cardinal Vertical Settings")]
    public float verticalSpeed = 1.0f; // Vertical speed of Cardinal obstacle
    public float destroyZ; // Z-coordinate at which the Cardinal gets destroyed

    [Header("Cardinal Sine Wave Settings")]
    public float horizontalSpeedCap = 3.0f; // Highest horizontal speed of Cardinal sine wave
    private float amplitude; // Amplitude of sine wave
    private float startX; // Midline of sine wave

    public float horizontalRangePercentage = 30f; // Percentage of screen the sine wave occupies

    private float phase; // Randomized phase shift of sine wave to offset all instances of Cardinal
    private float speedScale; // Scale for speed impacted by score of minigame

    KiteMinigameManager kiteMinigame;

    void Start()
    {
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        amplitude = camHalfWidth * horizontalRangePercentage * 0.01f; //30% of screen width

        startX = transform.position.x;

        // Randomized phase shift of sine wave to offset all instances of Cardinal
        phase = Random.Range(0f, Mathf.PI * 2f);

        destroyZ = CameraBounds.MinZ - 1f; // Arbitrary line below camera's view to destroy objects

        this.speedScale = kiteMinigame.enemyManager.GetSpeedScale(); // Speed scale is determined by score
    }

    void Update()
    {
        transform.Translate(Vector3.back * verticalSpeed * speedScale * Time.deltaTime, Space.World); // Vertical movement

        float xOffset = amplitude * Mathf.Sin(Time.time * horizontalSpeedCap + phase); // Define sine-wave x offset
        Vector3 pos = transform.position; // Fetch current position as a vector3
        pos.x = startX + xOffset; // Add offset to current position
        transform.position = pos; // Set position to new calculated position

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