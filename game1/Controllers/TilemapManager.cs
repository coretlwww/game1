﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;

namespace game1
{
    public class TilemapManager
    {
        readonly TmxMap map;
        readonly Texture2D tileset;
        readonly RenderTarget2D renderTarget;
        readonly int tilesetTilesWide;
        readonly int tileWidth;
        readonly int tileHeight;

        public TilemapManager(TmxMap map, Texture2D tileset, int tilesetTilesWide, int tileWidth, int tileHeight, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            this.map = map;
            this.tileset = tileset;
            this.tilesetTilesWide = tilesetTilesWide;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;

            renderTarget = new RenderTarget2D(graphicsDevice, 1080, 990);
            DrawTilemap(graphicsDevice, spriteBatch);
        }

        public void DrawTilemap(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            for (var i = 0; i < map.Layers.Count; i++)
            {
                for (var j = 0; j < map.Layers[i].Tiles.Count; j++)
                {
                    int gid = map.Layers[i].Tiles[j].Gid;

                    if (gid == 0)
                    {
                        //do nothing
                    }

                    else
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);
                        float x = (j % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(j / (double)map.Width) * map.TileHeight;
                        Rectangle tilesetRec = new((tileWidth) * column, (tileHeight) * row, tileWidth, tileHeight);
                        spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);
                    }
                }
            }
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(renderTarget, new Vector2(0,0), Color.White);
        }
    }
}
