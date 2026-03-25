using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
///  Attached to every note instance, controls note movement and deletion if missed 
/// </summary>
public class NoteMover : MonoBehaviour
{
    public static float MaxGap = 2.5f;
    public static float Speed = 7f; // Units per second
    //1.79 sec to travel 12 units s = d/t
    public static float TargetY = -2.5f; // Y position of the hitzone
    
    public float targetTime; 

    public int lane; 
    public int beat; 
    public Conductor conductor; 
    public SisterAnimator sisterAnimator; 
    public float detectionRange = 0.05f; 
    public bool wasSisterMoved = false; 

  void Update()
    {
        // Move downward
        transform.position += Vector3.down * Speed * Time.deltaTime;

        //Call sister animator 
        if (Math.Abs(transform.position.y - TargetY) <= detectionRange && !wasSisterMoved)
        {
            switch (lane)
            {
                case 0 :
                    Debug.Log("animator called left"); 
                    sisterAnimator.triggerLeft(); 
                    break;
                case 1 : 
                    Debug.Log("animator called up"); 
                    sisterAnimator.triggerUp(); 
                    break; 
                case 2 : 
                    Debug.Log("animator called dpwn"); 
                    sisterAnimator.triggerDown(); 
                    break;
                case 3 : 
                    Debug.Log("animator called right"); 
                    sisterAnimator.triggerRight(); 
                    break;
                default:
                    break; 
            }

            wasSisterMoved = true; 
        }

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

            conductor.PastHitzone(); 
            Destroy(gameObject);
        } 
    }

    public static float getSpeed()
    {
        return Speed; 
    }

    public void setSisterMoved(bool state)
    {
        this.wasSisterMoved = state; 
    }
}
