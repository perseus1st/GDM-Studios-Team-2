// Created by Daniil Makarenko

using UnityEngine;

public class DodgeballFriendlyBallSpawner : MonoBehaviour
{
    // References
    [Header("References")]
    public GameObject FriendlyBall; // Prefab for the friendly ball
    public Transform player; // Reference to player
    public DodgeballPlayerController playerController; // Reference to player controller
    public DodgeballEnemyManager enemyManager; // Reference to enemy manager
    
    // Spawn settings
    [Header("Spawn Settings")]
    public float spawnInterval = 5f; // Time between spawns (seconds)
    public float spawnLineZ = -12f; // Z position of spawn line (below screen)
    public float spawnLineMinX = -9f; // Leftmost point of spawn line
    public float spawnLineMaxX = 9f; // Rightmost point of spawn line
    public float spawnHeight = 0.5f; // Y position of spawn
    public int maxBallsInPlay = 5; // Maximum number of friendly balls at once
    
    private float nextSpawnTime; // When next ball will spawn
    private int currentBallCount = 0; // How many balls are currently in play
    
    void Start()
    {
        // Schedule first spawn
        nextSpawnTime = Time.time + spawnInterval;
    }
    
    void Update()
    {
        // Check if it's time to spawn and we're under the limit
        if (Time.time >= nextSpawnTime && currentBallCount < maxBallsInPlay)
        {
            SpawnFriendlyBall();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    // Spawn a friendly ball
    void SpawnFriendlyBall()
    {
        if (FriendlyBall == null)
        {
            Debug.LogError("FriendlyBall prefab not assigned!");
            return;
        }
        
        // Pick random point along spawn line
        float randomX = Random.Range(spawnLineMinX, spawnLineMaxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, spawnLineZ);
        
        // Create ball at random spawn position
        GameObject ball = Instantiate(FriendlyBall, spawnPosition, FriendlyBall.transform.rotation);
        
        // Get ball controller and initialize it
        DodgeballFriendlyBall ballController = ball.GetComponent<DodgeballFriendlyBall>();
        if (ballController != null)
        {
            // Pass references
            ballController.playerController = playerController;
            ballController.player = player;
            ballController.enemyManager = enemyManager;
            
            // Initialize flight from spawn position
            ballController.Initialize(spawnPosition);
            
            // Track ball count
            currentBallCount++;
            
            // Decrement count when ball is destroyed
            StartCoroutine(TrackBallDestruction(ball));
        }
        else
        {
            Debug.LogError("DodgeballFriendlyBall script not found on prefab!");
        }
    }
    
    // Track when ball is destroyed to decrement count
    System.Collections.IEnumerator TrackBallDestruction(GameObject ball)
    {
        // Wait until ball is destroyed
        while (ball != null)
        {
            yield return null;
        }
        
        // Ball was destroyed, decrement count
        currentBallCount--;
    }
    
    // Visual debug in editor
    void OnDrawGizmosSelected()
    {
        // Draw spawn line
        Gizmos.color = Color.cyan;
        Vector3 leftPoint = new Vector3(spawnLineMinX, spawnHeight, spawnLineZ);
        Vector3 rightPoint = new Vector3(spawnLineMaxX, spawnHeight, spawnLineZ);
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.DrawWireSphere(leftPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPoint, 0.3f);
    }
}