using UnityEngine;
using System.Collections;

public class KeypressEmissionBlinkAnimatorTrigger : MonoBehaviour
{
    private new Renderer renderer;
    private Material material;
    public Animator animatorVirtualUI;
    public Color baseColor = Color.white;
    public Color targetColor = Color.red;
    public float blinkSpeed = 1f;
    private float currentLerpTime = 0f;
    private bool blinkInProgress = false; 

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
        if (Input.GetKeyDown(KeyCode.Space) && !blinkInProgress)
        {
            StartCoroutine(BlinkAfterDelay(5f));
        }
    }

    private IEnumerator BlinkAfterDelay(float delay)
    {
        blinkInProgress = true;
        yield return new WaitForSeconds(delay);

        animatorVirtualUI.SetBool("Open Fade In", true);

        while (currentLerpTime <= 2f)
        {
            if (currentLerpTime <= 1f)
            {
                Color newColor = EaseInOutLerp(baseColor, targetColor, currentLerpTime);
                material.SetColor("_EmissionColor", newColor);
            }
            else
            {
                float adjustedTime = currentLerpTime - 1f;
                Color newColor = EaseInOutLerp(targetColor, baseColor, adjustedTime);
                material.SetColor("_EmissionColor", newColor);
            }
            
            currentLerpTime += Time.deltaTime * blinkSpeed;
            yield return null;
        }

        blinkInProgress = false;
        currentLerpTime = 0f;
    }

    private Color EaseInOutLerp(Color start, Color end, float time)
    {
        return Color.Lerp(start, end, Mathf.SmoothStep(0f, 1f, time));
    }
}
