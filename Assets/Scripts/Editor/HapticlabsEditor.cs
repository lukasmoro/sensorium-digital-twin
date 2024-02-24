using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HapticlabsManager))]
public class HapticlabsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        HapticlabsManager hapticlabs = (HapticlabsManager)target;

        if (GUILayout.Button("Send to Hapticlabs"))
        {
            hapticlabs.SendMessageToHapticlabs();
        }
    }
}
