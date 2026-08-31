using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pixel_Gacha;

public interface IDrawable
{
    public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}