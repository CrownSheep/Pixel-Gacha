using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PixelGacha.Packs;

public static class PackCatalog
{
    public static readonly PackType Basic = new(
        "Basic Pack", cost: 50, cardCount: 3, themeColor: new Color(120, 70, 160),
        rarityWeights: new Dictionary<ColorRarity, int>
        {
            { ColorRarity.Common, 55 },
            { ColorRarity.Rare, 28 },
            { ColorRarity.Epic, 13 },
            { ColorRarity.Legendary, 4 },
        });

    public static readonly PackType Premium = new(
        "Premium Pack", cost: 120, cardCount: 4, themeColor: new Color(40, 90, 200),
        rarityWeights: new Dictionary<ColorRarity, int>
        {
            { ColorRarity.Common, 30 },
            { ColorRarity.Rare, 40 },
            { ColorRarity.Epic, 22 },
            { ColorRarity.Legendary, 8 },
        });

    public static readonly PackType Legendary = new(
        "Legendary Pack", cost: 300, cardCount: 3, themeColor: new Color(200, 160, 30),
        rarityWeights: new Dictionary<ColorRarity, int>
        {
            { ColorRarity.Common, 1 },
            { ColorRarity.Rare, 35 },
            { ColorRarity.Epic, 49 },
            { ColorRarity.Legendary, 15 },
        });

    public static readonly IReadOnlyList<PackType> All = [Basic, Premium, Legendary];
}