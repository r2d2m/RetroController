#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class AutoLightBake : MonoBehaviour
{
    void Start()
    {
        if (Lightmapping.lightingDataAsset == null)
        {
            string message = "To display the Tutorial scene correctly, you need to " +
                "bake the lightmap. Do you wanna do this automatically?";
            if (EditorUtility.DisplayDialog("Lightmap Bake", message, "Yes", "No"))
            {
                Lightmapping.completed = () =>
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                };

                Lightmapping.Bake();
            }
        }
    }
}
#endif