using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace game1.source
{
    public class Player : Enemies
    {
        public Vector2 velocity;
        public Rectangle playerFallRect;
        public float playerSpeed = 2;
        public float fallSpeed = 2;
        public bool isFalling = true;
        public bool isIntersecting = false;
        public Animation[] playerAnimation;
        public CurrentAnimation playerAnimationController;

        public Player(Texture2D idleSprite, Texture2D runSprite)
        {
            playerAnimation = new Animation[2];
            position = new Vector2();
            velocity = new Vector2();
            playerAnimation[0] = new Animation(idleSprite);
            playerAnimation[1] = new Animation(runSprite);
            hitBox = new Rectangle((int)position.X, (int)position.Y, 32, 32);
            playerFallRect = new Rectangle((int)position.X, (int)position.Y, 32, (int)fallSpeed);
        }

        public override void Update()
        {
            KeyboardState keyboard = Keyboard.GetState();

            playerAnimationController = CurrentAnimation.Idle;

            if (isFalling)
                velocity.Y += fallSpeed;


            if (keyboard.IsKeyDown(Keys.A))
            {
                velocity.X -= playerSpeed;
                playerAnimationController = CurrentAnimation.Run;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                velocity.X += playerSpeed;
                playerAnimationController = CurrentAnimation.Run;
            }

            position = velocity;
            hitBox.X = (int)position.X;
            hitBox.Y = (int)position.Y;
            playerFallRect.X = (int)position.X;
            playerFallRect.Y = (int)(velocity.Y+40);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            switch (playerAnimationController)
            {
                case CurrentAnimation.Idle:
                    playerAnimation[0].Draw(spriteBatch, position, gameTime);
                    break;
                case CurrentAnimation.Run:
                    playerAnimation[1].Draw(spriteBatch, position, gameTime);
                    break;
            }
        }
    }
}
