using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public float spawnInterval = 1.2f;

    public float camWidth;
    public float branchSize; 
    public float eagleSize;

    float timer;
    KiteMinigameManager kiteMinigame;

    void Start() {
        // calculations to find screen width
        float screenAspect = (float) Screen.width / (float) Screen.height;
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = screenAspect * camHalfHeight;
        camWidth = 2.0f * camHalfWidth;

        branchSize = 0.40f * camWidth; // Branch covers 40% of screen width
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();
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
        SpawnBranch(0f);
        SpawnCardinal(0f);
        SpawnEagle(0f);
        SpawnPole(0f);
        // // float x = Random.Range(CameraBounds.MinX, CameraBounds.MaxX);

        // int side = Random.Range(-1, 2); // side = -1 (left) or 0 (middle) or +1 (right)
        // float x = (CameraBounds.MaxX - 0.5f * branchSize) * side;

        // // Spawn slightly above the camera's view
        // float z = CameraBounds.MaxZ + 1.0f;

        // Vector3 pos = new Vector3(x, 0, z);

        // Instantiate(obstaclePrefabs[3], pos, Quaternion.Euler(90f, 0f, 0f));

        // if (side == 0) { // if in the middle
        //     Instantiate(obstaclePrefabs[1], pos, Quaternion.Euler(90f, 0f, 0f)); // spawn telephone pole
        //     side = (Random.Range(0,2) * 2) - 1;
        //     x = (CameraBounds.MaxX*0.75f - 0.5f * eagleSize) * side;
        //     pos = new Vector3(x, 0, z);
        //     Instantiate(obstaclePrefabs[2], pos, Quaternion.Euler(90f, 0f, 0f)); // spawn eagle
        // } 
        // else { // if on either side
        //     Instantiate(obstaclePrefabs[0], pos, Quaternion.Euler(90f, 0f, 0f)); // spawn branch
        // }
    }

    void SpawnBranch(float x)
    {
        float z = CameraBounds.MaxZ + 1.0f;
        Vector3 pos = new Vector3(x, 0, z);
        Instantiate(obstaclePrefabs[0], pos, Quaternion.Euler(90f, 0f, 0f));
    }
    void SpawnPole(float x)
    {
        float z = CameraBounds.MaxZ + 1.0f;
        Vector3 pos = new Vector3(x, 0, z);
        Instantiate(obstaclePrefabs[1], pos, Quaternion.Euler(90f, 0f, 0f));
    }

    void SpawnEagle(float x)
    {
        int side = (Random.Range(0,2) * 2) - 1;
        // float x = (CameraBounds.MaxX*0.75f - 0.5f * eagleSize) * side;
        float z = CameraBounds.MaxZ + 1.0f;
        Vector3 pos = new Vector3(x, 0, z);
        Instantiate(obstaclePrefabs[2], pos, Quaternion.Euler(90f, 0f, 0f));
    }

    void SpawnCardinal(float x)
    {
        // int side = Random.Range(-1, 2); // side = -1 (left) or 0 (middle) or +1 (right)
        // float x = (CameraBounds.MaxX - 0.5f * branchSize) * side;

        float z = CameraBounds.MaxZ + 1.0f;

        Vector3 pos = new Vector3(x, 0, z);

        Instantiate(obstaclePrefabs[3], pos, Quaternion.Euler(90f, 0f, 0f));
    }

    void PatternOne()
    {
        
    }
}