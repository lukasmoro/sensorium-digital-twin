using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(ParticleSystem))]
public class SpatialAudioEmission : MonoBehaviour
{	
	//Define reference for particle system
    private ParticleSystem parentParticleSystem;

	// Dictionary to track the particles in the particle system
	// The dictionary uses the randomSeed of each particle as the key and the corresponding ParticleSystem.Particle as the value
	// This allows efficient tracking and lookup of particles based on their unique identifier (randomSeed)
	private IDictionary<uint, ParticleSystem.Particle> trackedParticles = new Dictionary<uint, ParticleSystem.Particle>();
    
	// Array of audio clips for particle birth
    public AudioClip[] birthSound;

	// Array of audio clips for particle death 
    public AudioClip[] deathSound; 

    void Start()
    {	
		// Get the ParticleSystem component attached to the game object
        parentParticleSystem = this.GetComponent<ParticleSystem>(); 
    }
    
    void Update()
    {
        var liveParticles = new ParticleSystem.Particle[parentParticleSystem.particleCount];

		// Get the array of live particles from the ParticleSystem component
        parentParticleSystem.GetParticles(liveParticles); 
        
		// Calculate the added and removed particles since the last frame
        var particleDelta = GetParticleDelta(liveParticles);
    
        foreach (var particleAdded in particleDelta.Added)
        {	
			// Play a random birth sound at the position of the added particle
            AudioSource.PlayClipAtPoint(birthSound[Random.Range(0, birthSound.Length)], particleAdded.position); 

			// Output debug information
            Debug.Log("Start at:" + particleAdded.position); 
        }
    
        foreach (var particleRemoved in particleDelta.Removed)
        {	
			// Play a random death sound at the position of the removed particle
            AudioSource.PlayClipAtPoint(deathSound[Random.Range(0, deathSound.Length)], particleRemoved.position); 

			// Output debug information
            Debug.Log("Death at:" + particleRemoved.position); 
        }
    }
    
    private ParticleDelta GetParticleDelta(ParticleSystem.Particle[] liveParticles)
    {
        var deltaResult = new ParticleDelta();
    
        foreach (var activeParticle in liveParticles)
        {
            ParticleSystem.Particle foundParticle;

            if (trackedParticles.TryGetValue(activeParticle.randomSeed, out foundParticle))
            {	
				// Update the tracked particle's position
                trackedParticles[activeParticle.randomSeed] = activeParticle; 
            }
            else
            {	
				// Add the particle to the added list
                deltaResult.Added.Add(activeParticle); 

				// Add the particle to the tracked particles dictionary
                trackedParticles.Add(activeParticle.randomSeed, activeParticle); 
            }
        }
    
        var updatedParticleAsDictionary = liveParticles.ToDictionary(x => x.randomSeed, x => x);
        var dictionaryKeysAsList = trackedParticles.Keys.ToList();
    
        foreach (var dictionaryKey in dictionaryKeysAsList)
        {
            if (updatedParticleAsDictionary.ContainsKey(dictionaryKey) == false)
            {	
				// Add the particle to the removed list
                deltaResult.Removed.Add(trackedParticles[dictionaryKey]); 

				// Remove the particle from the tracked particles dictionary
                trackedParticles.Remove(dictionaryKey); 
            }
        } 

		// Return the particle delta result   
        return deltaResult; 
    }
    
    private class ParticleDelta
    {	
		// List of added particles
        public IList<ParticleSystem.Particle> Added { get; set; } = new List<ParticleSystem.Particle>(); 

		// List of removed particles
        public IList<ParticleSystem.Particle> Removed { get; set; } = new List<ParticleSystem.Particle>(); 
    }
}
