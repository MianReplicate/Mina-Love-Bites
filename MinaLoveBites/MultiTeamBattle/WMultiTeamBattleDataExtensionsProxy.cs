using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
using Lua.Proxy;
using Lua.Wrapper;
using MoonSharp.Interpreter;
using MultiTeamBattle;

namespace MinaLoveBites.MultiTeamBattleExtensions;

[Proxy(typeof(WMultiTeamBattleDataExtensions))]
public class WMultiTeamBattleDataExtensionsProxy
{
    [MoonSharpHidden]
    public object GetValue() => throw new InvalidOperationException("Proxied type is static.");
    
    public static bool IsInstalled => Chainloader.PluginInfos.ContainsKey(LoveBites.MultiTeamBattleGuid);
    
    public static int GetTeamCount()
    {
        return WMultiTeamBattleDataExtensions.GetTeamCount();
    }

    public static Dictionary<object, object> GetTeams()
    {
        if (!IsInstalled)
            return new Dictionary<object, object>()
            {
                [WTeam.Blue] = "Blue",
                [WTeam.Red] = "Red",
                [WTeam.Neutral] = "Neutral"
            };
        return WMultiTeamBattleDataExtensions.GetTeams().ToDictionary(pair => (object)pair.Key, pair => (object)pair.Value);
    }

}