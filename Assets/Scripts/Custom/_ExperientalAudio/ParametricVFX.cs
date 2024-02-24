using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ParametricVFX : MonoBehaviour
{
    public int band;
    public float startScale, maxScale;
    public bool useBuffer;
    
    [SerializeField]
    private VisualEffect visualEffect;

    private float intensity = 1f;
    private float colorswitch = 1f;
    

    void Update() 
    {
        intensity = (AudioPeer.audioBandBuffer[band]*maxScale) + startScale;
        visualEffect.SetFloat("Turbulence Intensity", intensity);

        colorswitch = (AudioPeer.audioBandBuffer[band]*maxScale) + startScale;

        if (colorswitch < 3f)
        {
            visualEffect.SetFloat("Color Switch", 0f);
        }
        else
        {
            visualEffect.SetFloat("Color Switch", 1f);
        }
    }
    
}
