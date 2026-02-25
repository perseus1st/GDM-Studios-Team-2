using UnityEngine;

/// <summary>
///  Attached to every note instance, controls note movement and deletion if missed 
/// </summary>
public class NoteMover : MonoBehaviour
{
    public static float MaxGap = 3.5f;
    public static float Speed = 6.7f; // Units per second
    //1.79 sec to travel 12 units s = d/t
    public static float TargetY = -6f; // Y position of the hitzone
    
    public float targetTime; 

    public int lane; 
    public int beat; 
    public bool isLongNote; 
    public int lenghtInBeats; 
    public bool isHolding = false; 
    public Conductor conductor; 
    public DDR_ScoreManager scoreManager; 

    void Update()
    {
        // Move downward
        transform.position += Vector3.down * Speed * Time.deltaTime;

        // Destroy note if it goes past the hitzone
        if (transform.position.y < TargetY - MaxGap)
        {
            switch (lane)
            {
                case 0 :
                    conductor.AactiveNotes.Dequeue();
                    break;
                case 1 : 
                    conductor.WactiveNotes.Dequeue();
                    break; 
                case 2 : 
                    conductor.SactiveNotes.Dequeue();
                    break;
                case 3 : 
                    conductor.DactiveNotes.Dequeue();
                    break;
                default:
                    break; 
            }
            
            // tell scoreManager to lose life 
            conductor.PastHitzone(); 

            Destroy(gameObject);
        } 
    }

    public static float getSpeed()
    {
        return Speed; 
    }
}
