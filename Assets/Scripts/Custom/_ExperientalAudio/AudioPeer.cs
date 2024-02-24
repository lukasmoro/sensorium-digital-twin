using UnityEngine;

// Require AudioSource from Unity
[RequireComponent (typeof(AudioSource))]

public class AudioPeer : MonoBehaviour
{
   AudioSource audioSource;

   // Narrows down 20000Hz into 512 samples
   public static float[] samples = new float[512];
   
   // Defines array position of 8 frequency bands
   public static float[] frequencyBand = new float[8];

   // Floats for buffering
   public static float[] bandBuffer = new float[8];
   float[] bufferDecrease = new float[8];

   // Floats for filtering
   float[] frequencyBandHighest = new float[8];
   public static float[] audioBand = new float[8];
   public static float[] audioBandBuffer = new float[8];

   // Floats for average amplitude & buffer
   public static float amplitude, amplitudeBuffer;
   float amplitudeHighest;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Listen to audio source every frame & getting all samples from spectrum data into array
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        AddBandBuffer();
        CreateAudioBands();
        GetAmplitude();
    }

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    void CreateAudioBands()
    {
        for (int h = 0; h < 8; ++h)
        {
            if(frequencyBand[h] > frequencyBandHighest[h])
            {
                frequencyBandHighest[h] = frequencyBand[h];
            }
            audioBand[h] = (frequencyBand[h] / frequencyBandHighest[h]);
            audioBandBuffer[h] = (bandBuffer[h] / frequencyBandHighest[h]);
        }

    }

    void GetAmplitude()
    {
        float currentAmplitude = 0;
        float currentAmplitudeBuffer = 0;

        for (int i = 0; i < 8; i++)
        {  
            currentAmplitude += audioBand[i];
            currentAmplitudeBuffer += audioBandBuffer[i];

            if(currentAmplitude > amplitudeHighest)
            {
                amplitudeHighest = currentAmplitude;
            }
            
            amplitude = currentAmplitude / amplitudeHighest;
            amplitudeBuffer = currentAmplitudeBuffer / amplitudeHighest;
        }

    }

    void MakeFrequencyBands()
    {   
        // Counting current sample
        int count = 0;

        for (int i = 0; i < 8; i++)
        {   
            // Sets average back to 0, if new sample area is reached
            float average = 0;

            // Calculating the end of the current sample area
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                // Cleaner to consider the last two samples (510 -> 512)
                sampleCount += 2;
            }

            //Calculate current average
            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1);
                count ++; 
            }

            average /= count;
            frequencyBand[i] = average * 10;
        }
    }

    void AddBandBuffer()
    {
         for (int g = 0; g < 8; g++)
        {   
            // Determines if the frequencyband is higher or lower than the bandbuffer. If the frequencyband is higher than the bandbuffer replaces the frequencyband. If it is lower the bandbuffer decreases.
            if (frequencyBand[g] > bandBuffer[g])
            {
                bandBuffer[g] = frequencyBand[g];
                // Initial buffer decrease value
                bufferDecrease[g] = 0.005f;
            }

            if (frequencyBand[g] < bandBuffer[g])
            {
                bandBuffer[g] -= bufferDecrease[g];
                //Rises by 20% if statement is active
                bufferDecrease[g] *= 1.2f;
            }
        }

    }
}
