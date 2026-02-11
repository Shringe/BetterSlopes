using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace HammerBlending;

public enum BlendVariant
{
  Basic,
  Invisible
}

public class HammerBlendingConfig : ModConfig
{
  public override ConfigScope Mode => ConfigScope.ClientSide;

  [DefaultValue(BlendVariant.Basic)]
  [Slider]
  public BlendVariant Variant;

  [DefaultValue(false)]
  public bool EnableDebugOverlay;
}
