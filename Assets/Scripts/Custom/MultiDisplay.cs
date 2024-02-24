using UnityEngine;
using System.Collections.Generic;

public class MultiDisplay : MonoBehaviour
{   
    [SerializeField] 
    private Camera[] cameras;

    private Dictionary<Vector2Int, int> resolutionToCameraIndex;

    void Start ()
    {   
        Debug.Log ("Displays connected: " + Display.displays.Length);
        
        resolutionToCameraIndex = new Dictionary<Vector2Int, int>()
        {
            // Fascia
            { new Vector2Int(1280, 480), 0 },
            { new Vector2Int(1920, 720), 0 },
            { new Vector2Int(1920, 1200), 0 },
            { new Vector2Int(3440, 1440), 0 },
            { new Vector2Int(1920, 1080), 0 },
            { new Vector2Int(2880, 864), 0 },

            // Treadplate
            { new Vector2Int(1440, 240), 1 },
                
            // Projector
            { new Vector2Int(864, 480), 2 },
            { new Vector2Int(1280, 800), 2 },

            // Toughbook
            { new Vector2Int(1366, 768), 3 }
        };

        for (int i = 0; i < Display.displays.Length; i++)
        {   
            Display.displays[i].Activate();

            int matchingCameraIndex;
            Vector2Int resolution = new Vector2Int(Display.displays[i].systemWidth, Display.displays[i].systemHeight);

            if (resolutionToCameraIndex.TryGetValue(resolution, out matchingCameraIndex))
            {
                Debug.Log("Matched Camera: " + matchingCameraIndex + " with Display: " + i + " Resolution: " + resolution.x + " x " + resolution.y);
                cameras[matchingCameraIndex].targetDisplay = i;
            }
            
            else
            {
                Debug.Log("Unmatched Display: " + i);
                if (i == 0)
                {
                    cameras[0].targetDisplay = 0;
                }
            }
        }
    }   
} 