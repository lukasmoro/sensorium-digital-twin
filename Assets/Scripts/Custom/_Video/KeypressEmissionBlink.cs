using UnityEngine;

public class KeypressEmissionBlink : MonoBehaviour
{
    private new Renderer renderer;
    private Material material; 
    public Color baseColor = Color.white;
    public Color targetColor = Color.red;
    public float blinkSpeed = 1f;
    private float currentLerpTime = 0f;
    private bool shouldBlink = false; 

    private void Start()
    {
        renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("EmissionBlink script requires a Renderer component!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shouldBlink = !shouldBlink;
        }

        if (material != null && shouldBlink)
        {
            Color newColor = EaseInOutLerp(baseColor, targetColor, currentLerpTime);

            material.SetColor("_EmissionColor", newColor);

            currentLerpTime += Time.deltaTime * blinkSpeed;

            if (currentLerpTime >= 1f)
            {
                currentLerpTime = 0f;
                Color tempColor = baseColor;
                baseColor = targetColor;
                targetColor = tempColor;
            }
        }
    }

    private Color EaseInOutLerp(Color start, Color end, float time)
    {
        return Color.Lerp(start, end, Mathf.SmoothStep(0f, 1f, time));
    }
}
