using UnityEditor;
using UnityEngine;
using vnc.Utils;

namespace vnc.Editor.Utils
{
    [CustomPropertyDrawer(typeof(FancyHeaderAttribute))]
    public class FancyHeaderDecoratorDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            FancyHeaderAttribute headerAtt = (FancyHeaderAttribute)attribute;

            position.y += 5;
            position.height -= 9;

            EditorGUI.LabelField(position, headerAtt.Header, GetStyle());
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight + 20;
        }

        static GUIStyle labelStyle = null;
        public static GUIStyle GetStyle()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.helpBox);
                labelStyle.fontSize = 14;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.alignment = TextAnchor.UpperCenter;
            }
            return labelStyle;
        }
    }
}
