using epiplon.Movements;
using UnityEditor;

namespace epiplon.Editor
{
    [CustomEditor(typeof(RetroGrapple))]
	public class RetroGrappleEditor : UnityEditor.Editor
	{
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Experimental feature.\nSome behaviors might not work properly and are subject to change.", MessageType.Warning);
            base.OnInspectorGUI();
        }
    }
}
