using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace HammerBlending;

public class HammerBlendingConfig : ModConfig
{
  public override ConfigScope Mode => ConfigScope.ClientSide;

  [Header("Style")]
  [DefaultValue(true)]
  public bool DrawSlope;
  [DefaultValue(true)]
  public bool DrawHalfBlock;
  [DefaultValue(true)]
  public bool DrawBlendingHalfBlock;
  [DefaultValue(6)]
  [Slider]
  [Range(1, 15)]
  public int BlendingHalfBlockOffset;

  [Header("Debugging")]
  [DefaultValue(true)]
  public bool EnableMod;

  [DefaultValue(false)]
  public bool EnableDebugOverlay;
}
