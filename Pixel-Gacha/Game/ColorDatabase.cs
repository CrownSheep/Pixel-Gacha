using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace PixelGacha;

public static class ColorDatabase
{
    public static readonly Dictionary<ColorRarity, int> RarityWeights = new()
    {
        { ColorRarity.Common,    55 },
        { ColorRarity.Rare,      28 },
        { ColorRarity.Epic,      13 },
        { ColorRarity.Legendary,  4 },
    };

    public static readonly List<GachaColor> AllColors = new()
    {
        new GachaColor("Ink Black",   new Color(25, 25, 28),    ColorRarity.Base),
        new GachaColor("Paper White", new Color(250, 250, 245), ColorRarity.Base),
        new GachaColor("Graphite",    new Color(120, 120, 125), ColorRarity.Base),
        
        new GachaColor("Basic Red",    new Color(220, 60, 60),   ColorRarity.Common),
        new GachaColor("Basic Blue",   new Color(60, 100, 220),  ColorRarity.Common),
        new GachaColor("Basic Green",  new Color(60, 180, 90),   ColorRarity.Common),
        new GachaColor("Slate Gray",   new Color(110, 120, 130), ColorRarity.Common),
        new GachaColor("Sandy Beige",  new Color(210, 180, 140), ColorRarity.Common),
        new GachaColor("Sky Blue",     new Color(135, 195, 235), ColorRarity.Common),
        new GachaColor("Olive Drab",   new Color(110, 120, 60),  ColorRarity.Common),
        new GachaColor("Clay Brown",   new Color(150, 95, 65),   ColorRarity.Common),
        new GachaColor("Blush Pink",   new Color(235, 170, 185), ColorRarity.Common),
        new GachaColor("Mint Leaf",    new Color(150, 220, 180), ColorRarity.Common),
        
        new GachaColor("Sunset Orange", new Color(255, 130, 60),  ColorRarity.Rare),
        new GachaColor("Deep Violet",   new Color(140, 60, 200),  ColorRarity.Rare),
        new GachaColor("Sea Teal",      new Color(40, 180, 170),  ColorRarity.Rare),
        new GachaColor("Crimson Rose",  new Color(200, 30, 70),   ColorRarity.Rare),
        new GachaColor("Electric Lime", new Color(190, 240, 30),  ColorRarity.Rare),
        new GachaColor("Storm Blue",    new Color(50, 70, 130),   ColorRarity.Rare),
        new GachaColor("Amberglow",     new Color(240, 165, 30),  ColorRarity.Rare),
        new GachaColor("Orchid Bloom",  new Color(190, 90, 190),  ColorRarity.Rare),

        new GachaColor("Molten Gold",    new Color(255, 200, 40),  ColorRarity.Epic),
        new GachaColor("Void Black",     new Color(20, 20, 25),    ColorRarity.Epic),
        new GachaColor("Aurora Green",   new Color(60, 255, 180),  ColorRarity.Epic),
        new GachaColor("Royal Magenta",  new Color(210, 30, 150),  ColorRarity.Epic),
        new GachaColor("Glacier Cyan",   new Color(120, 240, 245), ColorRarity.Epic),
        new GachaColor("Ember Red",      new Color(255, 70, 40),   ColorRarity.Epic),
        
        new GachaColor("Prismatic White", Color.White,               ColorRarity.Legendary),
        new GachaColor("Chroma Shift",    new Color(255, 0, 180),    ColorRarity.Legendary),
        new GachaColor("Stardust Violet", new Color(150, 90, 255),   ColorRarity.Legendary),
        new GachaColor("Solar Flare",     new Color(255, 140, 0),    ColorRarity.Legendary),
    };

    public static IEnumerable<GachaColor> ByRarity(ColorRarity rarity) =>
        AllColors.Where(c => c.Rarity == rarity);
}