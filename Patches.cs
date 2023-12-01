using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LethalPaintings;

internal class Patches
{
    private static ManualLogSource Logger { get; set; }
    
    public static void Init(ManualLogSource logger)
    {
        Logger = logger;
    }
    
    private static void UpdateMaterials(int seed)
    {
        Logger.LogInfo("Patching the textures");

        Plugin.Rand = new System.Random(seed);

        foreach (var obj in (GameObject[])Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.name == "Painting" || obj.name == "Painting(Clone)")
            {
                UpdateTexture(Plugin.PaintingFiles, obj.GetComponent<MeshRenderer>().material);
            }
        }
    }

    [HarmonyPatch(typeof(GrabbableObject), "SetScrapValue")]
    [HarmonyPostfix]
    private static void SetScrapValuePatch(GrabbableObject __instance)
    {
        if (__instance.itemProperties.itemName == "Painting")
        {
            UpdateTexture(Plugin.PaintingFiles, __instance.mainObjectRenderer.material);
        }
    }

    private static void UpdateTexture(IReadOnlyList<string> files, Material material)
    {
        if (files.Count == 0) {return;}
        
        var index = Plugin.Rand.Next(files.Count);
        
        var texture = new Texture2D(512, 512);
        Logger.LogInfo($"Patching {material.name} with {files[index]}");
        texture.LoadImage(File.ReadAllBytes(files[index]));
        material.mainTexture = texture;
    }
}