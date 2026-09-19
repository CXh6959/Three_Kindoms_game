using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AndroidBuild
{
	private const string DefaultOutputPath = "Builds/Android/ThreeKingdoms.apk";

	private static readonly string[] BuildScenes =
	{
		"Assets/Scenes/开始游戏场景.unity",
		"Assets/Scenes/主场景.unity"
	};

	[MenuItem("构建/Android APK")]
	public static void BuildApkFromMenu()
	{
		BuildApk(DefaultOutputPath);
	}

	public static void BuildApkCommandLine()
	{
		BuildApk(ReadOutputPath());
	}

	private static void BuildApk(string outputPath)
	{
		ValidateScenes();
		if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
		{
			throw new InvalidOperationException("无法切换到 Android 平台，请通过 Unity Hub 安装 Android Build Support。");
		}

		PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
		PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
		EditorUserBuildSettings.buildAppBundle = false;

		string projectRoot = Directory.GetParent(Application.dataPath).FullName;
		string fullOutputPath = Path.IsPathRooted(outputPath) ? outputPath : Path.Combine(projectRoot, outputPath);
		fullOutputPath = Path.GetFullPath(fullOutputPath);
		Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));
		BuildPlayerOptions options = new BuildPlayerOptions
		{
			scenes = BuildScenes,
			locationPathName = fullOutputPath,
			target = BuildTarget.Android,
			options = BuildOptions.None
		};
		BuildReport report = BuildPipeline.BuildPlayer(options);
		if (report.summary.result != BuildResult.Succeeded)
		{
			throw new InvalidOperationException("Android APK 构建失败：" + report.summary.result + "，错误数：" + report.summary.totalErrors);
		}
		Debug.Log("Android APK 构建完成：" + fullOutputPath);
	}

	private static void ValidateScenes()
	{
		for (int i = 0; i < BuildScenes.Length; i++)
		{
			if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BuildScenes[i]) == null)
			{
				throw new FileNotFoundException("构建场景不存在", BuildScenes[i]);
			}
		}
	}

	private static string ReadOutputPath()
	{
		string[] args = Environment.GetCommandLineArgs();
		for (int i = 0; i < args.Length - 1; i++)
		{
			if (args[i] == "-outputPath" && !string.IsNullOrEmpty(args[i + 1]))
			{
				return args[i + 1];
			}
		}
		return DefaultOutputPath;
	}
}
