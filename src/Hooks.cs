using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace HammerBlending;

internal class TileBlending : GlobalTile
{
  public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
  {
    Tile tile = Main.tile[i, j];

    bool shouldRegister = tile.Slope switch
    {
      SlopeType.SlopeDownLeft => Main.tile[i + 1, j].IsHalfBlock,
      SlopeType.SlopeDownRight => Main.tile[i - 1, j].IsHalfBlock,
      _ => false
    };

    if (shouldRegister)
      Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomSolid);

    return !shouldRegister;
  }

  public override void SpecialDraw(int i, int j, int type, SpriteBatch spriteBatch)
  {
    HammerBlendingConfig config = ModContent.GetInstance<HammerBlendingConfig>();
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
