using UnityEditor;

namespace vnc.Editor.Utils
{
    // Reimport the necessary tags
    public class ReimportTags : EditorWindow
    {
        [MenuItem("Window/Retro Controller/Reimport Tags")]
        public static void OpenWindow()
        {
            bool hasPlatform = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Platform");
            bool hasWater = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Water");
            bool hasLadder = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Ladder");

            if(hasPlatform && hasWater && hasLadder)
            {
                EditorUtility.DisplayDialog("Retro Controller", "All tags are already on the project.", "OK");
                return;
            }

            if (!hasPlatform) UnityEditorInternal.InternalEditorUtility.AddTag("Platform");
            if (!hasWater) UnityEditorInternal.InternalEditorUtility.AddTag("Water");
            if (!hasLadder) UnityEditorInternal.InternalEditorUtility.AddTag("Ladder");

            EditorUtility.DisplayDialog("Retro Controller", "Tags successfully imported!", "OK");
        }
    }
}