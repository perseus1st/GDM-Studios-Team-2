using UnityEngine;

public class NoteMover : MonoBehaviour
{
    public float MAXGAP = 3.5f;
    public static float speed = 6.7f; // Units per second
    //1.79 sec to travel 12 units s = d/t
    public static float targetY = -6f; // Y position of the hitzone
    
    public bool active; 

    public int lane; 
    public int beat; 
    public Conductor conductor; 

    void Update()
    {
        // Move downward
        transform.position += Vector3.down * speed * Time.deltaTime;

        // Destroy note if it goes past the hitzone
        if (transform.position.y < targetY - MAXGAP)
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

            Destroy(gameObject);
        } 
    }
}
