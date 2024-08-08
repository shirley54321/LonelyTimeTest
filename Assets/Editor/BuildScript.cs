using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Build/PerformBuild")]
    public static void PerformBuild()
    {
        // string[] scenes = { "Assets/Scene1.unity", "Assets/Scene2.unity" };
        string buildPath = "Builds/Android/mygame.apk";

        // 切換到Android平台
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        // 檢查當前平台並創建標誌文件
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            Debug.Log("Successfully changed build target to Android.");
            File.WriteAllText("Assets/BuildTargetChanged.txt", "Build target successfully changed to Android.");
        }
        else
        {
            Debug.LogError("Failed to change build target to Android.");
            return;
        }

        // 執行構建
        // BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.None);

        Debug.Log("Build Completed");
    }
}
