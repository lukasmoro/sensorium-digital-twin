using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametricGeometryAmplitude : MonoBehaviour
{
    public int band;
    public float startScale, scaleMultiplier;
    public bool useBuffer;

    void Update()
    {   
        if (!useBuffer)
        {
        transform.localScale = new Vector3((AudioPeer.amplitude*scaleMultiplier) + startScale, (AudioPeer.amplitude*scaleMultiplier) + startScale, (AudioPeer.amplitude*scaleMultiplier) + startScale);
        }

        if(useBuffer)
         {
        transform.localScale = new Vector3((AudioPeer.amplitudeBuffer*scaleMultiplier) + startScale, (AudioPeer.amplitudeBuffer*scaleMultiplier) + startScale, (AudioPeer.amplitudeBuffer*scaleMultiplier) + startScale);
        }
    }
}
