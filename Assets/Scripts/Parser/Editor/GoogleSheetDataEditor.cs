using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GoogleSheetData))]
public class GoogleSheetDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GoogleSheetData script = (GoogleSheetData)target;

        if (GUILayout.Button("Download & Parse Sheet"))
        {
            script.DownloadAndParseSheet();
            EditorUtility.SetDirty(script);
        }

        if (GUILayout.Button("Open Sheet in Browser"))
        {
            script.OpenSheetInBrowser();
        }
    }
}