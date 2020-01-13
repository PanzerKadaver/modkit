using System;
using UnityEngine;
using System.Reflection;
using Harmony;

[assembly: AssemblyTitle("RealisticMagazineManagement")] // ENTER MOD TITLE
[assembly: AssemblyFileVersion("0.1")] // MOD VERSION
[assembly: AssemblyVersion("1.1101")] // GAME VERSION


public class ModEntryPoint : MonoBehaviour // ModEntryPoint - RESERVED LOOKUP NAME
{
    void Start()
    {
        var assembly = GetType().Assembly;
        string modName = assembly.GetName().Name;
        string dir = System.IO.Path.GetDirectoryName(assembly.Location);

        Debug.Log($"Mod Init: {modName} ({dir})");

        ResourceManager.AddBundle(modName, AssetBundle.LoadFromFile(dir + "/" + modName + "_resources"));

		try
		{
			Debug.Log($"[{GetType().Assembly.GetName().Name}] : Booting up Harmony");

			HarmonyInstance.Create("io.github.panzerkadaver.RealisticMagazineManagement").PatchAll(Assembly.GetExecutingAssembly());
		}
		catch (Exception ex)
		{
			Debug.LogError($"[{GetType().Assembly.GetName().Name}] : Harmony patch fail. Error : {ex}");
		}
		

        GlobalEvents.AddListener<GlobalEvents.GameStart>(GameLoaded);
        GlobalEvents.AddListener<GlobalEvents.LevelLoaded>(LevelLoaded);
    }

    void GameLoaded(GlobalEvents.GameStart evnt)
    {
        //Localization.LoadStrings("mymod_strings_");

        Game.World.console.DeveloperMode();
    }

    void LevelLoaded(GlobalEvents.LevelLoaded evnt)
    {
        Debug.Log(evnt.levelName);
    }

    void Update()
    {
        
    }
}
