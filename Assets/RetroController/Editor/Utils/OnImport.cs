using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace vnc.Editor.Utils
{
    public class OnImport : AssetPostprocessor
    {
        static string configPath
        {
            get
            {
                var root = Directory.GetParent(Application.dataPath);
                string path = Path.Combine(root.FullName, "ProjectSettings\\RetroPackageConfig.asset");
                return path;
            }
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 0)
                return;

            //var config = AssetDatabase.LoadAssetAtPath<RetroPackageConfig>(configPath);
            if (!File.Exists(configPath))
            {
                bool hasPlatform, hasWater, hasLadder;

                hasPlatform = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Platform");
                hasWater = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Water");
                hasLadder = ArrayUtility.Contains(UnityEditorInternal.InternalEditorUtility.tags, "Ladder");

                string message = string.Format("For the Sample Scene to work with all the RetroController " +
                    "functionalities, the package needs to add the following Tags:");

                if (hasPlatform) message += "\n Platform";
                if (hasWater) message += "\n Water";
                if (hasLadder) message += "\n Ladder";

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

                var config = ScriptableObject.CreateInstance<RetroPackageConfig>();
                config.importedTime = DateTime.Now;

                AssetDatabase.CreateAsset(config, "Assets/RetroPackageConfig.asset");
                string sourcePath = Path.Combine(Application.dataPath, "RetroPackageConfig.asset");
                if (File.Exists(sourcePath))
                    File.Move(sourcePath, configPath);
                else
                    Debug.LogError(string.Format("Error while creating RetroPackageConfig.\n'{0}' does not exist.", sourcePath));
            }

        }
    }
}
