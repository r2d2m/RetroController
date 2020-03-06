﻿using UnityEditor;
using UnityEngine;

public class ChangelogWindow : EditorWindow
{
    public static ChangelogWindow Singleton;
    GUIStyle style, versionStyle, texStyle;
    Texture changelogTex;
    Texture2D changelogBackgroundTex;
    TextAsset changelogText;
    TextAsset versionText;
    Vector2 scrollPos;

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
        texStyle.normal.background = changelogBackgroundTex;

        style = new GUIStyle();
        style.richText = true;
        style.fontSize = 16;
        style.normal.background = changelogBackgroundTex;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;
        style.padding = new RectOffset(10, 10, 20, 10);

        versionStyle = new GUIStyle();
        versionStyle.fontSize = 22;
        versionStyle.normal.textColor = Color.white;
        versionStyle.normal.background = changelogBackgroundTex;
        versionStyle.alignment = TextAnchor.UpperCenter;
        versionStyle.padding = new RectOffset(5, 5, 5, 5);

        changelogTex = Resources.Load<Texture>("retrocontroller_changelog");
        changelogText = Resources.Load<TextAsset>("retrocontroller_changelog");
        versionText = Resources.Load<TextAsset>("retrocontroller_version");

        minSize = new Vector2(640, 600);
        maxSize = new Vector2(640, 800);
    }

    private void OnGUI()
    {
        GUILayout.Label(changelogTex, texStyle);
        GUILayout.Label(string.Format("Version {0}", versionText), versionStyle);

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.Label(changelogText.text, style, GUILayout.Width(625));
        GUILayout.EndScrollView();
    }
}
