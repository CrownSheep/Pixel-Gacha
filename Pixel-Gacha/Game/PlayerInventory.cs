using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PixelGacha;

public class PlayerInventory
{
    public int Currency { get; private set; }
    
    private Dictionary<string, int> owned = new();
    private HashSet<Point> paintedPixels = new();

    public const int CURRENCY_PER_PIXEL = 1;

    public PlayerInventory()
    {
        foreach (var baseColor in ColorDatabase.ByRarity(ColorRarity.Base))
            AddColor(baseColor);
    }

    public IReadOnlyDictionary<string, int> Owned => owned;

    public bool IsOwned(string colorName) => owned.ContainsKey(colorName);

    public void AddCurrency(int amount) => Currency += amount;

    public bool SpendCurrency(int amount)
    {
        if (Currency < amount) return false;
        Currency -= amount;
        return true;
    }

    public void AddColor(GachaColor color)
    {
        owned[color.Name] = owned.GetValueOrDefault(color.Name) + 1;
    }
    
    public bool RegisterPixelPainted(Point gridPos)
    {
        if (paintedPixels.Add(gridPos))
        {
            AddCurrency(CURRENCY_PER_PIXEL);
            return true;
        }
        return false;
    }
}