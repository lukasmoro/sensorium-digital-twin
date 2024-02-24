using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarlightAnimator : MonoBehaviour
{   
    [SerializeField]
    GameObject fascia;
    
    [SerializeField]
    ParticleSystem starlight;

    [SerializeField]
    float minLifetime = 0.2f;

    [SerializeField]
    float maxLifetime = 0.8f;

    [SerializeField]
    float minSize = 0.01f;

    [SerializeField]
    float maxSize = 1f;

    [SerializeField]
    GameObject animatedObject;

    private float maxDistance = 2f;
    private Transform[] starPositions;
    private ParticleSystem.Particle[] starArray;

    void Start()
    {   
        starPositions = new Transform[fascia.transform.childCount];
        starArray = new ParticleSystem.Particle[starPositions.Length];

        for (int i = 0; i < fascia.transform.childCount; i++)
        {
            Transform starTransform = fascia.transform.GetChild(i);
            starPositions[i] = starTransform;
        }

        starlight.Emit(starPositions.Length);
        starlight.GetParticles(starArray);

        for (int j = 0; j < starPositions.Length; j++)
        {
            starArray[j].position = starPositions[j].position;
            starArray[j].startSize = maxSize;
        }

        var size = starlight.sizeOverLifetime;
        size.enabled = true;

        var color = starlight.colorOverLifetime;
        color.enabled = true;
        
        starlight.SetParticles(starArray, starArray.Length);
        StartCoroutine(BlinkCoroutine());
    }

    // private IEnumerator BlinkCoroutine()
    // {
    //     float time = 0;

    //     while (true)
    //     {
    //         Collider collider = animatedObject.GetComponent<Collider>();
    //         maxDistance = ((SphereCollider)collider).radius;


    //         for (int i = 0; i < starArray.Length; i++)
    //         {
    //             float distance = Vector3.Distance(starArray[i].position, animatedObject.transform.position);

    //             if (distance <= maxDistance)
    //             {
    //                 float proximity = 1 - (distance / maxDistance);
    //                 float red = 1f;
    //                 float green = 1f;
    //                 float blue = 1f;
    //                 Color color = new Color(red, green, blue, proximity);
    //                 starArray[i].startColor = color;
    //                 starArray[i].startSize = 1f;
    //             }

    //             if (distance > maxDistance)
    //             {
    //                 float proximity = 1 - (distance / maxDistance);
    //                 float red = 0f;
    //                 float green = 0f;
    //                 float blue = 0f;
    //                 Color color = new Color(red, green, blue, proximity);
    //                 starArray[i].startColor = color;
    //                 starArray[i].startSize = 1f;
    //             }

    //             starArray[i].startLifetime = Mathf.Lerp(minLifetime, maxLifetime, time);

    //         }

    //         time += Time.deltaTime * 0.1f;
    //         starlight.SetParticles(starArray, starArray.Length);
    //         yield return null;
    //     }
    // }

    private IEnumerator BlinkCoroutine()
    {
        float time = 0;
        
        while (true)
        {
            Collider collider = animatedObject.GetComponent<Collider>();
            
            if (collider.GetType() == typeof(SphereCollider))
            {
                maxDistance = ((SphereCollider)collider).radius;
            }

            // add more collider shapes here

            for (int i = 0; i < starArray.Length; i++)
            {
                float distance = Vector3.Distance(starArray[i].position, animatedObject.transform.position);
                float size = Mathf.Lerp(minSize, maxSize, time);
                
                if (distance <= maxDistance)
                {   
                    if (collider.GetType() == typeof(SphereCollider))
                    {
                        float proximity = 1 - (distance / maxDistance);
                        size = Random.Range(1f, 1.2f) * size * Mathf.Lerp(1f, 2f, proximity);
                    }

                    // add more collider shape prox. calculations here

                }

                if (distance > maxDistance)
                {
                   size =  1f;
                }

                starArray[i].startLifetime = Mathf.Lerp(minLifetime, maxLifetime, time);
                starArray[i].startSize = size;
            }

            time += Time.deltaTime * 0.1f;
            starlight.SetParticles(starArray, starArray.Length);
            yield return null;
        }
    }
}