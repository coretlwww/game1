using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;

namespace game1.source
{
    public class TilemapManager
    {
        readonly TmxMap map;
        readonly Texture2D tileset;
        readonly int tilesetTilesWide;
        readonly int tileWidth;
        readonly int tileHeight;

        public TilemapManager(TmxMap map, Texture2D tileset, int tilesetTilesWide, int tileWidth, int tileHeight)
        {
            this.map = map;
            this.tileset = tileset;
            this.tilesetTilesWide = tilesetTilesWide;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
           for (var i = 0; i < map.Layers.Count; i++)
           {
                for (var j = 0; j < map.Layers[i].Tiles.Count; j++)
                {
                    int gid = map.Layers[i].Tiles[j].Gid;
                    if (gid == 0)
                    {
                       
                    }

                    else
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame /(double)tilesetTilesWide);
                        float x = (j % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(j / (double)map.Width)*map.TileHeight;
                        Rectangle tilesetRec = new((tileWidth)*column, (tileHeight)*row, tileWidth, tileHeight);
                        spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);
                    }
                }
           } 
        }
    }
}
