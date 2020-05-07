#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using vnc.Development;
using vnc.Samples;

namespace vnc.Development
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