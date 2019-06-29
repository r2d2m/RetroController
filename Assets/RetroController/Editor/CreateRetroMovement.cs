using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace vnc.Editor
{
    public class CreateRetroMovement : EditorWindow
    {
        private static CreateRetroMovement window;
        static string movementName;
        const float width = 400;
        const float height = 200;

        [MenuItem("Assets/Create/Retro Controller/New Retro Movement Script")]
        public static void CreateWindow()
        {
            window = GetWindow<CreateRetroMovement>();
            window.titleContent = new GUIContent("Create Movement");
            window.maxSize = window.minSize = new Vector2(width, height);
            window.position = new Rect(Screen.width / 2 + (width/2), Screen.height/2 + (height/2), width, height);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Give a script name to the new custom movement");
            movementName = EditorGUILayout.TextField("Name", movementName);
            if (GUILayout.Button("Create"))
            {
                movementName = Regex.Replace(movementName, @"\s", string.Empty);
                Create();
                Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }

        public static void Create()
        {
            string currentFolder = AssetDatabase.GetAssetPath(Selection.activeObject);
            bool isFolder = AssetDatabase.IsValidFolder(currentFolder);
            if (!isFolder)
            {
                currentFolder = Path.GetDirectoryName(currentFolder);
            }

            string newAssetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(currentFolder, movementName + ".cs"));
            string fileName = Path.GetFileNameWithoutExtension(newAssetPath);
            string path = Path.Combine(Application.dataPath.Replace("Assets", string.Empty), newAssetPath);
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
