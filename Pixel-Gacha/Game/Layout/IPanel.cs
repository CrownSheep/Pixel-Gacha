using Microsoft.Xna.Framework;

namespace PixelGacha.Layout;

public interface IPanel : IUpdatable, IDrawable
{
    void SetBounds(Rectangle bounds);
}