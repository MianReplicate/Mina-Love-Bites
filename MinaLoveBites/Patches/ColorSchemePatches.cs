using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MinaLoveBites.ColorScheme;
using UnityEngine;

namespace MinaLoveBites.Patches;

[HarmonyPatch]
public static class ColorSchemePatches {
    [HarmonyPatch(typeof(GameManager), "StartGame")]
    [HarmonyPostfix]
    public static void OnGameManagerStart()
    {
        ColorSchemeExtensions.Destroy();
    }
    
    [HarmonyPatch(typeof(Actor), "SpawnWeapon")]
    [HarmonyPostfix]
    public static void OnSpawnWeapon(Actor __instance)
    {
        var mainRT = ColorSchemeExtensions.Instance.GetOverrideRT(__instance);
        if (mainRT == null || __instance.aiControlled)
            return;

        foreach (var renderer in LocalPlayer.controller.fpParent.shoulderParent.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.material.mainTexture = mainRT;
        }
    }
    
    [HarmonyPatch(typeof(Actor), "SwitchWeapon")]
    [HarmonyPostfix]
    public static void OnSwitchWeapon(Actor __instance)
    {
        var mainRT = ColorSchemeExtensions.Instance.GetOverrideRT(__instance);
        if (mainRT == null || __instance.aiControlled)
            return;

        foreach (var renderer in LocalPlayer.controller.fpParent.shoulderParent.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.material.mainTexture = mainRT;
        }
    }

    [HarmonyPatch(typeof(Actor), nameof(Actor.Damage))]
    [HarmonyTranspiler]
    [HarmonyDebug]
    public static IEnumerable<CodeInstruction>  OnDamageTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var list = instructions.ToList();
        for(int i = 0; i < list.Count; i++)
        {
            var instruction = list[i];
            if (instruction.Calls(AccessTools.Method(typeof(DecalManager), nameof(DecalManager.CreateBloodDrop))))
            {
                list[i] = CodeInstruction.Call(typeof(DecalManagerExtras.DecalManagerExtras), nameof(DecalManagerExtras.DecalManagerExtras.CreateBloodDrop));
                list.RemoveAt(i - 1);
                i--;
            } else if (instruction.Calls(AccessTools.Method(typeof(DecalManager), nameof(DecalManager.EmitBloodEffect))))
            {
                list[i] = CodeInstruction.Call(typeof(DecalManagerExtras.DecalManagerExtras), nameof(DecalManagerExtras.DecalManagerExtras.EmitBloodEffect));
                list.RemoveAt(i - 1);
                i--;
            }
        }
        return list.AsEnumerable();
    }
}