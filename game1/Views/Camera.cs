using Microsoft.Xna.Framework;

namespace game1
{
    public class Camera
    {
        public Matrix Transform;

        public Matrix Follow(Rectangle target)
        {
            target.X = MathHelper.Clamp(target.X, -278 + (int)Game1.ScreenWidth / 2, (int)(Game1.ScreenWidth / 2 + 200));
            target.Y = (int)Game1.ScreenHeight/2 + 10;

            Vector3 translation = new(-target.X - target.Width / 2, -target.Y - target.Height / 2, 0);
            Vector3 offset = new(Game1.ScreenWidth / 4, Game1.ScreenHeight / 2, 0);

            Transform = Matrix.CreateTranslation(translation) * Matrix.CreateTranslation(offset);
            return Transform;
        }
    }
}
