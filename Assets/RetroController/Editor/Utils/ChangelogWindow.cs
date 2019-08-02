using UnityEditor;
using UnityEngine;

public class ChangelogWindow : EditorWindow
{
    public static ChangelogWindow Singleton;
    GUIStyle style, versionStyle, texStyle;
    Texture changelogTex;
    Texture2D changelogBackgroundTex;
    TextAsset changelogText;
    TextAsset versionText;

    [MenuItem("Window/Retro Controller/Changelog")]
    public static void OpenWindow()
    {
        Singleton = GetWindow<ChangelogWindow>();
        Singleton.titleContent = new GUIContent("CHANGELOG");
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
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.normal.background = changelogBackgroundTex;
        style.alignment = TextAnchor.UpperLeft;
        style.padding = new RectOffset(10, 10, 20, 10);

        versionStyle = new GUIStyle();
        versionStyle.fontSize = 22;
        versionStyle.normal.textColor = new Color(1f, 0.55f, 0.06f);
        versionStyle.alignment = TextAnchor.UpperCenter;
        versionStyle.padding = new RectOffset(5, 5, 5, 5);

        changelogTex = Resources.Load<Texture>("retrocontroller_changelog");
        changelogText = Resources.Load<TextAsset>("retrocontroller_changelog");
        versionText = Resources.Load<TextAsset>("retrocontroller_version");

        minSize = new Vector2(600, 400);
        maxSize = new Vector2(600, 800);
    }

    private void OnGUI()
    {
        GUILayout.Label(changelogTex, texStyle);
        GUILayout.Label(string.Format("Version {0}", versionText), versionStyle);
        GUILayout.Label(changelogText.text, style);
    }
}
