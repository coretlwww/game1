using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace game1
{
    public class Animation
    {
        private readonly Texture2D spritesheet;
        private readonly int frames;
        private readonly int row;
        readonly int width;
        readonly int height;
        int currentFrameNumber = 0;
        float timeSinceLastFrame = 0;

        public Animation(Texture2D spritesheet, int width, int heigth, int row = 0)
        {
            this.spritesheet = spritesheet;
            this.row = row;
            this.width = width;
            height = heigth;
            frames = (spritesheet.Width / width);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, GameTime gameTime, float msPerFrames = 200, SpriteEffects effect = SpriteEffects.None)
        {
            if (currentFrameNumber < frames)
            {
                var rectangle = new Rectangle(width * currentFrameNumber, height * row, width, height);
                
                spriteBatch.Draw(spritesheet, position, rectangle, Color.White, 0f, new Vector2(), 1f, effect, 1);
                
                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timeSinceLastFrame > msPerFrames)
                {
                    timeSinceLastFrame -= msPerFrames;
                    currentFrameNumber++;
                    
                    if (currentFrameNumber == frames)
                        currentFrameNumber = 0;
                }
            }
        }
    }
}
