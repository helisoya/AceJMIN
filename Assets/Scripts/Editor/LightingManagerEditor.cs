using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightingManager))]
public class LightingManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        if (GUILayout.Button("Load default data"))
        {
            target.GetComponent<LightingManager>().SetDataToDefault();
        }
    }
}
