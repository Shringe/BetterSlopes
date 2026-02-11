using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
        specialTile.DrawHalfBlock();
        break;
    }
  }
}
