using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;

namespace PixelGacha;

public static class ButtonUtil
{
    public static Color GetColor(Rectangle rect, Color baseColor, bool disabled = false)
    {
        if (disabled) return Color.Gray;

        var mouse = MouseExtended.GetState();
        if (rect.Contains(mouse.Position))
            return mouse.IsButtonDown(MouseButton.Left) ? baseColor * 0.7f : Lighten(baseColor, 0.15f);

        return baseColor;
    }

    private static Color Lighten(Color c, float amount) => new Color(
        (byte)MathHelper.Min(255, c.R + 255 * amount),
        (byte)MathHelper.Min(255, c.G + 255 * amount),
        (byte)MathHelper.Min(255, c.B + 255 * amount));
}