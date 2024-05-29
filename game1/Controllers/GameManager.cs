using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace game1.source
{
    public class GameManager
    {
        private Rectangle endRectangle;
        public GameManager(Rectangle endRectangle) 
        { 
            this.endRectangle = endRectangle;
        }

        public bool IsGameEnded(Rectangle playerHitbox)
        {
            return endRectangle.Intersects(playerHitbox);
        }
    }
}
