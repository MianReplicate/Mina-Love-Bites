using Lua;
using UnityEngine;

namespace MinaLoveBites.ColorSchemeExtensions;

[Name("ColorSchemeExtensions")]
public static class WColorSchemeExtensions
{
    public static void RemoveOverrideActorColor(Actor actor)
    {
        ColorSchemeExtensions.instance?.RemoveOverrideActorColor(actor);
    }

    public static void OverrideActorColor(Actor actor, Color color)
    {
        ColorSchemeExtensions.instance?.OverrideActorColor(actor, color);
    }
}