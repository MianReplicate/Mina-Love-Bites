using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace MinaLoveBites.ColorSchemeExtensions;

public class ColorSchemeExtensions
{
        public static ColorSchemeExtensions instance;
        public Dictionary<Actor, RenderTexture> teamActorRTOverrides = new Dictionary<Actor, RenderTexture>();
        public Dictionary<Actor, Color> teamActorColorOverrides = new Dictionary<Actor, Color>();

        public static void Create()
        {
            instance = new ColorSchemeExtensions();
        }

        public Color GetActorColor(Actor actor)
        {
            var success = teamActorColorOverrides.TryGetValue(actor, out var color);

            return success ? color : ColorScheme.TeamColor(actor.team);
        }

        public void RemoveOverrideActorColor(Actor actor)
        {
            teamActorRTOverrides.TryGetValue(actor, out var mainRT);
            if(mainRT != null)
                GameObject.Destroy(mainRT);
            teamActorRTOverrides.Remove(actor);
            teamActorColorOverrides.Remove(actor);
            actor.skinnedRenderer.material.mainTexture = ActorManager.instance.teamActorRT[actor.team];
            actor.skinnedRendererRagdoll.material.mainTexture = ActorManager.instance.teamActorRT[actor.team];
            
            if (!actor.aiControlled)
            {
                foreach (var renderer in LocalPlayer.controller.fpParent.shoulderParent.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    renderer.material.mainTexture = ActorManager.instance.teamActorRT[actor.team];
                }
            }
        }
    
        public void OverrideActorColor(Actor actor, Color color)
        {
            var team = actor.team;
            teamActorRTOverrides.TryGetValue(actor, out var mainRT);
            if (mainRT == null)
            {
                RenderTexture originalRT = ActorManager.instance.teamActorRT[team];
                mainRT = new RenderTexture(originalRT);
                mainRT.Create();

                teamActorRTOverrides[actor] = mainRT;

                actor.skinnedRenderer.material.mainTexture = mainRT;
                actor.skinnedRendererRagdoll.material.mainTexture = mainRT;
            }

            teamActorColorOverrides[actor] = color;
            
            if (!actor.aiControlled)
            {
                foreach (var renderer in LocalPlayer.controller.fpParent.shoulderParent.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    renderer.material.mainTexture = mainRT;
                }
            }
         
            Color white = Color.white;
            Color.RGBToHSV(color, out white.r, out white.g, out white.b);
            ActorManager.instance.blitTeamColorMaterial.SetColor("_Color", white);
            Graphics.Blit((Texture) ActorManager.instance.teamActorTextureOriginal, mainRT, ActorManager.instance.blitTeamColorMaterial);
        }
    
    [HarmonyPatch(typeof(ActorManager), "Awake")]
    [HarmonyPostfix]
    public static void OnActorManagerAwake()
    {
        ColorSchemeExtensions.Create();
    }
    
    [HarmonyPatch(typeof(Actor), "SpawnWeapon")]
    [HarmonyPostfix]
    public static void OnSpawnWeapon(Actor __instance)
    {
        ColorSchemeExtensions.instance.teamActorRTOverrides.TryGetValue(__instance, out var mainRT);
        if (mainRT == null)
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
        ColorSchemeExtensions.instance.teamActorRTOverrides.TryGetValue(__instance, out var mainRT);
        if (mainRT == null)
            return;

        foreach (var renderer in LocalPlayer.controller.fpParent.shoulderParent.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.material.mainTexture = mainRT;
        }
    }

    [HarmonyPatch(typeof(Actor), nameof(Actor.Damage))]
    [HarmonyTranspiler]
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
            }
                
            if (instruction.Calls(AccessTools.Method(typeof(DecalManager), nameof(DecalManager.EmitBloodEffect))))
            {
                list[i] = CodeInstruction.Call(typeof(DecalManagerExtras.DecalManagerExtras), nameof(DecalManagerExtras.DecalManagerExtras.EmitBloodEffect));
                list.RemoveAt(i - 1);
            }
        }
        return list.AsEnumerable();
    }
}