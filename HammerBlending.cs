using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace HammerBlending
{
  // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
  public class HammerBlending : Mod
  {

  }

  internal class TileBlending : GlobalTile
  {
    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
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
    }

    public override void SpecialDraw(int i, int j, int type, SpriteBatch spriteBatch)
    {
      Tile tile = Main.tile[i, j];

      Texture2D texture = Main.instance.TilesRenderer.GetTileDrawTexture(tile, i, j);
      Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition;

      // Get the bottom half of the full block texture
      // I'm not sure if this is the correct way to draw a halfBlock
      Rectangle halfBlockSource = new Rectangle(
        tile.TileFrameX,
        tile.TileFrameY + 8,
        16,
        8
      );

      // Adjust destination to draw bottom half only
      Rectangle destination = new Rectangle(
        (int)position.X,
        (int)position.Y + 8,
        16,
        8
      );

      spriteBatch.Draw(texture, destination, halfBlockSource, Lighting.GetColor(i, j));
    }
  }
}
