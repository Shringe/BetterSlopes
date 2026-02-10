using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace HammerBlending
{
  // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
  public class HammerBlending : Mod
  {

  }

  internal class HammerBlendingTile : GlobalTile
  {
    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
      // This function is currently registering every tile as a special point
      Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
    }

    public override void SpecialDraw(int i, int j, int type, SpriteBatch spriteBatch)
    {
      // This draws special effects on registered tiles
    }
  }
}
