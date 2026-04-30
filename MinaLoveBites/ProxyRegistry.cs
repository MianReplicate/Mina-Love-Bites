using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Lua;
using Lua.Proxy;
using MoonSharp.Interpreter;
using UnityEngine;

namespace MinaLoveBites;

public class ProxyRegistry
{
    private static Dictionary<string, Type> _types;

    public static void CreateTypes()
    {
        _types = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => (ProxyAttribute[]) t.GetCustomAttributes(typeof(ProxyAttribute), false), resultSelector: (type, proxy) => new {type, proxy})
            .Where(list => list.proxy != null)
            .ToDictionary(list => 
                ((NameAttribute[])list.proxy.proxiedType.GetCustomAttributes(typeof(NameAttribute), false))[0].name, 
                list => list.type);

        foreach (var pair in _types)
        {
            LoveBites.Logger.LogInfo("Registered proxy for " + pair.Key);
        }
    }
    
    public static void ExposeTypes(Script script)
    {
        foreach (var pair in _types)
        {
            script.Globals[pair.Key] = pair.Value;
        }
    }

    public static void RegisterTypes()
    {
        foreach (var pair in _types)
        {
            UserData.RegisterType(pair.Value);
        }
    }

    public static void RegisterIfNeeded()
    {
        var ravenscriptManager = RavenscriptManager.instance;
        if (ravenscriptManager != null)
        {
            var scriptEngine = (ScriptEngine) AccessTools.Field(typeof(RavenscriptManager), "_engine").GetValue(ravenscriptManager);

            if (scriptEngine != null)
            {
                var proxyListField = AccessTools.Field(typeof(ScriptEngine), "proxyTypes");
                var script = (Script) AccessTools.Field(typeof(RavenscriptManager), "script").GetValue(scriptEngine);
                var proxyList = (Type[]) proxyListField.GetValue(scriptEngine);
                if (proxyList?.Length > 0) // has Ravenscript already been registered?
                {
                    proxyListField.SetValue(scriptEngine, GetProxyTypes(proxyList));
                    RegisterTypes();
                    ExposeTypes(script);
                }
            }
        }
    }

    public static void UnregisterIfNeeded()
    {
        var ravenscriptManager = RavenscriptManager.instance;
        if (ravenscriptManager != null)
        {
            var scriptEngine = (ScriptEngine) AccessTools.Field(typeof(RavenscriptManager), "_engine").GetValue(ravenscriptManager);

            if (scriptEngine != null)
            {
                var proxyListField = AccessTools.Field(typeof(ScriptEngine), "proxyTypes");
                var script = (Script) AccessTools.Field(typeof(RavenscriptManager), "script").GetValue(scriptEngine);
                var proxyList = (Type[]) proxyListField.GetValue(scriptEngine);
                if (proxyList?.Length > 0) // has Ravenscript already been registered?
                {
                    UnexposeTypes(script);
                    UnregisterTypes();
                    proxyListField.SetValue(scriptEngine, proxyList.Where(type => !_types.ContainsValue(type)).ToArray());
                }
            }
        }
    }

    public static Type[] GetProxyTypes([CanBeNull] Type[] existing)
    {
        List<Type> additionalProxies = existing != null ? new List<Type>(existing) : new List<Type>();
        
        foreach (var pair in _types)
        {
            additionalProxies.Add(pair.Value);
        }

        return additionalProxies.ToArray();
    }

    public static void UnregisterTypes()
    {
        foreach (var pair in _types)
        {
            UserData.UnregisterType(pair.Value);
        }
    }

    public static void UnexposeTypes(Script script)
    {
        foreach (var pair in _types)
        {
            script.Globals.Remove(pair.Key);
        }   
    }
}