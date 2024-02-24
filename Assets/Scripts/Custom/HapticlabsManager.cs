using UnityEngine;

public class HapticlabsManager : MonoBehaviour
{
    public string[] trackNames;
    public bool queue = false;
    public float amplitudeScale = 1.0f;
    private string currentTrackName = "1";
    private ContentManager contentManager;

    void Start()
    {   
        contentManager = FindObjectOfType<ContentManager>();
    }

    // Setting current track through events

    // approaching far
    public void SetTrackNameElement0()
    {
        SetCurrentTrack(trackNames[0]);
    }

    // approaching close
    public void SetTrackNameElement1()
    {
        SetCurrentTrack(trackNames[1]);
    }

    // data in hand
    public void SetTrackNameElement2()
    {
        SetCurrentTrack(trackNames[2]);
    }

    // steering at peak levels
    public void SetTrackNameElement3()
    {
        SetCurrentTrack(trackNames[3]);
    }

    // heartbeat (build up)
    public void SetTrackNameElement4()
    {
        SetCurrentTrack(trackNames[4]);
    }
    // heartbeat (confirmation)
    public void SetTrackNameElement5()
    {
        SetCurrentTrack(trackNames[5]);
    }

    public void SetCurrentTrack(string trackName)
    {
        if (System.Array.Exists(trackNames, track => track == trackName))
        {
            currentTrackName = trackName;
            Debug.Log($"Track set to '{trackName}'");
        }
        else
        {
            Debug.LogError($"Track '{trackName}' not found in trackNames array.");
        }
    }

    public void SendMessageToHapticlabs()
    {
        if (string.IsNullOrEmpty(currentTrackName))
        {
            Debug.LogError("No track set. Use SetCurrentTrack() method to set a track.");
            return;
        }

        string message = (!queue ? "stop();\n" : "") + "startTrack(\"" + currentTrackName + "\" " + amplitudeScale + ");";
        SerialCommunication.SendSerialMessageHapticlabs(message);
    }
}
