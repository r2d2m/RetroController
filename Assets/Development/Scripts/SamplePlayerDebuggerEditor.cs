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

            if (samplePlayer.isPlaying)
            {
                if (GUILayout.Button("Stop Playing"))
                {
                    samplePlayer.StopPlaying();
                }
            }
            else
            {
                if (samplePlayer.isRecording)
                {
                    if (GUILayout.Button("Stop Recording"))
                    {
                        samplePlayer.StopRecording();
                    }
                }
                else
                {
                    if (GUILayout.Button("Record Input"))
                    {
                        samplePlayer.StartRecording();
                    }
                }
            }

            if (!samplePlayer.isRecording && samplePlayer.logCount > 0)
            {
                if (GUILayout.Button("Play Record"))
                {
                    samplePlayer.StartPlaying();
                }
            }

            if (samplePlayer.logCount > 0)
            {
                GUILayout.Label(string.Format("{0} logs recorded.", samplePlayer.logCount));
            }

            GUILayout.Space(10);
            base.OnInspectorGUI();
        }

    }

}
#endif