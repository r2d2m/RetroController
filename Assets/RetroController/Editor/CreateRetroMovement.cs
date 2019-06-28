using System.IO;
using UnityEditor;
using UnityEngine;

namespace vnc.Editor
{
    public static class CreateRetroMovement
    {
        [MenuItem("Assets/Create/Retro Controller/New Retro Movement Script")]
        public static void Create()
        {
            string currentFolder = AssetDatabase.GetAssetPath(Selection.activeObject);
            bool isFolder = AssetDatabase.IsValidFolder(currentFolder);
            if (!isFolder)
            {
                currentFolder = Path.GetDirectoryName(currentFolder);
            }

            string newAsset = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(currentFolder, "NewRetroMovement.cs"));
            string fileName = Path.GetFileNameWithoutExtension(newAsset);
            string path = Path.Combine(Application.dataPath.Replace("Assets", string.Empty), newAsset);
            File.WriteAllText(path, CreateTemplate(fileName));
            AssetDatabase.Refresh();
            Debug.Log("File created on: " + path);
        }

        public static string CreateTemplate(string fileName)
        {
            return "using UnityEngine;\n" +
                "using vnc;\n" +
                "public class " + fileName + " : RetroMovement\n{\n\n" +
                "\tpublic override bool DoMovement()\n\t{\n\t\treturn false;\n\t}\n" +
                "\tpublic override void OnCharacterMove()\n\t{\n\n\t}\n" +
                "}\n";
        }
    }
}
