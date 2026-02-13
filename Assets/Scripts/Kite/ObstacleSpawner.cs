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
        int lastchoice = -1;
        while (true)
        {
            if (kiteMinigame.IsRunning)
            {

                int choice;

                do
                {
                    choice = Random.Range(1,7);
                }
                while (choice == lastchoice); // to prevent choice from being made twice in a row

                lastchoice = choice;

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
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator PatternOne()
    {
        Debug.Log("==============Beginning Pattern #1==============");

        SpawnBranch(RIGHT);
        SpawnCardinal(LEFT);
        SpawnEagle(MIDDLE);

        yield return new WaitForSeconds(1f);

        SpawnPole();

        yield return new WaitForSeconds(1f);

        SpawnEagle(RIGHT);
        SpawnCardinal(LEFT);
        SpawnPole();

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PatternTwo()
    {
        Debug.Log("==============Beginning Pattern #2==============");

        SpawnBranch(RIGHT);
        SpawnBranch(LEFT);

        yield return new WaitForSeconds(2f);

        SpawnEagle(MIDDLE);

        yield return new WaitForSeconds(1f);

        SpawnCardinal(LEFT);
        SpawnCardinal(MIDDLE);
        SpawnCardinal(RIGHT);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PatternThree()
    {
        Debug.Log("==============Beginning Pattern #3==============");

        SpawnCardinal(LEFT);
        SpawnCardinal(MIDDLE);
        SpawnEagle(RIGHT);

        yield return new WaitForSeconds(1f);
        
        SpawnBranch(RIGHT);
    
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PatternFour()
    {
        Debug.Log("==============Beginning Pattern #4==============");

        SpawnEagle(RIGHT);
        SpawnPole();
        SpawnEagle(LEFT);

        yield return new WaitForSeconds(1f);

        SpawnCardinal(RIGHT);
        SpawnCardinal(LEFT);

        yield return new WaitForSeconds(1f);

        SpawnEagle(MIDDLE);
        SpawnEagle(MIDDLE);
        SpawnEagle(LEFT);
        SpawnEagle(LEFT);
        SpawnEagle(RIGHT);
        SpawnEagle(RIGHT);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PatternFive()
    {
        Debug.Log("==============Beginning Pattern #5==============");

        SpawnPole();

        yield return new WaitForSeconds(1f);

        SpawnBranch(LEFT);
        SpawnBranch(RIGHT);

        yield return new WaitForSeconds(1f);

        SpawnPole();

        yield return new WaitForSeconds(1f);

        SpawnBranch(LEFT);
        SpawnBranch(RIGHT);

        yield return new WaitForSeconds(1f);

        SpawnPole();

        yield return new WaitForSeconds(1f);

        SpawnBranch(LEFT);
        SpawnBranch(RIGHT);

        yield return new WaitForSeconds(1f);

        SpawnCardinal(LEFT);
        SpawnCardinal(MIDDLE);
        SpawnCardinal(RIGHT);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PatternSix()
    {
        Debug.Log("==============Beginning Pattern #6==============");
        
        int reps = 5;

        while (reps > 0)
        {
            SpawnEagle(Random.Range(-1,2));
            yield return new WaitForSeconds(0.5f);
            reps--;
        }

        yield return new WaitForSeconds(0.5f);

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