using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using MinaLoveBites.ColorScheme;

namespace MinaLoveBites;

[BepInPlugin("dotnet.mian.lovebites", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.ravenfield.multiteambattle", BepInDependency.DependencyFlags.SoftDependency)]
public class LoveBites : BaseUnityPlugin
{
    public static Harmony Harmony;
    public static string ExtensionID = "lovebites";
    internal static new ManualLogSource Logger;
    internal static string MultiTeamBattleGuid = "com.ravenfield.multiteambattle";
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Mina is ready to give you love bites! :3");

        Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "dotnet.mian.lovebites");
        
        RavenfieldExtensions.AVAILABLE_EXTENSIONS_LOWERCASE = RavenfieldExtensions.AVAILABLE_EXTENSIONS_LOWERCASE.AddToArray(ExtensionID);
        
        ProxyRegistry.CreateTypes();
        ProxyRegistry.RegisterIfNeeded();
    }

    private void OnDestroy()
    {
        LoveBites.Logger.LogInfo("Unregistering everything...");

        Harmony?.UnpatchSelf();
        RavenfieldExtensions.AVAILABLE_EXTENSIONS_LOWERCASE = RavenfieldExtensions.AVAILABLE_EXTENSIONS_LOWERCASE.Where(value => !value.Equals(ExtensionID)).ToArray();
        
        ProxyRegistry.UnregisterIfNeeded();
        ColorSchemeExtensions.Destroy();
    }
}
