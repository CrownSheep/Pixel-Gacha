using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace PixelGacha;

public static class ColorDatabase
{
    public static readonly List<GachaColor> AllColors = new()
    {
        new GachaColor("Black", Color.Black, ColorRarity.Base),
        new GachaColor("White", Color.White, ColorRarity.Base),
        new GachaColor("Gray", new Color(120, 120, 125), ColorRarity.Base),

        new GachaColor("Basic Red", new Color(220, 60, 60), ColorRarity.Common),
        new GachaColor("Basic Blue", new Color(60, 100, 220), ColorRarity.Common),
        new GachaColor("Basic Green", new Color(60, 180, 90), ColorRarity.Common),
        new GachaColor("Slate Gray", new Color(110, 120, 130), ColorRarity.Common),
        new GachaColor("Sandy Beige", new Color(210, 180, 140), ColorRarity.Common),
        new GachaColor("Sky Blue", new Color(135, 195, 235), ColorRarity.Common),
        new GachaColor("Olive Drab", new Color(110, 120, 60), ColorRarity.Common),
        new GachaColor("Clay Brown", new Color(150, 95, 65), ColorRarity.Common),
        new GachaColor("Blush Pink", new Color(235, 170, 185), ColorRarity.Common),
        new GachaColor("Mint Leaf", new Color(150, 220, 180), ColorRarity.Common),
        new GachaColor("Dusty Rose", new Color(190, 125, 130), ColorRarity.Common),
        new GachaColor("Pale Yellow", new Color(235, 220, 130), ColorRarity.Common),
        new GachaColor("Forest Green", new Color(45, 120, 70), ColorRarity.Common),
        new GachaColor("Ocean Blue", new Color(45, 130, 175), ColorRarity.Common),
        new GachaColor("Lavender", new Color(180, 160, 210), ColorRarity.Common),
        new GachaColor("Terracotta", new Color(190, 100, 75), ColorRarity.Common),
        new GachaColor("Peach", new Color(245, 175, 135), ColorRarity.Common),
        new GachaColor("Moss Green", new Color(100, 125, 75), ColorRarity.Common),

        new GachaColor("Sunset Orange", new Color(255, 130, 60), ColorRarity.Rare),
        new GachaColor("Deep Violet", new Color(140, 60, 200), ColorRarity.Rare),
        new GachaColor("Sea Teal", new Color(40, 180, 170), ColorRarity.Rare),
        new GachaColor("Crimson Rose", new Color(200, 30, 70), ColorRarity.Rare),
        new GachaColor("Electric Lime", new Color(190, 240, 30), ColorRarity.Rare),
        new GachaColor("Storm Blue", new Color(50, 70, 130), ColorRarity.Rare),
        new GachaColor("Amberglow", new Color(240, 165, 30), ColorRarity.Rare),
        new GachaColor("Orchid Bloom", new Color(190, 90, 190), ColorRarity.Rare),
        new GachaColor("Neon Coral", new Color(255, 80, 100), ColorRarity.Rare),
        new GachaColor("Cobalt", new Color(30, 80, 190), ColorRarity.Rare),
        new GachaColor("Jade", new Color(30, 170, 120), ColorRarity.Rare),
        new GachaColor("Royal Blue", new Color(70, 50, 180), ColorRarity.Rare),
        new GachaColor("Hot Pink", new Color(245, 55, 150), ColorRarity.Rare),
        new GachaColor("Turquoise", new Color(30, 210, 200), ColorRarity.Rare),
        new GachaColor("Burnt Orange", new Color(200, 75, 25), ColorRarity.Rare),
        new GachaColor("Plum", new Color(110, 45, 120), ColorRarity.Rare),
        new GachaColor("Chartreuse", new Color(150, 210, 40), ColorRarity.Rare),
        new GachaColor("Copper", new Color(190, 100, 55), ColorRarity.Rare),

        new GachaColor("Molten Gold", new Color(255, 200, 40), ColorRarity.Epic),
        new GachaColor("Void Black", new Color(20, 20, 25), ColorRarity.Epic),
        new GachaColor("Aurora Green", new Color(60, 255, 180), ColorRarity.Epic),
        new GachaColor("Royal Magenta", new Color(210, 30, 150), ColorRarity.Epic),
        new GachaColor("Glacier Cyan", new Color(120, 240, 245), ColorRarity.Epic),
        new GachaColor("Ember Red", new Color(255, 70, 40), ColorRarity.Epic),
        new GachaColor("Plasma Purple", new Color(180, 50, 255), ColorRarity.Epic),
        new GachaColor("Laser Blue", new Color(50, 140, 255), ColorRarity.Epic),
        new GachaColor("Toxic Green", new Color(100, 255, 40), ColorRarity.Epic),
        new GachaColor("Inferno Orange", new Color(255, 95, 15), ColorRarity.Epic),
        new GachaColor("Cosmic Pink", new Color(255, 60, 210), ColorRarity.Epic),
        new GachaColor("Deep Space", new Color(35, 30, 75), ColorRarity.Epic),
        new GachaColor("Electric Purple", new Color(120, 40, 255), ColorRarity.Epic),
        new GachaColor("Arctic Blue", new Color(100, 190, 255), ColorRarity.Epic),
        new GachaColor("Blood Orange", new Color(220, 40, 20), ColorRarity.Epic),
        new GachaColor("Opal Green", new Color(100, 240, 200), ColorRarity.Epic),

        new GachaColor("Prismatic White", new Color(253, 255, 250), ColorRarity.Legendary),
        new GachaColor("Chroma Shift", new Color(255, 0, 180), ColorRarity.Legendary),
        new GachaColor("Stardust Violet", new Color(150, 90, 255), ColorRarity.Legendary),
        new GachaColor("Solar Flare", new Color(255, 140, 0), ColorRarity.Legendary),
        new GachaColor("Neon Spectrum", new Color(0, 255, 255), ColorRarity.Legendary),
        new GachaColor("Celestial Blue", new Color(70, 130, 255), ColorRarity.Legendary),
        new GachaColor("Supernova", new Color(255, 70, 20), ColorRarity.Legendary),
        new GachaColor("Moonlight", new Color(210, 225, 255), ColorRarity.Legendary),
        new GachaColor("Galaxy Purple", new Color(100, 30, 200), ColorRarity.Legendary),
        new GachaColor("Golden Radiance", new Color(255, 225, 70), ColorRarity.Legendary),
        new GachaColor("Abyssal Blue", new Color(15, 25, 100), ColorRarity.Legendary),
        new GachaColor("Mystic Rose", new Color(255, 100, 190), ColorRarity.Legendary),
        new GachaColor("Aurora Prism", new Color(100, 255, 220), ColorRarity.Legendary),
        new GachaColor("Blacklight", new Color(70, 0, 120), ColorRarity.Legendary),
    };

    public static IEnumerable<GachaColor> ByRarity(ColorRarity rarity) =>
        AllColors.Where(c => c.Rarity == rarity);
}