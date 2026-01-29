using UnityEngine;

public class BranchSpawner : MonoBehaviour
{
    public GameObject branchPrefab;
    public float spawnInterval = 1.2f;
    public float branchSize = 0.40f * 19.6f;

    float timer;
    KiteMinigameManager kiteMinigame;

    void Start() {
        kiteMinigame = FindObjectOfType<KiteMinigameManager>();
    }

    void Update() {
        if (!kiteMinigame.IsRunning) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            Spawn();
            timer = 0f;
        }
    }

    void Spawn() {
    // float x = Random.Range(CameraBounds.MinX, CameraBounds.MaxX);

    int side = Random.Range(0, 2) * 2 - 1; // side = -1 (left) or +1 (right)
    float x = (CameraBounds.MaxX - 0.5f * branchSize) * side;

    // Spawn slightly above the camera's view
    float z = CameraBounds.MaxZ + 1.0f;

    Vector3 pos = new Vector3(x, 0, z);

    Instantiate(branchPrefab, pos, Quaternion.Euler(90f, 0f, 0f)
    );
}
}
