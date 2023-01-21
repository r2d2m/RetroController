using epiplon.Utils;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace epipon.Editor
{
    [CustomPropertyDrawer(typeof(LayerMaskProfileProperty))]
    public class LayerMaskProfilePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var layerMaskProperty = property.FindPropertyRelative("Value");
            layerMaskProperty.intValue = EditorGUI.MaskField(position, label, InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMaskProperty.intValue), InternalEditorUtility.layers);
        }
    }
}
