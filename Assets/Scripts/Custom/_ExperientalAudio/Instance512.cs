using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instance512 : MonoBehaviour
{
    public GameObject sampleGeometryPrefab;
    GameObject[] sampleGeometry = new GameObject[512];
    public float maxScale;

    void Start()
    {
        // Load a geometry instance for 512 samples
        for(int i = 0; i < 512; i++)
        {
            GameObject instanceSampleGeometry = (GameObject)Instantiate(sampleGeometryPrefab);
            instanceSampleGeometry.transform.position = this.transform.position;
            instanceSampleGeometry.transform.parent = this.transform;
            instanceSampleGeometry.name = "Sample" + i;
            this.transform.eulerAngles = new Vector3 (0, -0.703125f * i, 0);
            instanceSampleGeometry.transform.position = Vector3.forward * 100;
            sampleGeometry[i] = instanceSampleGeometry;
        }
    }

    void Update()
    {   
        // Update transformation (localScale) on every frame according to AudioPeer.samples[i]*maxScale
        for(int i = 0; i < 512; i++)
        {
            if(sampleGeometry != null)
            {
                sampleGeometry[i].transform.localScale = new Vector3(1,(AudioPeer.samples[i]*maxScale) + 2,1);
            }
        }
    }
}
