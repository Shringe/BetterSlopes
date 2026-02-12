using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace HammerBlending;

internal class TileBlending : GlobalTile
{
  private bool ShouldBlendTile(int i, int j)
  {
    Tile tile = Main.tile[i, j];
    return tile.Slope switch
    {
      SlopeType.SlopeDownLeft => IsHalfBlock(Main.tile[i + 1, j]),
      SlopeType.SlopeDownRight => IsHalfBlock(Main.tile[i - 1, j]),
      _ => false
    };
  }

  private bool IsHalfBlock(Tile tile)
  {
    return tile.IsHalfBlock;
  }

  public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
  {
    // If ModContent.GetInstance has no overhead, it should probably by checked first, but I am assuming it has some overhead here
    bool shouldBlend = ShouldBlendTile(i, j) && ModContent.GetInstance<HammerBlendingConfig>().EnableMod;
    if (shouldBlend)
      Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomSolid);
    return !shouldBlend;
  }

  public override void SpecialDraw(int i, int j, int type, SpriteBatch spriteBatch)
  {
    if (!ShouldBlendTile(i, j))
      return;

    HammerBlendingConfig config = ModContent.GetInstance<HammerBlendingConfig>();
    if (!config.EnableMod)
      return;

    SpecialTile specialTile = new(i, j, spriteBatch);

    switch (config.Variant)
    {
      case BlendVariant.Basic:
        specialTile.DrawSlope();
        specialTile.DrawHalfBlock();
        break;
      case BlendVariant.DrawJustHalfBlock:
        specialTile.DrawHalfBlock();
        break;
      case BlendVariant.DrawJustSlope:
        specialTile.DrawSlope();
        break;
    }

    if (config.EnableDebugOverlay)
      specialTile.DrawDebugOverlay();
  }
}
