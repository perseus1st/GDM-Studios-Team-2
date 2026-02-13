using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    public float camHalfWidth;
    private const float LEFT = -1;
    private const float MIDDLE = 0;
    private const float RIGHT = 1;

    private float branchSize;
    private float eagleSize;
    private float cardinalRange;

    KiteMinigameManager kiteMinigame;

    void Start() {
        // calculations to find screen width
        float screenAspect = (float) Screen.width / (float) Screen.height;
        float camHalfHeight = Camera.main.orthographicSize;
        camHalfWidth = screenAspect * camHalfHeight;
        // camWidth = 2.0f * camHalfWidth;
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();

        branchSize = 0.4f * camHalfWidth; // 40% of width of camera
        eagleSize = 0.1f * camHalfWidth; // 10% of width of camera
        cardinalRange = 0.3f * camHalfWidth;

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (kiteMinigame.IsRunning)
            {
                yield return StartCoroutine(PatternOne());
                yield return StartCoroutine(PatternFour());
                yield return StartCoroutine(PatternThree());
                yield return StartCoroutine(PatternOne());
                yield return StartCoroutine(PatternTwo());
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator PatternOne()
    {
        Debug.Log("Beginning Pattern #1");
        SpawnBranch(RIGHT);
        SpawnCardinal(LEFT);
        SpawnEagle(MIDDLE);
        yield return new WaitForSeconds(1f);
        SpawnPole();
        yield return new WaitForSeconds(1f);
        SpawnEagle(RIGHT);
        SpawnCardinal(LEFT);
        SpawnPole();
    }

    IEnumerator PatternTwo()
    {
        Debug.Log("Beginning Pattern #2");
        SpawnBranch(RIGHT);
        SpawnBranch(LEFT);

        yield return new WaitForSeconds(1.5f);

        SpawnEagle(LEFT);

        yield return new WaitForSeconds(1f);

        SpawnCardinal(LEFT);
        SpawnCardinal(RIGHT);
    }

    IEnumerator PatternThree()
    {
        Debug.Log("Beginning Pattern #3");
        SpawnBranch(RIGHT);
        yield return new WaitForSeconds(1f);
        SpawnBranch(LEFT);
    }

    IEnumerator PatternFour()
    {
        Debug.Log("Beginning Pattern #4");
        SpawnEagle(RIGHT);
        yield return new WaitForSeconds(1f);
    }

    void SpawnBranch(float side)
    {
        float x = (camHalfWidth - branchSize) * side;
        float z = CameraBounds.MaxZ + 1.0f;
        Vector3 pos = new Vector3(x, 0, z);
        Instantiate(obstaclePrefabs[0], pos, Quaternion.Euler(90f, 0f, 0f));
    }
    void SpawnPole()
    {
        float z = CameraBounds.MaxZ + 1.0f;
        Vector3 pos = new Vector3(0, 0, z);
        Instantiate(obstaclePrefabs[1], pos, Quaternion.Euler(90f, 0f, 0f));
    }

    void SpawnEagle(float side)
    {
        Debug.Log("Side: " + side);
        float range = Random.Range(-1f/3f * camHalfWidth + eagleSize, 1f/3f * camHalfWidth - eagleSize);
        float x = range + side*camHalfWidth * (2f/3f);
        Debug.Log("Range: " + range);
        Debug.Log("X: " + x);
        float z = CameraBounds.MaxZ + 1.0f;
        Vector3 pos = new Vector3(x, 0, z);
        Instantiate(obstaclePrefabs[2], pos, Quaternion.Euler(90f, 0f, 0f));
    }

    void SpawnCardinal(float side)
    {
        float x = (camHalfWidth - cardinalRange) * side;
        float z = CameraBounds.MaxZ + 1.0f;
        Vector3 pos = new Vector3(x, 0, z);
        Instantiate(obstaclePrefabs[3], pos, Quaternion.Euler(90f, 0f, 0f));   
    }
}