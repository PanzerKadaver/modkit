using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSon;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Assets.Scripts;

#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: AssemblyTitle("SpecTree")] // ENTER MOD TITLE

public class ModEntryPoint : MonoBehaviour // ModEntryPoint - RESERVED LOOKUP NAME
{
	void Start()
	{
		var assembly = GetType().Assembly;
		string modName = assembly.GetName().Name;
		string dir = System.IO.Path.GetDirectoryName(assembly.Location);

		Utils.Log("Mod Init <" + modName + "(" + dir + ")>");
		ResourceManager.AddBundle(modName, AssetBundle.LoadFromFile(dir + "/" + modName + "_resources"));

		try
		{
			Utils.Log("Booting up Harmony");
			var harmony = new Harmony("io.github.PzKd.SpecTree");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
		catch (System.Exception ex)
		{
			Utils.LogError($"Harmony patch fail. Error : {ex}");
		}

		GlobalEvents.AddListener<GlobalEvents.GameStart>(GameLoaded);
		GlobalEvents.AddListener<GlobalEvents.LevelLoaded>(LevelLoaded);
	}

	void GameLoaded(GlobalEvents.GameStart evnt)
	{
		Localization.LoadStrings("SpecTree_strings_");
		Localization.LoadTexts("SpecTree_text_");
#if DEBUG
		
		Game.World.console.DeveloperMode();
#endif
	}

	void LevelLoaded(GlobalEvents.LevelLoaded evnt)
	{
		Utils.Log($"Level loaded <{evnt.levelName}>");
	}

	void Update()
	{
		
	}

#if UNITY_EDITOR

	[InitializeOnLoad]
	internal class LocalizationPreviewInEditor
	{
		static LocalizationPreviewInEditor()
		{
			EditorApplication.update += Init;
		}

		static void Init()
		{
			if (!EditorApplication.isCompiling && ResourceManager.bundles.Count > 0)
			{
				EditorApplication.update -= Init;
				Localization.Setup("en", false);
				Localization.LoadStrings("SpecTree_strings_");
				Localization.LoadTexts("SpecTree_text_");
			}
		}
	}
#endif
}
