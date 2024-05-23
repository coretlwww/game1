using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace game1.source
{
    public class EnemyAnim
    {
        private readonly Texture2D spritesheet;
        private readonly int frames;
        private readonly int row;
        int currentFrameNumber = 0;
        float timeSinceLastFrame = 0;

        public EnemyAnim(Texture2D spritesheet, int width = 16, int row = 0)
        {
            this.spritesheet = spritesheet;
            this.row = row;
            frames = (spritesheet.Width / width);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, GameTime gameTime, float msPerFrames = 200, SpriteEffects effect = SpriteEffects.None)
        {
            if (currentFrameNumber < frames)
            {
                var rectangle = new Rectangle(16 * currentFrameNumber, 32 * row, 16, 32);

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
