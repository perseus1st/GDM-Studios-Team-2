using UnityEngine;

public class EagleObject : MonoBehaviour
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

        if (other.CompareTag("PlayerKite"))
        {
            Debug.Log("PlayerKite hit an eagle!");
            kiteMinigame.GameOver();
        }
    }
}