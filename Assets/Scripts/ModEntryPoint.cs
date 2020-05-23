using System.Reflection;
using HarmonyLib;
using Debug = Assets.Scripts.Debug;

#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: AssemblyTitle("mod_name")] // ENTER MOD TITLE

public class ModEntryPoint : UnityEngine.MonoBehaviour // ModEntryPoint - RESERVED LOOKUP NAME
{
	public static string ModName => Assembly.GetExecutingAssembly().GetName().Name;
	public void Start()
	{
		var assembly = GetType().Assembly;
		string dir = System.IO.Path.GetDirectoryName(assembly.Location);

		Debug.Log("Mod Init <" + ModName + "(" + dir + ")>");
		ResourceManager.AddBundle(ModName, UnityEngine.AssetBundle.LoadFromFile(dir + "/" + ModName + "_resources"));

		try
		{
			Debug.Log("Booting up Harmony");
			var harmony = new Harmony("io.github.PzKd." + ModName);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Harmony patch fail. Error : {ex}");
		}

		GlobalEvents.AddListener<GlobalEvents.GameStart>(GameLoaded);
		GlobalEvents.AddListener<GlobalEvents.LevelLoaded>(LevelLoaded);
	}

	public void GameLoaded(GlobalEvents.GameStart evnt)
	{
		Localization.LoadStrings(ModName + "_strings_");
		Localization.LoadTexts(ModName + "_text_");
#if DEBUG
		
		Game.World.console.DeveloperMode();
#endif
	}

	public void LevelLoaded(GlobalEvents.LevelLoaded evnt)
	{
		Debug.Log($"Level loaded <{evnt.levelName}>");
	}

	public void Update()
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
				Localization.LoadStrings(ModName + "_strings_");
				Localization.LoadTexts(ModName + "_text_");
			}
		}
	}
#endif
}
