using UnityEngine;

public class CardinalObject : MonoBehaviour
{
    public float verticalSpeed = 1.0f;
    public float horizontalSpeedCap = 3.0f;
    // public float horizontalSpeed = 0f;
    public float destroyZ;
    private float amplitude;
    private float startX;
    public float horizontalRangePercentage = 30f;
    private float phase;
    private float speedScale;

    KiteMinigameManager kiteMinigame;

    void Start() {
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        amplitude = camHalfWidth * horizontalRangePercentage * 0.01f; //30% of screen width

        startX = transform.position.x;
        phase = Random.Range(0f, Mathf.PI * 2f);
        
        destroyZ = CameraBounds.MinZ - 1f; // arbitrary line below camera's view to destroy objects

        this.speedScale = kiteMinigame.enemyManager.GetSpeedScale();
    }

    void Update() {
        if (!kiteMinigame.IsRunning) return;

        transform.Translate(Vector3.back * verticalSpeed * speedScale * Time.deltaTime, Space.World);

        float xOffset = amplitude * Mathf.Sin(Time.time * horizontalSpeedCap + phase); // define sine-wave x offset
        Vector3 pos = transform.position; // fetch current position as a vector3
        pos.x = startX + xOffset; // add offset to current position
        transform.position = pos; // set position to new calculated position

        if (transform.position.z < destroyZ) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other){
        // Debug.Log("Triggered by: " + other.name);

        if (!kiteMinigame.IsRunning) return;

        if (other.CompareTag("PlayerKite") && !other.GetComponent<KitePlayerController>().invincible)
        {
            // Debug.Log("PlayerKite hit a cardinal!");
            kiteMinigame.LoseLife();
            other.GetComponent<KitePlayerController>().isHit();
        }
    }
}