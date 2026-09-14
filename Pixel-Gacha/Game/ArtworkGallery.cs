using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PixelGacha;

public class ArtworkGallery
{
    private List<SavedArtwork> saves = new();
    private int nextArtworkNumber = 1;

    public IReadOnlyList<SavedArtwork> Saves => saves;

    public SavedArtwork Save(int width, int height, Color[] pixelData)
    {
        var artwork = new SavedArtwork($"Artwork {nextArtworkNumber}", width, height, pixelData);
        nextArtworkNumber++;
        saves.Insert(0, artwork); // newest first
        return artwork;
    }

    public void Delete(SavedArtwork artwork) => saves.Remove(artwork);

    public void Rename(SavedArtwork artwork, string newName) => artwork.Name = newName;
}