using Microsoft.Xna.Framework;

namespace game1.source
{
    public class Camera
    {
        public Matrix Tranform;

        public Matrix Follow(Rectangle target)
        {
            target.X = MathHelper.Clamp(target.X, -8 + (int)Game1.screenWidth/2, (int)(1000-Game1.screenWidth/2));
            target.Y = (int)Game1.screenHeight/2;

            Vector3 translation = new Vector3(-target.X-target.Width/2, -target.Y-target.Height/2, 0);
            Vector3 offset = new Vector3(Game1.screenWidth/2, Game1.screenHeight/2,0);

            Tranform = Matrix.CreateTranslation(translation)*Matrix.CreateTranslation(offset);
            return Tranform;
        }
    }
}
