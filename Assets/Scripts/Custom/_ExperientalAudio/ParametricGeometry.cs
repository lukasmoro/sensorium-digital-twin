using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametricGeometry : MonoBehaviour
{
    public int band;
    public float startScale, scaleMultiplier;
    public bool useBuffer;

    void Update()
    {   
        if (useBuffer)
        {
        transform.localScale = new Vector3((AudioPeer.bandBuffer[band]*scaleMultiplier) + startScale, (AudioPeer.bandBuffer[band]*scaleMultiplier) + startScale, (AudioPeer.bandBuffer[band]*scaleMultiplier) + startScale);
        }

        if(!useBuffer)
         {
        transform.localScale = new Vector3((AudioPeer.bandBuffer[band]*scaleMultiplier) + startScale, (AudioPeer.frequencyBand[band]*scaleMultiplier) + startScale, (AudioPeer.bandBuffer[band]*scaleMultiplier) + startScale);
        }
    }
}
