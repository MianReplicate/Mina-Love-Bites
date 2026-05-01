using System.Collections.Generic;
using HarmonyLib;
using Lua;
using Lua.Wrapper;
using MultiTeamBattle;

namespace MinaLoveBites.MultiTeamBattleExtensions;

[Name("MultiTeamBattleDataExtensions")]
public class WMultiTeamBattleDataExtensions
{
    public static int GetTeamCount()
    {
        return MultiTeamBattleData.TeamCount;
    }

    public static Dictionary<WTeam, string> GetTeams()
    {
        var teamCount = GetTeamCount();
        var getProxyTeam = AccessTools.Method("MultiTeamBattle.MutatorAffinityRuntime:GetScriptProxyTeam");
        var dictionary = new Dictionary<WTeam, string>(teamCount);
        
        for (int i = 0; i < teamCount; i++)
        {
            dictionary.Add((WTeam) getProxyTeam.Invoke(null, [i]), GameManager.instance.GetTeamName(i));
        }

        dictionary.Add(WTeam.Neutral, "Neutral");
        return dictionary;
    }
}