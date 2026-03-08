using UnityEngine;

public class WindGustObject : MonoBehaviour
{
    [Header("Wind Gust Settings")]
    public float speed = 1.0f; // Vertical speed of Wind Gust obstacle
    public float destroyZ; // Z-coordinate at which the Wind Gust gets destroyed
    private float speedScale; // Scale for speed impacted by score of minigame

    KiteMinigameManager kiteMinigame;

    void Start()
    {
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();
        destroyZ = CameraBounds.MinZ - 1f; // Destroys Wind Gust just below camera boundaries

        this.speedScale = kiteMinigame.enemyManager.GetSpeedScale(); // Speed scale is determined by score
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * speedScale * Time.deltaTime, Space.World); // Movement

        if (transform.position.z < destroyZ) // Check to destroy if Wind Gust is below play area
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        kiteMinigame.AddScore();
        Destroy(gameObject);
    }
}