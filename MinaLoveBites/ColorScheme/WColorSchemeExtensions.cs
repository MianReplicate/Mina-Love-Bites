using Lua;
using UnityEngine;

namespace MinaLoveBites.ColorScheme;

[Name("ColorSchemeExtensions")]
public static class WColorSchemeExtensions
{
    public static void RemoveOverrideActorColor(Actor actor)
    {
        ColorSchemeExtensions.Instance?.RemoveOverrideActorColor(actor);
    }

    public static void OverrideActorColor(Actor actor, Color color)
    {
        ColorSchemeExtensions.Instance?.OverrideActorColor(actor, color);
    }
}