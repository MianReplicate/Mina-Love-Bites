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
        script.Globals["ColorSchemeExtensions"] = typeof(WColorSchemeExtensionsProxy);
        script.Globals["MultiTeamBattleDataExtensions"] = typeof(WMultiTeamBattleDataExtensionsProxy);
    }
    
    [HarmonyPatch(nameof(Registrar.RegisterTypes))]
    [HarmonyPostfix]
    public static void RegisterTypes()
    {
        UserData.RegisterType(typeof(WColorSchemeExtensionsProxy));
        UserData.RegisterType(typeof(WMultiTeamBattleDataExtensionsProxy));
    }

    [HarmonyPatch(nameof(Registrar.GetProxyTypes))]
    [HarmonyPostfix]
    public static void GetProxyTypes(ref Type[] __result)
    {
        List<Type> additionalProxies = new List<Type>(__result);
        
        additionalProxies.Add(typeof(WColorSchemeExtensionsProxy));
        additionalProxies.Add(typeof(WMultiTeamBattleDataExtensionsProxy));
        
        __result = additionalProxies.ToArray();
    }
    
    
}