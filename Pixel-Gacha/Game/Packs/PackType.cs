using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PixelGacha.Packs;

public class PackType
{
    public string Name { get; }
    public int Cost { get; }
    public int CardCount { get; }
    public Color ThemeColor { get; }
    public Dictionary<ColorRarity, int> RarityWeights { get; }

    public PackType(string name, int cost, int cardCount, Color themeColor, Dictionary<ColorRarity, int> rarityWeights)
    {
        Name = name;
        Cost = cost;
        CardCount = cardCount;
        ThemeColor = themeColor;
        RarityWeights = rarityWeights;
    }
}