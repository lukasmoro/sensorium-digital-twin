using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class KeyEventsRefik : MonoBehaviour
{
   [SerializeField]
   AudioClip[] clips;

    void Start()
    {
        GetComponent<AudioSource>().clip = clips[0];
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            
            int currentClipIndex = -1;

            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] == audioSource.clip)
                {
                    currentClipIndex = i;
                    break;
                }
            }

            if(currentClipIndex < clips.Length - 1)
            {
                audioSource.clip = clips[currentClipIndex + 1];
            }

            else
            {
                audioSource.clip = clips[0];
            }

            audioSource.Play();
        }        
    }
}
