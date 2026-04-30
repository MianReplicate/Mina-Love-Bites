using System;
using System.Collections.Generic;
using HarmonyLib;
using Lua.Proxy;
using MinaLoveBites.ColorSchemeExtensions;
using MinaLoveBites.MultiTeamBattleExtensions;
using MoonSharp.Interpreter;

namespace MinaLoveBites.Patches;

[HarmonyPatch(typeof(Registrar))]
public static class RegisterProxies
{
    [HarmonyPatch(nameof(Registrar.ExposeTypes))]
    [HarmonyPostfix]
    public static void ExposeTypes(Script script)
    {
        ProxyRegistry.ExposeTypes(script);
    }
    
    [HarmonyPatch(nameof(Registrar.RegisterTypes))]
    [HarmonyPostfix]
    public static void RegisterTypes()
    {
        ProxyRegistry.RegisterTypes();
    }

    [HarmonyPatch(nameof(Registrar.GetProxyTypes))]
    [HarmonyPostfix]
    public static void GetProxyTypes(ref Type[] __result)
    {
        __result = ProxyRegistry.GetProxyTypes(__result);
    }
}