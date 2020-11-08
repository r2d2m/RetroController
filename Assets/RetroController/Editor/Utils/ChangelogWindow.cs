using UnityEditor;
using UnityEngine;

namespace vnc.Editor
{
    public class ChangelogWindow : EditorWindow
    {
        public static ChangelogWindow Singleton;
        GUIStyle style, versionStyle, texStyle;
        Texture changelogTex;
        Texture2D changelogBackgroundTex;
        TextAsset changelogText;
        TextAsset versionText;
        Vector2 scrollPos;
        const int WIDTH = 700;
        const int HEIGHT = 800;

        [MenuItem("Window/Retro Controller/Changelog")]
        public static void OpenWindow()
        {
            Singleton = GetWindow<ChangelogWindow>();
            Singleton.titleContent = new GUIContent("CHANGELOG");
            Singleton.minSize = new Vector2(WIDTH, HEIGHT);
            Singleton.maxSize = new Vector2(WIDTH, HEIGHT);
            Singleton.Show();
        }

        private void OnEnable()
        {
            changelogBackgroundTex = Resources.Load<Texture2D>("retrocontroller_changelog_background");

            texStyle = new GUIStyle();
            texStyle.alignment = TextAnchor.MiddleCenter;
            texStyle.padding = new RectOffset(5, 5, 5, 0);

            style = new GUIStyle();
            style.richText = true;
            style.fontSize = 16;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperLeft;
            style.padding = new RectOffset(10, 0, 20, 10);

            versionStyle = new GUIStyle();
            versionStyle.fontSize = 22;
            versionStyle.normal.textColor = Color.white;
            versionStyle.alignment = TextAnchor.UpperCenter;
            versionStyle.padding = new RectOffset(5, 5, 5, 5);

            changelogTex = Resources.Load<Texture>("retrocontroller_changelog");
            changelogText = Resources.Load<TextAsset>("retrocontroller_changelog");
            versionText = Resources.Load<TextAsset>("retrocontroller_version");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(Vector2.zero, new Vector2(WIDTH, HEIGHT)), changelogBackgroundTex);

            GUILayout.Label(changelogTex, texStyle);
            GUILayout.Label(string.Format("Version {0}", versionText), versionStyle);

            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
            GUILayout.Label(changelogText.text, style);
            GUILayout.EndScrollView();

            if (GUILayout.Button("Complete Changelog [Opens on Browser]", EditorStyles.miniButton))
            {
                Application.OpenURL("https://epiplonstudio.netlify.app/#/projects/retrocontroller/changelog");
            }
            GUILayout.EndArea();
        }
    }
}
