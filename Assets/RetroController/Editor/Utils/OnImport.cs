using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace vnc.Editor.Utils
{
    public class OnImport : AssetPostprocessor
    {
        static string AssetKey = "RetroController";

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 0)
                return;
            
            if (!PlayerPrefs.HasKey(AssetKey))
            {
                bool hasPlatform = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Platform");
                bool hasWater = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Water");
                bool hasLadder = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Ladder");

                string message = string.Format("For the Sample Scene to work with all the RetroController " +
                    "functionalities, the package needs to add the following Tags:");

                if (!hasPlatform) message += "\n Platform";
                if (!hasWater) message += "\n Water";
                if (!hasLadder) message += "\n Ladder";

                message += "\n\n You can add them later but it's highly recommended that you do this" +
                    "process automatically. " +
                    "\nDo you wish to proceed?";

                if (!hasPlatform || !hasWater || !hasLadder)
                {
                    if (EditorUtility.DisplayDialog("Retro Controller", message, "Yes", "No"))
                    {
                        if (!hasPlatform) UnityEditorInternal.InternalEditorUtility.AddTag("Platform");
                        if (!hasWater) UnityEditorInternal.InternalEditorUtility.AddTag("Water");
                        if (!hasLadder) UnityEditorInternal.InternalEditorUtility.AddTag("Ladder");
                        EditorUtility.DisplayDialog("Retro Controller", "Tags successfully imported!", "OK");
                    }
                    else
                    {
                        string msg = "You won't be asked again.\nIf you change your mind, you can import the " +
                                "tags again in Window > Retro Controller > Reimport Tags.";
                        EditorUtility.DisplayDialog("Retro Controller", msg, "OK");
                    }
                }

                PlayerPrefs.SetString(AssetKey, string.Empty);
                PlayerPrefs.Save();
            }
        }

        static Version AssetVersion()
        {
            var path = AssetDatabase.FindAssets("retrocontroller_version").FirstOrDefault();
            TextAsset newVersionText = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (newVersionText != null)
            {
                return new Version(newVersionText.text);
            }

            return null;
        }
    }
}