using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace vnc.Editor.Utils
{
    public static class EditorUtils {

        public static void SetIcon(Object obj, string resourceTexture)
        {
            var ty = typeof(EditorGUIUtility);
            Texture2D texture = Resources.Load<Texture2D>(resourceTexture);
            var mi = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
            mi.Invoke(null, new object[] { obj, texture });
        }
    }
}
