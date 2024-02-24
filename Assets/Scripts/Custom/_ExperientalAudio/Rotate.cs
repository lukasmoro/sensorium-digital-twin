using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 30.0f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
