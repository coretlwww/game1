using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace game1.source
{
    public abstract class Enemies
    {
        public Texture2D spritesheet;
        public Vector2 position;
        public Rectangle hitBox;

        public enum CurrentAnimation
        {
            Idle,
            Run,
        }

        public abstract void Update();
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
