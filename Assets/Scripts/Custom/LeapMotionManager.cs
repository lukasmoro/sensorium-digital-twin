using UnityEngine;

public class LeapMotionManager : MonoBehaviour
{   
    public bool grabDetected = false;
    public bool releaseDetected = false;

    //Function calls on point pose
    public void OnPointPoseDetected()
    {
        Debug.Log("Point pose detected!");
    }
    
    //Function calls on grab pose
    public void OnGrabPoseDetected()
    {
        Debug.Log("Grab pose detected!");
        grabDetected = true;
    }

    //Function calls on release pose
    public void OnReleasePoseDetected()
    {
        Debug.Log("Release pose detected!");
        releaseDetected = true;
    }
}
