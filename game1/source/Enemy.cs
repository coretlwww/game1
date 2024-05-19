using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace game1.source
{
    public class Enemy : Objects
    {
        private EnemyAnim enemyAnimation;
        
        private Rectangle path;
        
        private float speed = 2;
        
        private bool isFacingRight = true;

        public Enemy(Texture2D enemySpriteSheet, Rectangle path, float speed = 2)
        {
            enemyAnimation = new EnemyAnim(enemySpriteSheet);
            this.path = path;
            position = new Vector2(path.X, path.Y);
            hitBox = new Rectangle(path.X, path.Y, 16, 16);
            this.speed = speed;
        }

        public override void Update()
        {
            if (!path.Contains(hitBox))
            {
                speed = -speed;
                isFacingRight = !isFacingRight;
            
            }
               
            position.X += speed;

            hitBox.X = (int) position.X;
            hitBox.Y = (int) position.Y;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (isFacingRight) 
                enemyAnimation.Draw(spriteBatch, position, gameTime, 600);
            else
                enemyAnimation.Draw(spriteBatch, position, gameTime, 600, SpriteEffects.FlipHorizontally);
        }
    }
}
