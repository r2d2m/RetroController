using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using vnc.Editor.Utils;

namespace vnc.Editor
{
    [CustomEditor(typeof(RetroController))]
    public class RetroControllerEditor : UnityEditor.Editor
    {
        SerializedProperty profile;
        SerializedProperty view;

        SerializedProperty retroMovements;
        SerializedProperty autoFillMovements;
        ReorderableList movementsList;

        static Dictionary<int, RetroControllerEditorState> callbackEventsFold = new Dictionary<int, RetroControllerEditorState>();
        SerializedProperty jumpCallback, landingCallback, fixedUpdateEndCalback;
        GUIStyle foldBold;
        bool showDebugInfo = false;

        private void OnEnable()
        {
            profile = serializedObject.FindProperty("Profile");
            view = serializedObject.FindProperty("controllerView");
            autoFillMovements = serializedObject.FindProperty("autoFillMovements");
            retroMovements = serializedObject.FindProperty("retroMovements");
            movementsList = new ReorderableList(serializedObject, retroMovements);
            movementsList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Movements List", EditorStyles.boldLabel);
            };
            movementsList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = movementsList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (foldBold == null)
            {
                foldBold = new GUIStyle(EditorStyles.foldout);
                foldBold.fontStyle = FontStyle.Bold;
            }

            EditorGUILayout.Space();

            if (profile.objectReferenceValue == null)
                EditorGUILayout.HelpBox("The controller doesn't have a Profile attached and won't work.", MessageType.Warning);
            else
            {
                if (GUILayout.Button("Open Profile"))
                {
                    //EditorGUIUtility.PingObject(profile.objectReferenceValue);
                    Selection.activeObject = profile.objectReferenceValue;
                }
            }

            if (view.objectReferenceValue == null)
                EditorGUILayout.HelpBox("The controller doesn't have a view object attached and won't work.", MessageType.Warning);

            EditorUtils.SetIcon(serializedObject.targetObject, "retro_controller");

            DrawDefaultInspectorWithoutScriptField();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Retro Movements", EditorStyles.boldLabel);
            var autoFillGUIContent = new GUIContent("Autofill Movements", "Searches in the RetroController Game Object hierarchy" +
                " for components that match the RetroMovement and fill the list on runtime.");
            autoFillMovements.boolValue = EditorGUILayout.Toggle(autoFillGUIContent, autoFillMovements.boolValue);

            if(!autoFillMovements.boolValue)
                movementsList.DoLayoutList();

            DrawEvents();

            DrawDebugInfo();
        }

        public bool DrawDefaultInspectorWithoutScriptField()
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            SerializedProperty Iterator = serializedObject.GetIterator();

            Iterator.NextVisible(true);

            while (Iterator.NextVisible(false))
            {
                EditorGUILayout.PropertyField(Iterator, true);
            }

            serializedObject.ApplyModifiedProperties();

            return (EditorGUI.EndChangeCheck());
        }

        public void DrawEvents()
        {
            // search for folding state
            RetroControllerEditorState state = new RetroControllerEditorState(); ;
            int id = serializedObject.targetObject.GetInstanceID();
            callbackEventsFold.TryGetValue(id, out state);

            EditorGUILayout.Space();
            state.fold = EditorGUILayout.Foldout(state.fold, "Callback Events", foldBold);

            jumpCallback = serializedObject.FindProperty("OnJumpCallback");
            landingCallback = serializedObject.FindProperty("OnLandingCallback");
            fixedUpdateEndCalback = serializedObject.FindProperty("OnFixedUpdateEndCallback");

            if (state.fold)
            {
                EditorGUILayout.PropertyField(jumpCallback);
                EditorGUILayout.PropertyField(landingCallback);
                EditorGUILayout.PropertyField(fixedUpdateEndCalback);
            }

            // store folding state
            if (callbackEventsFold.ContainsKey(id))
            {
                callbackEventsFold[id] = state;
            }
            else
            {
                callbackEventsFold.Add(id, state);
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public void DrawDebugInfo()
        {
            EditorGUILayout.Space();
            showDebugInfo = EditorGUILayout.Foldout(showDebugInfo, "Debug Info", foldBold);
            if (showDebugInfo)
            {

            }
        }

        [System.Serializable]
        public struct RetroControllerEditorState
        {
            public bool fold;
        }
    }
}
