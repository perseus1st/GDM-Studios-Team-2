using UnityEngine;

public class CardinalObject : MonoBehaviour
{
    public float verticalSpeed = 1.0f;
    public float horizontalSpeedCap = 3.0f;
    public float horizontalSpeed = 0f;
    public float destroyZ = CameraBounds.MinZ - 1f;
    public float amplitude;
    private float startX;
    public float horizontalRangePercentage = 30f;
    private float phase;

    KiteMinigameManager kiteMinigame;

    void Start() {
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        amplitude = camHalfWidth * horizontalRangePercentage * 0.01f;
        startX = transform.position.x;
        phase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update() {
        if (!kiteMinigame.IsRunning) return;

        transform.Translate(Vector3.back * verticalSpeed * Time.deltaTime, Space.World);

        float xOffset = amplitude * Mathf.Sin(Time.time * horizontalSpeedCap + phase);
        Vector3 pos = transform.position;
        pos.x = startX +xOffset;
        transform.position = pos;

        if (transform.position.z < destroyZ) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other){
        Debug.Log("Triggered by: " + other.name);

        if (!kiteMinigame.IsRunning) return;

        if (other.CompareTag("PlayerKite"))
        {
            Debug.Log("PlayerKite hit a cardinal!");
            kiteMinigame.GameOver();
        }
    }
}