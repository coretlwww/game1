using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace game1.source
{
    public class Animation
    {
        private readonly Texture2D spritesheet;
        private readonly int frames;
        private readonly int row;
        int currentFrameNumber = 0;
        float timeSinceLastFrame = 0;

        public Animation(Texture2D spritesheet, int width = 80, int row = 0)
        {
            this.spritesheet = spritesheet;
            this.row = row;
            frames = (spritesheet.Width / width);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, GameTime gameTime, float msPerFrames = 100)
        {
            if (currentFrameNumber < frames)
            {
                var rectangle = new Rectangle(80 * currentFrameNumber, 80 * row, 80, 80);
                
                spriteBatch.Draw(spritesheet, position, rectangle, Color.White);
                
                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timeSinceLastFrame > msPerFrames)
                {
                    timeSinceLastFrame -= msPerFrames;
                    currentFrameNumber++;
                    
                    if (currentFrameNumber == frames)
                    {
                        currentFrameNumber = 0;
                    }
                }

            }
        }
    }
}
