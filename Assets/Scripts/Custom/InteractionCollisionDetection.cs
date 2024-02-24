using UnityEngine;

public class InteractionCollisionDetection : MonoBehaviour
{
    public bool collisionDetected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            collisionDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            collisionDetected = false;
        }
    }
}
