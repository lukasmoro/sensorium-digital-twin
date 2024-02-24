using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEventsFireworks : MonoBehaviour
{
    // Firework Start Particle System
    [SerializeField]
    ParticleSystem firework1;
    
    [SerializeField]
    ParticleSystem firework2;

    void Update()
    {   
        // Activate & Deactivate Firework Wind 
        if (Input.GetKeyDown(KeyCode.A))
        {   
            firework1.Play();
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            firework1.Stop();
        }

        // Activate & Deactivate Firework Wind 
        if (Input.GetKeyDown(KeyCode.S))
        {   
            firework2.Play();
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            firework2.Stop();
        }

    }
}