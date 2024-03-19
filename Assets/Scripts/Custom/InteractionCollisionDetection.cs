using UnityEngine;

public class InteractionCollisionDetection : MonoBehaviour
{
    public bool collisionDetected = false;
    public Renderer objectRenderer;
    private Color originalFresnelColor;
    private Color originalMainColor;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalFresnelColor = objectRenderer.material.GetColor("Color_6C516763");
        originalMainColor = objectRenderer.material.GetColor("Color_694FDA6");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            collisionDetected = true;
            Color hdrPurple = new(2f, 0f, 2f, 1f);
            objectRenderer.material.SetColor("Color_6C516763", hdrPurple);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            collisionDetected = false;
            objectRenderer.material.SetColor("Color_6C516763", originalFresnelColor);
            objectRenderer.material.SetColor("Color_694FDA6", originalMainColor);
        }
    }
}
