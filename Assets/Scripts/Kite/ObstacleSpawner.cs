using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    public float camHalfWidth;
    private const float LEFT = -1; // A constant to spawn object on left side of screen
    private const float MIDDLE = 0; // A constant to spawn object in middle of screen
    private const float RIGHT = 1; // A constant to spawn object on right side of screen

    private const float SPAWN_PATTERN_CUSHION = 2f; // Time between spawn patterns
    [Header("Speed Settings")]
    public float speedScale; // Speed at which the game progresses
    private float processSpeedChange; // To make sure the speed doesn't change mid-pattern.
    public float defaultSpeedScale;
    private bool levelChange = false;

    private float branchSize; 
    private float eagleSize;
    private float cardinalRange; // Amplitude of cardinal sine wave

    private int attempt = 0;

    KiteMinigameManager kiteMinigame;

    void Start() {
        Debug.Log("OBSTACLE SPAWNER STARTING");
        // calculations to find screen width
        float screenAspect = (float) Screen.width / (float) Screen.height;
        float camHalfHeight = Camera.main.orthographicSize;
        camHalfWidth = screenAspect * camHalfHeight;
        // camWidth = 2.0f * camHalfWidth;
        kiteMinigame = FindAnyObjectByType<KiteMinigameManager>();

        branchSize = 0.4f * camHalfWidth; // 40% of width of camera
        eagleSize = 0.1f * camHalfWidth; // 10% of width of camera
        cardinalRange = 0.3f * camHalfWidth; // 30% of width of camera

        this.speedScale = 1/kiteMinigame.speedScale;
        processSpeedChange = speedScale;
        this.defaultSpeedScale = kiteMinigame.levelOneSpeed;

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        int lastchoice = -1;
        while (kiteMinigame.IsRunning)
        {
            Debug.Log("Speed Scale:" + 1/speedScale);
            Debug.Log("BUFFER:" + 1/processSpeedChange);
            int choice;

            if (attempt == 0)
            {
                choice = 7;
            } 
            else if (levelChange)
            {
                choice = 8; // delay next spawn pattern to prevent different speed obstacles from overlapping during level changes
            } 
            else
                {
                do
                {
                    choice = Random.Range(1,7);
                }
                while (choice == lastchoice); // to prevent choice from being made twice in a row

                lastchoice = choice;
            }

            switch(choice)
            {
                case 1:
                    yield return StartCoroutine(PatternOne());
                    break;
                case 2:
                    yield return StartCoroutine(PatternTwo());
                    break;
                case 3:
                    yield return StartCoroutine(PatternThree());
                    break;
                case 4:
                    yield return StartCoroutine(PatternFour());
                    break;
                case 5:
                    yield return StartCoroutine(PatternFive());
                    break;
                case 6:
                    yield return StartCoroutine(PatternSix());
                    break;
                case 7:
                    // on first run, to have small delay before beginning of game
                    Debug.Log("==============BEGINNING RUN==============");
                    // yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION);
                    SpawnBranch(LEFT);
                    SpawnBranch(RIGHT);
                    SpawnWindGust(MIDDLE);
                    yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION);
                    attempt++;
                    break;
                case 8:
                    // when speed changes, to avoid overlapping spawns
                    Debug.Log("==============CHANGING LEVELS==============");
                    speedScale=processSpeedChange;
                    yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION);
                    levelChange = false;
                    break;
            }
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
        // speedScale=processSpeedChange;
        yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION);
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
        // speedScale=processSpeedChange;
        yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION);
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

        // speedScale=processSpeedChange;
        yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION);
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
        // speedScale=processSpeedChange;
        yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION);
    }

    IEnumerator PatternFive()
    {
        Debug.Log("==============Beginning Pattern #5==============");

        SpawnPole();

        yield return new WaitForSeconds(1.3f * speedScale);

        SpawnBranch(LEFT);
        SpawnBranch(RIGHT);

        yield return new WaitForSeconds(1.3f * speedScale);

        SpawnPole();


        yield return new WaitForSeconds(0.6f * speedScale);

        SpawnWindGust(LEFT);
        SpawnWindGust(RIGHT);

        yield return new WaitForSeconds(0.6f * speedScale);

        SpawnBranch(LEFT);
        SpawnBranch(RIGHT);

        yield return new WaitForSeconds(1.2f * speedScale);

        // SpawnPole();

        // yield return new WaitForSeconds(1f * speedScale);

        // SpawnBranch(LEFT);
        // SpawnBranch(RIGHT);

        // yield return new WaitForSeconds(1f * speedScale);

        SpawnCardinal(LEFT);
        SpawnCardinal(MIDDLE);
        SpawnCardinal(RIGHT);
        SpawnWindGust(MIDDLE);
        // speedScale=processSpeedChange;
        yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION);
    }

    IEnumerator PatternSix()
    {
        Debug.Log("==============Beginning Pattern #6==============");
        
        int reps = 10;

        SpawnWindGust(LEFT);

        while (reps > 0)
        {
            SpawnEagle(Random.Range(-1,2)); // LEFT, RIGHT OR MIDDLE RANDOMLY
            SpawnEagle(Random.Range(-1,2)); // LEFT, RIGHT OR MIDDLE RANDOMLY
            yield return new WaitForSeconds(0.7f * speedScale);
            reps--;
        }
        SpawnWindGust(RIGHT);
        // speedScale=processSpeedChange;
        yield return new WaitForSeconds(SPAWN_PATTERN_CUSHION);

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
        // Debug.Log("Side: " + side);
        float range = Random.Range(-1f/3f * camHalfWidth + eagleSize, 1f/3f * camHalfWidth - eagleSize);
        float x = range + side*camHalfWidth * (2f/3f);
        // Debug.Log("Range: " + range);
        // Debug.Log("X: " + x);
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
        processSpeedChange=1/speedScale;
    }

    public void Reset()
    {
        SetSpeedScale(defaultSpeedScale);
        attempt++;
    }

    public float GetSpeedScale()
    {
        return 1/speedScale;
    }
}