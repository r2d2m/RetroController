#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace epiplon.Development
{
    [CustomEditor(typeof(SamplePlayerDebugger))]
    public class SamplePlayerEditor : UnityEditor.Editor
    {
        SamplePlayerDebugger samplePlayer;

        public override void OnInspectorGUI()
        {
            samplePlayer = ((SamplePlayerDebugger)target);
            GUILayout.Space(10);
            base.OnInspectorGUI();
        }
    }
}
#endif