using Microsoft.Xna.Framework;

namespace PixelGacha;

public interface IPanel : IUpdatable, IDrawable
{
    void SetBounds(Rectangle bounds);
}