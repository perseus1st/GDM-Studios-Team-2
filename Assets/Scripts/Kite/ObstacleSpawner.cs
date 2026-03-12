using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    // obstaclePrefabs[0] is Branch
    // obstaclePrefabs[1] is Pole
    // obstaclePrefabs[2] is Eagle
    // obstaclePrefabs[3] is Cardinal
    // obstaclePrefabs[4] is Wind Gust

    private float camHalfWidth;
    private const float LEFT = -1; // A constant to spawn object on left side of screen
    private const float MIDDLE = 0; // A constant to spawn object in middle of screen
    private const float RIGHT = 1; // A constant to spawn object on right side of screen

    [Header("Speed Settings")]
    public float speedScale; // Speed at which the game progresses
    public float defaultSpeedScale;
    private float processSpeedChange; // To make sure the speed doesn't change mid-pattern.
    private bool levelChange = false;
    public const float SPAWN_PATTERN_CUSHION = 2f; // Time between spawn patterns

    private float branchSize;
    private float eagleSize;
    private float cardinalRange; // Amplitude of cardinal's horizontal sine wave motion

    private int attempt = 0;

    private System.Func<IEnumerator>[] patterns;

    KiteMinigameManager kiteMinigame;

    void Start()
    {
        Debug.Log("OBSTACLE SPAWNER STARTING");

        // Calculations to find screen width
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float camHalfHeight = Camera.main.orthographicSize;
        camHalfWidth = screenAspect * camHalfHeight;

        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();

        branchSize = 0.4f * camHalfWidth; // 40% of width of camera
        eagleSize = 0.1f * camHalfWidth; // 10% of width of camera
        cardinalRange = 0.3f * camHalfWidth; // 30% of width of camera

        this.speedScale = 1 / kiteMinigame.speedScale; // Working with time, so scale is inverted

        // In order to prevent speeds from changing in the middle of patterns, 
        // the speed is buffered so that it can take effect after the pattern is finished
        processSpeedChange = speedScale;

        this.defaultSpeedScale = kiteMinigame.levelOneSpeed; // init speed

        patterns = new System.Func<IEnumerator>[] // An array of functions
        {
            PatternOne,
            PatternTwo,
            PatternThree,
            PatternFour,
            PatternFive,
            PatternSix
        };

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        int lastchoice = -1;

        while (kiteMinigame.isRunning)
        {

            if (attempt == 0) // To be run on first attempt
            {
                Debug.Log("==============BEGINNING RUN==============");
                SpawnBranch(LEFT);
                SpawnBranch(RIGHT);
                SpawnWindGust(MIDDLE);
                attempt++;
                yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION * speedScale);
                continue;
            }

            if (levelChange) // To be run in-between levels
            {
                Debug.Log("==============CHANGING LEVELS==============");
                speedScale = processSpeedChange; // Load the buffered speed
                levelChange = false;
                continue;
            }

            int choice;

            do
            {
                choice = Random.Range(0, patterns.Length);
            } while (choice == lastchoice); // To prevent same pattern from playing twice

            lastchoice = choice;

            yield return StartCoroutine(patterns[choice]()); // Calls the function stored in the array "patterns"

            yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION * speedScale); // Cushion between patterns
        }
    }

    IEnumerator PatternOne()
    {
        Debug.Log("==============Beginning Pattern #1==============");

        SpawnBranch(RIGHT);
        SpawnCardinal(LEFT);
        SpawnEagle(MIDDLE);

        yield return new WaitForSeconds(1.5f * speedScale);

        SpawnPole();
        SpawnWindGust(LEFT);
        SpawnWindGust(RIGHT);

        yield return new WaitForSeconds(1.5f * speedScale);

        SpawnEagle(LEFT);
        SpawnEagle(RIGHT);
        SpawnCardinal(MIDDLE);

        yield return new WaitForSeconds(1f * speedScale);

        SpawnBranch(LEFT);
        SpawnBranch(RIGHT);

        yield return new WaitForSeconds(1.5f * speedScale);

        SpawnEagle(RIGHT);
        SpawnCardinal(LEFT);
        SpawnPole();

        yield return new WaitForSeconds(1f * speedScale);

        SpawnWindGust(MIDDLE);
    }

    IEnumerator PatternTwo()
    {
        Debug.Log("==============Beginning Pattern #2==============");

        SpawnBranch(RIGHT);
        SpawnBranch(LEFT);

        yield return new WaitForSeconds(2f * speedScale);

        SpawnEagle(MIDDLE);
        SpawnCardinal(LEFT);
        SpawnCardinal(MIDDLE);
        SpawnCardinal(RIGHT);

        yield return new WaitForSeconds(1.5f * speedScale);

        SpawnCardinal(LEFT);
        SpawnCardinal(MIDDLE);
        SpawnCardinal(RIGHT);
        SpawnWindGust(RIGHT);
    }

    IEnumerator PatternThree()
    {
        Debug.Log("==============Beginning Pattern #3==============");

        SpawnCardinal(LEFT);
        SpawnCardinal(MIDDLE);
        SpawnEagle(RIGHT);

        yield return new WaitForSeconds(0.5f * speedScale);

        SpawnWindGust(RIGHT);

        yield return new WaitForSeconds(0.5f * speedScale);

        SpawnBranch(RIGHT);

        yield return new WaitForSeconds(1.5f * speedScale);

        SpawnEagle(LEFT);
        SpawnCardinal(LEFT);
        SpawnPole();
        SpawnWindGust(RIGHT);
    }

    IEnumerator PatternFour()
    {
        Debug.Log("==============Beginning Pattern #4==============");

        SpawnEagle(RIGHT);
        SpawnPole();
        SpawnEagle(LEFT);

        yield return new WaitForSeconds(1f * speedScale);

        SpawnCardinal(RIGHT);
        SpawnCardinal(LEFT);

        yield return new WaitForSeconds(1f * speedScale);

        SpawnEagle(MIDDLE);
        SpawnEagle(MIDDLE);
        SpawnEagle(LEFT);
        SpawnEagle(RIGHT);
        SpawnWindGust(LEFT);
    }

    IEnumerator PatternFive()
    {
        Debug.Log("==============Beginning Pattern #5==============");

        SpawnPole();

        yield return new WaitForSeconds(1.5f * speedScale);

        SpawnBranch(LEFT);
        SpawnBranch(RIGHT);

        yield return new WaitForSeconds(1.5f * speedScale);

        SpawnPole();


        yield return new WaitForSeconds(0.75f * speedScale);

        SpawnWindGust(LEFT);
        SpawnWindGust(RIGHT);

        yield return new WaitForSeconds(0.75f * speedScale);

        SpawnBranch(LEFT);
        SpawnBranch(RIGHT);

        yield return new WaitForSeconds(1.5f * speedScale);

        SpawnCardinal(LEFT);
        SpawnCardinal(MIDDLE);
        SpawnCardinal(RIGHT);
        SpawnWindGust(MIDDLE);
    }

    IEnumerator PatternSix()
    {
        Debug.Log("==============Beginning Pattern #6==============");

        SpawnWindGust(LEFT);

        // Spawns in 10 pairs of Eagles in random locations
        int reps = 10;
        while (reps > 0)
        {
            SpawnEagle(Random.Range(-1, 2)); // LEFT, RIGHT OR MIDDLE RANDOMLY
            SpawnEagle(Random.Range(-1, 2)); // LEFT, RIGHT OR MIDDLE RANDOMLY
            yield return new WaitForSeconds(0.7f * speedScale);
            reps--;
        }
        SpawnWindGust(RIGHT);
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
        float range = Random.Range(-1f / 3f * camHalfWidth + eagleSize, 1f / 3f * camHalfWidth - eagleSize);
        float x = range + side * camHalfWidth * (2f / 3f);
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

    void SpawnWindGust(float side)
    {
        float x = (camHalfWidth - cardinalRange) * side;
        float z = CameraBounds.MaxZ + 1.0f;
        Vector3 pos = new Vector3(x, 0, z);
        Instantiate(obstaclePrefabs[4], pos, Quaternion.Euler(90f, 0f, 0f));
    }

    public void SetSpeedScale(float speedScale)
    {
        levelChange = true;
        processSpeedChange = 1 / speedScale; // Invert because we are scaling time here, which is inversely proportional to speed
    }

    public void Reset()
    {
        SetSpeedScale(defaultSpeedScale);
        attempt++;
    }

    public float GetSpeedScale()
    {
        return 1 / speedScale; // Invert because all other scripts work with speed, which is inversely proportional to time
    }
}