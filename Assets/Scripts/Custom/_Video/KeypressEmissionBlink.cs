using UnityEngine;
using System.Collections;

public class KeypressEmissionBlink : MonoBehaviour
{
    private new Renderer renderer;
    private Material material; 
    public Color baseColor = Color.white;
    public Color targetColor = Color.red;
    public float blinkSpeed = 1f;
    private float currentLerpTime = 0f;
    private bool shouldBlink = false;
    private bool isWaitingToStartBlink = false;

    private void Start()
    {
        if (TryGetComponent<Renderer>(out renderer))
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
        if (Input.GetKeyDown(KeyCode.S) && !isWaitingToStartBlink)
        {
            StartCoroutine(DelayedBlinkStart(10f));
        }

        if (material != null && shouldBlink)
        {
            BlinkLogic();
        }
    }

    private IEnumerator DelayedBlinkStart(float delay)
    {
        isWaitingToStartBlink = true;
        yield return new WaitForSeconds(delay);
        shouldBlink = !shouldBlink;
        isWaitingToStartBlink = false;
    }

    private void BlinkLogic()
    {
        Color newColor = EaseInOutLerp(baseColor, targetColor, currentLerpTime);
        material.SetColor("_EmissionColor", newColor);
        currentLerpTime += Time.deltaTime * blinkSpeed;

        if (currentLerpTime >= 1f)
        {
            currentLerpTime = 0f;
            (targetColor, baseColor) = (baseColor, targetColor);
        }
    }

    private Color EaseInOutLerp(Color start, Color end, float time)
    {
        return Color.Lerp(start, end, Mathf.SmoothStep(0f, 1f, time));
    }
}
