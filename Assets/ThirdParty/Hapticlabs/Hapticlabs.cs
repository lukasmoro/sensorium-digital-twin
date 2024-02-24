using UnityEngine;
using UnityEditor;

public class Hapticlabs : MonoBehaviour
{
    public bool logDebugInfos = true;
    public bool enableTCP = true;
    public bool enableSerial = false;
    public static bool useTCP = false;
    public static bool useSerial = false;
    private static bool h_debug;

    void Update(){
        if(h_debug != logDebugInfos){
            h_debug = logDebugInfos;
        }
        if (useSerial != enableSerial){
            useSerial = enableSerial;
            if (useSerial){
                Serial.EnableOperation();
            } else {
                Serial.DisableOperation();
            }
        }
        if (useTCP != enableTCP){
            useTCP = enableTCP;
            if (useTCP){
                Debug.Log("enabled");
                TCPClient.EnableOperation();
            } else {
                TCPClient.DisableOperation();
            }
        }
    }

    // Example: StartTrack("trackName");    --> trackName needs to be loaded on the Satellite from Hapticlabs Studio!
    public static void StartTrack(string trackName, bool queue = false, float amplitudeScale = 1.0f){
        string message = (!queue ? "stop();\n" : "") + "startTrack(\"" + trackName + "\" " + amplitudeScale + ");";
        if(h_debug){Debug.Log(message);}
        WriteToSatellite(message);
    }

    public static void Stop(){
        const string message = "stop();";
        if(h_debug){Debug.Log(message);}
        WriteToSatellite(message);
    }

    public static void SetAmplitudeScale(float amplitudeScale){
        string message = "setAmplitudeScale(" + amplitudeScale + ");";
        if(h_debug){Debug.Log(message);}
        WriteToSatellite(message);
    }

    private static void WriteToSatellite(string message){
        if(useTCP && TCPClient.IsConnected()){
            TCPClient.WriteLn(message);
        }
        if (useSerial && Serial.checkOpen()){
            Serial.Write(message);
        }
    }

}

// Adding the test button to the inspector GUI
[CustomEditor(typeof(Hapticlabs))]
public class HapticlabsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // var hapticlabs = target as Hapticlabs;
        if (GUILayout.Button("Test Serial connection with satellite"))
        {
            Serial.Write("a(\"s()disableLoop()\")b(\"s()disableLoop()\");a(\"v(1 120 100000)\")b(\"v(1 120 100000)\");");
            Debug.Log("Test message: a(\"s()disableLoop()\")b(\"s()disableLoop()\");a(\"v(1 120 100000)\")b(\"v(1 120 100000)\");");
        }
        if (GUILayout.Button("Test TCP connection with Hapticlabs Studio"))
        {
            Debug.Log(TCPClient.IsConnected());
            TCPClient.WriteLn("stop();startTrack(\"\");");
            Debug.Log("Test message: stop();startTrack(\"\");");
        }
        // if (GUILayout.Button("Disconnect satellite"))
        // {
        //     Serial.Close();
        // }      
    }
}
