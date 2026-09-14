using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;

namespace PixelGacha;

public class GalleryWindow : IPanel
{
    private ArtworkGallery gallery;
    private DrawingGrid grid;
    private BitmapFont font;
    private GraphicsDevice graphicsDevice;
    private Rectangle bounds;

    private const int THUMB_SIZE = 64;
    private const int CARD_HEIGHT = 84;
    private const int PADDING = 10;

    private Dictionary<SavedArtwork, Texture2D> thumbnailCache = new();

    public GalleryWindow(GraphicsDevice graphicsDevice, ArtworkGallery gallery, DrawingGrid grid, BitmapFont font)
    {
        this.graphicsDevice = graphicsDevice;
        this.gallery = gallery;
        this.grid = grid;
        this.font = font;
    }

    public void SetBounds(Rectangle newBounds) => bounds = newBounds;

    public void Update(GameTime gameTime)
    {
        var mouse = MouseExtended.GetState();
        if (!mouse.WasButtonPressed(MouseButton.Left)) return;

        int y = bounds.Y + PADDING;
        foreach (var artwork in gallery.Saves)
        {
            var cardRect = new Rectangle(bounds.X + PADDING, y, bounds.Width - PADDING * 2, CARD_HEIGHT);
            var loadButtonRect = new Rectangle(cardRect.Right - 60, cardRect.Y + 8, 50, 24);
            var deleteButtonRect = new Rectangle(cardRect.Right - 60, cardRect.Y + 38, 50, 24);

            if (loadButtonRect.Contains(mouse.Position))
            {
                grid.LoadPixelData(artwork.Width, artwork.Height, artwork.PixelData);
            }
            else if (deleteButtonRect.Contains(mouse.Position))
            {
                thumbnailCache.Remove(artwork, out var tex);
                tex?.Dispose();
                gallery.Delete(artwork);
                break;
            }

            y += CARD_HEIGHT + PADDING;
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (gallery.Saves.Count == 0)
        {
            spriteBatch.DrawString(font, "No saved artwork yet -\npaint something and hit Save!",
                new Vector2(bounds.X + PADDING, bounds.Y + PADDING), Color.Gray);
            return;
        }

        int y = bounds.Y + PADDING;
        foreach (var artwork in gallery.Saves)
        {
            var cardRect = new Rectangle(bounds.X + PADDING, y, bounds.Width - PADDING * 2, CARD_HEIGHT);
            spriteBatch.FillRectangle(cardRect, Color.White);
            spriteBatch.DrawRectangle(cardRect, Color.Black);

            var thumbRect = new Rectangle(cardRect.X + 8, cardRect.Y + 10, THUMB_SIZE, THUMB_SIZE);
            var thumbnail = GetOrCreateThumbnail(artwork);
            spriteBatch.Draw(thumbnail, thumbRect, Color.White);
            spriteBatch.DrawRectangle(thumbRect, Color.Gray);

            spriteBatch.DrawString(font, artwork.Name, new Vector2(thumbRect.Right + 8, cardRect.Y + 12), Color.Black);
            spriteBatch.DrawString(font, artwork.SavedAt.ToString("MMM d, h:mm tt"),
                new Vector2(thumbRect.Right + 8, cardRect.Y + 30), Color.Gray);

            var loadButtonRect = new Rectangle(cardRect.Right - 60, cardRect.Y + 8, 50, 24);
            var deleteButtonRect = new Rectangle(cardRect.Right - 60, cardRect.Y + 38, 50, 24);

            spriteBatch.FillRectangle(loadButtonRect, ButtonFeel.GetColor(loadButtonRect, Color.CornflowerBlue));
            spriteBatch.DrawRectangle(loadButtonRect, Color.Black);
            spriteBatch.DrawString(font, "Load", new Vector2(loadButtonRect.X + 8, loadButtonRect.Y + 5), Color.White);

            spriteBatch.FillRectangle(deleteButtonRect, ButtonFeel.GetColor(deleteButtonRect, Color.IndianRed));
            spriteBatch.DrawRectangle(deleteButtonRect, Color.Black);
            spriteBatch.DrawString(font, "Delete", new Vector2(deleteButtonRect.X + 2, deleteButtonRect.Y + 5), Color.White);

            y += CARD_HEIGHT + PADDING;
        }
    }

    private Texture2D GetOrCreateThumbnail(SavedArtwork artwork)
    {
        if (thumbnailCache.TryGetValue(artwork, out var existing))
            return existing;

        var tex = new Texture2D(graphicsDevice, artwork.Width, artwork.Height);
        tex.SetData(artwork.PixelData);
        thumbnailCache[artwork] = tex;
        return tex;
    }
}