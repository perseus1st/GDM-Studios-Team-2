using UnityEngine;

public class EagleObject : MonoBehaviour
{
    public float speed = 1.0f;
    public float destroyZ;
    private float speedScale;

    KiteMinigameManager kiteMinigame;

    void Start() {
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();
        destroyZ = CameraBounds.MinZ - 1f;

        this.speedScale = kiteMinigame.enemyManager.GetSpeedScale();
    }

    void Update() {
        if (!kiteMinigame.IsRunning) return;

        transform.Translate(Vector3.back * speed * speedScale * Time.deltaTime, Space.World);

        if (transform.position.z < destroyZ) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other){
        // Debug.Log("Triggered by: " + other.name);

        if (!kiteMinigame.IsRunning) return;

        if (other.CompareTag("PlayerKite") && !other.GetComponent<KitePlayerController>().invincible)
        {
            // Debug.Log("PlayerKite hit an eagle!");
            kiteMinigame.LoseLife();
            other.GetComponent<KitePlayerController>().isHit();
        }
    }
}