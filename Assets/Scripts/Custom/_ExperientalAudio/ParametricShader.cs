using UnityEngine;

public class ParametricShader : MonoBehaviour
{
    public int band;
    public float startScale, maxScale;
    public bool useBuffer;
    Material material;

    void Start() 
    {
        material = GetComponent<MeshRenderer>().materials[0];

    }

    void Update()
    {   
        if (useBuffer)
        {
        // transform.localScale = new Vector3((AudioPeer.audioBandBuffer[band]*maxScale) + startScale, (AudioPeer.audioBandBuffer[band]*maxScale) + startScale, (AudioPeer.audioBandBuffer[band]*maxScale) + startScale);
        Color color = new Color (1, AudioPeer.audioBandBuffer[band], 0.3f);
        material.SetColor("_EmissiveColor", color);
        }

        if(!useBuffer)
        {
        // transform.localScale = new Vector3((AudioPeer.audioBandBuffer[band]*maxScale) + startScale, (AudioPeer.audioBandBuffer[band]*maxScale) + startScale, (AudioPeer.audioBandBuffer[band]*maxScale) + startScale);
        Color color = new Color (1, AudioPeer.audioBandBuffer[band], 0.5f);
        material.SetColor("_EmissiveColor", color);
        }
    }

}
