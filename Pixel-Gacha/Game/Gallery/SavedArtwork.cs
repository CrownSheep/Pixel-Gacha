using System;
using Microsoft.Xna.Framework;

namespace PixelGacha;

public class SavedArtwork
{
    public string Name;
    public DateTime SavedAt;
    public int Width;
    public int Height;
    public Color[] PixelData;

    public SavedArtwork(string name, int width, int height, Color[] pixelData)
    {
        Name = name;
        SavedAt = DateTime.Now;
        Width = width;
        Height = height;
        PixelData = (Color[])pixelData.Clone();
    }
}