using UnityEngine;

public class McRoomDoor : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        // Add interaction logic here
    }
}
