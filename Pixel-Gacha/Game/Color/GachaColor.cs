using Microsoft.Xna.Framework;

namespace PixelGacha;

public class GachaColor(string name, Color value, ColorRarity rarity)
{
    public string Name { get; } = name;
    public Color Value { get; } = value;
    public ColorRarity Rarity { get; } = rarity;
}