using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace HammerBlending;

// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public class HammerBlending : Mod
{

}

// Used for drawing the custom blended tile sprites
internal class SpecialTile
{
  private readonly Tile _tile;
  private readonly Vector2 _position;
  private readonly Texture2D _texture;
  private readonly Color _lightingColor;
  private readonly SpriteBatch _spriteBatch;

  public SpecialTile(int i, int j, SpriteBatch spriteBatch)
  {
    Tile tile = Main.tile[i, j];
    Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition;
    Texture2D texture = Main.instance.TilesRenderer.GetTileDrawTexture(tile, i, j);
    Color lightingColor = Lighting.GetColor(i, j);

    _tile = tile;
    _position = position;
    _texture = texture;
    _lightingColor = lightingColor;
    _spriteBatch = spriteBatch;
  }

  public void DrawDebugOverlay()
  {
    // Draw a red filter over the tile to distinguish it
    Texture2D pixel = TextureAssets.MagicPixel.Value;
    Rectangle destination = new Rectangle(
      (int)_position.X,
      (int)_position.Y,
      16,
      16
    );

    _spriteBatch.Draw(pixel, destination, Color.DarkRed * 0.3f);
  }

  public void DrawHalfBlock()
  {
    // Get the bottom half of the full block texture
    Rectangle halfBlockSource = new Rectangle(
      _tile.TileFrameX,
      _tile.TileFrameY + 8,
      16,
      8
    );

    // Adjust destination to draw bottom half only
    Rectangle destination = new Rectangle(
      (int)_position.X,
      (int)_position.Y + 8,
      16,
      8
    );

    _spriteBatch.Draw(_texture, destination, halfBlockSource, _lightingColor);
  }

  // WARNING: this is largely decompiled code from tModLoader, I don't fully understand what it does, but it works
  public void DrawSlope()
  {
    for (int stripIndex = 0; stripIndex < 8; stripIndex++)
    {
      int drawYOffset = stripIndex * -2;
      int stripHeight = 16 - stripIndex * 2;
      int sourceYOffset = 16 - stripHeight;
      int stripXPosition;

      switch (_tile.Slope)
      {
        case SlopeType.SlopeDownLeft:
          drawYOffset = 0;
          stripXPosition = stripIndex * 2;
          stripHeight = 14 - stripIndex * 2;
          sourceYOffset = 0;
          break;
        case SlopeType.SlopeDownRight:
          drawYOffset = 0;
          stripXPosition = 16 - stripIndex * 2 - 2;
          stripHeight = 14 - stripIndex * 2;
          sourceYOffset = 0;
          break;
        case SlopeType.SlopeUpLeft:
          stripXPosition = stripIndex * 2;
          break;
        case SlopeType.SlopeUpRight:
          stripXPosition = 16 - stripIndex * 2 - 2;
          break;
        default:
          throw new UnreachableException();
      }

      Rectangle sourceRect = new Rectangle(
        _tile.TileFrameX + stripXPosition,
        _tile.TileFrameY + sourceYOffset,
        2,
        stripHeight
      );

      Vector2 drawPosition = _position + new Vector2(stripXPosition, stripIndex * 2 + drawYOffset);

      _spriteBatch.Draw(_texture, drawPosition, sourceRect, _lightingColor);
    }

    int horizontalStripY = _tile.TopSlope ? 14 : 0;

    Rectangle horizontalStripSource = new Rectangle(
      _tile.TileFrameX,
      _tile.TileFrameY + horizontalStripY,
      16,
      2
    );

    Vector2 horizontalStripPosition = _position + new Vector2(0f, horizontalStripY);

    _spriteBatch.Draw(_texture, horizontalStripPosition, horizontalStripSource, _lightingColor);
  }
}

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
