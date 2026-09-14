using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelGacha;

public static class GachaService
{
    private static readonly Random random = new();

    public static GachaColor RollColor()
    {
        int totalWeight = ColorDatabase.RarityWeights.Values.Sum();
        int roll = random.Next(totalWeight);

        int cumulative = 0;
        ColorRarity chosenRarity = ColorRarity.Common;

        foreach (var (rarity, weight) in ColorDatabase.RarityWeights)
        {
            cumulative += weight;
            if (roll < cumulative)
            {
                chosenRarity = rarity;
                break;
            }
        }

        var pool = ColorDatabase.ByRarity(chosenRarity).ToList();
        return pool[random.Next(pool.Count)];
    }

    public static List<GachaColor> OpenPack(int count = 1)
    {
        var results = new List<GachaColor>();
        for (int i = 0; i < count; i++)
            results.Add(RollColor());
        return results;
    }
}