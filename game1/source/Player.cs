using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace game1.source
{
    public class Player : Objects
    {
        public Vector2 velocity;
        public Rectangle playerFallRect;
        
        public float playerSpeed = 2;
        public float fallSpeed = 3;
        public float jumpSpeed = -10;
        public float startY;
        
        public bool isFalling = true;
        public bool isJumping;
        
        public Animation[] playerAnimation;
        public CurrentAnimation playerAnimationController;

        public Player(Vector2 position, Texture2D idleSprite, Texture2D runSprite, Texture2D jumpSprite, Texture2D fallSprite)
        {
            playerAnimation = new Animation[4];
           
            this.position = position;
            velocity = new Vector2();

            playerAnimation[0] = new Animation(idleSprite);
            playerAnimation[1] = new Animation(runSprite);
            playerAnimation[2] = new Animation(jumpSprite);
            playerAnimation[3] = new Animation(fallSprite);

            //платформы
            hitBox = new Rectangle((int)position.X, (int)position.Y, 64, 40);
            playerFallRect = new Rectangle((int)position.X, (int)position.Y, 64, 40);
        }

        public override void Update()
        {
            KeyboardState keyboard = Keyboard.GetState();

            playerAnimationController = CurrentAnimation.Idle;
            
            position = velocity;
            startY = position.Y;

            Move(keyboard);
            Jump(keyboard);

            if (isFalling)
            {
                velocity.Y += fallSpeed;
                playerAnimationController = CurrentAnimation.Falling;
            }

            hitBox.X = (int)position.X;
            hitBox.Y = (int)position.Y;
            playerFallRect.X = (int)position.X;
            playerFallRect.Y = (int)(velocity.Y);
        }

        private void Move(KeyboardState keyboard)
        {
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
        }

        private void Jump(KeyboardState keyboard)
        {

            if (isJumping)
            {
                playerAnimationController= CurrentAnimation.Jumping;
                velocity.Y += jumpSpeed;
                jumpSpeed += 1;
                //Move(keyboard);
                

                if (velocity.Y >= startY)
                {
                    velocity.Y = startY;
                    isJumping = false;
                }
            }

            else
            {
                if (keyboard.IsKeyDown(Keys.W) && !isFalling)
                {
                    isJumping = true;
                    isFalling = false;
                    jumpSpeed = -10;
                }
            }
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
                case CurrentAnimation.Jumping:
                    playerAnimation[2].Draw(spriteBatch, position, gameTime);
                    break;
                case CurrentAnimation.Falling:
                    playerAnimation[3].Draw(spriteBatch, position, gameTime);
                    break;
            }
        }
    }
}
