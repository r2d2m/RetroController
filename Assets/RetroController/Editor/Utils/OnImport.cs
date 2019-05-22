using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace vnc.Editor.Utils
{
    public class OnImport : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 0)
                return;

            CheckChangelog(importedAssets);

            if (!EditorPrefs.HasKey("RetroControllerDate"))
            {
                bool hasPlatform, hasWater, hasLadder;

                hasPlatform = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Platform");
                hasWater = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Water");
                hasLadder = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Ladder");

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
                    if (EditorUtility.DisplayDialog("Altering Project Settings", message, "Yes", "No"))
                    {
                        if (!hasPlatform) UnityEditorInternal.InternalEditorUtility.AddTag("Platform");
                        if (!hasWater) UnityEditorInternal.InternalEditorUtility.AddTag("Water");
                        if (!hasLadder) UnityEditorInternal.InternalEditorUtility.AddTag("Ladder");
                    }
                }

                EditorPrefs.SetString("RetroControllerDate", DateTime.Now.ToString());
            }
        }

        static void CheckChangelog(string[] importedAssets)
        {
            string newVersionPath = importedAssets.FirstOrDefault(p => p.Contains("retrocontroller_version"));
            TextAsset newVersionText = AssetDatabase.LoadAssetAtPath<TextAsset>(newVersionPath);
            if (newVersionText != null)
                ChangelogWindow.OpenWindow();
        }
    }
}