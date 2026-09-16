using System;
using System.Collections.Generic;
using System.Linq;
using PixelGacha.Packs;

namespace PixelGacha;

public static class GachaService
{
    private static readonly Random random = new();

    public static GachaColor RollColor(PackType packType)
    {
        int totalWeight = packType.RarityWeights.Values.Sum();
        int roll = random.Next(totalWeight);

        int cumulative = 0;
        ColorRarity chosenRarity = ColorRarity.Common;

        foreach (var (rarity, weight) in packType.RarityWeights)
        {
            if (weight <= 0) continue; // skip zero-weight rarities entirely, never eligible to be chosen
            cumulative += weight;
            if (roll < cumulative)
            {
                chosenRarity = rarity;
                break;
            }
        }

        var pool = ColorDatabase.ByRarity(chosenRarity).ToList();

        // Guard: if a rarity tier has weight but ColorDatabase has no colors defined for it yet
        // (e.g. you add a pack type before adding matching colors), fall back rather than crash.
        if (pool.Count == 0)
            pool = ColorDatabase.ByRarity(ColorRarity.Common).ToList();

        return pool[random.Next(pool.Count)];
    }

    public static List<GachaColor> OpenPack(PackType packType)
    {
        var results = new List<GachaColor>();
        for (int i = 0; i < packType.CardCount; i++)
            results.Add(RollColor(packType));
        return results;
    }
}