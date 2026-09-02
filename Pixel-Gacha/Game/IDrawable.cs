using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelGacha;

public interface IDrawable
{
    public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}