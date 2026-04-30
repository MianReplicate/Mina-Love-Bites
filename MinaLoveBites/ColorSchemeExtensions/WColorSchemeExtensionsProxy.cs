using System;
using Lua.Proxy;
using MoonSharp.Interpreter;

namespace MinaLoveBites.ColorSchemeExtensions;

[Proxy(typeof(WColorSchemeExtensions))]
public class WColorSchemeExtensionsProxy
{
    [MoonSharpHidden]
    public object GetValue() => throw new InvalidOperationException("Proxied type is static.");
        
    public static void RemoveOverrideActorColor(Actor actor)
    {
        WColorSchemeExtensions.RemoveOverrideActorColor(actor);
    }

    public static void OverrideActorColor(Actor actor, ColorProxy color)
    {
        WColorSchemeExtensions.OverrideActorColor(actor, color._value);
    }
}