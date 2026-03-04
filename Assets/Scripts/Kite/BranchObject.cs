using UnityEngine;

// NOTE: Because Telephone Pole and Branch have the exact same qualities,
// I am using the BranchObject class for Telephone Poles as well.

public class BranchObject : MonoBehaviour
{
    public float speed = 1.0f;
    public float destroyZ;

    KiteMinigameManager kiteMinigame;

    void Start() {
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();
        destroyZ = CameraBounds.MinZ - 1f;
    }

    void Update() {
        if (!kiteMinigame.IsRunning) return;

        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        if (transform.position.z < destroyZ) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other){
        Debug.Log("Triggered by: " + other.name);

        if (!kiteMinigame.IsRunning) return;

        if (other.CompareTag("PlayerKite") && !other.GetComponent<KitePlayerController>().invincible)
        {
            Debug.Log("PlayerKite hit a branch!");
            kiteMinigame.LoseLife();
            other.GetComponent<KitePlayerController>().isHit();
        }
    }
}
